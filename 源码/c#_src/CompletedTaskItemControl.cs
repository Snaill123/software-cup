using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OcrTesseract
{

    /// <summary>
    /// 任务视图
    /// </summary>
    public partial class CompletedTaskItemControl : UserControl
    {
        // 委托，用于不同线程间同步更新TaskItemControl控件内容
        delegate void TaskItemChangedCallback(TaskItem taskItem);

        // 委托
        public delegate void TaskRenamedHandler(TaskItem taskItem);
        // 事件
        public event TaskRenamedHandler TaskRenamed = null;

        private TaskItem taskItem = null;

        public PictureBox PictureBox2
        {
            get => this.pictureBox2;
        }

        public CheckBox CheckBox1
        {
            get => this.checkBox1;
        }

        public TaskItem TaskItem
        {
            get => this.taskItem;
        }
        

        public CompletedTaskItemControl()
        {
            InitializeComponent();
        }

        public CompletedTaskItemControl(TaskItem taskItem):this()
        {
            this.taskItem = taskItem;
            this.taskItem.TaskItemChanged += new TaskItem.TaskItemChangedHandler(this.TaskItemChanged);
        }

        private void TaskItemControl_Load(object sender, EventArgs e)
        {
            if(taskItem != null)
            {
                this.Width = this.Parent.Width;

                this.label1.Text = taskItem.Name.Length > 15? taskItem.Name.Substring(0, 15) + "...." : taskItem.Name;
                this.label2.Text = taskItem.pictureCount.ToString();
                this.label3.Text = TaskItem.createTime;
            }
        }

        // taskItem 更新时更新视图
        private void TaskItemChanged(TaskItem taskItem)
        {
            if (this.InvokeRequired)
            {
                TaskItemChangedCallback d = new TaskItemChangedCallback(TaskItemChanged);
                this.Invoke(d, new object[] { taskItem });
            }
            else
            {
                this.label1.Text = taskItem.Name.Length > 10 ? taskItem.Name.Substring(0, 10) + "...." : taskItem.Name;
                // 任务重命名时触发事件，通知TaskListControl处理
                if (this.TaskRenamed != null)
                {
                    this.TaskRenamed(this.taskItem);
                    // TODO:还没写，比较复杂（需要修改任务文件夹名称等）
                }
            }
        }


    }
}
