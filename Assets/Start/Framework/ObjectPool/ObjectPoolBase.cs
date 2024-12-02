namespace Start.Framework
{
    public abstract class ObjectPoolBase
    {
        public abstract int Priority
        {
            get;
            set;
        }
        public abstract void Update(float elapseSeconds, float realElapseSeconds);
        public abstract void Release();
        public abstract void ReleaseAllUnused();
        
        public abstract void DeInitialize();
    }
}