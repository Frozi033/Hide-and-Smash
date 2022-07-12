using UnityEngine;

namespace EnemyLogic
{
    public abstract class Enemy : MonoBehaviour
    {
        public abstract void PlayerDetected(Transform player);
    }
}