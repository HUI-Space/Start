using System.Threading.Tasks;

namespace Start
{
    public class UIMask : UIModuleBase<UIMask>
    {
        protected override int Priority => 4;
        
        public override Task UIMiddleware(UIAction action)
        {
            throw new System.NotImplementedException();
        }
    }
}