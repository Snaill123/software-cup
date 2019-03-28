using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Configuration;
using System.Web;
using Microsoft.Office.Interop;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using System.IO;

namespace OcrTesseract
{
    class ExcelTool
    {
        public string mFilename;
        private string tempFile;

        private List<int> removedRows = new List<int>(); // 需要被删除的行

        public Microsoft.Office.Interop.Excel.Application app;
        public Microsoft.Office.Interop.Excel.Workbooks wbs;
        public Microsoft.Office.Interop.Excel.Workbook wb;
        public Microsoft.Office.Interop.Excel.Worksheets wss;
        public Microsoft.Office.Interop.Excel.Worksheet ws;
        public ExcelTool()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        //创建一个Microsoft.Office.Interop.Excel对象
        public void Create()
        {
            app = new Microsoft.Office.Interop.Excel.Application();
            wbs = app.Workbooks;
            wb = wbs.Add(true);
        }

        //打开一个Microsoft.Office.Interop.Excel文件
        public void Open(string FileName)
        {
            app = new Microsoft.Office.Interop.Excel.Application();
            wbs = app.Workbooks;
            // wb = wbs.Add(FileName);
            wb = wbs.Open(FileName);

            //wss = (Microsoft.Office.Interop.Excel.Worksheets)wb.Worksheets;
            ws = wb.Worksheets[1];
            
            //wb = wbs.Open(FileName, 0, true, 5,"", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "t", false, false, 0, true,Type.Missing,Type.Missing);
            //wb = wbs.Open(FileName,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Microsoft.Office.Interop.Excel.XlPlatform.xlWindows,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing);
            mFilename = FileName;
        }

