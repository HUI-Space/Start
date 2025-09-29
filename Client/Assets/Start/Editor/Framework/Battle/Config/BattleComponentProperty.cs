using System;
using System.Collections.Generic;

namespace Start.Editor
{
    /// <summary>
    /// 动态属性
    /// </summary>
    [Serializable]
    public class BattleComponentProperty
    {
        /// <summary>
        /// 属性名
        /// </summary>
        public string PropertyName;

        /// <summary>
        /// 属性类型
        /// </summary>
        public string PropertyType;
            
        /// <summary>
        /// 属性默认值
        /// </summary>
        public string PropertyDefaultValue;

        /// <summary>
        /// 属性注释
        /// </summary>
        public string PropertyComment;

        /// <summary>
        /// 属性特性
        /// </summary>
        public List<string> PropertyAttributes = new List<string>();
    }
}