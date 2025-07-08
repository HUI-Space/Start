namespace Start.Editor
{
    /// <summary>
    /// 收集的资源数据（可用代码加载的资源）
    /// MainResource 为配置的收集的资源数据 例如某个 ResourceGroupItem 中 ResourcePath 路径下的所有有效资源 
    /// Resource 为运行时收集的所有有效资源数据
    /// </summary>
    public class MainResource
    {
        /// <summary>
        /// 资源包名称
        /// </summary>
        public string ResourcePackageName { get; private set; }
        
        /// <summary>
        /// ResourceGroup Name
        /// </summary>
        public string ResourceGroupName { get; private set; }
        
        /// <summary>
        /// Resource Path
        /// </summary>
        public string ResourceGroupItemPath { get; private set; }
        
        /// <summary>
        /// 收集器类型
        /// </summary>
        public CollectorType CollectorType { get; private set; }
        
        /// <summary>
        /// 构建类型
        /// </summary>
        public PackType PackType { get; private set; }
        
        /// <summary>
        /// 可寻址类型
        /// </summary>
        public AddressType AddressType { get; private set; }
        
        /// <summary>
        /// 资源分类标签
        /// </summary>
        public string Tags { get; private set; }
        
        /// <summary>
        /// 资源数据
        /// </summary>
        public Resource Resource { get; private set; }
        
        
        public MainResource(string resourceGroupName , string resourceGroupItemPath, CollectorType collectorType,
            PackType packType, AddressType addressType, string tags, string asset,string resourcePackageName = null)
        {
            ResourceGroupName = resourceGroupName;
            ResourceGroupItemPath = resourceGroupItemPath;
            CollectorType = collectorType;
            PackType = packType;
            AddressType = addressType;
            Tags = tags;
            Resource = new Resource(asset,resourceGroupName,resourcePackageName);
            ResourcePackageName = resourcePackageName;
        }

        public MainResource(Resource resource,string resourceGroupName, string resourceGroupItemPath, CollectorType collectorType,
            PackType packType, AddressType addressType, string tags, string resourcePackageName = null)
        {
            Resource = resource;
            ResourceGroupName = resourceGroupName;
            ResourceGroupItemPath = resourceGroupItemPath;
            CollectorType = collectorType;
            PackType = packType;
            AddressType = addressType;
            Tags = tags;
            ResourcePackageName = resourcePackageName;
        }
        
    }
}