using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OcrTesseract
{
    class EnterpriseInfo
    {
        private int row; // 企业信息所在excel表格的行 
        private string name;  // 企业名称
        private string registrationNo;  // 企业注册号
        private string enterpriseConfidence; // 企业名称置信度
        private string registrationNoConfidence; // 企业注册号置信度
        private string businessLicenseFile; // 营业执照图片位置

        // 修改
        // 委托
        public delegate void EnterpriseInfoChangedHandler(EnterpriseInfo enterpriseInfo, string changedAttr, string value, int row);
        // 事件
        public event EnterpriseInfoChangedHandler EnterpriseInfoChanged = null;

        // 删除
        // 委托
        public delegate void EnterpriseInfoRemovedHandler(EnterpriseInfo enterpriseInfo, int row);
        // 事件
        public event EnterpriseInfoRemovedHandler EnterpriseInfoRemoved = null;

        // 前台界面表单的显示顺序是通过构造函数确定的
        public EnterpriseInfo(int row, string registrationNo, string name, string enterpriseConfidence, string registrationNoConfidence, string businessLicenseFile)
        {
            this.row = row;
            this.name = name;
            this.registrationNo = registrationNo;
            this.enterpriseConfidence = enterpriseConfidence;
            this.registrationNoConfidence = registrationNoConfidence;
            this.businessLicenseFile = businessLicenseFile;
        }

        // 将此企业信息从excel中移除
        public void Remove()
        {
            Console.WriteLine("企业信息被删除");
            if (EnterpriseInfoRemoved != null)
            {
                EnterpriseInfoRemoved(this, row);
            }
        }

        public string RegistrationNo
        {
            get => this.registrationNo;
            set
            {
                this.registrationNo = value;
                if (EnterpriseInfoChanged != null)
                {
                    EnterpriseInfoChanged(this, "RegistrationNo", value, row);
                }
            }
        }

        public string Name
        {
            get => this.name;
            set
            {
                this.name = value;
                Console.WriteLine("企业名称被修改");
                if (EnterpriseInfoChanged != null)
                {
                    EnterpriseInfoChanged(this, "Name", value, row);
                }
            }
        }

        public string EnterpriseConfidence
        {
            get => enterpriseConfidence;
            set
            {
                enterpriseConfidence = value;
            }
        }

        public string RegistrationNoConfidence
        {
            get => registrationNoConfidence;
            set
            {
                registrationNoConfidence = value;
            }
        }

        public string BusinessLicenseFile
        {
            get => businessLicenseFile;
            set
            {
                businessLicenseFile = value;
            }
        }
    }
}
