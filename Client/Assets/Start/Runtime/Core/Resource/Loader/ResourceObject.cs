using UnityEngine;

namespace Start
{
    public class ResourceObject : ObjectBase<AssetBundle>
    {
        public static ResourceObject Create(string name, object target)
        {
            ResourceObject resourceObject = RecyclableObjectPool.Acquire<ResourceObject>();
            resourceObject.Initialize((AssetBundle)target,name);
            return resourceObject;
        }

        public override void DeInitialize(bool isShutdown)
        {
            Target.Unload(true);
        }
    }
}