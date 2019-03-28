using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using Microsoft.Win32;

namespace OcrTesseract
{
    public partial class TaskDetailControl : UserControl
    {
        // 委托，用于不同线程间同步更新TaskItemControl控件内容
        delegate void TaskItemChangedCallback(TaskItem taskItem);

        private TaskItem taskItem;
        public TaskItem TaskItem
        {
            get => this.taskItem;
        }

        // 与高置信度识别结果（excel文件）的绑定
        private EnterpriseInfoBindWithExcel enterpriseInfoBindWithExcel;
        // 与低置信度识别结果（excel文件）的绑定
        private EnterpriseInfoBindWithExcel enterpriseInfoBindWithExcel2;

        // 当前显示的置信度高的数据集
        private List<EnterpriseInfo> enterpriseInfoList1 = null;
        // 当前显示的置信度低的数据集
        private List<EnterpriseInfo> enterpriseInfoList2 = null;

        // DataGridView 翻页属性
        // dataGridView1
        private int recordCount1; //总记录数
        private int pageCount1;  //  总页数
        private int pageSize1 = 20;  //每页记录数        
        private int currentPage1 = 1;  // 当前页
        // dataGridView2
        private int recordCount2; //总记录数
        private int pageCount2;  //  总页数
        private int pageSize2 = 10;  //每页记录数        
        private int currentPage2 = 1;  // 当前页

        public TaskDetailControl()
        {
            InitializeComponent();
        }

        //// 判断用户是否安装了office
        //private bool IsInstalledOffice()
        //{
        //    // 通过读取注册表判断是否安装了office
        //    RegistryKey rk = Registry.LocalMachine;

        //    RegistryKey office2000 = rk.OpenSubKey(@"SOFTWARE\Microsoft\Office\9.0\Excel\InstallRoot\");
        //    RegistryKey office2003 = rk.OpenSubKey(@"SOFTWARE\Microsoft\Office\11.0\Excel\InstallRoot\");
        //    RegistryKey office2007 = rk.OpenSubKey(@"SOFTWARE\Microsoft\Office\12.0\Excel\InstallRoot\");
        //    RegistryKey office2010 = rk.OpenSubKey(@"SOFTWARE\Microsoft\Office\14.0\Excel\InstallRoot\");
        //    RegistryKey office2016 = rk.OpenSubKey(@"SOFTWARE\Microsoft\Office\16.0\Excel\InstallRoot\");
        //    //RegistryKey office = rk.OpenSubKey(@"SOFTWARE\Microsoft\Office\*\Excel\InstallRoot\");

        //    return office2000 != null || office2003 != null || office2007 != null || office2010 != null || office2016 != null;
        //}

        public TaskDetailControl(TaskItem taskItem) : this()
        {
            this.taskItem = taskItem;

            // 判断用户是否安装了office
            //if (!IsInstalledOffice())
            //{
            //    MessageBox.Show("查看结果需要本地计算机安装相应版本的office软件，请安装后重试！", "需要office支持", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    // 返回之前的视图
            //    this.label1_Click(this.label1, new EventArgs());  // 在label1_Click方法中报parentForm为null错误
            //    return;
            //}

            // 添加taskItem更新响应事件
            this.taskItem.TaskItemChanged += new TaskItem.TaskItemChangedHandler(this.ShowUpdatedTaskInfo);

            // 显示任务详情信息
            // 初始化任务详情
            ShowTaskInfo();

            // 显示识别结果（excel列表）
            if (this.taskItem.Status.Equals("已完成"))
            {
                this.label29.Visible = false;
                // 显示任务识别结果
                ShowTaskResult();
            }
            else
            {
                this.dataGridView1.Visible = false;
                this.dataGridView2.Visible = false;

                this.label19.Visible = false;
                this.label20.Visible = false;

                this.label28.Visible = false;
                this.label27.Visible = false;
                this.label23.Visible = false;

                this.label25.Visible = false;
                this.label26.Visible = false;
                this.label24.Visible = false;

                this.button1.Visible = false;
                this.button2.Visible = false;
                this.button3.Visible = false;
                this.button4.Visible = false;

                this.comboBox1.Visible = false;
                this.comboBox2.Visible = false;

                this.label31.Visible = false;
                this.button5.Visible = false;
            }
        }

        // 显示任务详情信息
        public void ShowTaskInfo()
        {
            ShowUpdatedTaskInfo(this.taskItem);
        }

