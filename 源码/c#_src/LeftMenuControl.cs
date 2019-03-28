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
    public partial class LeftMenuControl : UserControl
    {
        //private Icon rightIcon = global::OcrTesseract.Properties.Resources.navigate_right;
        private Label lastSelectedLabel = null;
        private Label focusedLabel = null;
        private Image beforefocusedImage = null;

        private Color backColor = SystemColors.InactiveCaptionText;
        private Color foreColor = SystemColors.ControlDark;
        private Image lastSelectedIcon = null;
        /*
        private Image lastSelectedLabelIcon = null;
        private Color lastSelectedLabelForeColor = Color.Empty;
        private Color lastSelectedLabelBackColor = Color.Empty;
        */
        public LeftMenuControl()
        {
            InitializeComponent();
        }

        // 单击“正在识别”按钮
        public void label1_Click(object sender, EventArgs e)
        {
            if (lastSelectedLabel != label1)
            {
                // 显示样式逻辑
                if (lastSelectedLabel != null)
                {
                    DrawlastSelectedLabel(lastSelectedLabel, foreColor, lastSelectedIcon, backColor);
                }
                lastSelectedLabel = label1;
                if (label1 == focusedLabel)
                {
                    lastSelectedIcon = beforefocusedImage;
                }
                DrawlastSelectedLabel(label1, global::OcrTesseract.Properties.Resources.scanning_blue_1296db);
            }

            // 事务逻辑
            // 显示"正在识别"任务列表视图
            Form1 form = (Form1)this.ParentForm;
            Control taskListControl;
            if ((taskListControl = form.GetTaskListControl()) == null)
            {
                taskListControl = new TaskListControl();
            }

            form.ShowTaskListControl(taskListControl);
        }
        
        // 单击“已完成”按钮
        public void label2_Click(object sender, EventArgs e)
        {
            if (lastSelectedLabel != label2)
            {
                // 显示样式逻辑
                if (lastSelectedLabel != null)
                {
                    DrawlastSelectedLabel(lastSelectedLabel, foreColor, lastSelectedIcon, backColor);
                }
                lastSelectedLabel = label2;
                if (label2 == focusedLabel)
                {
                    lastSelectedIcon = beforefocusedImage;
                }
                DrawlastSelectedLabel(label2, global::OcrTesseract.Properties.Resources.success_blue_1296db);
            }

            // 事务逻辑
            // 显示"已完成"任务列表视图
            Form1 form = (Form1)this.ParentForm;
            Control completedTaskListControl;
            if ((completedTaskListControl = form.GetCompletedTaskListControl()) == null)
            {
                completedTaskListControl = new CompletedTaskListControl();
            }

            form.ShowTaskListControl(completedTaskListControl);
        }
        // 单击“垃圾箱”按钮
        public void label3_Click(object sender, EventArgs e)
        {
            if (lastSelectedLabel != label3)
            {
                // 显示样式逻辑
                if (lastSelectedLabel != null)
                {
                    DrawlastSelectedLabel(lastSelectedLabel, foreColor, lastSelectedIcon, backColor);
                }
                lastSelectedLabel = label3;
                if (label3 == focusedLabel)
                {
                    lastSelectedIcon = beforefocusedImage;
                }
                DrawlastSelectedLabel(label3, global::OcrTesseract.Properties.Resources.delete_blue_1296db);
            }

            // 事务逻辑
            //显示"垃圾箱"任务列表视图
            Form1 form = (Form1)this.ParentForm;
            Control deletedTaskListControl;
            if ((deletedTaskListControl = form.GetDeletedTaskListControl()) == null)
            {
                deletedTaskListControl = new DeletedTaskListControl();
            }

            form.ShowTaskListControl(deletedTaskListControl);
        }

        // 绘制选中的Label的ForeColor
        private void DrawlastSelectedLabel(Label label, Color foreColor, Image icon, Color backColor)
        {
            /*   if (lastSelectedLabel != null)
               {
                   lastSelectedLabel.Image = lastSelectedLabelIcon;
                   lastSelectedLabel.ForeColor = lastSelectedLabelForeColor;
                   lastSelectedLabel.BackColor = lastSelectedLabelBackColor;
               }

               // 记录Label修改前的属性
               lastSelectedLabel = label;
               lastSelectedLabelIcon = label.Image;
               lastSelectedLabelForeColor = label.ForeColor;
               lastSelectedLabelBackColor = label.BackColor;
               */
            // 更新Label样式
            label.Image = icon;
            label.ForeColor = foreColor;
            label.BackColor = backColor;
        }
        private void DrawlastSelectedLabel(Label label, Image icon)
        {/*
            if (lastSelectedLabel != null)
            {
                lastSelectedLabel.Image = lastSelectedLabelIcon;
                lastSelectedLabel.ForeColor = lastSelectedLabelForeColor;
                lastSelectedLabel.BackColor = lastSelectedLabelBackColor;
            }
            
            // 记录Label修改前的属性
            lastSelectedLabel = label;
            lastSelectedLabelIcon = label.Image;
            lastSelectedLabelForeColor = label.ForeColor;
            lastSelectedLabelBackColor = label.BackColor;
            */
            // 更新Label样式
            label.Image = icon;
            label.ForeColor = Color.DodgerBlue;
            label.BackColor = Color.FromArgb(38, 38, 38);
        }


        private void label1_MouseHover(object sender, EventArgs e)
        {
            if (lastSelectedLabel != label1 && focusedLabel != label1)
            {
                focusedLabel = label1;
                beforefocusedImage = label1.Image;
                DrawHeighLigtLabel(label1, global::OcrTesseract.Properties.Resources.scanning_highlight);
            }
        }
        private void label1_MouseLeave(object sender, EventArgs e)
        {
            if (lastSelectedLabel != label1)
            {
                focusedLabel = null;
                DrawHeighLigtLabel(label1, label1.Parent.BackColor, SystemColors.ControlDark, global::OcrTesseract.Properties.Resources.scanning);
            }
        }

        private void label2_MouseHover(object sender, EventArgs e)
        {
            if (lastSelectedLabel != label2 && focusedLabel != label2)
            {
                focusedLabel = label2;
                beforefocusedImage = label2.Image;
                DrawHeighLigtLabel(label2, global::OcrTesseract.Properties.Resources.success_highlight);
            }
        }
        private void label2_MouseLeave(object sender, EventArgs e)
        {
            if (lastSelectedLabel != label2)
            {
                focusedLabel = null;
                DrawHeighLigtLabel(label2, label2.Parent.BackColor, SystemColors.ControlDark, global::OcrTesseract.Properties.Resources.success);
            }
        }

        private void label3_MouseHover(object sender, EventArgs e)
        {
            if (lastSelectedLabel != label3 && focusedLabel != label3)
            {
                focusedLabel = label3;
                beforefocusedImage = label3.Image;
                DrawHeighLigtLabel(label3, global::OcrTesseract.Properties.Resources.delete_highlight);
            }
        }
        private void label3_MouseLeave(object sender, EventArgs e)
        {
            if (lastSelectedLabel != label3)
            {
                focusedLabel = null;
                DrawHeighLigtLabel(label3, label3.Parent.BackColor, SystemColors.ControlDark, global::OcrTesseract.Properties.Resources.delete);
            }
        }

        private void DrawHeighLigtLabel(Label label, Color backColor, Color foreColor, Image icon)
        {
            label.ForeColor = foreColor;
            label.BackColor = backColor;
            label.Image = icon;
        }
        private void DrawHeighLigtLabel(Label label, Image icon)
        {
            label.ForeColor = SystemColors.HighlightText;
            label.BackColor = Color.FromArgb(60, 60, 60);
            label.Image = icon;
        }
    }
}