        //获取一个工作表
        public Microsoft.Office.Interop.Excel.Worksheet GetSheet(string SheetName)        
        {
            Microsoft.Office.Interop.Excel.Worksheet s = (Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets[SheetName];
            return s;
        }

        /// <summary>
        /// 修改第x行，y列的单元格
        /// </summary>
        public void SetCellValue(int x, int y, object value)
        {
            // 单元格自动调整列宽
            ws.Columns.AutoFit();
            // 单元格格式为文本
            ws.Columns.NumberFormatLocal = "@";

            if (value is string)
            {
                ws.Cells[x, y] = (string)value;
            }
            else
            {
                ws.Cells[x, y] = value;
            }
            this.Save();
            Console.WriteLine("单元格（" + x + "," + y + "）的数据已修改为：" + ws.Cells[x, y].Text);
        }

        /// <summary>
        /// 删除第row行的一行数据
        /// </summary>
        public void RemoveRow(int row)
        {
            this.removedRows.Add(row); 
        }

        /// <summary>
        /// 删除被记录的需要删除的所有行
        /// </summary>
        private void RemoveRowsBeforeClose()
        {
            int rowsCount = this.removedRows.Count;
            if (rowsCount > 0)
            {
                this.removedRows.Sort((a, b) => b - a);  // 按从大到小的顺序排序

                for (int i = 0; i < rowsCount; i++)
                {
                    // 删除整行
                    int row = this.removedRows[i];
                    ws.Rows[row].Delete(Microsoft.Office.Interop.Excel.XlDirection.xlDown);
                }
                this.Save();
            }
        }

        // 获取第一个工作表的所有数据
        public List<EnterpriseInfo> GetSheetData()
        {
            List<EnterpriseInfo> enterpriseInfoList = new List<EnterpriseInfo>();
            EnterpriseInfo enterpriseInfo;

            Range cols = ws.UsedRange.Columns;
            Range rows = ws.UsedRange.Rows;

            for (int i = 2; i < rows.Count; i++)
            {
                /*
                for (int j = 1; j < cols.Count; j++)
                {
                    Console.WriteLine(ws.Cells[i, j].Text);
                }
                */
                enterpriseInfo = new EnterpriseInfo(i, ws.Cells[i, 1].Text, ws.Cells[i, 2].Text, ws.Cells[i, 3].Text, ws.Cells[i, 4].Text, ws.Cells[i, 5].Text);
                enterpriseInfoList.Add(enterpriseInfo);
            }

            return enterpriseInfoList;
        }

        // 向第一个工作表写入所有数据
        public bool WriteSheetData(string excelFile, List<EnterpriseInfo> enterpriseInfos, string sheetName, string[] tableHeader)
        {
            if (sheetName == null || sheetName == "")
            {
                sheetName = "Sheet1";
            }

            // 创建sheet
            this.Create();
            this.ws = wb.Sheets.Add();
            ws.Name = sheetName;

            // 单元格自动调整列宽
            ws.Columns.AutoFit(); // 没用
            // 可尝试 ws.ColumnWidth=15;     //设置单元格的宽度
            // 单元格格式为文本
            ws.Columns.NumberFormatLocal = "@";

            // 写入表头
            for (int i = 0; i < tableHeader.Length; i++)
            {
                ws.Cells[1, i + 1] = tableHeader[i];
            }

            // 写入数据
            for (int i = 0; i < enterpriseInfos.Count; i++)
            {
                ws.Cells[i + 2, 1] = enterpriseInfos[i].RegistrationNo;
                ws.Cells[i + 2, 2] = enterpriseInfos[i].Name;
            }

            return this.SaveAs(excelFile);
        }

        // 向第一个工作表结尾添加数据
        public void AppendSheetData(string excelFile, List<EnterpriseInfo> enterpriseInfos, int fromRowIndex)//(string excelFile, string sheetName)
        {
            // 写入数据
            for (int i = 0; i < enterpriseInfos.Count; i++)
            {
                ws.Cells[fromRowIndex, 1] = enterpriseInfos[i].RegistrationNo;
                ws.Cells[fromRowIndex, 2] = enterpriseInfos[i].Name;
                fromRowIndex++;
            }

            this.Save();
        }

        //添加一个工作表
        public Microsoft.Office.Interop.Excel.Worksheet AddSheet(string SheetName)        
        {
            Microsoft.Office.Interop.Excel.Worksheet s = (Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            s.Name = SheetName;
            return s;
        }

        //删除一个工作表
        public void DelSheet(string SheetName)
        {
            ((Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets[SheetName]).Delete();
        }
        public Microsoft.Office.Interop.Excel.Worksheet ReNameSheet(string OldSheetName, string NewSheetName)//重命名一个工作表一
        {
            Microsoft.Office.Interop.Excel.Worksheet s = (Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets[OldSheetName];
            s.Name = NewSheetName;
            return s;
        }

        //重命名一个工作表二
        public Microsoft.Office.Interop.Excel.Worksheet ReNameSheet(Microsoft.Office.Interop.Excel.Worksheet Sheet, string NewSheetName)
        {
            Sheet.Name = NewSheetName;
            return Sheet;
        }

        //ws：要设值的工作表     X行Y列     value   值
        public void SetCellValue(Microsoft.Office.Interop.Excel.Worksheet ws, int x, int y, object value)        
        {
            ws.Cells[x, y] = value;
        }

        //ws：要设值的工作表的名称 X行Y列 value 值
        public void SetCellValue(string ws, int x, int y, object value)        
        {
            GetSheet(ws).Cells[x, y] = value;
        }

        //设置一个单元格的属性   字体，   大小，颜色   ，对齐方式
        public void SetCellProperty(Microsoft.Office.Interop.Excel.Worksheet ws, int Startx, int Starty, int Endx, int Endy, int size, string name, Microsoft.Office.Interop.Excel.Constants color, Microsoft.Office.Interop.Excel.Constants HorizontalAlignment)        
        {
            name = "宋体";
            size = 12;
            color = Microsoft.Office.Interop.Excel.Constants.xlAutomatic;
            HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight;
            ws.get_Range(ws.Cells[Startx, Starty], ws.Cells[Endx, Endy]).Font.Name = name;
            ws.get_Range(ws.Cells[Startx, Starty], ws.Cells[Endx, Endy]).Font.Size = size;
            ws.get_Range(ws.Cells[Startx, Starty], ws.Cells[Endx, Endy]).Font.Color = color;
            ws.get_Range(ws.Cells[Startx, Starty], ws.Cells[Endx, Endy]).HorizontalAlignment = HorizontalAlignment;
        }

        public void SetCellProperty(string wsn, int Startx, int Starty, int Endx, int Endy, int size, string name, Microsoft.Office.Interop.Excel.Constants color, Microsoft.Office.Interop.Excel.Constants HorizontalAlignment)
        {
            //name = "宋体";
            //size = 12;
            //color = Microsoft.Office.Interop.Excel.Constants.xlAutomatic;
            //HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlRight;

            Microsoft.Office.Interop.Excel.Worksheet ws = GetSheet(wsn);
            ws.get_Range(ws.Cells[Startx, Starty], ws.Cells[Endx, Endy]).Font.Name = name;
            ws.get_Range(ws.Cells[Startx, Starty], ws.Cells[Endx, Endy]).Font.Size = size;
            ws.get_Range(ws.Cells[Startx, Starty], ws.Cells[Endx, Endy]).Font.Color = color;

            ws.get_Range(ws.Cells[Startx, Starty], ws.Cells[Endx, Endy]).HorizontalAlignment = HorizontalAlignment;
        }

        //合并单元格
        public void UniteCells(Microsoft.Office.Interop.Excel.Worksheet ws, int x1, int y1, int x2, int y2)        
        {
            ws.get_Range(ws.Cells[x1, y1], ws.Cells[x2, y2]).Merge(Type.Missing);
        }

        //合并单元格
        public void UniteCells(string ws, int x1, int y1, int x2, int y2)        
        {
            GetSheet(ws).get_Range(GetSheet(ws).Cells[x1, y1], GetSheet(ws).Cells[x2, y2]).Merge(Type.Missing);
        }

        //将内存中数据表格插入到Microsoft.Office.Interop.Excel指定工作表的指定位置 为在使用模板时控制格式时使用一
        public void InsertTable(System.Data.DataTable dt, string ws, int startX, int startY)        
        {
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                for (int j = 0; j <= dt.Columns.Count - 1; j++)
                {
                    GetSheet(ws).Cells[startX + i, j + startY] = dt.Rows[i][j].ToString();
                }
            }
        }

        //将内存中数据表格插入到Microsoft.Office.Interop.Excel指定工作表的指定位置二
        public void InsertTable(System.Data.DataTable dt, Microsoft.Office.Interop.Excel.Worksheet ws, int startX, int startY)        
        {
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                for (int j = 0; j <= dt.Columns.Count - 1; j++)
                {
                    ws.Cells[startX + i, j + startY] = dt.Rows[i][j];
                }
            }
        }

        //将内存中数据表格添加到Microsoft.Office.Interop.Excel指定工作表的指定位置一
        public void AddTable(System.Data.DataTable dt, string ws, int startX, int startY)        
        {
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                for (int j = 0; j <= dt.Columns.Count - 1; j++)
                {
                    GetSheet(ws).Cells[i + startX, j + startY] = dt.Rows[i][j];
                }
            }
        }

        //将内存中数据表格添加到Microsoft.Office.Interop.Excel指定工作表的指定位置二
        public void AddTable(System.Data.DataTable dt, Microsoft.Office.Interop.Excel.Worksheet ws, int startX, int startY)
        {
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                for (int j = 0; j <= dt.Columns.Count - 1; j++)
                {
                    ws.Cells[i + startX, j + startY] = dt.Rows[i][j];
                }
            }
        }

