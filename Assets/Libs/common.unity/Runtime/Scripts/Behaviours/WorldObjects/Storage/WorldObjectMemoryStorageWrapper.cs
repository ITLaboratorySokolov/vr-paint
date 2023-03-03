using ZCU.TechnologyLab.Common.Unity.Models.WorldObjects.Storage;

namespace ZCU.TechnologyLab.Common.Unity.Behaviours.WorldObjects.Storage
{
    /// <summary>
    /// Storage of world objects. World objects are saved in a dictionary.
    /// </summary>
    public class WorldObjectMemoryStorageWrapper : WorldObjectStorageWrapper
    {
        protected override IWorldObjectStorage CreateStorage()
        {
            return new WorldObjectMemoryStorage();
        }
    }
}
