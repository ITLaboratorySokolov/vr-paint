using UnityEngine;
using UnityEngine.Serialization;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.AssetVariables
{
    /// <summary>
    /// A string variable that can be saved to assets.
    /// </summary>
    [CreateAssetMenu(fileName = "Variable", menuName = "TechnologyLab.Common/Variables/String")]
    public class StringVariable : ScriptableObject
    {
        [SerializeField]
        [FormerlySerializedAs("value")]
        private string _value;

        /// <summary>
        /// Value of the variable.
        /// </summary>
        public string Value
        {
            get => _value;
            set => _value = value;
        }
    }
}
