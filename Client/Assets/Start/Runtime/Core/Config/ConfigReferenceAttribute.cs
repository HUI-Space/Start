using UnityEngine;

namespace Start
{
    public class ConfigReferenceAttribute : PropertyAttribute
    {
        /// <summary>
        /// 配置表名称
        /// </summary>
        public string ConfigName;

        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName;

        /// <summary>
        /// 字段描述
        /// </summary>
        public string FieldNameDescription;

        /// <summary>
        /// 配置表起始Id
        /// </summary>
        public long StartId;
        
        /// <summary>
        /// 配置表结束Id
        /// </summary>
        public long EndId;
        
        public ConfigReferenceAttribute(string configName,string fieldName)
        {
            ConfigName = configName;
            FieldName = fieldName;
            FieldNameDescription = default;
            StartId = -1;
            EndId = long.MaxValue;
        }
        
        
        public ConfigReferenceAttribute(string configName, string fieldName, string fieldNameDescription, long startId=-1, long endId=long.MaxValue)
        {
            ConfigName = configName;
            FieldName = fieldName;
            FieldNameDescription = fieldNameDescription;
            StartId = startId;
            EndId = endId;
        }
    }
}