using System.Collections;
using System.Collections.Generic;
using EnemyLogic.FieldOfView;
using PlayerLogic;
using UnityEngine;
using UnityEngine.AI;

namespace EnemyLogic.EnemyAstronaut
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(RagDollControls))]
    public class EnemyAstronaut : Enemy
    {
        [SerializeField] private float _movingDelay;
        [SerializeField] private float _minKillDistance;
        [SerializeField] private float _minDistanceToStop = 0.1f;
        [SerializeField] private float _minDistanceToSearch = 5f;

        [SerializeField] private List<Transform> _movingPoints = new List<Transform>();

        private Queue<Transform> _movingPointsQueue;

        private Animator _myAnimator;

        private NavMeshAgent _myAgent;

        private RagDollControls _ragDoll;

        private FieldOfView.FieldOfView _fieldOfView;

        private Vector3 _currentTargetPos;

        private static readonly int Speed = Animator.StringToHash("Speed");

        private delegate void Arrived();

        private Arrived OnWay;

        private void Start()
        {
            _myAnimator = GetComponent<Animator>();
            _myAgent = GetComponent<NavMeshAgent>();
            _ragDoll = GetComponent<RagDollControls>();

            _fieldOfView = GetComponentInChildren<FieldOfView.FieldOfView>();

            AlarmObserver.AlarmEnemiesHandler += StartPlayerSearching;

            _movingPointsQueue = new Queue<Transform>(_movingPoints);

            StartPatrolling();
        }

        private void Update()
        {
            OnWay?.Invoke();
        }

        private void StartPatrolling()
        {
            MoveTo(_movingPointsQueue.Peek().position);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                KillPLayer(other.transform);
            }
        }

        private void StartPlayerSearching(Transform targetPos)
        { 
            if ((targetPos.position - transform.position).magnitude <= _minDistanceToSearch)
            {
                StopAllCoroutines();
                MoveTo(targetPos.position);
            }
        }
        
        public override void PlayerDetected(Transform player)
        {
            if ((player.position - transform.position).magnitude <= _minKillDistance)
            {
                KillPLayer(player);
            }
            else if (player.position != _currentTargetPos)
            {
                LookAt(player);
            
                Stop();

                StopAllCoroutines();

                MoveTo(player.position);
            }
            _currentTargetPos = player.position;
        }

        private void KillPLayer(Transform player)
        {
            StopAllCoroutines();
            Stop();
            LookAt(player);
            
            AlarmObserver.AlarmEnemiesHandler += StartPlayerSearching;

            _fieldOfView.gameObject.SetActive(false);
                
            player.gameObject.GetComponent<Player>().Dead();
        }

        private void MoveTo(Vector3 target)
        {
            _myAgent.isStopped = false;
            _myAgent.SetDestination(target);

            float speed = (_myAgent.destination - transform.position).magnitude;
            MoveAnimationSpeed(speed);

            OnWay = DistanceToDestination;
        }
        
        private IEnumerator MovingDelay(float time, Vector3 target)
        {
            yield return new WaitForSeconds(time);

            MoveTo(target);
        }
        
        private void DistanceToDestination()
        {
            if ((_myAgent.destination - transform.position).magnitude <= _minDistanceToStop)
            {
                StartCoroutine(MovingDelay( _movingDelay,NextQueue()));
                
                Stop();
            }
        }

        private void Stop()
        {
            OnWay = null;
            
            _myAgent.isStopped = true;
            
            MoveAnimationSpeed(0);
        }

        private void LookAt(Transform target)
        {
            var direction = (target.position - gameObject.transform.position).normalized;

            Quaternion lookRotation =
                Quaternion.LookRotation(new Vector3(direction.x, 0,
                    direction.z));

            gameObject.transform.rotation = lookRotation;
        }

        private Vector3 NextQueue()
        {
            Transform nextVector = _movingPointsQueue.Dequeue();

            _movingPointsQueue.Enqueue(nextVector);

            return nextVector.position;
        }

        private void MoveAnimationSpeed(float speed)
        {
            _myAnimator.SetFloat(Speed, speed);
        }
        
        public void Dead()
        {
            _ragDoll.SetRagdoll(true);
            _myAgent.enabled = !enabled;
            _fieldOfView.gameObject.SetActive(false);
        }
    }
}
