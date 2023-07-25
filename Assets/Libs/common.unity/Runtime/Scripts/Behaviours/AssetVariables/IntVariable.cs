using System;
using UnityEngine;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.AssetVariables
{
    [CreateAssetMenu(fileName = "Variable", menuName = "TechnologyLab.Common/Variables/Integer")]
    public class IntVariable : ScriptableObject
    {
        public event Action<int> ValueChanged;
        
        [SerializeField]
        private int _value;

        [SerializeField] 
        private bool _saveToPlayerPrefs = false;

        private void Awake()
        {
            if (_saveToPlayerPrefs)
            {
                _value = PlayerPrefs.GetInt(name, _value);
            }
        }

        /// <summary>
        /// Value of the variable.
        /// </summary>
        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                if (_saveToPlayerPrefs)
                {
                    PlayerPrefs.SetInt(name, value);
                }
                
                ValueChanged?.Invoke(value);
            }
        }
    }
}