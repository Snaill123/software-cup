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
using System.Media;

namespace OcrTesseract
{
    public partial class TaskListControl : UserControl
    {
        // 一个任务完成
        // 委托
        public delegate void ModifyCompletedConfigHandler(TaskItem taskItem);
        // 事件
        public event ModifyCompletedConfigHandler ModifyCompletedConfig = null;

        private TaskManager taskManager = new TaskManager(); // 任务管理器

        // @"E:\Visual Studio\source\repos\OcrTesseract\OcrTesseract\task\task_list_config.xml";  
        private string taskListConfigFile = Config.TaskListConfigFile; // 任务列表配置文件

        private List<TaskItem> taskList = null; // 任务列表
        private Dictionary<string, TaskItemControl> taskItemControlsCache = new Dictionary<string, TaskItemControl>(); // 任务项视图缓冲

        private Point nextTaskItemPoint = new Point(0, 0);

        private TaskItemControl selectedTaskItemControl = null;

        private TaskItemControl focusedTaskItemControl = null;

        public TaskListControl()
        {
            // 调试用。 显示任务保存路径
            // MessageBox.Show("TASK PATH: " + Config.AppTaskDirectory);

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
            if (File.Exists(this.taskListConfigFile))
            {
                this.taskList = XMLTool.LoadTaskList(this.taskListConfigFile);
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
            /*
            TaskItem task1 = new TaskItem("任务任务任务任务任务任务任务1", @"E:\Visual Studio\source\repos\OcrTesseract\OcrTesseract");
            taskList.Add(task1);
            taskList.Add(task1);
            taskList.Add(task1);
            taskList.Add(task1);
            taskList.Add(task1);
            */
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
            TaskItemControl taskItemControl = null;
            if (!this.taskItemControlsCache.TryGetValue(taskItem.Name, out taskItemControl))
            {
                taskItemControl = new TaskItemControl(taskItem);
                this.taskItemControlsCache.Add(taskItem.Name, taskItemControl);

                // 设置Anchor
                taskItemControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Left)));

                // 为TaskItemControl添加Click事件
                taskItemControl.Click += new EventHandler(this.TaskItemControl_Click);
                // 为TaskItemControl pictureBox2添加Click事件
                taskItemControl.PictureBox2.Click += new EventHandler(this.PictureBox2_Click);
                // 为TaskItemControl添加任务完成事件
                taskItemControl.TaskAccomplished += new TaskItemControl.TaskAccomplishedHandler(this.TaskItemAccomplished);
            }

            taskItemControl.Location = this.nextTaskItemPoint;

            // 留2像素的间隔
            nextTaskItemPoint.Y = nextTaskItemPoint.Y + taskItemControl.Height + 2;

