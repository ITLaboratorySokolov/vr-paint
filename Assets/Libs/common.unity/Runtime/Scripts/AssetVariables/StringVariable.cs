using UnityEngine;

namespace ZCU.TechnologyLab.Common.Unity.AssetVariables
{
    /// <summary>
    /// A string variable that can be saved to assets.
    /// </summary>
    [CreateAssetMenu(fileName = "Variable", menuName = "TechnologyLab.Common/Variables/String")]
    public class StringVariable : ScriptableObject
    {
        /// <summary>
        /// Value of this variable.
        /// </summary>
        [SerializeField]
        private string value;

        /// <summary>
        /// Gets and sets a value of this variable.
        /// </summary>
        public string Value
        {
            get => this.value;
            set => this.value = value;
        }
    }
}
