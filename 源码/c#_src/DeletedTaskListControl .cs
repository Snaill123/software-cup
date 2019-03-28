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

namespace OcrTesseract
{
    public partial class DeletedTaskListControl : UserControl
    {
        private string deletedTaskListConfigFile = Config.DeletedTaskListConfigFile; // 任务列表配置文件

        private List<TaskItem> taskList = null; // 任务列表
        private Dictionary<string, DeletedTaskItemControl> taskItemControlsCache = new Dictionary<string, DeletedTaskItemControl>(); // 任务项视图缓冲

        private Point nextTaskItemPoint = new Point(0, 0);

        private DeletedTaskItemControl selectedTaskItemControl = null;

        private DeletedTaskItemControl focusedTaskItemControl = null;

        public DeletedTaskListControl()
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
            if (File.Exists(this.deletedTaskListConfigFile))
            {
                this.taskList = XMLTool.LoadTaskList(this.deletedTaskListConfigFile);
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
            DeletedTaskItemControl deletedTaskItemControl = null;
            if (!this.taskItemControlsCache.TryGetValue(taskItem.Name, out deletedTaskItemControl))
            {
                deletedTaskItemControl = new DeletedTaskItemControl(taskItem);
                this.taskItemControlsCache.Add(taskItem.Name, deletedTaskItemControl);

                // 设置Anchor
                deletedTaskItemControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Left)));

                // 为DeletedTaskItemControl添加Click事件
                deletedTaskItemControl.Click += new EventHandler(this.TaskItemControl_Click);
                // 添加任务还原事件，处理方法
                // 定义 TaskRestore 方法 
            }

            deletedTaskItemControl.Location = nextTaskItemPoint;

            // 留2像素的间隔
            nextTaskItemPoint.Y = nextTaskItemPoint.Y + deletedTaskItemControl.Height + 2;

            this.panel2.Controls.Add(deletedTaskItemControl);

        }

        // 添加一个已完成任务项
        public void Append(TaskItem taskItem)
        {
            this.taskList.Add(taskItem);
            this.RepaintTaskList();
        }

        // 重绘任务列
        public void RepaintTaskList()
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
            DeletedTaskItemControl taskItemControl = (DeletedTaskItemControl)sender;

            if (selectedTaskItemControl != taskItemControl)
            {
                if (selectedTaskItemControl != null)
                {
                    selectedTaskItemControl.BackColor = SystemColors.Window;
                }
                taskItemControl.BackColor = Color.PowderBlue;

                selectedTaskItemControl = taskItemControl;

                // 可删除任务
                this.label4.Enabled = true;
                // 可恢复任务
                this.label1.Enabled = true;
            }
        }

        // 点击恢复
        private void label1_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("您确定要恢复此任务吗？", "恢复确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (dialogResult == DialogResult.OK)
            {
                // 记录已恢复的任务名称
                List<string> restoredTaskNames = new List<string>();

                // 获取要恢复的任务所属列表视图
                Form1 form = (Form1)this.ParentForm;
                TaskListControl taskListControl = (TaskListControl)form.GetTaskListControl();
                CompletedTaskListControl completedTaskListControl = (CompletedTaskListControl)form.GetCompletedTaskListControl();

                foreach (DeletedTaskItemControl deletedTaskItemControl in this.taskItemControlsCache.Values)
                {
                    if (deletedTaskItemControl.CheckBox1.Checked)
                    {
                        // 更新任务列表信息到配置文件
                        XMLTool.DeleteTaskFromDeletedTaskList(Config.DeletedTaskListConfigFile, deletedTaskItemControl.TaskItem.taskPath);

                        // 删除已删除任务项
                        this.taskList.Remove(deletedTaskItemControl.TaskItem);

                        // 添加删除任务项（不能在枚举操作中修改集合）
                        restoredTaskNames.Add(deletedTaskItemControl.TaskItem.Name);

                        // 重绘恢复任务所属的任务列表视图
                        if (completedTaskListControl != null && deletedTaskItemControl.TaskItem.Status.Equals("已完成"))
                        {
                            // 向已完成任务列表配置文件添加恢复项
                            XMLTool.AddCompletedTaskList(Config.CompletedTaskListConfigFile, deletedTaskItemControl.TaskItem.taskPath);
                            completedTaskListControl.Append(deletedTaskItemControl.TaskItem);
                        }
                        else if (taskListControl != null)
                        {
                            // 向正在识别任务列表配置文件添加恢复项
                            XMLTool.AddTaskList(Config.TaskListConfigFile, deletedTaskItemControl.TaskItem.taskPath);
                            taskListControl.Append(deletedTaskItemControl.TaskItem);
                        }
                        //Console.WriteLine("删除------");

                    }  //if end
                }  //foreach end

                // 删除已恢复任务项的视图
                foreach (string restoredTaskName in restoredTaskNames)
                {
                    this.taskItemControlsCache.Remove(restoredTaskName);
                }

                // 重绘已完成任务列表视图
                this.RepaintTaskList();

                MessageBox.Show("已恢复！", "", MessageBoxButtons.OK);
            }
        }

        // 点击“删除”
        private void label4_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("您确定要删除此任务吗？", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (dialogResult == DialogResult.OK)
            {
                // 记录已删除的任务名称
                List<string> deletedTaskNames = new List<string>();
                foreach (DeletedTaskItemControl deletedTaskItemControl in this.taskItemControlsCache.Values)
                {
                    if (deletedTaskItemControl.CheckBox1.Checked)
                    {
                        // 更新任务列表信息到配置文件
                        XMLTool.DeleteTaskFromDeletedTaskList(Config.DeletedTaskListConfigFile, deletedTaskItemControl.TaskItem.taskPath);

                        // 删除已删除任务项
                        this.taskList.Remove(deletedTaskItemControl.TaskItem);

                        // 添加删除任务项（不能在枚举操作中修改集合）
                        deletedTaskNames.Add(deletedTaskItemControl.TaskItem.Name);
                    } // id end
                } // foreach end

                // 删除已删除任务项的视图
                foreach (string deletedTaskName in deletedTaskNames)
                {
                    this.taskItemControlsCache.Remove(deletedTaskName);
                }

                // 重绘已完成任务列表视图
                this.RepaintTaskList();

                MessageBox.Show("已删除！", "", MessageBoxButtons.OK);
            } // if end
        }

        // 点击“多选”
        private int label5ClickCount = 0; // 复选框点击次数
        private void label5_Click(object sender, EventArgs e)
        {
            if (this.label5ClickCount % 2 == 0)
            {
                foreach (DeletedTaskItemControl deletedTaskItemControl in this.taskItemControlsCache.Values)
                {
                    deletedTaskItemControl.CheckBox1.Visible = true;
                }
                this.label4.Enabled = true;
            }
            else
            {
                foreach (DeletedTaskItemControl deletedTaskItemControl in this.taskItemControlsCache.Values)
                {
                    deletedTaskItemControl.CheckBox1.Visible = false;
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

        private void label1_MouseEnter(object sender, EventArgs e)
        {
            DrawMouseHoverLable(label1, global::OcrTesseract.Properties.Resources.restore);
        }
        private void label1_MouseLeave(object sender, EventArgs e)
        {
            DrawMouseLeaveLable(label1, global::OcrTesseract.Properties.Resources.restore);
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
