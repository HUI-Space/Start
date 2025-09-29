using System;
using System.Collections.Generic;

namespace Start.Editor
{
    /// <summary>
    /// 动态类
    /// </summary>
    [Serializable]
    public class BattleComponentClass
    {
        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName;

        /// <summary>
        /// 类注释
        /// </summary>
        public string ClassComment;

        /// <summary>
        /// 类特性
        /// </summary>
        public List<string> ClassAttributes = new List<string>();

        /// <summary>
        /// 属性列表
        /// </summary>
        public List<BattleComponentProperty> DynamicProperties = new List<BattleComponentProperty>();
    }
}