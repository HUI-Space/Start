using System;
using System.Collections.Generic;


namespace Start
{
    public class DownloadCounter
    {
        private readonly LinkedList<DownloadCounterNode> m_DownloadCounterNodes;
        private float _updateInterval;
        private float _recordInterval;
        private float _accumulator;
        private float _timeLeft;

        public DownloadCounter(float updateInterval, float recordInterval)
        {
            if (updateInterval <= 0f)
            {
                throw new Exception("更新间隔无效.");
            }

            if (recordInterval <= 0f)
            {
                throw new Exception("记录间隔无效.");
            }

            m_DownloadCounterNodes = new LinkedList<DownloadCounterNode>();
            _updateInterval = updateInterval;
            _recordInterval = recordInterval;
            Reset();
        }

        public float UpdateInterval
        {
            get => _updateInterval;
            set
            {
                if (value <= 0f)
                {
                    throw new Exception("更新间隔无效.");
                }

                _updateInterval = value;
                Reset();
            }
        }

        public float RecordInterval
        {
            get => _recordInterval;
            set
            {
                if (value <= 0f)
                {
                    throw new Exception("记录间隔无效.");
                }

                _recordInterval = value;
                Reset();
            }
        }

        public float CurrentSpeed { get; private set; }

        public void DeInitialize()
        {
            Reset();
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (m_DownloadCounterNodes.Count <= 0)
            {
                return;
            }

            _accumulator += realElapseSeconds;
            if (_accumulator > _recordInterval)
            {
                _accumulator = _recordInterval;
            }

            _timeLeft -= realElapseSeconds;
            foreach (DownloadCounterNode downloadCounterNode in m_DownloadCounterNodes)
            {
                downloadCounterNode.Update(elapseSeconds, realElapseSeconds);
            }

            while (m_DownloadCounterNodes.Count > 0)
            {
                DownloadCounterNode downloadCounterNode = m_DownloadCounterNodes.First.Value;
                if (downloadCounterNode.ElapseSeconds < _recordInterval)
                {
                    break;
                }

                RecyclableObjectPool.Recycle(downloadCounterNode);
                m_DownloadCounterNodes.RemoveFirst();
            }

            if (m_DownloadCounterNodes.Count <= 0)
            {
                Reset();
                return;
            }

            if (_timeLeft <= 0f)
            {
                long totalDeltaLength = 0L;
                foreach (DownloadCounterNode downloadCounterNode in m_DownloadCounterNodes)
                {
                    totalDeltaLength += downloadCounterNode.DeltaLength;
                }

                CurrentSpeed = _accumulator > 0f ? totalDeltaLength / _accumulator : 0f;
                _timeLeft += _updateInterval;
            }
        }

        public void RecordDeltaLength(int deltaLength)
        {
            if (deltaLength <= 0)
            {
                return;
            }

            DownloadCounterNode downloadCounterNode = null;
            if (m_DownloadCounterNodes.Count > 0)
            {
                downloadCounterNode = m_DownloadCounterNodes.Last.Value;
                if (downloadCounterNode.ElapseSeconds < _updateInterval)
                {
                    downloadCounterNode.AddDeltaLength(deltaLength);
                    return;
                }
            }

            downloadCounterNode = DownloadCounterNode.Create();
            downloadCounterNode.AddDeltaLength(deltaLength);
            m_DownloadCounterNodes.AddLast(downloadCounterNode);
        }

        private void Reset()
        {
            m_DownloadCounterNodes.Clear();
            CurrentSpeed = 0f;
            _accumulator = 0f;
            _timeLeft = 0f;
        }
    }
}