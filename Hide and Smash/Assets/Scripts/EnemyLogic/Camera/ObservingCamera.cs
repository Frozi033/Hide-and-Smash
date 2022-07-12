using System;
using UnityEngine;

namespace EnemyLogic.Camera
{
    public class ObservingCamera : Enemy
    {
        public static Action<Transform> PlayerScannedHandler;
        public override void PlayerDetected(Transform player)
        {
            PlayerScannedHandler?.Invoke(player);
        }
    }
}