using System.Collections.Generic;

namespace Start
{
    /// <summary>
    /// 回放控制器
    /// </summary>
    public class ReplayController : SingletonBase<ReplayController>
    {

        private List<FrameData> _frameData;
        
        public void InitializeData()
        {
            _frameData = new List<FrameData>(10000);
        }
        
        
        public void DeInitializeData()
        {
            _frameData.Clear();
            _frameData = null;
        }
        
        public void LogicUpdate(FrameData frame)
        {
            _frameData.Add(frame);
        }

        public void SaveReplay(string path)
        {
            SerializerUtility.SerializeObject(path, _frameData);
        }
    }
}