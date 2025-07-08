using UnityEngine.UI;

namespace Start
{
    public class ScrollerItem : UIElement<int>
    {
        public Text Text;
        
        protected override void Render(int data)
        {
            Text.text = data.ToString();
        }
    }
}