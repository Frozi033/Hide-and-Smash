using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace PlayerLogic
{
    public class InjuredParts : MonoBehaviour
    { 
        [SerializeField] private List<TwoBoneIKConstraint> _injuredParts = new List<TwoBoneIKConstraint>();
        
        [SerializeField] private float _lerpSpeed;
        
        private int _id;
        
        private delegate void AnimationLerpDelegate(int id);
        
        private AnimationLerpDelegate _lerpAnimation;

        private void Start()
        {
            _lerpAnimation = null;
        }

        private void Update()
        {
            _lerpAnimation?.Invoke(_id);
        }

        public bool GetHealing()
        {
            if (_id >= _injuredParts.Count)
            {
                return true;
            }
            _lerpAnimation = LerpAnimation;
            
            return false;
        }
        
        private void LerpAnimation(int id)
        {
            if (_injuredParts[_id].weight <= 0.1)
            {
                _lerpAnimation = null;
       
                _id++;
            }
            else
            {
                _injuredParts[id].weight = Mathf.Lerp(_injuredParts[id].weight, 0, Time.deltaTime * _lerpSpeed);
            }
        }
    }
}