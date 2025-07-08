using System.Collections.Generic;

namespace Start
{
    public class TALRankModule
    {



        public string[] GetRankData()
        {
            string rankData = SettingManager.Instance.GetString("RankData", "");
            string[] ranks = rankData.Split('_');
            return ranks;
        }
        
    }
}