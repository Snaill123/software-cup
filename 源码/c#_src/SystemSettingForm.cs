using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OcrTesseract
{
    public partial class SystemSettingForm : Form
    {
        public SystemSettingForm()
        {
            InitializeComponent();

            // 设置图标
            this.Icon = System.Drawing.Icon.FromHandle(global::OcrTesseract.Properties.Resources.set_blue_1296db.GetHicon());

            // 设置combobox值域
            comboBox1.DataSource = this.GetComboBoxDataource();
            comboBox1.Text = "3";
        }

        public int[] GetComboBoxDataource()
        {
            int[] data = new int[5];
            for (int i = 0; i < 5; i++)
            {
                data[i] = i + 1;
            }
            return data;
        }

        // 选择java运行路径对话框
        private void button1_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.ShowDialog();
            String selectedPath = this.folderBrowserDialog1.SelectedPath;
            this.textBox2.Text = selectedPath;
        }

        // 选择tesseract运行路径对话框
        private void button2_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.ShowDialog();
            String selectedPath = this.folderBrowserDialog1.SelectedPath;
            this.textBox1.Text = selectedPath;
        }

        // 点击“应用”
        private void button4_Click(object sender, EventArgs e)
        {
            // int maxTaskCount = (int)this.comboBox1.SelectedValue; //s手动输入会报错
            // System.NullReferenceException:“未将对象引用设置到对象的实例

            try
            {
                float confidence = float.Parse(this.textBox3.Text);
            }
            catch (Exception e1)
            {
                Console.WriteLine(e1.StackTrace);
                MessageBox.Show("置信度数值不合法！请重新输入。", "输入有误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            string javaPath = this.textBox2.Text;
            if (!Directory.Exists(javaPath))
            {
                MessageBox.Show("路径不存在！请重新输入。", "路径不存在", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // 判断java是否安装
           /* else if ()
            {

            }
            */

            string tesseractPath = this.textBox1.Text;
            if (!Directory.Exists(tesseractPath))
            {
                MessageBox.Show("路径不存在！请重新输入。", "路径不存在", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // TODO:添加内容
            this.Close();
        }

        // 点击“取消”
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }


}
