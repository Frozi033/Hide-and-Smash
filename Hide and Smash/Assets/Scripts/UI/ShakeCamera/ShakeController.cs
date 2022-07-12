using System;
using UnityEngine;

namespace ShakerLogic
{
    public class ShakeController : MonoBehaviour
    {
        public static Action<ShakeType> ShakingHandler;

        private void ShakingAnimHandler(ShakeType _shakeType)
        {
            ShakingHandler?.Invoke(_shakeType);
        }
    }

    public enum ShakeType
    {
        OnStep,
        OnEnemyKill,
        Roaring,
        End
    }
}