        // 收到更新事件时，显示更新的任务信息
        private void ShowUpdatedTaskInfo(TaskItem taskItem)
        {
            if (this.InvokeRequired)
            {
                TaskItemChangedCallback d = new TaskItemChangedCallback(this.ShowUpdatedTaskInfo);
                this.Invoke(d, taskItem);
            }
            else
            {
                // 初始化任务详情
                this.label4.Text = taskItem.Name.Length > 15 ? taskItem.Name.Substring(0, 15) + "...." : taskItem.Name;
                this.label5.Text = taskItem.pictureCount.ToString();
                this.linkLabel1.Text = taskItem.picturePath.Length > 15 ? taskItem.picturePath.Substring(0, 15) + "...." : taskItem.picturePath;
                this.label9.Text = taskItem.Status;
                this.label7.Text = taskItem.pictureFinishedCount.ToString();
                this.label12.Text = taskItem.createTime;
                this.label16.Text = taskItem.progress + "%";  // taskItem.pictureFinishedCount / taskItem.pictureCount
                this.linkLabel2.Text = taskItem.resultExcel.Length > 15 ? taskItem.resultExcel.Substring(0, 15) + "...." : taskItem.resultExcel;
            }
        }

        // 显示任务识别结果
        public void ShowTaskResult()
        {
            dataGridView1.DataSource = new EnterpriseInfoBindWithExcel(); //每次打开清空内容            
            //dataGridView2.DataSource = null; //每次打开清空内容
            dataGridView2.DataSource = new EnterpriseInfoBindWithExcel();

            //DataTable dt = GetDataFromExcel().Tables[0];

            // 设置datagGridView1的数据表
            //enterpriseInfoBindWithExcel = new EnterpriseInfoBindWithExcel(@"E:\Visual Studio\source\repos\OcrTesseract\OcrTesseract\Resources\result\test2.xlsx");
            //dataGridView1.DataSource = enterpriseInfoBindWithExcel.EnterpriseInfoList;
            enterpriseInfoBindWithExcel = new EnterpriseInfoBindWithExcel(this.taskItem.taskPath + "\\result\\high_confidence_excel.xls");
            enterpriseInfoBindWithExcel2 = new EnterpriseInfoBindWithExcel(this.taskItem.taskPath + "\\result\\low_confidence_excel.xls");

            this.recordCount1 = this.enterpriseInfoBindWithExcel.RecordCount;
            this.pageCount1 = (int)Math.Ceiling((double)this.recordCount1 / this.pageSize1);
            this.comboBox1.DataSource = this.GetComboBoxDataSource(this.pageCount1);
            this.label23.Text = "共 " + this.pageCount1 + " 页";

            this.recordCount2 = this.enterpriseInfoBindWithExcel2.RecordCount;
            this.pageCount2 = (int)Math.Ceiling((double)this.recordCount2 / this.pageSize2);
            this.comboBox2.DataSource = this.GetComboBoxDataSource(this.pageCount2);
            this.label24.Text = "共 " + this.pageCount2 + " 页";

            this.enterpriseInfoList1 = this.GetEnterpriseInfosByPage(this.enterpriseInfoBindWithExcel, this.currentPage1, this.pageSize1);
            this.enterpriseInfoList2 = this.GetEnterpriseInfosByPage(this.enterpriseInfoBindWithExcel2, this.currentPage2, this.pageSize2);

            this.dataGridView1.DataSource = this.enterpriseInfoList1;
            this.dataGridView2.DataSource = this.enterpriseInfoList2;
            //dataGridView2.DataSource = dt;
        }


        // 从指定的绑定excel文件中获取一页数据
        private List<EnterpriseInfo> GetEnterpriseInfosByPage(EnterpriseInfoBindWithExcel enterpriseInfoBindWithExcel, int page, int pageSize)
        {
            return enterpriseInfoBindWithExcel.GetEnterpriseInfosAtPage(page, pageSize);
        }

        // 生成ComboBox 组件的数据
        private int[] GetComboBoxDataSource(int pageCount)
        {
            int[] comboBoxDataSource = new int[pageCount];
            for (int i = 0; i < pageCount;)
            {
                comboBoxDataSource[i] = ++i;
            }
            return comboBoxDataSource;
        }

