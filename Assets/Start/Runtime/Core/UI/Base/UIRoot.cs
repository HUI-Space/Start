using UnityEngine;

public class UIRoot : MonoBehaviour
{
    public static UIRoot Instance;
    
    public RectTransform Window;
    public RectTransform Loading;
    public RectTransform Fade;
    public RectTransform Tips;
    public RectTransform Guide;
    public RectTransform Mask;
    
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
