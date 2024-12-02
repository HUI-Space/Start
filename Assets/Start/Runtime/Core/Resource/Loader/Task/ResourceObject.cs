using Start.Framework;

namespace Start.Runtime
{
    public class ResourceObject : ObjectBase
    {
        public static ResourceObject Create(string name, object target)
        {
            ResourceObject resourceObject = ReferencePool.Acquire<ResourceObject>();
            resourceObject.Initialize(target,name);
            return resourceObject;
        }

        public override void DeInitialize(bool isShutdown)
        {
            ResourceHelper.UnloadAssetBundle(Target);
        }
    }
}