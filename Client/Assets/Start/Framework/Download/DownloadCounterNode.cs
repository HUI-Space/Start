namespace Start
{
    public class DownloadCounterNode : IRecycle
    {
        public long DeltaLength { get; private set; }

        public float ElapseSeconds { get; private set; }

        public static DownloadCounterNode Create()
        {
            return RecyclablePool.Acquire<DownloadCounterNode>();
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            ElapseSeconds += realElapseSeconds;
        }

        public void AddDeltaLength(int deltaLength)
        {
            DeltaLength += deltaLength;
        }

        public void Recycle()
        {
            DeltaLength = 0L;
            ElapseSeconds = 0f;
        }
    }
}