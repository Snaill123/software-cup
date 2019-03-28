using System;
using System.IO;

namespace OcrTesseract
{
    /// <summary>
    /// 任务项
    /// 描述任务项包含的信息
    /// </summary>
    public class TaskItem
    {
        // 委托
        public delegate void TaskItemChangedHandler(TaskItem taskItem);
        // 事件
        public event TaskItemChangedHandler TaskItemChanged = null;

        public TaskItem()
        {

        }

        public TaskItem(string name, string picturePath)
        {            
            this.name = name;   // 任务名称
            this.taskPath = Config.AppTaskDirectory + this.name; // 任务配置文件路径
            this.picturePath = picturePath;  // 识别的图片文件夹
            this.pictureCount = GetPictureCount();  // 图片数量
            this.pictureFinishedCount = 0;  // 已识别图片的数量
            this.progress = 0.0F;
            this.runtimeLength = "0";
            this.completionRate = "0/" + pictureCount;
            this.status = "正在创建任务";
            this.createTime = DateTime.Now.ToString();
            this.resultExcel = ""; //taskPath + @"\result.xls"; //@"E:\Visual Studio\source\repos\OcrTesseract\OcrTesseract\Resources\result\temp.xlsx";
        }

        public TaskItem(string name, string picturePath, int pictureCount, int pictureFinishedCount, float progress, string runtimeLength, string status, string creatTime, string resultExcel)
        {
            this.name = name;
            this.picturePath = picturePath;
            this.pictureCount = pictureCount;
            this.pictureFinishedCount = pictureFinishedCount;
            this.completionRate = "" + this.pictureFinishedCount + "/" + this.pictureCount;
            this.progress = progress;
            this.runtimeLength = runtimeLength;
            this.status = status;
            this.createTime = createTime;
            this.resultExcel = resultExcel;
        }

        /// <summary>
        /// 任务路径
        /// </summary>
        public string taskPath
        {
            get;
            set;
        }

        private string name;
        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                if (this.TaskItemChanged != null)
                {
                    this.TaskItemChanged(this);
                }
            }
        }

        /// <summary>
        /// 图片文件夹路径
        /// </summary>
        public string picturePath
        {
            get;
            set;
        }

        /// <summary>
        /// 图片数量
        /// </summary>
        public int pictureCount
        {
            get;
            set;
        }

        /// <summary>
        /// 已识别图片的数量
        /// </summary>
        public int pictureFinishedCount
        {
            get;
            set;
        }

        /// <summary>
        /// 任务进度
        /// </summary>
        public float progress
        {
            get;
            set;
        }

        /// <summary>
        /// 运行时长
        /// </summary>
        public string runtimeLength
        {
            get;
            set;
        }

        /// <summary>
        /// 完成比例
        /// </summary>
        private string completionRate;
        public string CompletionRate
        {
            get
            {
                return "" + this.pictureFinishedCount + "/" + this.pictureCount;
            }
            set
            {
                this.completionRate = value;
                if (this.TaskItemChanged != null)
                {
                    this.TaskItemChanged(this);
                }
            }
        }

        private string status;
        /// <summary>
        /// 任务当前运行状态
        /// </summary>
        public string Status
        {
            get
            {
                return this.status;
            }
            set
            {
                this.status = value;
                if (this.TaskItemChanged != null)
                {
                    this.TaskItemChanged(this);
                }
            }
        }

        /// <summary>
        /// 任务创建时间
        /// </summary>
        public string createTime
        {
            get;
            set;
        }

        /// <summary>
        /// 用户保存识别结果的最终excel文件
        /// </summary>
        public string resultExcel
        {
            get;
            set;
        }

        /// <summary>
        /// 获取指定目录下的图片数量
        /// </summary>
        /// <returns></returns>
        private int GetPictureCount()
        {
            int count = 0;
            string[] files = Directory.GetFiles(this.picturePath);
            foreach(string file in files)
            {
                string lowerCase = file.ToLower();
                if (lowerCase.EndsWith(".jpg") | lowerCase.EndsWith(".jpeg") | lowerCase.EndsWith(".png") | lowerCase.EndsWith(".bmp") | lowerCase.EndsWith(".tif") | lowerCase.EndsWith(".tiff"))
                {
                    count++;
                }
                Console.WriteLine(file);
            }

            return count;
        }
    }
}