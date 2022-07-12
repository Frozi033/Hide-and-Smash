using UnityEngine;

namespace EnemyLogic
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class RagDollControls : MonoBehaviour
    {
        private Animator _myAnimator;

        private Rigidbody _myRigidbody;

        private BloodSpawn[] _bloodSpawners;

        private CapsuleCollider _myCollider;

        private Rigidbody[] _rigidbodies;

        private Collider[] _colliders;

        public bool on;

        private void Start()
        {
            _rigidbodies = GetComponentsInChildren<Rigidbody>();
            _colliders = GetComponentsInChildren<Collider>();
            _bloodSpawners = GetComponentsInChildren<BloodSpawn>();
            
            _myAnimator = GetComponent<Animator>();
           // _myRigidbody = GetComponent<Rigidbody>();
            _myCollider = GetComponent<CapsuleCollider>();

            SetRagdoll(false);
        }

        private void Update()
        {
            if (on)
            {
                SetRagdoll(true);
                on = false;
            }
        }

        public void SetRagdoll(bool enabled)
        {
            _myAnimator.enabled = !enabled;
            
            SetRigidbodies(_rigidbodies, enabled);
            SetColliders(_colliders, enabled);
            SetBloodSpawners(_bloodSpawners, enabled);

            _myCollider.enabled = !enabled;
            
         //   _myRigidbody.isKinematic = enabled;
           // _myRigidbody.useGravity = !enabled;
        }

        private void SetRigidbodies(Rigidbody[] rigidbodies, bool enebled)
        {
            foreach (var _rigidbody in rigidbodies)
            {
                _rigidbody.isKinematic = !enebled;
                _rigidbody.useGravity = enebled;
            }
        }
        
        private void SetColliders(Collider[] colliders, bool enebled)
        {
            foreach (var _collider in colliders)
            {
                _collider.isTrigger = !enebled;
            }
        }
        
        private void SetBloodSpawners(BloodSpawn[] bloodSpawners, bool enebled)
        {
            foreach (var bloodSpawner in bloodSpawners)
            {
                bloodSpawner.enabled = enebled;
            }
        }
    }
}
