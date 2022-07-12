using System;
using EnemyLogic.EnemyAstronaut;
using UnityEngine;

namespace PlayerLogic
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(InjuredParts))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private FloatingJoystick _joystick;
        
        [SerializeField] private ParticleSystem _dieParticlesPrefab;

        [SerializeField] private float _mySpeed;
        [SerializeField] private float _attackForce;
        [SerializeField] private float _attackForceRadius;
        [SerializeField] private float _gravityValue = 10f;

        private CharacterController _myController;

        private Animator _myAnimator;

        private InjuredParts _injuredParts;

        private Vector3 _velocity;
        private Vector3 _playerVelocityY;

        private Transform _targetAnim;

        private EnemyAstronaut _currentEnemy;

        private bool _rageMode;

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int RageRoaring = Animator.StringToHash("RageRoaring");
        private static readonly int TakingHeal = Animator.StringToHash("TakingHeal");


        public static Action RageModeHandler;

        private void Start()
        {
            _myAnimator = GetComponent<Animator>();
            _myController = GetComponent<CharacterController>();
            _injuredParts = GetComponent<InjuredParts>();

            EnemyTrigger.EnemyInTrigger += RageAttack;
        }

        private void OnDisable()
        {
            EnemyTrigger.EnemyInTrigger -= RageAttack;
        }

        void Update()
        {
            if (_myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Move"))
            {
                Move();
            }
            else
            {
                _myAnimator.SetFloat(Speed, 0);
                if (_targetAnim != null)
                {
                    LookAt(_targetAnim);
                }
            }
        }
        
        private void LookAt(Transform target)
        {
            var direction = (target.position - gameObject.transform.position).normalized;

            Quaternion lookRotation =
                Quaternion.LookRotation(new Vector3(direction.x, 0,
                    direction.z));

            gameObject.transform.rotation = lookRotation;
        }

        private void Move()
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);

            Vector3 direction = new Vector3(_joystick.Horizontal, 0, _joystick.Vertical);
            
            _myController.Move(direction *  Time.deltaTime * _mySpeed);
            _myController.Move(_playerVelocityY * -1f * Time.deltaTime);

            MoveAnimationSpeed(direction.magnitude);
            
            Gravity();

            if (direction != Vector3.zero)
                transform.forward =
                    Vector3.SmoothDamp(transform.forward, direction, ref _velocity, 1f * Time.deltaTime);
        }
        
        private void Gravity()
        {
            if (!_myController.isGrounded)
            {
                _playerVelocityY.y += _gravityValue * Time.deltaTime;
            }
            else
            {
                _playerVelocityY.y = 0f;
            }
        }

        public void Dead()
        {
            if (!_rageMode)
            {
                _dieParticlesPrefab.gameObject.SetActive(true);
                _myAnimator.Play("Dead", 0);
            }
        }

        public void TakeHeal(Transform target)
        {
            _targetAnim = target;

            _myAnimator.SetBool(TakingHeal, true);
        }

        private void TakingOverAnimHandler()
        {
            _myAnimator.SetBool(TakingHeal, false);

            if (_injuredParts.GetHealing())
            {
                ActivateRageMode();
            }
        }

        private void MoveAnimationSpeed(float speed)
        {
            _myAnimator.SetFloat(Speed, speed);
        }

        private void ActivateRageMode()
        {
            _myAnimator.SetBool(RageRoaring, true);
            _myAnimator.SetLayerWeight(1,1);

            _mySpeed *= 3;
            _rageMode = true;

            RageModeHandler.Invoke();
            Debug.Log("Rage Mode");
        }

        private void RageAttack(EnemyAstronaut enemy)
        {
            _currentEnemy = enemy;
            _myAnimator.Play("RageAttack", 1);
            Debug.Log("attack");
        }

        private void RageAttackAnimHandler()
        {
            _currentEnemy.Dead();
            _currentEnemy.GetComponentInChildren<Rigidbody>().AddExplosionForce(_attackForce, transform.position, _attackForceRadius);
        }
    }
}
