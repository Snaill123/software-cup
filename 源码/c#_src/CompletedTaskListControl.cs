using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

namespace OcrTesseract
{
    public partial class CompletedTaskListControl : UserControl
    {
        private string completedTaskListConfigFile = Config.CompletedTaskListConfigFile; // 任务列表配置文件

        private List<TaskItem> taskList = null; // 任务列表
        private Dictionary<string, CompletedTaskItemControl> taskItemControlsCache = new Dictionary<string, CompletedTaskItemControl>(); // 任务项视图缓冲

        private Point nextTaskItemPoint = new Point(0, 0);

        private CompletedTaskItemControl selectedTaskItemControl = null;

        private CompletedTaskItemControl focusedTaskItemControl = null;

        public CompletedTaskListControl()
        {
            InitializeComponent();

            // 获取所有任务项
            this.InitTaskList();

            // 读取任务列表，并绘制对应的视图
            ShowTaskList();
        }

        /// <summary>
        /// 设置TaskList
        /// </summary>
        private void InitTaskList()
        {
            if (File.Exists(this.completedTaskListConfigFile))
            {
                Console.WriteLine("completedTaskListConfigFile exist");
                this.taskList = XMLTool.LoadTaskList(this.completedTaskListConfigFile);
                SortTaskListDesc(); // 任务列表按创建时间倒序排列
            }
            else
            {
                this.taskList = new List<TaskItem>();
            }
        }

        private void SortTaskListDesc()
        {
            this.taskList.Sort((task1, task2) => -DateTime.Compare(Convert.ToDateTime(task1.createTime), Convert.ToDateTime(task2.createTime)));
        }

        // 绘制任务列表对应的视图
        private void ShowTaskList()
        {
            // 显示所有任务
            foreach (TaskItem taskItem in this.taskList)
            {
                // 显示任务项
                ShowTaskItem(taskItem);
            }
        }

        // 显示任务项
        public void ShowTaskItem(TaskItem taskItem)
        {
            CompletedTaskItemControl completedTaskItemControl = null;
            if (!this.taskItemControlsCache.TryGetValue(taskItem.Name, out completedTaskItemControl))
            {
                completedTaskItemControl = new CompletedTaskItemControl(taskItem);
                this.taskItemControlsCache.Add(taskItem.Name, completedTaskItemControl);

                // 设置Anchor
                completedTaskItemControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Left)));

