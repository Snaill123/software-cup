using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OcrTesseract
{
    class Config
    {
        /// <summary>
        /// 当前应用程序所在目录的路径，最后包含‘\’
        /// </summary>
        public static readonly string AppBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// 应用程序task(任务)的路径，最后包含‘\’
        /// </summary>
        public static readonly string AppTaskDirectory = AppBaseDirectory + @"task\";

        /// <summary>
        /// task(任务)识别结果保存路径，最后包含‘\’
        /// </summary>
        //public static readonly string TaskResultDirectory = AppTaskDirectory + @"result\";

        /// <summary>
        /// TaskListConfigTemplet文件路径
        /// </summary>
        public static readonly string TaskListConfigTempletFile = AppTaskDirectory + "task_list_config_templet.xml";

        /// <summary>
        /// TaskConfigTemplet文件路径
        /// </summary>
        public static readonly string TaskConfigTempletFile = AppTaskDirectory + "task_config_templet.xml";

        /// <summary>
        /// TaskListConfig（任务列表）配置文件路径
        /// </summary>
        public static readonly string TaskListConfigFile = AppTaskDirectory + "task_list_config.xml";

        /// <summary>
        /// CompletedTaskListConfigTemplet文件路径
        /// </summary>
        public static readonly string CompletedTaskListConfigTempletFile = AppTaskDirectory + "completed_task_list_config_templet.xml";

        /// <summary>
        /// CompletedTaskListConfig（已完成任务列表）配置文件路径
        /// </summary>
        public static readonly string CompletedTaskListConfigFile = AppTaskDirectory + "completed_task_list_config.xml";

        /// <summary>
        /// DeletedTaskListConfigTemplet文件路径
        /// </summary>
        public static readonly string DeletedTaskListConfigTempletFile = AppTaskDirectory + "deleted_task_list_config_templet.xml";

        /// <summary>
        /// DeletedTaskListConfig（已删除任务列表）配置文件路径
        /// </summary>
        public static readonly string DeletedTaskListConfigFile = AppTaskDirectory + "deleted_task_list_config.xml";

        /// <summary>
        /// Java Jar文件位置
        /// 通过 "" 包围路径，防止java无法识别带空格的路径
        /// </summary>
        /// 
        public static readonly string JarFile = "\"" + AppBaseDirectory + @"java\" + "tesseract_ocr-1.0.0.jar" + "\"";

        /// <summary>
        /// java配置文件模板，用于初始化java运行环境
        /// </summary>
        public static readonly string JavaProgramConfigTempletFile = AppTaskDirectory + "java_program_config_templet.xml";
    }
}
