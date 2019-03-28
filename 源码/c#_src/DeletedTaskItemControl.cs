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
    public partial class DeletedTaskItemControl : UserControl
    {
        private TaskItem taskItem = null;

        public TaskItem TaskItem
        {
            get => this.taskItem;
        }

        public CheckBox CheckBox1 { get => this.checkBox1; }

        public DeletedTaskItemControl()
        {
            InitializeComponent();
        }

        public DeletedTaskItemControl(TaskItem taskItem):this()
        {
            this.taskItem = taskItem;
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

    }
}
