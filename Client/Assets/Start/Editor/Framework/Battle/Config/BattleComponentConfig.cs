using System;
using System.Collections.Generic;

namespace Start.Editor
{
    /// <summary>
    /// 动态类
    /// </summary>
    [Serializable]
    public class BattleComponentConfig
    {
        public List<BattleComponentClass> DataList = new List<BattleComponentClass>();
    }
}