            this.panel2.Controls.Add(taskItemControl);
        }

        // 添加一个正在识别任务项
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

        // TaskItemControl任务完成事件的处理方法
        private void TaskItemAccomplished(TaskItem taskItem)
        {
            // 更新任务信息到配置文件
            XMLTool.Update(taskItem.taskPath + "\\config.xml", taskItem);

            // 更新任务列表信息到配置文件
            XMLTool.DeleteTaskFromTaskList(Config.TaskListConfigFile, taskItem.taskPath);

            // 向已完成任务列表配置文件添加已完成项
            XMLTool.AddCompletedTaskList(Config.CompletedTaskListConfigFile, taskItem.taskPath);


            // 通知主界面更新已完成任务列表视图
            Form1 form = (Form1)this.ParentForm;
            if (this.ModifyCompletedConfig != null)
            {
                // 触发事件，通知已完成任务视图更新
                this.ModifyCompletedConfig(taskItem);
            }

            // 删除已完成任务项
            this.taskList.Remove(taskItem);

            // 删除已完成任务项的视图
            this.taskItemControlsCache.Remove(taskItem.Name);

            // 重绘任务列表视图
            this.RepaintTaskList();

            // 播放已完成提示音
            PlayCompletedSound();

            // 重新加载任务列表视图
            //TaskListControl taskListControl = new TaskListControl();
            //Form1 form = (Form1)this.ParentForm;
            //form.ShowTaskListControl(taskListControl);

            // 释放当前视图使用的资源
            //this.Dispose();
        }

        // 播放已完成提示音
        private void PlayCompletedSound()
        {
            System.Media.SoundPlayer player = new SoundPlayer(global::OcrTesseract.Properties.Resources.sound);
           
            player.LoadAsync();
            player.Play();
            //player.PlayLooping();                         //asynchronous (loop)playing in new thread
            //Thread.Sleep(5000);
           // player.Stop();
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

        // 点击“新建”，创建新建任务窗体
        private void label1_Click(object sender, EventArgs e)
        {
            NewTaskForm newTaskForm = new NewTaskForm();
            newTaskForm.NewTaskEvent += new NewTaskForm.NewTaskEventHandler(this.NewTask);
            newTaskForm.ShowDialog();
        }

        // 新建任务项
        private void NewTask(TaskItem task)
        {
            // 提交任务，更新taskList及配置文件

            // 将新任务项添加到任务列表
            this.taskList.Add(task);

            // 更新任务列表视图
            //ShowTaskItem(task);
            RepaintTaskList();

            // 更新任务列表配置文件
            XMLTool.AddTaskList(this.taskListConfigFile, task.taskPath);

            // 启动任务
            StartTaskItem(task);
        }

        // 启动任务项
        private void StartTaskItem(TaskItem task)
        {
            // "开始"按钮不可操作
            //this.label2.Enabled = false;

            // 通过 "" 包围路径，防止java无法识别带空格的路径
            string customArgs = "\"" + task.taskPath + "\\java_program_config.xml" + "\"";
            this.taskManager.NewProcess(new JavaProcess(customArgs, task));

            // "暂停"按钮可操作
            //this.label3.Enabled = true;
        }

        // 点击“暂停”，暂停选中任务
        private void label3_Click(object sender, EventArgs e)
        {
            // "暂停"按钮不可操作
            //this.label3.Enabled = false;

            // 暂停选中任务
            this.WaitTaskItem(this.selectedTaskItemControl.TaskItem);

            // 选中任务暂停计时
            this.selectedTaskItemControl.Timer_Stop();

            // "开始"按钮可操作
            //this.label2.Enabled = true;
        }

        // 暂停任务项对应的进程
        private void WaitTaskItem(TaskItem task)
        {
            this.taskManager.WaitProcess(task);
        }

        // 点击“开始”，启动选中任务
        private void label2_Click(object sender, EventArgs e)
        {
            // "开始"按钮不可操作
            //this.label2.Enabled = false;

            // 启动选中任务
            this.RunTaskItem(this.selectedTaskItemControl.TaskItem);

            // 选中任务开始计时
            this.selectedTaskItemControl.Timer_Start();

            // "暂停"按钮可操作
            //this.label3.Enabled = true;
        }

        // 运行任务项对应的进程
        private void RunTaskItem(TaskItem task)
        {
            this.taskManager.RunProcess(task);
        }

        // 点击任务项
        private void TaskItemControl_Click(object sender, EventArgs e)
        {
            TaskItemControl taskItemControl = (TaskItemControl)sender;

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
                // 可开始、暂停、删除任务项
                this.label2.Enabled = true;
                this.label3.Enabled = true;
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
            TaskItemControl taskItemControl = (TaskItemControl)pictureBox2.Parent;

            TaskDetailControl taskDetailControl = new TaskDetailControl(taskItemControl.TaskItem);
            Form1 form = (Form1)this.ParentForm;
            form.ShowTaskDetailControl(taskDetailControl);

        }

        // 判断用户是否安装了office
        private bool IsInstalledOffice()
        {
            // 通过读取注册表判断是否安装了office
            RegistryKey rk = Registry.LocalMachine;

            // 为空，不知道为啥
            //RegistryKey office2000 = rk.OpenSubKey(@"SOFTWARE\Microsoft\Office\9.0\Excel\InstallRoot\");
            //RegistryKey office2003 = rk.OpenSubKey(@"SOFTWARE\Microsoft\Office\11.0\Excel\InstallRoot\");
            //RegistryKey office2007 = rk.OpenSubKey(@"SOFTWARE\Microsoft\Office\12.0\Excel\InstallRoot\");
            //RegistryKey office2010 = rk.OpenSubKey(@"SOFTWARE\Microsoft\Office\14.0\Excel\InstallRoot\");
            //RegistryKey office2016 = rk.OpenSubKey(@"SOFTWARE\Microsoft\Office\16.0\Word");

            //Console.WriteLine(office2016==null);

            //RegistryKey office = rk.OpenSubKey(@"SOFTWARE\Microsoft\Office\*\Excel\InstallRoot\");

            //return (office2000 != null || office2003 != null || office2007 != null || office2010 != null || office2016 != null);

            RegistryKey office = rk.OpenSubKey(@"SOFTWARE\Microsoft\Office");
            return office != null;
        }

        // 点击“删除”
        private void label4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("任务未完成不可删除！");
        }

        // 点击“多选”
        private int label5ClickCount = 0; // 复选框点击次数
        private void label5_Click(object sender, EventArgs e)
        {
            if (this.label5ClickCount % 2 == 0)
            {
                foreach (TaskItemControl taskItemControl in this.taskItemControlsCache.Values)
                {
                    taskItemControl.CheckBox1.Visible = true;
                }
                this.label4.Enabled = true;
            }
            else
            {
                foreach (TaskItemControl taskItemControl in this.taskItemControlsCache.Values)
                {
                    taskItemControl.CheckBox1.Visible = false;
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

        private void label1_MouseHover(object sender, EventArgs e)
        {
            DrawMouseHoverLable(label1, global::OcrTesseract.Properties.Resources.add_blue_1296db);
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            DrawMouseLeaveLable(label1, global::OcrTesseract.Properties.Resources.add);
        }

        private void label2_MouseHover(object sender, EventArgs e)
        {
            DrawMouseHoverLable(label2, global::OcrTesseract.Properties.Resources.jumplist_startalltask);
        }

        private void label2_MouseLeave(object sender, EventArgs e)
        {
            DrawMouseLeaveLable(label2, global::OcrTesseract.Properties.Resources.startalltask);
        }

        private void label3_MouseHover(object sender, EventArgs e)
        {
            DrawMouseHoverLable(label3, global::OcrTesseract.Properties.Resources.jumplist_pausealltask);
        }

        private void label3_MouseLeave(object sender, EventArgs e)
        {
            DrawMouseLeaveLable(label3, global::OcrTesseract.Properties.Resources.pause);
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



        /*
            // 绘制任务列表
            private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
            {

            }

            // 点击任务项
            private void listView1_Click(object sender, EventArgs e)
            {
                ListView.SelectedIndexCollection selectedIndexCollection = this.listView1.SelectedIndices;
                ListView.SelectedListViewItemCollection selectedItemCollection = this.listView1.SelectedItems;

                if (selectedItemCollection.Count > 0)
                {
                    ListViewItem item = selectedItemCollection[0];
                    // 显示任务列表视图
                    TaskDetailControl taskDetailControl = new TaskDetailControl();
                    Form1 form = (Form1)this.ParentForm;
                    form.ShowTaskDetailControl(taskDetailControl);
                }
            }
    */

    }
}
