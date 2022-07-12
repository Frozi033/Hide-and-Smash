using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using PlayerLogic;

namespace ShakerLogic
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class ShakeCamera : MonoBehaviour
    {
        [SerializeField] private List<ShakeData> _shakeData = new List<ShakeData>();

        private CinemachineVirtualCamera _cinemachineVirtualCam;
        private CinemachineBasicMultiChannelPerlin _perlinChannel;
        
        private float _shakingTime;
        private float _intensity;
        private float _frequency;

        private Vector3 _pivotOffset;
        
        private void Start()
        {
            _cinemachineVirtualCam = GetComponent<CinemachineVirtualCamera>();
            _perlinChannel = _cinemachineVirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            
            SetShaking(0,0, new Vector3(0, 0, 0));

            Player.RageModeHandler += SetRageMode;
        }

        private void OnDisable()
        {
            Player.RageModeHandler -= SetRageMode;
            ShakeController.ShakingHandler -= StartShaking;
        }

        private void SetRageMode()
        {
            ShakeController.ShakingHandler += StartShaking;
        }

        private void StartShaking(ShakeType currentShakeType)
        {
            foreach (var shakeType in _shakeData)
            {
                if (shakeType.GetShakeType == currentShakeType)
                {
                    _intensity = shakeType.GetIntensity;
                    _frequency = shakeType.GetFrequency;
                    _pivotOffset = shakeType.GetPivotOffset;
                    _shakingTime = shakeType.GetShakingTime;
                    
                    if (_shakingTime != 0)
                    {
                        StartCoroutine(TemporaryShaking(_shakingTime));
                    }
                    else
                    {
                        SetShaking(_intensity, _frequency, _pivotOffset);
                    }
                    
                    break;
                }
            }
        }

        IEnumerator TemporaryShaking(float time)
        {
            SetShaking(_intensity, _frequency, _pivotOffset);
            
            yield return new WaitForSeconds(time);

            SetShaking(0, 0 , new Vector3(0, 0,0));
        }

        private void SetShaking(float intensity, float frequency, Vector3 pivotOffset)
        {
            SetAmplitudeGain(intensity);
            SetFrequencyGain(frequency); 
            SetPivotOffset(pivotOffset);
        }
        
        private void SetAmplitudeGain(float intensity)
        {
            _perlinChannel.m_AmplitudeGain = intensity;
        }
        
        private void SetFrequencyGain(float frequency)
        {
            _perlinChannel.m_FrequencyGain = frequency;
        }

        private void SetPivotOffset(Vector3 pivotOffset)
        {
            _perlinChannel.m_PivotOffset = pivotOffset;
        }
    }
}
