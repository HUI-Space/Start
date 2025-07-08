using System.Threading.Tasks;

namespace Start
{
    public class UIGuide : UIModuleBase<UIGuide>
    {
        protected override int Priority => 2;
        
        public override Task UIMiddleware(UIAction action)
        {
            throw new System.NotImplementedException();
        }
    }
}