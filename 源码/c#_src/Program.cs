using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OcrTesseract
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new TestForm());

            Application.Run(new Form1());

            //Application.Run(new NewTaskForm());
            /*ExcelTool excelTool = new ExcelTool();
            excelTool.Open(@"E:\Visual Studio\source\repos\OcrTesseract\OcrTesseract\Resources\result\test2.xlsx");
            excelTool.GetSheetData();
            //excelTool.SetCellValue(4, 5, "E:\\test2");
            //excelTool.Save();
            excelTool.Close();*/
        }
    }
}
