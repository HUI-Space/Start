

namespace Start
{
    public class DownloadCounterNode : IReference
    {
        public long DeltaLength { get; private set; }

        public float ElapseSeconds { get; private set; }

        public static DownloadCounterNode Create()
        {
            return ReferencePool.Acquire<DownloadCounterNode>();
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            ElapseSeconds += realElapseSeconds;
        }

        public void AddDeltaLength(int deltaLength)
        {
            DeltaLength += deltaLength;
        }

        public void Clear()
        {
            DeltaLength = 0L;
            ElapseSeconds = 0f;
        }
    }
}