using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OcrTesseract
{
    public partial class Form1 : Form
    {
        private LeftMenuControl leftMenuControl = null;  // 左菜单视图 
        private ControlCache controlCache = new ControlCache();

        public Form1()
        {
            InitializeComponent();

            // 设置图标
            this.Icon = System.Drawing.Icon.FromHandle(global::OcrTesseract.Properties.Resources.logo.GetHicon());

            // 显示左菜单视图
            LeftMenuControl leftMenuControl = new LeftMenuControl();
            this.leftMenuControl = leftMenuControl;
            ShowLeftMenuControl(leftMenuControl);
            
            // 添加“正在识别”任务列表视图到缓冲中
            TaskListControl taskListControl = new TaskListControl();
            this.controlCache.TaskListControl = taskListControl;
            // 添加事件完成响应
            taskListControl.ModifyCompletedConfig += new TaskListControl.ModifyCompletedConfigHandler(this.UpdateCompletedTaskListControl);

            // 触发点击“正在识别”按钮，显示正在识别任务列表视图
            this.leftMenuControl.label1_Click(this, new EventArgs());
            // 显示任务列表视图
            //ShowTaskListControl(taskListControl);
        }

        // 更新CompletedTaskListControl视图（添加新的已完成任务）
        private void UpdateCompletedTaskListControl(TaskItem taskItem)
        {
            CompletedTaskListControl completedTaskListControl = (CompletedTaskListControl)this.GetCompletedTaskListControl();
            if (completedTaskListControl != null)
            {
                completedTaskListControl.Append(taskItem);
            }
        }

        // 显示左菜单视图
        public void ShowLeftMenuControl(LeftMenuControl leftMenuControl)
        {
            leftMenuControl.Dock = System.Windows.Forms.DockStyle.Fill;
            leftMenuControl.Location = new System.Drawing.Point(0, 0);
            this.panel1.Controls.Clear();
            this.panel1.Controls.Add(leftMenuControl);
        }

        // 显示任务列表视图
        public void ShowTaskListControl(Control taskListControl)
        {
            // 将视图添加到缓冲中
            if (taskListControl is TaskListControl)
            {
                this.controlCache.TaskListControl = taskListControl;
            }
            else if (taskListControl is CompletedTaskListControl)
            {
                this.controlCache.CompletedTaskListControl = taskListControl;
            }
            else
            {
                //this.controlCache.DeletedTaskListControl = del
            }

            // 显示视图
            taskListControl.Dock = System.Windows.Forms.DockStyle.Fill;
            taskListControl.Location = new System.Drawing.Point(0, 0);
            this.panel2.Controls.Clear();
            this.panel2.Controls.Add(taskListControl);
            
        }

        // 显示任务详情视图
        public void ShowTaskDetailControl(TaskDetailControl taskDetailControl)
        {
            taskDetailControl.Dock = System.Windows.Forms.DockStyle.Fill;
            taskDetailControl.Location = new System.Drawing.Point(0, 0);
            this.panel2.Controls.Clear();
            this.panel2.Controls.Add(taskDetailControl);
        }


        // 获取左菜单视图
        public LeftMenuControl GetLeftMenuControl()
        {
            return this.leftMenuControl;
        }

        // 获取缓冲的“正在识别”任务列表视图
        public Control GetTaskListControl()
        {
            return this.controlCache.TaskListControl;
        }

        // 添加“正在识别”任务列表视图到缓冲中
        public void SetTaskListControl(Control taskListControl)
        {
            this.controlCache.TaskListControl = taskListControl;
        }

        // 获取缓冲的“已完成”任务列表视图
        public Control GetCompletedTaskListControl()
        {
            return this.controlCache.CompletedTaskListControl;
        }

        // 添加“已完成”任务列表视图到缓冲中
        public void SetCompletedTaskListControl(Control completedTaskListControl)
        {
            this.controlCache.CompletedTaskListControl = completedTaskListControl;
        }

        // 获取缓冲的“垃圾箱”任务列表视图
        public Control GetDeletedTaskListControl()
        {
            return this.controlCache.DeletedTaskListControl;
        }

        // 添加“垃圾箱”任务列表视图到缓冲中
        public void SetDeletedTaskListControl(Control deletedTaskListControl)
        {
            this.controlCache.DeletedTaskListControl = deletedTaskListControl;
        }

        // 显示“系统设置”窗口
        public void ShowSystemSettingForm()
        {
            SystemSettingForm systemSettingForm;
            if ((systemSettingForm = this.controlCache.SystemSettingForm) == null)
            {
                systemSettingForm = new SystemSettingForm();
            }
            this.controlCache.SystemSettingForm = systemSettingForm;
            systemSettingForm.ShowDialog();
        }


    }
}
