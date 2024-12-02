using Start.Framework;

namespace Start.Script
{
    public class AccountDataEntity:DataEntity
    {
        public override void Initialize()
        {
            throw new System.NotImplementedException();
        }

        public override void ResetTempData()
        {
            throw new System.NotImplementedException();
        }

        public override void DeInitialize()
        {
            throw new System.NotImplementedException();
        }
    }

    public class AccountDataEntityCollection : DataEntityCollection<AccountDataEntity>
    {
        public override void Register()
        {
            
        }

        public override void UnRegister()
        {
           
        }
    }

    public class TAccountDataEntityCollection
    {
        public void Test()
        {
            
        }
    }
}