using UnityEngine;
using UnityEngine.UI;

namespace Start
{
    [RequireComponent(typeof(RawImage))]
    public class UIEffectRenderTexture : UIElement
    {
        private RawImage _rawImage;

        public override void Initialize()
        {
            _rawImage = GetComponent<RawImage>();
        }

        public void Play(string path)
        {
            
        }
    }
}