using UnityEngine;

namespace PlayerLogic
{
    public class VesselTrigger : MonoBehaviour
    {
        private Collider _curretnCollider;

        private Animator _myAnimator;

        private void Start()
        {
            _curretnCollider = GetComponent<Collider>();
            _myAnimator = GetComponent<Animator>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                player.TakeHeal(gameObject.transform);
                
                VesselTaken();
            }
        }

        private void VesselTaken()
        {
            _myAnimator.Play("VesselTaken", 0);

            _curretnCollider.isTrigger = false;
        }

        private void DestroyVessel()
        {
            Destroy(gameObject);
        }
    }
}
