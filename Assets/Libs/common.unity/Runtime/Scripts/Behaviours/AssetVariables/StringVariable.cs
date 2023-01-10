using UnityEngine;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.AssetVariables
{
    /// <summary>
    /// A string variable that can be saved to assets.
    /// </summary>
    [CreateAssetMenu(fileName = "Variable", menuName = "TechnologyLab.Common/Variables/String")]
    public class StringVariable : ScriptableObject
    {
        [SerializeField]
        private string value;

        /// <summary>
        /// Value of the variable.
        /// </summary>
        public string Value
        {
            get => this.value;
            set => this.value = value;
        }
    }
}
