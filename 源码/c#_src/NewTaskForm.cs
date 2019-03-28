using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OcrTesseract
{
    public partial class NewTaskForm : Form
    {
        // 委托
        public delegate void NewTaskEventHandler(TaskItem task);
        // 事件
        public event NewTaskEventHandler NewTaskEvent = null;

        public NewTaskForm()
        {
            InitializeComponent();
        }

        // 选择图片文件对话框
        private void button1_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.ShowDialog();
            String selectedPath = this.folderBrowserDialog1.SelectedPath;
            this.textBox2.Text = selectedPath;
        }

        // 创建任务，并保存任务信息
        private void button2_Click(object sender, EventArgs e)
        {
            string name = this.textBox1.Text;
            string picturePath = this.textBox2.Text;

            if (CheckTaskName(name) && CheckPicturePath(picturePath))
            {
                TaskItem task = new TaskItem(name, picturePath);
                this.Close();
                SaveNewTask(task);
                // 触发事件
                if (this.NewTaskEvent != null)
                {
                    this.NewTaskEvent(task);
                }
            }
        }

        // 判断图片路径是否存在
        private bool CheckPicturePath(string picturePath)
        {
            if (!Directory.Exists(picturePath))
            {
                MessageBox.Show("图片路径不存在！", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        // 判断任务名是否合法
        private bool CheckTaskName(string taskName)
        {
            if (taskName == null || taskName == "")
            {
                MessageBox.Show("任务名不能为空！", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (Regex.IsMatch(taskName, @"[\u4e00-\u9fa5]"))
            {
                MessageBox.Show("任务名不能含中文！请以字母、数字或_命名。", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                // TODO:判断任务名是否重复
                
            }
            return true;
        }

        // 保存任务信息
        private void SaveNewTask(TaskItem task)
        {
            if (!Directory.Exists(task.taskPath))
            {
                Directory.CreateDirectory(task.taskPath);
            }
            string xmlFile = task.taskPath + "\\config.xml";
            // 生成任务配置文件，保存任务信息
            XMLTool.Save(xmlFile, task);

            // 生成java配置文件，用于初始化java运行环境
            string javaConfigFile = task.taskPath + "\\java_program_config.xml";
            XMLTool.SaveJavaConfig(javaConfigFile, task);
        }

        // 点击取消，则关闭此窗体
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
