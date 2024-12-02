using Start.Framework;

namespace Start.Script
{
    public class CharacterDataEntity:DataEntity<int>
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

    public class CharacterDataEntityCollection : DataEntityCollection<int, CharacterDataEntity>
    {
        public override void Register()
        {
            throw new System.NotImplementedException();
        }

        public override void UnRegister()
        {
            throw new System.NotImplementedException();
        }

        protected override bool HasKey(int key)
        {
            throw new System.NotImplementedException();
        }
    }

    public class TCharacterDataEntityCollection
    {
        public void Test()
        {
            
            
        }
    }
}