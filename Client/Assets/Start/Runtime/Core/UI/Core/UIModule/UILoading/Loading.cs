using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Start
{
    public class Loading : MonoBehaviour
    {
        public CanvasGroup CanvasGroup;
        public RawImage RawImage;
        public Slider Slider;
        public Text ProgressText;
        private AsyncOperationHandle<Texture> _asyncOperationHandle;

        private float _currentValue;
        private float _targetValue;
        private bool _isShow;
        private void Awake()
        {
            CanvasGroup.Switch(false);
        }
        public async Task ShowLoading(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                _asyncOperationHandle = ResourceManager.Instance.LoadAssetAsync<Texture>(path);
                await _asyncOperationHandle.Task;
                RawImage.texture = _asyncOperationHandle.Result;
            }
            _currentValue = 0.00f;
            _targetValue = 0;
            Slider.value = 0;
            ProgressText.text = "0%";
            CanvasGroup.Switch(true);
            _isShow = true;
        }

        public void HideLoading()
        {
            _isShow = false;
            if (_asyncOperationHandle != null)
            {
                ResourceManager.Instance.Unload(_asyncOperationHandle);
                _asyncOperationHandle = null;
            }
            CanvasGroup.Switch(false);
        }

        public void UpdateLoadingProgress(float progress)
        {
            _targetValue = Mathf.Clamp(progress, 0, 1);
        }

        private void Update()
        {
            if (_isShow && Mathf.Approximately(_currentValue, _targetValue) == false)
            {
                Slider.value = _currentValue;
                _currentValue = Mathf.Lerp(_currentValue, _targetValue, 0.05f);
                
            }
            if (_isShow)
            {
                ProgressText.text = $"{(int)(_currentValue * 100)}%";
            }
        }

        private void OnDestroy()
        {
            if (_asyncOperationHandle != null)
            {
                ResourceManager.Instance.Unload(_asyncOperationHandle);
                _asyncOperationHandle = null;
            }
        }
    }
}