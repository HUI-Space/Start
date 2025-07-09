using System;
using UnityEngine.Serialization;

namespace Start.Editor
{
    [Serializable]
    public class ExcelConfig
    {
        /// <summary>
        /// Excel路径
        /// </summary>
        public string ExcelPath;
        
        /// <summary>
        /// 输出脚本路径
        /// </summary>
        public string OutputScriptPath;
        
        /// <summary>
        /// 输出配置路径
        /// </summary>
        public string OutputConfigPath;
        
        /// <summary>
        /// Excel起始页
        /// </summary>
        public int ExcelStartSheet;

        /// <summary>
        /// 配置字段类型索引
        /// </summary>
        public int ConfigFieldType;

        /// <summary>
        /// 配置字段名行索引
        /// </summary>
        public int ConfigFieldName;

        /// <summary>
        /// 配置字段描述索引
        /// </summary>
        public int ConfigFieldDescription;
        
        /// <summary>
        /// 配置开始索引
        /// </summary>
        public int ConfigStart;
    }
}