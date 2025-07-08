namespace Start.Editor
{
    public class ResourceBuilderController
    {
        private ResourceBuilderModel _resourceBuilderModel;
        public ResourceBuildConfig ResourceBuildConfig =>_resourceBuilderModel.ResourceBuildConfig;
        
        public void OnEnable()
        {
            _resourceBuilderModel = new ResourceBuilderModel();
            _resourceBuilderModel.OnEnable();
        }

        public void Reload()
        {
            _resourceBuilderModel.Reload();
        }

        public void SaveConfig()
        {
            _resourceBuilderModel.SaveConfig();
        }

        public void OnDestroy()
        {
            _resourceBuilderModel.OnDestroy();
            _resourceBuilderModel = default;
        }

        public void Build()
        {
            ResourceBuilder resourceBuilder = new ResourceBuilder();
            resourceBuilder.Build(ResourceBuildConfig.ResourceModeType);
        }
        
        public void BuildPatch()
        {
            ResourceBuilder resourceBuilder = new ResourceBuilder();
            resourceBuilder.BuildUpdatablePatch();
        }
    }
}