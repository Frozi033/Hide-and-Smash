using System;
using EnemyLogic.EnemyAstronaut;
using UnityEngine;

namespace PlayerLogic
{
    [RequireComponent(typeof(Collider))]
    public class EnemyTrigger : MonoBehaviour
    {
        private Collider _myCollider;
        
        public static Action<EnemyAstronaut> EnemyInTrigger;

        private void Start()
        {
            Player.RageModeHandler += SetActiveGameObject;

            _myCollider = GetComponent<Collider>();
            Debug.Log("Subscribed");
        }

        private void OnDisable()
        {
            Player.RageModeHandler += SetActiveGameObject;
        }

        private void SetActiveGameObject()
        {
            _myCollider.enabled = true;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out EnemyAstronaut enemy))
            {
                EnemyInTrigger?.Invoke(enemy);
                Debug.Log("Енеми");
            }
        }
    }
}