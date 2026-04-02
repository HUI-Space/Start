using UnityEngine;

namespace Start
{
    public class ResourceResourceObject : ResourceObjectBase<AssetBundle>
    {
        public static ResourceResourceObject Create(string name, object target)
        {
            ResourceResourceObject resourceResourceObject = RecyclablePool.Acquire<ResourceResourceObject>();
            resourceResourceObject.Initialize((AssetBundle)target,name);
            return resourceResourceObject;
        }

        public override void DeInitialize(bool isShutdown)
        {
            Target.Unload(true);
        }
    }
}