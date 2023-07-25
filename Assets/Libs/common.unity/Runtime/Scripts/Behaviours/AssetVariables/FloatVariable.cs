using System;
using UnityEngine;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.AssetVariables
{
    [CreateAssetMenu(fileName = "Variable", menuName = "TechnologyLab.Common/Variables/Float")]
    public class FloatVariable : ScriptableObject
    {
        public event Action<float> ValueChanged;
        
        [SerializeField]
        private float _value;

        [SerializeField] 
        private bool _saveToPlayerPrefs = false;

        private void Awake()
        {
            if (_saveToPlayerPrefs)
            {
                _value = PlayerPrefs.GetFloat(name, _value);
            }
        }

        /// <summary>
        /// Value of the variable.
        /// </summary>
        public float Value
        {
            get => _value;
            set
            {
                _value = value;
                if (_saveToPlayerPrefs)
                {
                    PlayerPrefs.SetFloat(name, value);
                }
                
                ValueChanged?.Invoke(value);
            }
        }
    }
}