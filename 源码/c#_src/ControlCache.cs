using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OcrTesseract
{
    /// <summary>
    /// 用于缓冲任务列表视图
    /// </summary>
    class ControlCache
    {
        // “正在识别”任务列表视图
        private Control taskListControl = null;
        public Control TaskListControl { get => taskListControl; set => taskListControl = value; }
        
        // “正已完成”任务列表视图
        private Control completedTaskListControl = null;
        public Control CompletedTaskListControl { get => completedTaskListControl; set => completedTaskListControl = value; }
        
        // “垃圾箱”任务列表视图
        private Control deletedTaskListControl = null;
        public Control DeletedTaskListControl { get => deletedTaskListControl; set => deletedTaskListControl = value; }
       
        // “系统设置”视图
        private SystemSettingForm systemSettingForm = null;
        public SystemSettingForm SystemSettingForm { get => systemSettingForm; set => systemSettingForm = value; }

    }
}
