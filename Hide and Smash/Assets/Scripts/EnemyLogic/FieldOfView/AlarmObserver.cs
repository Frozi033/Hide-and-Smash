using System;
using System.Collections;
using EnemyLogic.Camera;
using UnityEngine;

namespace EnemyLogic.FieldOfView
{
    public class AlarmObserver : MonoBehaviour
    {
        [SerializeField] private float _alarmTime;
        
        public static Action<bool> AlarmLightSystemHandler;
        public static Action<Transform> AlarmEnemiesHandler;

        private void Awake()
        { 
            ObservingCamera.PlayerScannedHandler += PlayerScanned;
        }

        private void OnDisable()
        {
            ObservingCamera.PlayerScannedHandler -= PlayerScanned;
        }

        private void PlayerScanned(Transform targetPos)
        {
            AlarmLightSystemHandler?.Invoke(true);
            
            AlarmEnemiesHandler?.Invoke(targetPos);

            StartCoroutine(AlarmTimer());
        }

        private IEnumerator AlarmTimer()
        {
            yield return new WaitForSeconds(_alarmTime);
            
            AlarmLightSystemHandler?.Invoke(false);
        }
    }
}