        //插入图片操作一
        public void InsertPictures(string Filename, string ws)        
        {
            GetSheet(ws).Shapes.AddPicture(Filename, MsoTriState.msoFalse, MsoTriState.msoTrue, 10, 10, 150, 150);
            //后面的数字表示位置
        }

        //public void InsertPictures(string Filename, string ws, int Height, int Width)
        //插入图片操作二
        //{
        //    GetSheet(ws).Shapes.AddPicture(Filename, MsoTriState.msoFalse, MsoTriState.msoTrue, 10, 10, 150, 150);
        //    GetSheet(ws).Shapes.get_Range(Type.Missing).Height = Height;
        //    GetSheet(ws).Shapes.get_Range(Type.Missing).Width = Width;
        //}
        //public void InsertPictures(string Filename, string ws, int left, int top, int Height, int Width)
        //插入图片操作三
        //{

        //    GetSheet(ws).Shapes.AddPicture(Filename, MsoTriState.msoFalse, MsoTriState.msoTrue, 10, 10, 150, 150);
        //    GetSheet(ws).Shapes.get_Range(Type.Missing).IncrementLeft(left);
        //    GetSheet(ws).Shapes.get_Range(Type.Missing).IncrementTop(top);
        //    GetSheet(ws).Shapes.get_Range(Type.Missing).Height = Height;
        //    GetSheet(ws).Shapes.get_Range(Type.Missing).Width = Width;
        //}

        //插入图表操作
        public void InsertActiveChart(Microsoft.Office.Interop.Excel.XlChartType ChartType, string ws, int DataSourcesX1, int DataSourcesY1, int DataSourcesX2, int DataSourcesY2, Microsoft.Office.Interop.Excel.XlRowCol ChartDataType)
        {
            ChartDataType = Microsoft.Office.Interop.Excel.XlRowCol.xlColumns;
            wb.Charts.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            {
                wb.ActiveChart.ChartType = ChartType;
                wb.ActiveChart.SetSourceData(GetSheet(ws).get_Range(GetSheet(ws).Cells[DataSourcesX1, DataSourcesY1], GetSheet(ws).Cells[DataSourcesX2, DataSourcesY2]), ChartDataType);
                wb.ActiveChart.Location(Microsoft.Office.Interop.Excel.XlChartLocation.xlLocationAsObject, ws);
            }
        }

        //保存文档
        public bool Save()        
        {
            if (mFilename == "")
            {
                return false;
            }
            else
            {
                try
                {
                    wb.Save();
                    /*
                    if (this.tempFile == null || this.tempFile == "")
                    {                       
                        string templetFile = @"E:\Visual Studio\source\repos\OcrTesseract\OcrTesseract\Resources\result\templet.xlsx";

                        string tempFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
                        string tempPath = Path.GetTempPath();
                        string tempExcel = tempPath + "\\" + tempFileName + Path.GetExtension(templetFile);
                        File.Copy(templetFile, tempExcel);

                        this.tempFile = tempExcel;
                    }
                    */
                    //屏蔽掉系统跳出的Alert
                    //app.AlertBeforeOverwriting = false;
                    //wb.SaveAs(this.tempFile, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    return false;
                }
            }
        }

        //文档另存为
        public bool SaveAs(object FileName)        
        {
            try
            {
                wb.SaveAs(FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //关闭一个Microsoft.Office.Interop.Excel对象，销毁对象
        public void Close()        
        {
            // 关闭文档前先删除删除项
            this.RemoveRowsBeforeClose();

            //wb.Save();
            wb.Close(Type.Missing, Type.Missing, Type.Missing);
            wbs.Close();
            app.Quit();
            wb = null;
            wbs = null;
            app = null;
            GC.Collect();
        }

    }
}
