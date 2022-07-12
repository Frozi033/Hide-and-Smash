using UnityEngine;
using DG.Tweening;
namespace PlayerLogic
{
    public class RageScaler : MonoBehaviour
    {
        [SerializeField] private Transform _hipsScaler;
        
        [SerializeField] private float _timeToScale;

        [SerializeField] private float _rageScale = 1.5f;
        
        
        private void Start()
        {

            Player.RageModeHandler += RageScale;
        }

        private void OnDisable()
        {
            Player.RageModeHandler -= RageScale;
        }
        
        private void RageScale()
        {
            _hipsScaler.DOScale(new Vector3(_rageScale, _rageScale, _rageScale), _timeToScale);
        }
    }
}