        /// <summary>
        /// 从Excel中读取数据
        /// </summary>
        /// <returns></returns>
        public DataSet GetDataFromExcel()
        {
            string filePath = @"E:\Visual Studio\source\repos\OcrTesseract\OcrTesseract\Resources\result\test2.xlsx";
            //判断文件后缀
            var path = Path.GetFileName(filePath);
            string fileSuffix = System.IO.Path.GetExtension(path);

            if (string.IsNullOrEmpty(fileSuffix))
                return null;

            using (DataSet ds = new DataSet())
            {
                //判断Excel文件是2003版本还是2007版本
                string connString = "";

                if (fileSuffix == ".xls")
                    connString = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filePath + ";" + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
                else
                    connString = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + filePath + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";

                //读取文件
                string sql_select = " SELECT * FROM [Sheet1$]";
                using (OleDbConnection conn = new OleDbConnection(connString))
                using (OleDbDataAdapter cmd = new OleDbDataAdapter(sql_select, conn))
                {
                    conn.Open();
                    cmd.Fill(ds);
                }
                if (ds == null || ds.Tables.Count <= 0) return null;
                return ds;
            }
        }

        // 点击“返回”
        private void label1_Click(object sender, EventArgs e)
        {
            // 关闭资源
            if (this.enterpriseInfoBindWithExcel != null)
            {
                enterpriseInfoBindWithExcel.Close();
                this.enterpriseInfoBindWithExcel = null;
            }
            if (this.enterpriseInfoBindWithExcel2 != null)
            {
                enterpriseInfoBindWithExcel2.Close();
                this.enterpriseInfoBindWithExcel2 = null;
            }

            // 显示任务列表视图
            if (this.taskItem.Status.Contains("已完成"))
            {
                // 显示“已完成”任务列表视图
                Form1 form = (Form1)this.ParentForm;
                LeftMenuControl leftMenuControl = form.GetLeftMenuControl();
                leftMenuControl.label2_Click(this, new EventArgs());
            }
            else
            {
                // 显示“正在识别”任务列表视图
                Form1 form = (Form1)this.ParentForm;
                LeftMenuControl leftMenuControl = form.GetLeftMenuControl();
                leftMenuControl.label1_Click(this, new EventArgs());
                //Control taskListControl = form.GetTaskListControl();
                //form.ShowTaskListControl(taskListControl);
            }
        }

        // 鼠标放置在标签上
        private void label1_MouseHover(object sender, EventArgs e)
        {
            DrawMouseHoverLable(label1, global::OcrTesseract.Properties.Resources.back_blue_1296db);
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            DrawMouseLeaveLable(label1, global::OcrTesseract.Properties.Resources.back);
        }

        private void label2_MouseHover(object sender, EventArgs e)
        {
            DrawMouseHoverLable(label2, global::OcrTesseract.Properties.Resources.wxb工具_blue_1296db);
        }

        private void label2_MouseLeave(object sender, EventArgs e)
        {
            DrawMouseLeaveLable(label2, global::OcrTesseract.Properties.Resources.wxb工具);
        }

        // 绘制MouseHover的Label的ForeColor
        private void DrawMouseHoverLable(Label label, Color color, Image icon)
        {
            label.Image = icon;
            label.ForeColor = color;
        }
        private void DrawMouseHoverLable(Label label, Image icon)
        {
            label.Image = icon;
            label.ForeColor = Color.DodgerBlue;
        }

        // 绘制MouseLeave的Label的ForeColor
        private void DrawMouseLeaveLable(Label label, Color color, Image icon)
        {
            label.Image = icon;
            label.ForeColor = color;
        }
        private void DrawMouseLeaveLable(Label label, Image icon)
        {
            label.Image = icon;
            label.ForeColor = SystemColors.ControlText;
        }

