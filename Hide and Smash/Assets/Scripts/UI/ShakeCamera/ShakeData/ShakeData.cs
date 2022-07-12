using UnityEngine;

namespace ShakerLogic
{
    [CreateAssetMenu(menuName = "CameraShakerLogic/ShakeData")]
    public class ShakeData : ScriptableObject
    {
        [SerializeField] private ShakeType _shakeType;
        
        [SerializeField] private Vector3 _pivotOffset;
        
        [SerializeField] private float _intensityGain;
        [SerializeField] private float _frequencyGain;
        [SerializeField] private float _shakingTime;

        public ShakeType GetShakeType => _shakeType;

        public float GetIntensity => _intensityGain;

        public float GetFrequency => _frequencyGain;

        public float GetShakingTime => _shakingTime;

        public Vector3 GetPivotOffset => _pivotOffset;
    }
}

