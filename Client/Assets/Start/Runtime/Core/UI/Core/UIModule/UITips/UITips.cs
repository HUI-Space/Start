using System.Threading.Tasks;
using UnityEngine;

namespace Start
{
    public class UITips : UIModuleBase<UITips>
    {
        private Tips _tips;
        private AsyncOperationHandle<GameObject> _asyncOperationHandle;

        protected override int Priority => 3;
        

        public override async Task UIMiddleware(UIAction action)
        {
            if (action.ActionType == UIActionTypes.ShowTips)
            {
                if (!_tips)
                {
                    _asyncOperationHandle = ResourceManager.Instance.LoadAssetAsync<GameObject>(UIModuleConst.Tips);
                    await _asyncOperationHandle.Task;
                    GameObject go = _asyncOperationHandle.Result;
                    _tips = Instantiate(go, transform).GetComponent<Tips>();
                }
                string message = action.GetData1<string>();
                float durationTime = action.GetData2<float>();
                bool isThroughAll = action.GetData3<bool>();
                int throughCount = action.GetData4<int>();
                if (string.IsNullOrEmpty(message))
                {
                    Logger.Error("显示空的Tips");
                }
                _tips.ShowMessage(message,durationTime,isThroughAll, throughCount);
            }
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            if (_tips)
            {
                Destroy(_tips);
                ResourceManager.Instance.Unload(_asyncOperationHandle);
            }
        }
    }
}