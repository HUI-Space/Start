using System.Collections.Generic;

namespace Start
{
    public class InputController : SingletonBase<InputController>
    {
        private IInputHelper _inputHelper;
        private FrameBuffer _frameBuffer = new FrameBuffer();

        private List<FrameData> _frameData;

        public void SetHelper(IInputHelper inputHelper)
        {
            _inputHelper = inputHelper;
        }

        public void InitializeData(string path)
        {
            if (BattleManager.Instance.BattleType == EBattleType.Observer)
            {
                _frameData = MessagePackUtility.DeserializeObject<List<FrameData>>(path);
            }
        }

        public void DeInitializeData()
        {
            _frameData?.Clear(); 
            _frameData = null;
            _inputHelper = null;
        }
        
        /// <summary>
        /// 获取本地帧数据
        /// </summary>
        /// <param name="authorityFrame">权威帧</param>
        /// <returns></returns>
        public FrameData GetLocalBattleFrameData(int authorityFrame)
        {
            if (BattleManager.Instance.BattleType == EBattleType.Observer)
            {
                if (authorityFrame >= _frameData.Count)
                {
                    return new FrameData(authorityFrame, 0);
                }
                return _frameData[authorityFrame];
            }
            //1.获取玩家的操作
            FrameInput frameInput = _inputHelper.GetFrameInput();
            //2.生成一个完整的逻辑帧数据
            return new FrameData(authorityFrame, new FrameInput[1] { frameInput });
        }
    }
}