namespace Start
{
    public class TALController : SingletonBase<TALController>
    {
        public TALLogicModule LogicModule;
        
        public TALSettingModule SettingModule;
        
        public TALRankModule RankModule;
        
        public override void Initialize()
        {
            LogicModule = new TALLogicModule();
            SettingModule = new TALSettingModule();
            RankModule = new TALRankModule();
            LogicModule.Initialize();
        }

        public override void DeInitialize()
        {
            LogicModule.DeInitialize();
            LogicModule = default;
            SettingModule = default;
            RankModule = default;
        }
    }

}