using System.Collections.Generic;
using UnityEngine;

namespace Start
{
    public class UIEffect : UIElement
    {
        public string EffectPath;
        public int LayerOffset = 1;
        public bool UseScale = true;
        public Vector3 Scale = Vector3.one;
        public Vector2 PosOffset = Vector2.zero;
        public int DesignSize = 20;
        public int Layer = 5;
        
        protected List<Renderer> ParticleRenderers = new List<Renderer>();
        protected List<ParticleSystem> ParticleSystems = new List<ParticleSystem>();
        
        private AsyncOperationHandle<GameObject> _handle;
        private GameObject _effect;
        private string _uiDataName;
        private int _oder;
        
        public override void Initialize()
        {
            UIBase parent = GetComponentInParent<UIBase>();
            UIConfig uiConfig = ConfigManager.Instance.GetConfig<UIConfig>();
            if (uiConfig.GetUIConfigItem(parent.GetType().Name,out UIConfigItem item))
            {
                _uiDataName = FileUtility.GetExtensionWithoutDot(item.UIDataName);
                if (UIWindow.Instance.GetUIData(_uiDataName, out IUIData uiData))
                {
                    _oder = uiData.Order;
                }
                PlayEffect();
                UIWindow.Instance.AddListener(_uiDataName,OnRender);
            }
        }

        public void Play(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            if (EffectPath.Equals(path))
            {
                return;
            }
            EffectPath = path;
            if (_handle != null)
            {
                ParticleRenderers.Clear();
                ParticleSystems.Clear();
                Destroy(_effect);
                ResourceManager.Instance.Unload(_handle);
                _handle = default;
                _effect = default;
            }
            PlayEffect();
        }
        
        private async void PlayEffect()
        {
            if (!string.IsNullOrEmpty(EffectPath))
            {
                _handle = ResourceManager.Instance.LoadAssetAsync<GameObject>(EffectPath);
                await _handle.Task;
                if (_handle.Result != null)
                {
                    _effect = Instantiate(_handle.Result, transform);
                    _effect.GetComponentsInChildren(ParticleRenderers);
                    _effect.GetComponentsInChildren(ParticleSystems);
                    _effect.transform.localPosition = PosOffset;
                    _effect.transform.localScale = Scale;
                    _effect.BatchChangeLayer(Layer);
                    foreach (Renderer render in ParticleRenderers)
                    {
                        render.sortingOrder = _oder * 1000 + LayerOffset;
                    }
                    foreach (var t in ParticleSystems)
                    {
                        t.Play(false);
                    }
                }
            }
        }
        
        private void OnRender(IUIData iUIData)
        {
            if (_oder != iUIData.Order)
            {
                _oder = iUIData.Order;
                foreach (Renderer render in ParticleRenderers)
                {
                    render.sortingOrder = _oder * 1000 + LayerOffset;
                }
            }
        }

        public override void DeInitialize()
        {
            UIWindow.Instance.RemoveListener(_uiDataName,OnRender);
            if (string.IsNullOrEmpty(EffectPath))
            {
                return;
            }
            if (_handle != null)
            {
                ParticleRenderers.Clear();
                ParticleSystems.Clear();
                Destroy(_effect);
                ResourceManager.Instance.Unload(_handle);
            }
            _handle = default;
            _effect = default;
            _uiDataName = default;
            _oder = default;
        }
    }
}