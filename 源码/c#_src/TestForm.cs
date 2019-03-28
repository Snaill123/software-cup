using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OcrTesseract
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();

            panel3.LostFocus += new System.EventHandler(this.panel3_LostFocus);

            
            /////////////////////////
            //dataGridView2.DataSource = null; //每次打开清空内容

           // DataTable dt = GetDataFromExcel().Tables[0];

           // dataGridView2.DataSource = dt;

            //dataGridView2.DataSource = GetDataFromExcel();
            /*
            // 写回
            dt = (DataTable)(dataGridView2.DataSource);

            DataSet dataSet = new DataSet();
            dataSet.
                */
        }

        private void panel3_Click(object sender, EventArgs e)
        {
            panel3.BackColor = Color.PowderBlue;
            
            Console.WriteLine("can focus" + panel3.CanFocus + "focused" + panel3.Focused);
        }

        private void panel3_LostFocus(object sender, EventArgs e)
        {
            panel3.BackColor = Color.PowderBlue;
            Console.WriteLine("panel3_LostFocus");
            
        }

        private void panel3_Leave(object sender, EventArgs e)
        {
            panel3.BackColor = Color.Pink;
            Console.WriteLine("panel3_Leave");

        }

        private void panel2_Click(object sender, EventArgs e)
        {
            panel3.BackColor = Color.Red;
            Console.WriteLine("panel2_Click"); 
        }

        /// <summary>
        /// 从Excel中读取数据
        /// </summary>
        /// <returns></returns>
        public DataSet GetDataFromExcel()
        {
            //打开文件
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Excel(*.xlsx)|*.xlsx|Excel(*.xls)|*.xls";
            //file.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            file.InitialDirectory = @"E:\Visual Studio\source\repos\OcrTesseract\OcrTesseract\Resources\result";
            file.Multiselect = false;
            if (file.ShowDialog() == DialogResult.Cancel)
                return null;

            //判断文件后缀
            var path = file.FileName;
            string fileSuffix = System.IO.Path.GetExtension(path);

            if (string.IsNullOrEmpty(fileSuffix))
                return null;

            using (DataSet ds = new DataSet())
            {
                //判断Excel文件是2003版本还是2007版本
                string connString = "";

                if (fileSuffix == ".xls")
                    connString = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + path + ";" + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
                else
                    connString = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + path + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";

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

        // 将数据写入Excel
        public bool SaveToExcel(DataSet dataSet)
        {
            //打开文件
            SaveFileDialog file = new SaveFileDialog();
            file.DefaultExt = "xls";
            file.FileName = "excel文件.xls";
            file.Filter = "Excel(*.xlsx)|*.xlsx|Excel(*.xls)|*.xls";
            file.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (file.ShowDialog() == DialogResult.Cancel)
                return false;

            //判断文件后缀
            var path = file.FileName;
            string fileSuffix = System.IO.Path.GetExtension(path);

            if (string.IsNullOrEmpty(fileSuffix))
                return false;

            String sConnectionString = "Provider=Microsoft.Jet.OLEDB4.0;" + "Data Source=c:/test.xls;" + "Extended Properties=Excel 8.0;";
            OleDbConnection cn = new OleDbConnection(sConnectionString);
            string sqlCreate = "CREATE TABLE TestSheet ([ID] INTEGER,[Username] VarChar,[UserPwd] VarChar)";
            OleDbCommand cmd = new OleDbCommand(sqlCreate, cn);
            //创建Excel文件：C:/test.xls
            cn.Open();
            //创建TestSheet工作表
            cmd.ExecuteNonQuery();
            //添加数据
            cmd.CommandText = "INSERT INTO TestSheet VALUES(1,'elmer','password')";
            cmd.ExecuteNonQuery();
            //关闭连接
            cn.Close();

            return true;
        }
        /*
                //该方法是以数据库的方式读取Excel的内容
                //OleDb 12.0一般机器装的驱动是Microsoft.Jet.Oledb.4.0,这适用于2003版本以下，2007版本以上应用下面的驱动，需要手动下载安装
                private void button1_Click(object sender, EventArgs e)
                {
                    string filename = null;
                    string strcon=null;
                    //ofd为OpenFileDialog控件的Name属性 
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        filename = ofd.FileName;
                        //获得选中文档的文件名，其实就是文件的绝对路径
                    }
                    strcon = "Provider=Microsoft.Ace.Oledb.12.0;Data Source=" + filename + ";Extended Properties=Excel 12.0;";
                    //提供数据库的连接驱动路径及其数据源，特别注意这里的Data Source中间是有空格的 
                    String strsql = "select * from [Sheet1$]";
                    //Excel文件中又分为多个工作表，选择需要读取的表 
                    OleDbConnection mycon = null;
                    try { mycon= new OleDbConnection(strcon);
                        Console.WriteLine("Connection is Successful");
                    } catch {
                        Console.WriteLine("Connection is defeated");
                    }
                    mycon.Open();
                    OleDbDataAdapter ad = new OleDbDataAdapter(strsql, mycon);
                    DataSet ds = new DataSet();
                    ad.Fill(ds,"[Sheet1$]");
                    //将适配器内的内容填充到数据集中，所谓数据集可看做是文件在内存中的副本 
                    data1.DataMember = "[Sheet1$]";
                    //获取或设置列表或表的名称。 
                    data1.DataSource = ds;
                    //设置或获取数据源 
                    data1.Show();
                    //显示该控件 
                    mycon.Close();
                } */
        
    }
}
