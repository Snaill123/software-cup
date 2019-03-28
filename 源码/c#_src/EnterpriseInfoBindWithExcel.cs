using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OcrTesseract
{
    class EnterpriseInfoBindWithExcel:List<EnterpriseInfo>
    {
        /*
        // excel 文件地址
        private string highConfidenceLevel = null;
        private string lowConfidenceLevel = null;

        private ExcelTool highConfidenceExcle;
        private ExcelTool lowConfidenceExcle;

        private List<EnterpriseInfo> highConfidenceList;
        private List<EnterpriseInfo> lowConfidenceList;

        public string LowConfidenceLevel { get => lowConfidenceLevel; set => lowConfidenceLevel = value; }
        public string HighConfidenceLevel { get => highConfidenceLevel; set => highConfidenceLevel = value; }

        internal List<EnterpriseInfo> HighConfidenceList { get => highConfidenceList; }
        internal List<EnterpriseInfo> LowConfidenceList { get => lowConfidenceList; }

        public EnterpriseInfoBindWithExcel(string highConfidenceLevel, string lowConfidenceLevel)
        {
            this.highConfidenceLevel = highConfidenceLevel;
            this.lowConfidenceLevel = lowConfidenceLevel;

            highConfidenceExcle = new ExcelTool();
            highConfidenceExcle.Open(HighConfidenceLevel);
            highConfidenceExcle.GetSheetData();
        }
        */
        // excel 文件地址
        private string excelFile = null;
        public string ExcelFile { get => excelFile; set => excelFile = value; }

        // 临时的 excel 文件地址
        private string tempExcel = null;

        // excel 文件访问工具
        private ExcelTool excelTool;

        // excel数据集
        private List<EnterpriseInfo> enterpriseInfoList;
        internal List<EnterpriseInfo> EnterpriseInfoList { get => enterpriseInfoList; }



        // excel数据行数
        private int recordCount;
        public int RecordCount { get => recordCount; set => recordCount = value; }
        /*
        // 总页数
        private int pageCount;
        public int PageCount { get => pageCount; set => pageCount = value; }

        // 每页记录数
        private int pageSize;
        public int PageSize { get => pageSize; set => pageSize = value; }
        */
        
        public EnterpriseInfoBindWithExcel()
        {
        }

        public EnterpriseInfoBindWithExcel(string excelFile)
        {
            this.excelFile = excelFile;            

            if (!this.HasTempExcel())
            {
                if (this.HasTempExcelInFilePath(excelFile))
                {
                  //  Console.WriteLine("///////////////////////////has temp excel");

                    this.tempExcel = this.GetTempExcelInFilePath(excelFile);
                }
                else
                {
                   // Console.WriteLine("////////////////////////has not temp excel");

                    string tempFileName = "_temp_" + Path.GetFileName(excelFile);
                    string path = Path.GetDirectoryName(excelFile);

                    this.tempExcel = path + '\\' + tempFileName;

                    File.Copy(this.excelFile, this.tempExcel);
                }
            }

            //Console.WriteLine("excel temp path:" + this.tempExcel);

            excelTool = new ExcelTool();
            excelTool.Open(this.tempExcel);
            enterpriseInfoList = excelTool.GetSheetData();
            this.recordCount = enterpriseInfoList.Count;  // 记录excel元素总量
            
            foreach(EnterpriseInfo info in enterpriseInfoList)
            {
                Console.WriteLine(info.Name);
                info.EnterpriseInfoChanged += new EnterpriseInfo.EnterpriseInfoChangedHandler(this.EnterpriseInfoChanged);
                info.EnterpriseInfoRemoved += new EnterpriseInfo.EnterpriseInfoRemovedHandler(this.EnterpriseInfoRemoved);
            }
        }

        private string GetTempExcelInFilePath(string excelFile)
        {
            string orignFileName = Path.GetFileName(excelFile);
            string path = Path.GetDirectoryName(excelFile);
            string[] files = Directory.GetFiles(path);

            foreach (string f in files)
            {
                if (Path.GetFileName(f).Remove(0, 6).Equals(orignFileName))
                {
                    return f;                    
                }
            }
            return null;
        }

        private bool HasTempExcelInFilePath(string excelFile)
        {
            if (this.GetTempExcelInFilePath(excelFile) != null)
            {
                return true;
            }
            return false;
        }

        private bool HasTempExcel()
        {
            if (this.tempExcel != null && this.tempExcel != "")
            {
                return true; 
            }
            return false;
        }

        // 修改excel相应位置的数据
        private void EnterpriseInfoChanged(EnterpriseInfo enterpriseInfo, string changedAttr, string value, int row)
        {
            switch (changedAttr)
            {
                case "Name":
                    excelTool.SetCellValue(row, 2, enterpriseInfo.Name);
                    break;
                case "RegistrationNo":
                    excelTool.SetCellValue(row, 1, enterpriseInfo.RegistrationNo);
                    break;
            }
        }

        // 删除excel的指定一行的数据
        private void EnterpriseInfoRemoved(EnterpriseInfo enterpriseInfo, int row)
        {
            // 总数据量减1
            this.recordCount--;

            // 删除缓冲中的企业信息
            this.enterpriseInfoList.Remove(enterpriseInfo);

            // excel企业信息
            excelTool.RemoveRow(row);
        }

        public void Close()
        {
            excelTool.Close();
        }
        

        // 获取页数为 page, 每页数量为 pageCount 的excel数据
        public List<EnterpriseInfo> GetEnterpriseInfosAtPage(int page, int pageSize)
        {
            List<EnterpriseInfo> enterpriseInfos = null;
            EnterpriseInfo[] arrayEnterpriseInfos;

            if (page < 1)
            {
                page = 1;
            }

            // 总页数
            int pageCount = (int)Math.Ceiling((double)this.recordCount / pageSize);
            if (page > pageCount)
            {
                page = pageCount;
            }

            int beginIndex = (page - 1) * pageSize;
            int count = page * pageSize > this.recordCount ? this.recordCount - beginIndex : page * pageSize - beginIndex;

            arrayEnterpriseInfos = new EnterpriseInfo[count];
            this.enterpriseInfoList.CopyTo(beginIndex, arrayEnterpriseInfos, 0, count);

            enterpriseInfos = new List<EnterpriseInfo>(arrayEnterpriseInfos);
            return enterpriseInfos;
        }

        // 将所有企业信息导入excel中
        public void ExportEnterpriseInfos(List<EnterpriseInfo> enterpriseInfos)
        {

        }

    }
}
