using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using ServiceMonitoringSystem.Common.Attributes;

namespace ServiceMonitoringSystem.Common.Common
{
    public class ExcelHelper
    {
        private static readonly IWorkbook Wb = new HSSFWorkbook();

        private static readonly string Path = AppDomain.CurrentDomain.BaseDirectory +
                                       ConfigurationManager.AppSettings["ExportPath"];

        public static string ExportCustom(IWorkbook wb,string fileName)
        {
            if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);
            using (var file = new FileStream(Path + fileName, FileMode.Create, FileAccess.Write))
            {
                wb.Write(file);
            }
            return Path + fileName;
        }
        public static string Export<T>(List<T> list,string fileName)
        {
            var sheet = Wb.CreateSheet();
            var r = sheet.CreateRow(0);
            var type = typeof(T);
            var properties =
                type.GetProperties().Where(t => t.GetCustomAttribute(typeof(ExcelAttribute), false) != null).ToArray();
            for (var i = 0; i < properties.Length; i++)
            {
                var c = r.CreateCell(i);
                var prop = properties[i].GetCustomAttribute(typeof(DisplayAttribute), false);
                c.SetCellValue(prop!=null ? ((DisplayAttribute) prop).Name : properties[i].Name);
            }
            for (var i = 0; i < list.Count; i++)
            {
                r = sheet.CreateRow(i + 1);
                T model = list[i];
                for (var j = 0; j < properties.Length; j++)
                {
                    var c = r.CreateCell(j);
                    var value = properties[j].GetValue(model);
                    c.SetCellValue(value != null ? value.ToString() : "");
                }
            }
            if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);
            using (var file = new FileStream(Path + fileName, FileMode.Create, FileAccess.Write))
            {
                Wb.Write(file);
            }
            return Path + fileName;
        }
        public static string Export(DataTable dt,string fileName)
        {
            var sheet = Wb.CreateSheet();
            var colCount = dt.Columns.Count;
            var r = sheet.CreateRow(0);
            for (int i = 0; i < colCount; i++)
            {
                var c=r.CreateCell(i);
                c.SetCellValue(dt.Columns[i].ColumnName);
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                r=sheet.CreateRow(i+1);
                for (int j = 0; j < colCount; j++)
                {
                    var c = r.CreateCell(j);
                    c.SetCellValue(dt.Rows[i][j].ToString());
                }
            }
            if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);
            using (var file = new FileStream(Path+fileName, FileMode.Create, FileAccess.Write))
            {
                Wb.Write(file);
            }
            return Path + fileName;
        }
    }
}
