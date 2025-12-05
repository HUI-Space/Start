namespace Start
{
    public class DownloadCounterNode : IReusable
    {
        public long DeltaLength { get; private set; }

        public float ElapseSeconds { get; private set; }

        public static DownloadCounterNode Create()
        {
            return RecyclableObjectPool.Acquire<DownloadCounterNode>();
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            ElapseSeconds += realElapseSeconds;
        }

        public void AddDeltaLength(int deltaLength)
        {
            DeltaLength += deltaLength;
        }

        public void Reset()
        {
            DeltaLength = 0L;
            ElapseSeconds = 0f;
        }
    }
}