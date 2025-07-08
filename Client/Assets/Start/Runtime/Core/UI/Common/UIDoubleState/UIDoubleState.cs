using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Start
{
    public class UIDoubleState : UIElement<bool>
    {
        /// <summary>
        /// 是否忽略父母自行控制
        /// </summary>
        public bool IgnoreParent;
        public List<UIDoubleState> Children = new List<UIDoubleState>();
        private Graphic _graphic;
        private RawImage _rawImage;
        private Image  _image;
        private Text _text;
        private bool _initialize;
        
        public EGraphicType GraphicType { get; private set; }
        public bool On { get; private set; }

        public bool TextureChange;
        public Texture OnTexture;
        public Texture OffTexture;
        
        //Image
        public bool SetNativeSize;
        public bool SpriteChange;
        public Sprite OnSprite;
        public Sprite OffSprite;
        
        public bool ColorChange;
        public Color OnColor  = Color.white;
        public Color OffColor = Color.gray;
        
        //Text
        public bool TextChange;
        public string OnText;
        public string OffText;

        public bool TextSizeChange;
        public int OnTextSize;
        public int OffTestSize;
        
        public bool RaycastChange;
        public bool OnRaycast;
        public bool OffRaycast;
        
        public bool ChangeActive;
        public bool OnActive;
        public bool OffActive;
        
        public bool ChangeCanvasGroup;
        public CanvasGroup CanvasGroup;
        public bool OnSwitch;
        public bool OffSwitch;
        
        public bool ChangeInteractable;
        public Selectable Selectable;
        public bool OnInteractable;
        public bool OffInteractable;
        
        /*private TextMeshProUGUI _textMeshProUgui;
        public string OnTextMeshProUgui;
        public string OffTextMeshProUgui;
        public bool TextMarginChange;
        public Vector4 OnMargin;
        public Vector4 OffMargin;*/
        
        public override void Initialize()
        {
            _initialize = true;
            _graphic = GetComponent<Graphic>();
            if (_graphic is RawImage rawImage)
            {
                _rawImage = rawImage;
                GraphicType = EGraphicType.RawImage;
            }
            else if (_graphic is Image image)
            {
                _image = image;
                GraphicType = EGraphicType.Image;
            }
            else if (_graphic is Text text)
            {
                _text = text;
                GraphicType = EGraphicType.Text;
            }
            /*else if (_graphic is TextMeshProUGUI textMeshProUgui)
            {
                _textMeshProUgui = textMeshProUgui;
                GraphicType = EGraphicType.TextMeshProUGUI;
            }*/
        }

        protected override void Render(bool data)
        {
            if (!_initialize)
            {
                Initialize();
            }
            On = data;

            if (TextureChange)
            {
                _rawImage.texture = data ? OnTexture : OffTexture;
            }
            
            if (SpriteChange)
            {
                _image.sprite = data ? OnSprite : OffSprite;
                if (SetNativeSize)
                {
                    _image.SetNativeSize();
                }
            }

            if (ColorChange)
            {
                _graphic.color = data ? OnColor : OffColor;
            }
            
            if (TextChange)
            {
                _text.text = data ? OnText : OffText;
            }
            
            if (TextSizeChange)
            {
                _text.fontSize = data ? OnTextSize : OffTestSize;
            }

            if (RaycastChange)
            {
                _graphic.raycastTarget = data;
            }
            
            if (ChangeCanvasGroup)
            {
                CanvasGroup.Switch(data);
            }

            if (ChangeActive)
            {
                gameObject.SetActive(data);
            }

            if (ChangeInteractable)
            {
                Selectable.interactable = data ? OnInteractable : OffInteractable;
            }
            
            foreach (UIDoubleState child in Children)
            {
                if (child.IgnoreParent == false)
                {
                    child.SetData(data);
                }
            }
        }

        public override void DeInitialize()
        {
            _graphic = default;
            _rawImage = default;
            _image = default;
            _text = default;
        }
    }
}