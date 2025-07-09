namespace Start.Editor
{
    public interface IBuildTask
    {
        void Run(IResourceBuilder builder);
    }
}