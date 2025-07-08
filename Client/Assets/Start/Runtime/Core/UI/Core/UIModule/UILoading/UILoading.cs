using System.Threading.Tasks;
using UnityEngine;

namespace Start
{
    public class UILoading : UIModuleBase<UILoading>
    {
        protected override int Priority => 1;

        private Loading _loading;
        private AsyncOperationHandle<GameObject> _asyncOperationHandle;
        
        public override async Task UIMiddleware(UIAction action)
        {
            if (action.ActionType == UIActionTypes.ShowLoading)
            {
                if (!_loading)
                {
                    _asyncOperationHandle = ResourceManager.Instance.LoadAssetAsync<GameObject>(UIModuleConst.Loading);
                    await _asyncOperationHandle.Task;
                    GameObject go = _asyncOperationHandle.Result;
                    _loading = Instantiate(go, transform).GetComponent<Loading>();
                }
                await _loading.ShowLoading(action.GetData1<string>());
            }
            else if (action.ActionType == UIActionTypes.HideLoading)
            {
                _loading.HideLoading();
            }
            else if (action.ActionType == UIActionTypes.UpdateLoadingProgress)
            {
                _loading.UpdateLoadingProgress(action.GetData1<float>());
            }
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            if (_loading)
            {
                Destroy(_loading);
                ResourceManager.Instance.Unload(_asyncOperationHandle);
            }
        }
    }
}