        // 点击dataGridView1 的"上一页"按钮
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.currentPage1 - 1 < 1)
            {
                MessageBox.Show("已经是第一页了！", "页数错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            this.currentPage1--;
            //this.comboBox1.SelectedValue = this.currentPage1;
            this.comboBox1.Text = this.currentPage1.ToString();
            this.enterpriseInfoList1 = this.GetEnterpriseInfosByPage(this.enterpriseInfoBindWithExcel, this.currentPage1, this.pageSize1);
            this.dataGridView1.DataSource = this.enterpriseInfoList1;
        }
        // 点击dataGridView1 的"下一页"按钮
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.currentPage1 + 1 > this.pageCount1)
            {
                MessageBox.Show("已经是第最后一页了！", "页数错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            this.currentPage1++;
            //this.comboBox1.SelectedValue = this.currentPage1;
            this.comboBox1.Text = this.currentPage1.ToString();
            this.enterpriseInfoList1 = this.GetEnterpriseInfosByPage(this.enterpriseInfoBindWithExcel, this.currentPage1, this.pageSize1);
            this.dataGridView1.DataSource = this.enterpriseInfoList1;
        }

        // 指定ComboBox1的值时
        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            int page = (int)this.comboBox1.SelectedValue;
            if (page != this.currentPage1)
            {
                this.currentPage1 = page;
                this.enterpriseInfoList1 = this.GetEnterpriseInfosByPage(this.enterpriseInfoBindWithExcel, this.currentPage1, this.pageSize1);
                this.dataGridView1.DataSource = this.enterpriseInfoList1;
            }
        }

        // 鼠标进入panel2时，panel2被选中
        private void panel2_MouseEnter(object sender, EventArgs e)
        {
            // panel2获取焦点，否则无法使用滚动条
            this.panel2.Select();
        }

