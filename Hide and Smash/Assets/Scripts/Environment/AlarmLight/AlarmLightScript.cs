using EnemyLogic.FieldOfView;
using UnityEngine;

namespace Environment
{
    [RequireComponent(typeof(Animator))]
    public class AlarmLightScript : MonoBehaviour
    {
        private Animator _myAnimator;
        private static readonly int AlarmRotate = Animator.StringToHash("AlarmRotate");

        private void Start()
        {
            _myAnimator = GetComponent<Animator>();

            AlarmObserver.AlarmLightSystemHandler += SetAlarmLight;
        }

        private void OnDisable()
        {
            AlarmObserver.AlarmLightSystemHandler -= SetAlarmLight;
        }

        private void SetAlarmLight(bool enebled)
        {
            _myAnimator.SetBool(AlarmRotate , enebled);
        }
    }
}
