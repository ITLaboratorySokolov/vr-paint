using UnityEngine;
using ZCU.TechnologyLab.Common.Unity.WorldObjects.Properties;

namespace ZCU.TechnologyLab.Common.Unity.Utility.Inventory
{
    /// <summary>
    /// Inventory of properties managers.
    /// </summary>
    /// <seealso cref="IPropertiesManager"/>
    [CreateAssetMenu(fileName = "Inventory", menuName = "TechnologyLab.Common/Inventory/GameObject")]
    public class GameObjectInventory : Inventory<GameObject>
    {
    }
}
