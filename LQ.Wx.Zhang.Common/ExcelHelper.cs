//using System;
//using System.IO;
//using System.Data;
//using System.Collections;
//using NPOI.SS.UserModel;
//using NPOI.HSSF.UserModel;
//using NPOI.XSSF.UserModel;
//using ServiceStack.Common.Extensions;
//using System.Collections.Generic;
//using System.Linq;

namespace AiBi.Test.Common
{
    /// <summary>
    /// Excel 编辑器
    /// </summary>
    //public class ExcelHelper
    //{
    //    /// <summary>
    //    /// 设置excel文件
    //    /// </summary>
    //    /// <param name="stream">文件流</param>
    //    /// <param name="streamType">xls xlsx</param>
    //    /// <param name="sheet">sheet号</param>
    //    /// <param name="rowid">行号</param>
    //    /// <param name="dt">数据</param>
    //    public static void SetExcelFromDataTable(ref Stream stream,string streamType, int sheet, int rowid,int columnid, DataTable dt) { 
    //        if(dt==null || dt.Rows.Count<=0 || dt.Columns.Count <= 0 || stream==null)
    //        {
    //            return;
    //        }
    //        stream.Position = 0;
    //        IWorkbook wb = null;
    //        if (streamType == "xls")
    //        {
    //            wb = new HSSFWorkbook(stream);
    //        }
    //        else
    //        {
    //            wb = new XSSFWorkbook(stream);
    //        }
            
    //        var sheetBook = wb.GetSheetAt(sheet);
    //        for(int i = 0; i < dt.Rows.Count; i++)
    //        {
    //            var row = dt.Rows[i];
    //            var xrow = sheetBook.GetRow(rowid+i);
    //            if (xrow == null)
    //            {
    //                xrow = sheetBook.CreateRow(rowid + i);
    //            }
    //            for(int j = 0; j < row.ItemArray.Length; j++)
    //            {
    //                ICell cell = null;
    //                if(xrow.Cells.Count<= j + columnid)
    //                {
    //                    cell = xrow.CreateCell(j + columnid);
    //                }
    //                else
    //                {
    //                    cell = xrow.Cells[j + columnid];
    //                }
    //                var val = row[j];
    //                if (row[j] == DBNull.Value)
    //                {
    //                    cell.SetCellValue("");
    //                    continue;
    //                }
    //                if (IsNumberic(dt.Columns[j].DataType))
    //                {
    //                    cell.SetCellValue(double.Parse(val+""));
    //                    continue;
    //                }
    //                if (IsBool(dt.Columns[j].DataType))
    //                {
    //                    cell.SetCellValue((bool)val);
    //                    continue;
    //                }
    //                if (IsString(dt.Columns[j].DataType))
    //                {
    //                    cell.SetCellValue((string)val);
    //                    continue;
    //                }
    //                if (IsDateTime(dt.Columns[j].DataType))
    //                {
    //                    cell.SetCellValue((DateTime)val);
    //                    continue;
    //                }

                    
    //            }
    //        }

    //        var ms = new MemoryStream();
    //        wb.Write(ms);
    //        ms.Position = 0;
    //        stream = ms;
    //    }

    //    public static DataTable ConvertExcel2DataTable(Stream stream,string streamType,int sheet,int headerLine,int dataLine) { 
    //        var dt = new DataTable();
    //        if(stream==null || !stream.CanSeek || stream.Length <= 0)
    //        {
    //            return dt;
    //        }
    //        IWorkbook wb = null;
    //        if (streamType == "xls")
    //        {
    //            wb= new HSSFWorkbook(stream);
    //        }
    //        else
    //        {
    //            wb= new XSSFWorkbook(stream);
    //        }
    //        var sheetBook = wb.GetSheetAt(sheet);
    //        if (sheetBook == null)
    //        {
    //            return dt;
    //        }
    //        var columnLine = sheetBook.GetRow(headerLine);
    //        if (columnLine==null)
    //        {
    //            return dt;
    //        }
    //        if (columnLine.Cells.Count <= 0)
    //        {
    //            return dt;
    //        }
    //        columnLine.Cells.ForEach(a => { dt.Columns.Add(new DataColumn(a.ToString(), typeof(string))); });

    //        if (sheetBook.LastRowNum < dataLine)
    //        {
    //            return dt;
    //        }
    //        for(int i=dataLine;i<= sheetBook.LastRowNum; i++)
    //        {
    //            var row = dt.NewRow();
    //            var rowx = sheetBook.GetRow(i);

    //            for(int j = 0; j < dt.Columns.Count; j++)
    //            {
    //                row[j] = rowx.Cells.Count<=j?"": rowx.Cells[j].ToString();
    //            }

    //            dt.Rows.Add(row);
    //        }

    //        return dt;
    //    }

    //    static bool IsNumberic(Type t) {
    //        var types = new Type[] {
    //            typeof(byte),
    //            typeof(sbyte),
    //            typeof(short),
    //            typeof(ushort),
    //            typeof(int),
    //            typeof(uint),
    //            typeof(long),
    //            typeof(ulong),
    //            typeof(float),
    //            typeof(double),
    //            typeof(decimal),
    //        };
    //        return types.Any(a=>t.IsSubclassOf(a) || t==a);
    //    }
    //    static bool IsString(Type t)
    //    {
    //        var types = new Type[] {
    //            typeof(string),
    //        };
    //        return types.Any(a => t.IsSubclassOf(a) || t == a);
    //    }
    //    static bool IsBool(Type t)
    //    {
    //        var types = new Type[] {
    //            typeof(bool),
    //        };
    //        return types.Any(a => t.IsSubclassOf(a) || t == a);
    //    }
    //    static bool IsDateTime(Type t)
    //    {
    //        var types = new Type[] {
    //            typeof(DateTime),
    //        };
    //        return types.Any(a => t.IsSubclassOf(a) || t == a);
    //    }
    //}
}