        // 打开Excel目录
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("Explorer.exe", this.taskItem.resultExcel);
        }

        // 打开识别的图片目录
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("Explorer.exe", this.taskItem.picturePath);
        }

        // 点击dataGridView2 的"上一页"按钮
        private void button4_Click(object sender, EventArgs e)
        {
            if (this.currentPage2 - 1 < 1)
            {
                MessageBox.Show("已经是第一页了！", "页数错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            this.currentPage2--;
            //this.comboBox1.SelectedValue = this.currentPage1;
            this.comboBox2.Text = this.currentPage2.ToString();
            this.enterpriseInfoList2 = this.GetEnterpriseInfosByPage(this.enterpriseInfoBindWithExcel2, this.currentPage2, this.pageSize2);
            this.dataGridView2.DataSource = this.enterpriseInfoList2;
        }


        // 点击dataGridView2 的"下一页"按钮
        private void button3_Click(object sender, EventArgs e)
        {
            if (this.currentPage2 + 1 > this.pageCount2)
            {
                MessageBox.Show("已经是第最后一页了！", "页数错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            this.currentPage2++;
            //this.comboBox1.SelectedValue = this.currentPage1;
            this.comboBox2.Text = this.currentPage2.ToString();
            this.enterpriseInfoList2 = this.GetEnterpriseInfosByPage(this.enterpriseInfoBindWithExcel2, this.currentPage2, this.pageSize2);
            this.dataGridView2.DataSource = this.enterpriseInfoList2;
        }

        // 指定ComboBox2的值时
        private void comboBox2_SelectedValueChanged(object sender, EventArgs e)
        {
            int page = (int)this.comboBox2.SelectedValue;
            if (page != this.currentPage2)
            {
                this.currentPage2 = page;
                this.enterpriseInfoList2 = this.GetEnterpriseInfosByPage(this.enterpriseInfoBindWithExcel2, this.currentPage2, this.pageSize2);
                this.dataGridView2.DataSource = this.enterpriseInfoList2;
            }
        }


        // dategridview1 的单元格内容更改时
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < dataGridView1.Rows.Count - 1)
            {
                DataGridViewRow dgrSingle = dataGridView1.Rows[e.RowIndex];
                dgrSingle.DefaultCellStyle.ForeColor = SystemColors.Highlight;
            }
        }

        // dategridview2 的单元格内容更改时
        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < dataGridView2.Rows.Count - 1)
            {
                DataGridViewRow dgrSingle = dataGridView2.Rows[e.RowIndex];
                dgrSingle.DefaultCellStyle.ForeColor = SystemColors.Highlight;
            }
        }

        // dategridview1 的单元格被点击时
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4 && e.RowIndex >= 0) // 查看原图
            {
                List<EnterpriseInfo> enterpriseInfos = (List<EnterpriseInfo>)dataGridView1.DataSource;

                EnterpriseInfo enterpriseInfo = enterpriseInfos[e.RowIndex];
                string businessLicenseFile = enterpriseInfo.BusinessLicenseFile;

                Console.WriteLine(dataGridView1.CurrentCell.Value.ToString());
                System.Diagnostics.Process.Start("explorer.exe", businessLicenseFile);
            }
            else if (e.ColumnIndex == 5 && e.RowIndex >= 0) // 删除一行
            {
                List<EnterpriseInfo> enterpriseInfos = (List<EnterpriseInfo>)dataGridView1.DataSource;

                EnterpriseInfo enterpriseInfo = enterpriseInfos[e.RowIndex];
                // 删除缓冲中的企业信息和excel中对应的信息
                enterpriseInfo.Remove();

                // 更新当前页（重新获取当前页的数据）
                this.enterpriseInfoList1 = this.GetEnterpriseInfosByPage(this.enterpriseInfoBindWithExcel, this.currentPage1, this.pageSize1);
                this.dataGridView1.DataSource = this.enterpriseInfoList1;
            }
        }

        // dategridview2 的单元格被点击时
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4 && e.RowIndex >= 0) // 查看原图
            {
                List<EnterpriseInfo> enterpriseInfos = (List<EnterpriseInfo>)dataGridView2.DataSource;

                EnterpriseInfo enterpriseInfo = enterpriseInfos[e.RowIndex];
                string businessLicenseFile = enterpriseInfo.BusinessLicenseFile;

                Console.WriteLine(dataGridView2.CurrentCell.Value.ToString());
                System.Diagnostics.Process.Start("explorer.exe", businessLicenseFile);
            }
            else if (e.ColumnIndex == 5 && e.RowIndex >= 0) // 删除一行
            {
                List<EnterpriseInfo> enterpriseInfos = (List<EnterpriseInfo>)dataGridView2.DataSource;

                EnterpriseInfo enterpriseInfo = enterpriseInfos[e.RowIndex];
                // 删除缓冲中的企业信息和excel中对应的信息
                enterpriseInfo.Remove();

                // 更新当前页（重新获取当前页的数据）
                this.enterpriseInfoList2 = this.GetEnterpriseInfosByPage(this.enterpriseInfoBindWithExcel2, this.currentPage2, this.pageSize2);
                this.dataGridView2.DataSource = this.enterpriseInfoList2;
            }
        }

        // 保存识别结果到excel
        private void button5_Click(object sender, EventArgs e)
        {
            /* SaveFileDialog sfd = new SaveFileDialog();
             sfd.Filter = "文本文件(*.txt)|*.txt|所有文件|*.*";//设置文件类型
             sfd.FileName = "保存";//设置默认文件名
             sfd.DefaultExt = "txt";//设置默认格式（可以不设）
             sfd.AddExtension = true;//设置自动在文件名中添加扩展名
             if (sfd.ShowDialog() == DialogResult.OK)
             {
                 txtPath.Text = "FileName:" + sfd.FileName + "\r\n";
                 using (StreamWriter sw = new StreamWriter(sfd.FileName))
                 {
                     sw.WriteLineAsync("今天是个好天气");
                 }
             }
             MessageBox.Show("ok");
             */
            this.saveFileDialog1.Filter = "Excel文件(*.xlsx)|*.xlsx";  // 设置文件类型
            this.saveFileDialog1.FileName = "识别结果";  // 设置默认文件名
            this.saveFileDialog1.DefaultExt = "xlsx";  // 设置默认格式（可以不设）
            this.saveFileDialog1.AddExtension = true;  // 设置自动在文件名中添加扩展名
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // 获取保存识别结果excel文件的位置
                string excelFile = this.saveFileDialog1.FileName;
                string excelPath = Path.GetDirectoryName(excelFile);

                if (Directory.Exists(excelPath))
                {
                    // 更新task信息
                    this.taskItem.resultExcel = excelFile;

                    // 更新任务项配置，写入保存识别结果excel文件的位置
                    XMLTool.Update(this.taskItem.taskPath + "\\config.xml", this.taskItem);

                    ExcelTool excelTool = new ExcelTool();
                    string sheetName = "营业执照信息";  //输出的excel文件工作表表名
                    string[] tableHeader = { "企业注册号", "企业名称" };  //excel工作表的标题

                    // 写入高置信度的企业信息
                    excelTool.WriteSheetData(excelFile, this.enterpriseInfoList1, sheetName, tableHeader);
                    // 写入低置信度的企业信息
                    excelTool.AppendSheetData(excelFile, this.enterpriseInfoList2, this.enterpriseInfoList1.Count + 2);

                    // 关闭excel
                    excelTool.Close();

                    // 更新视图
                    this.linkLabel2.Text = excelFile;
                    System.Diagnostics.Process.Start("explorer.exe", excelFile);
                    MessageBox.Show("导出成功！", "导出结果", MessageBoxButtons.OK);
                }
            }
        }

    }
}
