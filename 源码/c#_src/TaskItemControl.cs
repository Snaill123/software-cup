using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace OcrTesseract
{

    /// <summary>
    /// 任务视图
    /// </summary>
    public partial class TaskItemControl : UserControl
    {

        // 委托，用于不同线程间同步更新TaskItemControl控件内容
        delegate void TaskItemChangedCallback(TaskItem taskItem);
        // 委托，用于Timer到时时更新视图时间
        private delegate void TimerElapsedInvok(string value);

        // 委托
        public delegate void TaskAccomplishedHandler(TaskItem taskItem);
        // 事件
        public event TaskAccomplishedHandler TaskAccomplished = null;

        // 添加计时器
        private System.Timers.Timer timer = new System.Timers.Timer();
        // 运行时间
        private int time = 0;

        private TaskItem taskItem = null;

        public CheckBox CheckBox1
        {
            get => this.checkBox1;
        }

        public PictureBox PictureBox2
        {
            get => this.pictureBox2;
        }

        public TaskItem TaskItem
        {
            get => this.taskItem;
        }


        public TaskItemControl()
        {
            InitializeComponent();

            //设置timer可用
            timer.Enabled = true;

            //设置timer
            timer.Interval = 1000;

            //设置是否重复计时，如果该属性设为False,则只执行timer_Elapsed方法一次。
            timer.AutoReset = true;

            timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);

            // 开始计时
            timer.Start();
        }

        public TaskItemControl(TaskItem taskItem) : this()
        {
            this.taskItem = taskItem;
            this.taskItem.TaskItemChanged += new TaskItem.TaskItemChangedHandler(this.TaskItemChanged);
        }

        // 定时开始
        public void Timer_Start()
        {
            this.timer.Start();
        }

        // 定时停止
        public void Timer_Stop()
        {
            this.timer.Stop();
        }

        // Timer到时（1秒）
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.time++;
            string strTime = GetStrTime(this.time);

            if (this.InvokeRequired)
            {
                TimerElapsedInvok d = new TimerElapsedInvok(this.SetTime);
                this.Invoke(d, strTime);
            }
            else
            {
                this.SetTime(strTime);
            }

            // 记录运行时间
            this.taskItem.runtimeLength = strTime;
        }

        // 将数值时间（秒），转为字符时分秒
        private string GetStrTime(int time)
        {
            string hour = ((time / 3600) % 24).ToString("00");
            string minute = ((time / 60) % 60).ToString("00");
            string second = (time % 60).ToString("00");
            return hour + ":" + minute + ":" + second;
        }

        // 设置label5显示的时间
        private void SetTime(string time)
        {
            this.label5.Text = time;
        }

        private void TaskItemControl_Load(object sender, EventArgs e)
        {
            if (taskItem != null)
            {
                this.Width = this.Parent.Width;

                this.label1.Text = taskItem.Name.Length > 15 ? taskItem.Name.Substring(0, 15) + "...." : taskItem.Name;
                this.label2.Text = taskItem.pictureCount.ToString();
                this.label4.Text = taskItem.Status.Length > 15 ? taskItem.Status.Substring(0, 15) + "...." : taskItem.Status;
                this.label5.Text = taskItem.runtimeLength;
                this.label6.Text = taskItem.CompletionRate;
                this.progressBar1.Value = (int)taskItem.progress;

                //Console.WriteLine("TaskItemControl_Load ");
            }
        }

        /* 
         // 显示任务详情页
         private void pictureBox2_Click(object sender, EventArgs e)
         {
             Console.WriteLine("pictureBox2_Click");
         }
         */

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
                this.label6.Text = taskItem.CompletionRate;
                this.label4.Text = taskItem.Status.Length > 15 ? taskItem.Status.Substring(0, 15) + "...." : taskItem.Status;
                this.progressBar1.Value = (int)taskItem.progress;
                // 任务完成时触发事件，通知TaskListControl处理
                if (this.taskItem.Status.Equals("已完成") && this.TaskAccomplished != null)
                {
                    this.TaskAccomplished(this.taskItem);
                }
            }
        }
        /*
        private void TaskItemControl_Click(object sender, EventArgs e)
        {
            if (lastSelectedTaskItemControl != this)
            {
                if (lastSelectedTaskItemControl != null)
                {
                    lastSelectedTaskItemControl.BackColor = SystemColors.Window;
                    lastSelectedTaskItemControl.pictureBox2.Visible = false;
                }
                this.BackColor = Color.PowderBlue;
                this.pictureBox2.Visible = true;

                // 通知父容器
                ((TaskListControl)(this.Parent)).SelectedTaskItemControl = this;
            }
        }

        private void TaskItemControl_MouseHover(object sender, EventArgs e)
        {
            this.BackColor = Color.PowderBlue;
            // 通知父容器
            ((TaskListControl)(this.Parent)).focusedTaskItemControl = this;
        }

        private void TaskItemControl_MouseLeave(object sender, EventArgs e)
        {
            this.BackColor = SystemColors.Window;
        }
                */
    }
}