                // 为CompletedTaskItemControl添加Click事件
                completedTaskItemControl.Click += new EventHandler(this.TaskItemControl_Click);
                // 为CompletedTaskItemControl pictureBox2添加Click事件
                completedTaskItemControl.PictureBox2.Click += new EventHandler(this.PictureBox2_Click);
                // 添加任务重命名事件，处理方法
                // 定义 TaskRename 方法 
            }

            completedTaskItemControl.Location = nextTaskItemPoint;

            // 留2像素的间隔
            nextTaskItemPoint.Y = nextTaskItemPoint.Y + completedTaskItemControl.Height + 2;

            this.panel2.Controls.Add(completedTaskItemControl);

        }

        // 添加一个已完成任务项
        public void Append(TaskItem taskItem)
        {
            this.taskList.Add(taskItem);
            this.RepaintTaskList();
        }

        // 重绘任务列
        private void RepaintTaskList()
        {
            this.panel2.Controls.Clear();
            SortTaskListDesc(); // 任务列表按创建时间倒序排列
            this.nextTaskItemPoint = new Point(0, 0);
            ShowTaskList();
        }

        /// <summary>
        /// 获取TaskList
        /// </summary>
        /// <returns>return TaskList</returns>
        private List<TaskItem> GetTaskList()
        {
            return this.taskList;
        }

        /// <summary>
        /// 保存TaskList的信息到磁盘中
        /// </summary>
        private void SaveTaskList()
        {

        }

        // 点击任务项
        private void TaskItemControl_Click(object sender, EventArgs e)
        {
            CompletedTaskItemControl taskItemControl = (CompletedTaskItemControl)sender;

            if (selectedTaskItemControl != taskItemControl)
            {
                if (selectedTaskItemControl != null)
                {
                    selectedTaskItemControl.BackColor = SystemColors.Window;
                    selectedTaskItemControl.PictureBox2.Visible = false;
                }
                taskItemControl.BackColor = Color.PowderBlue;
                taskItemControl.PictureBox2.Visible = true;

                selectedTaskItemControl = taskItemControl;

                // 可删除任务
                this.label4.Enabled = true;
            }
        }

        // 查看任务详情
        private void PictureBox2_Click(object sender, EventArgs e)
        {
            // 判断用户是否安装了office
            if (!IsInstalledOffice())
            {
                MessageBox.Show("查看结果需要本地计算机安装相应版本的office软件，请安装后重试！", "需要office支持", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // 返回之前的视图
                return;
            }

            PictureBox pictureBox2 = (PictureBox)sender;
            CompletedTaskItemControl taskItemControl = (CompletedTaskItemControl)pictureBox2.Parent;

            // 创建任务详情视图
            TaskDetailControl taskDetailControl = new TaskDetailControl(taskItemControl.TaskItem);
            Form1 form = (Form1)this.ParentForm;
            form.ShowTaskDetailControl(taskDetailControl);
        }

        // 判断用户是否安装了office
        private bool IsInstalledOffice()
        {
            // 通过读取注册表判断是否安装了office
            RegistryKey rk = Registry.LocalMachine;

            //RegistryKey office2000 = rk.OpenSubKey(@"SOFTWARE\Microsoft\Office\9.0\Excel\InstallRoot\");
            //RegistryKey office2003 = rk.OpenSubKey(@"SOFTWARE\Microsoft\Office\11.0\Excel\InstallRoot\");
            //RegistryKey office2007 = rk.OpenSubKey(@"SOFTWARE\Microsoft\Office\12.0\Excel\InstallRoot\");
            //RegistryKey office2010 = rk.OpenSubKey(@"SOFTWARE\Microsoft\Office\14.0\Excel\InstallRoot\");
            //RegistryKey office2016 = rk.OpenSubKey(@"SOFTWARE\Microsoft\Office\16.0\Excel\InstallRoot\");
            ////RegistryKey office = rk.OpenSubKey(@"SOFTWARE\Microsoft\Office\*\Excel\InstallRoot\");

            //return (office2000 != null || office2003 != null || office2007 != null || office2010 != null || office2016 != null);

            RegistryKey office = rk.OpenSubKey(@"SOFTWARE\Microsoft\Office");
            return office != null;
        }

        // 点击“删除”
        private void label4_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("您确定要删除此任务吗？", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (dialogResult == DialogResult.OK)
            {
                // 记录已删除的任务名称
                List<string> deletedTaskNames = new List<string>();

                // 已删除任务列表视图
                Form1 form = (Form1)this.ParentForm;
                DeletedTaskListControl deletedTaskListControl = (DeletedTaskListControl)form.GetDeletedTaskListControl();
                
                foreach (CompletedTaskItemControl completedTaskItemControl in this.taskItemControlsCache.Values)
                {
                    if (completedTaskItemControl.CheckBox1.Checked)
                    {
                        // 更新任务列表信息到配置文件
                        XMLTool.DeleteTaskFromCompletedTaskList(Config.CompletedTaskListConfigFile, completedTaskItemControl.TaskItem.taskPath);

                        // 向已删除任务列表配置文件添加删除项
                        XMLTool.AddDeletedTaskList(Config.DeletedTaskListConfigFile, completedTaskItemControl.TaskItem.taskPath);

                        // 删除已删除任务项
                        this.taskList.Remove(completedTaskItemControl.TaskItem);

                        // 添加删除任务项（不能在枚举操作中修改集合）
                        deletedTaskNames.Add(completedTaskItemControl.TaskItem.Name);

                        // 重绘已删除任务列表视图
                        if (deletedTaskListControl != null)
                        {
                            deletedTaskListControl.Append(completedTaskItemControl.TaskItem);
                        }
                        //Console.WriteLine("删除------");

                    }  //if end
                }  //foreach end

                // 删除已删除任务项的视图
                foreach (string deletedTaskName in deletedTaskNames)
                {
                    this.taskItemControlsCache.Remove(deletedTaskName);
                }

                // 重绘已完成任务列表视图
                this.RepaintTaskList();

                MessageBox.Show("已删除！", "", MessageBoxButtons.OK);
            }
        }

        // 点击“多选”
        private int label5ClickCount = 0; // 复选框点击次数
        private void label5_Click(object sender, EventArgs e)
        {
            if (this.label5ClickCount % 2 == 0)
            {
                foreach (CompletedTaskItemControl completedTaskItemControl in this.taskItemControlsCache.Values)
                {
                    completedTaskItemControl.CheckBox1.Visible = true;
                }
                this.label4.Enabled = true;
            }
            else
            {
                foreach (CompletedTaskItemControl completedTaskItemControl in this.taskItemControlsCache.Values)
                {
                    completedTaskItemControl.CheckBox1.Visible = false;
                }
                this.label4.Enabled = false;
            }
            this.label5ClickCount++;
        }

        // 点击系统设置
        private void label6_Click(object sender, EventArgs e)
        {
            Form1 form = (Form1)this.ParentForm;
            form.ShowSystemSettingForm();
        }

        private void label4_MouseHover(object sender, EventArgs e)
        {
            DrawMouseHoverLable(label4, global::OcrTesseract.Properties.Resources.delete_item);
        }
        private void label4_MouseLeave(object sender, EventArgs e)
        {
            DrawMouseLeaveLable(label4, global::OcrTesseract.Properties.Resources.delete_item);
        }

        private void label5_MouseHover(object sender, EventArgs e)
        {
            DrawMouseHoverLable(label5, global::OcrTesseract.Properties.Resources.multiple_choice);
        }
        private void label5_MouseLeave(object sender, EventArgs e)
        {
            DrawMouseLeaveLable(label5, global::OcrTesseract.Properties.Resources.multiple_choice);
        }

        private void label6_MouseHover(object sender, EventArgs e)
        {
            DrawMouseHoverLable(label6, global::OcrTesseract.Properties.Resources.set_blue_1296db);
        }
        private void label6_MouseLeave(object sender, EventArgs e)
        {
            DrawMouseLeaveLable(label6, global::OcrTesseract.Properties.Resources.set);
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


    }
}
