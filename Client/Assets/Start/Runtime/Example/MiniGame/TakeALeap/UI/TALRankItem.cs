using UnityEngine.UI;

namespace Start
{
    public class TALRankItem : UIElement<string>
    {
        public Text RankText;
        
        protected override void Render(string data)
        {
            RankText.text = data;
        }
    }
}