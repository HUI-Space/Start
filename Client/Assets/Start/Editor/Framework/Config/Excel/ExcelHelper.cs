using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;
using UnityEngine;

namespace Start.Editor
{
    public static class ExcelHelper
    {
        public static List<string> ExcelExtension = new List<string>()
        {
            ".xlsx",
            ".xls",
            ".csv",
        };

        /// <summary>
        /// 获取所有Excel
        /// </summary>
        /// <returns></returns>
        public static List<string> GetExcels(string excelPath)
        {
            List<string> excels = new List<string>();
            if (!Directory.Exists(excelPath))
            {
                return excels;
            }
            DirectoryInfo dir = new DirectoryInfo(excelPath);
            foreach (var file in dir.GetFiles("*", SearchOption.AllDirectories))
            {
                if (!file.Name.StartsWith("~$") && ExcelExtension.Contains(file.Extension))
                {
                    excels.Add(file.FullName);
                }
            }
            return excels;
        }

        /// <summary>
        /// 通过路径获取Excel所有Sheet名称
        /// </summary>
        /// <param name="excelPath">路径</param>
        /// <returns></returns>
        public static List<string> GetSheetNames(string excelPath)
        {
            List<string> sheets = new List<string>();
            if (!File.Exists(excelPath))
            {
                return sheets;
            }
            FileStream stream = new FileStream(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);
            DataSet dataSet = reader.AsDataSet();
            stream.Close();
            foreach (DataTable dataTable in dataSet.Tables)
            {
                sheets.Add(dataTable.TableName);
            }
            return sheets;
        }
        
        /// <summary>
        /// 通过Excel路径判断是否存在sheet
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public static bool ContainSheet(string path, string sheet)
        {
            return GetSheetNames(path).Contains(sheet);
        }
        
        /// <summary>
        /// 通过Excel路径获取DataTable
        /// </summary>
        /// <param name="excelPath">路径</param>
        /// <param name="sheet">sheet名称</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string excelPath, string sheet)
        {
            FileStream stream = new FileStream(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);
            DataSet dataSet = reader.AsDataSet();
            stream.Close();
            // 遍历所有Sheet
            foreach (DataTable dataTable in dataSet.Tables)
            {
                if (dataTable.TableName.Equals(sheet))
                {
                    return dataTable;
                }
            }
            return default;
        }
    }
}