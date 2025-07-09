namespace Start.Editor
{
    public enum ToolbarType
    {
        Build,
        Config,
        Analysis,
    }

    public enum ChooseType
    {
        Groups,
        ResourceGroup,
        ResourceGroupItem,
    }

    /// <summary>
    /// 资源收集方式
    /// </summary>
    public enum CollectorType
    {
        /// <summary>
        /// 收集参与打包的主资源对象，并写入到资源清单的资源列表里（可以通过代码加载）。
        /// </summary>
        MainAssetCollector,

        //先处理最最常用的一种方式等以后有需求的时候再处理
        /*/// <summary>
        /// 收集参与打包的主资源对象，但不写入到资源清单的资源列表里（无法通过代码加载）。
        /// </summary>
        StaticAssetCollector,

        /// <summary>
        /// 收集参与打包的依赖资源对象，但不写入到资源清单的资源列表里（无法通过代码加载）。
        /// 注意：如果依赖资源对象没有被主资源对象引用，则不参与打包构建。
        /// </summary>
        DependAssetCollector,*/
    }

    /// <summary>
    /// 整合包的方式
    /// </summary>
    public enum PackType
    {
        /// <summary>
        /// 单独构建，一个资源一个资源包 （以资源路径为资源名称）
        /// </summary>
        PackSeparately,
        
        /// <summary>
        /// 文件夹下所有资源打进一个资源包 (以文件夹路径为资源名称)
        /// </summary>
        PackGroupItem,
        
        /// <summary>
        /// 分组下所有文件打进一个资源包 (以分组名称作为资源包名)
        /// </summary>
        PackGroup,
    }

    /// <summary>
    /// 可寻址方式
    /// </summary>
    public enum AddressType
    {
        /// <summary>
        /// 禁用
        /// </summary>
        Disable,

        /// <summary>
        /// 文件名
        /// </summary>
        FileName,

        /// <summary>
        /// 分组名_文件名
        /// </summary>
        GroupAndFileName,

        /// <summary>
        /// 文件夹名_文件名
        /// </summary>
        FolderAndFileName,
    }

    /// <summary>
    /// 对 AssetBundle 应用的压缩算法类型
    /// </summary>
    public enum CompressionType
    {
        /// <summary>
        /// 不使用压缩算法。
        /// </summary>
        Uncompressed = 0,

        /// <summary>
        /// 使用 LZ4 压缩算法。
        /// </summary>
        LZ4,

        /// <summary>
        /// 使用 LZMA 压缩算法。
        /// </summary>
        LZMA
    }

    /// <summary>
    /// 资源包流水线的构建模式
    /// </summary>
    public enum BuildModeType
    {
        /// <summary>
        /// 增量构建模式（会根据已经生成的AssetBundle文件，加快构建速度）
        /// </summary>
        IncrementalBuild,

        /// <summary>
        /// 强制重建模式 （所有AssetBundle重新构建）
        /// </summary>
        ForceRebuild,

        /// <summary>
        /// 演练构建模式 (不会生成实际的AssetBundle文件，但会生成AssetBundleManifest)
        /// </summary>
        DryRunBuild,

        /// <summary>
        /// 模拟构建模式
        /// </summary>
        SimulateBuild,
    }

    /// <summary>
    /// 命名方式
    /// </summary>
    public enum NameType
    {
        /// <summary>
        /// 哈希值命名
        /// </summary>
        HashName,
        
        /// <summary>
        /// AssetBundle命名
        /// </summary>
        AssetBundleName,
        
        /// <summary>
        /// AssetBundle名 + 哈希值名
        /// </summary>
        HashNameAndAssetBundleName,
    }
    
}