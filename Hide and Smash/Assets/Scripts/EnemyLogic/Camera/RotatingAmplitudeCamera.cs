using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace EnemyLogic.Camera.Rotating
{
    public class RotatingAmplitudeCamera : MonoBehaviour
    {
        [field: SerializeField, Range(1, 10)] public float ViewRadius { get; set; }
        [field: SerializeField, Range(0, 180)] public float RotationAplitude { get; set; }
        
        [field: SerializeField] public Transform _currentCamera { get; set; }

        [SerializeField] private float _timeToRotate = 3f;
        [SerializeField] private float _timeDelay = 5f;
        
        private Vector3 _viewAngleA;
        private Vector3 _viewAngleB;

        private Queue<Vector3> _anglesQueue = new Queue<Vector3>();


        private void Start()
        {
            _viewAngleA = GetDirectionFromAngle(-RotationAplitude / 2, false);
            _viewAngleB = GetDirectionFromAngle(RotationAplitude / 2, false);
            
            _anglesQueue.Enqueue(_viewAngleA);
            _anglesQueue.Enqueue(_viewAngleB);

            StartCoroutine(StartRotationQueue());
        }


        private IEnumerator StartRotationQueue()
        {
            var dequeuedVector = _anglesQueue.Dequeue();
            
            RotateTo(dequeuedVector);
            
            _anglesQueue.Enqueue(dequeuedVector);
            
            yield return new WaitForSeconds(_timeDelay);

            StartCoroutine(StartRotationQueue());
        }


        private void RotateTo(Vector3 _currentAngle)
        {
            var rotationB = transform.position + _currentAngle * ViewRadius;

            _currentCamera.transform.DOLookAt(rotationB, _timeToRotate);
        }
        
        public Vector3 GetDirectionFromAngle(float angleInDegrees, bool anglesIsGlobal) 
        {
            if (!anglesIsGlobal)
                angleInDegrees += transform.eulerAngles.y;
            
            var vector = new Vector3(Mathf
                .Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf
                .Cos(angleInDegrees * Mathf.Deg2Rad));

            return vector;
        }
    }
}
