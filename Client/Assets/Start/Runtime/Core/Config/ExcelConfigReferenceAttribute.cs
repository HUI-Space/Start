using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 引用Excel中的字段，目前只支持基本的int,bool,float,string四个类型
/// 无法引用Excel中的枚举和List
/// </summary>
public class ExcelConfigReferenceAttribute : PropertyAttribute
{
    public string excelName;
    public string fieldName;
    public string descFieldName;
    public long idStart;
    public long idEnd;
    /// <summary>
    /// 引用Excel数据的字段
    /// </summary>
    /// <param name="excelName">Excel名称</param>
    /// <param name="fieldName">Excel中某列数据名称</param>
    public ExcelConfigReferenceAttribute(string excelName,string fieldName)
    {
        this.excelName = excelName;
        this.fieldName = fieldName;
        this.descFieldName = null;
        this.idStart = -1;
        this.idEnd = long.MaxValue;
    }

    /// <summary>
    /// 引用Excel数据的字段
    /// </summary>
    /// <param name="excelName">Excel名称</param>
    /// <param name="fieldName">Excel中某列数据名称</param>
    /// <param name="descName">Excel中某种描述性数据名称,有就填没有就空着</param>
    /// <param name="idStart">起始ID,不为-1时，开始按范围筛选</param>
    /// <param name="idEnd">截止ID</param>
    public ExcelConfigReferenceAttribute(string excelName, string fieldName, string descName, long idStart=-1, long idEnd=long.MaxValue)
    {
        this.excelName = excelName;
        this.fieldName = fieldName;
        this.descFieldName = descName;
        this.idStart = idStart;
        this.idEnd = idEnd;
    }
}