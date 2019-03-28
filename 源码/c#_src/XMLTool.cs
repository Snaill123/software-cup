using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace OcrTesseract
{
    /// <summary>
    /// xml文件读写工具
    /// 注意读xml时，xml文件不能有注释信息
    /// </summary>
    class XMLTool
    {
        /// Task
        // 将task信息写入xml
        public static void Save(string xmlFile, TaskItem task)
        {
            XmlDocument xmlDoc = new XmlDocument();//新建XML文件  
            //XmlElement xm = null;

            // 文件不存在
            if (!File.Exists(xmlFile))
            {
                //@"E:\Visual Studio\source\repos\OcrTesseract\OcrTesseract\task\task_config_templet.xml"
                // 配置文件路径
                File.Copy(Config.TaskConfigTempletFile, xmlFile);
            }

            xmlDoc.Load(xmlFile); //加载XML文件  
            XmlElement root = xmlDoc.DocumentElement;

            SetTaskValue(root, task);

            xmlDoc.Save(xmlFile);
        }

        // 写入task信息
        private static void SetTaskValue(XmlNode taskNode, TaskItem task)
        {
            foreach (XmlElement node in taskNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "task-path":
                        node.InnerText = task.taskPath;
                        break;
                    case "name":
                        node.InnerText = task.Name;
                        break;
                    case "picture-path":
                        node.InnerText = task.picturePath;
                        break;
                    case "picture-count":
                        node.InnerText = task.pictureCount.ToString();
                        break;
                    case "picture-finished-count":
                        node.InnerText = task.pictureFinishedCount.ToString();
                        break;
                    case "progress":
                        node.InnerText = task.progress.ToString();
                        break;
                    case "runtime-length":
                        node.InnerText = task.runtimeLength;
                        break;
                    case "completion-rate":
                        node.InnerText = task.CompletionRate;
                        break;
                    case "status":
                        node.InnerText = task.Status;
                        break;
                    case "create-time":
                        node.InnerText = task.createTime;
                        break;
                    case "result-excel":
                        node.InnerText = task.resultExcel;
                        break;
                }
            }
        }

        // 更新task信息到xml文件
        public static void Update(string xmlFile, TaskItem task)
        {
            XMLTool.Save(xmlFile, task);
        }

        // 加载xml文件初始化task信息
        public static TaskItem Load(string xmlFile)
        {
            TaskItem task = null;

            XmlDocument xmlDoc = new XmlDocument();//新建XML文件  
            xmlDoc.Load(xmlFile);//加载XML文件  

            XmlElement root = xmlDoc.DocumentElement;

            task = GetTaskValue(root);

            return task;
        }

        // 读取task信息
        private static TaskItem GetTaskValue(XmlNode taskNode)
        {
            TaskItem task = new TaskItem();
            foreach (XmlElement node in taskNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "task-path":
                        task.taskPath = node.InnerText;
                        break;
                    case "name":
                        task.Name = node.InnerText;
                        break;
                    case "picture-path":
                        task.picturePath = node.InnerText;
                        break;
                    case "picture-count":
                        task.pictureCount = int.Parse(node.InnerText);
                        break;
                    case "picture-finished-count":
                        task.pictureFinishedCount = int.Parse(node.InnerText);
                        break;
                    case "progress":
                        task.progress = float.Parse(node.InnerText);
                        break;
                    case "runtime-length":
                        task.runtimeLength = node.InnerText;
                        break;
                    case "completion-rate":
                        task.CompletionRate = node.InnerText;
                        break;
                    case "status":
                        task.Status = node.InnerText;
                        break;
                    case "create-time":
                        task.createTime = node.InnerText;
                        break;
                    case "result-excel":
                        task.resultExcel = node.InnerText;
                        break;
                }
            }
            return task;
        }




        /// java Config
        // 生成java配置文件，用于初始化java运行环境
        public static void SaveJavaConfig(string javaConfigFile, TaskItem task)
        {
            XmlDocument xmlDoc = new XmlDocument();//新建XML文件

            // 文件不存在
            if (!File.Exists(javaConfigFile))
            {
                //@"E:\Visual Studio\source\repos\OcrTesseract\OcrTesseract\task\task_config_templet.xml"
                // 配置文件路径
                File.Copy(Config.JavaProgramConfigTempletFile, javaConfigFile);
            }

            xmlDoc.Load(javaConfigFile); //加载XML文件  
            XmlElement root = xmlDoc.DocumentElement;

            SetJavaProgramParamsValue(root, task);

            xmlDoc.Save(javaConfigFile);
        }

        // 写入java配置
        private static void SetJavaProgramParamsValue(XmlNode root, TaskItem task)
        {
            foreach (XmlElement node in root.ChildNodes)
            {
                switch (node.GetAttribute("name"))
                {
                    case "image_dir":                        
                        node.InnerText = task.picturePath;
                        break;
                    case "preprocessed_image_dir":
                        node.InnerText = task.taskPath + "\\preprocessed-images";
                        break;
                    case "excel_dir":
                        node.InnerText = task.taskPath + "\\result";
                        break;
                }
            }
        }

        

        /// TaskList
        // 加载xml文件初始化taskList信息
        public static List<TaskItem> LoadTaskList(string xmlFile)
        {
            List<TaskItem> taskList;

            XmlDocument xmlDoc = new XmlDocument();//新建XML文件
            xmlDoc.Load(xmlFile);//加载XML文件  

            XmlElement root = xmlDoc.DocumentElement;
            taskList = GetTaskList(root);

            return taskList;
        }

        // 读取taskList信息
        private static List<TaskItem> GetTaskList(XmlElement root)
        {
            List<TaskItem> taskList = new List<TaskItem>();
            List<string> taskPathList = new List<string>();

            foreach (XmlElement task in root.ChildNodes)
            {
                XmlElement taskPath = (XmlElement)task.FirstChild;
                taskList.Add(Load(taskPath.InnerText + "\\config.xml"));
            }

            return taskList;
        }

        // 添加taskList信息，并保存到xml
        public static void AddTaskList(string xmlFile, string taskPath)
        {
            // 文件不存在
            if (!File.Exists(xmlFile))
            {
                File.Copy(Config.TaskListConfigTempletFile, xmlFile);
            }

            XmlDocument xmlDoc = new XmlDocument();//新建XML文件  
            xmlDoc.Load(xmlFile);//加载XML文件  

            XmlElement root = xmlDoc.DocumentElement;

            XmlElement taskNode = xmlDoc.CreateElement("task");
            root.AppendChild(taskNode);

            XmlElement taskPathNode = xmlDoc.CreateElement("task-path");
            taskPathNode.InnerText = taskPath;
            taskNode.AppendChild(taskPathNode);

            xmlDoc.Save(xmlFile);
        }

        // 更新taskList(任务列表)信息到配置文件
        public static void UpdateTaskList(string xmlFile, string oldTaskPath, string newTaskPath)
        {
            XmlDocument xmlDoc = new XmlDocument();//新建XML文件  
            xmlDoc.Load(xmlFile);//加载XML文件  

            XmlElement root = xmlDoc.DocumentElement;

            XmlNodeList taskPathNodeList = root.GetElementsByTagName("task-path");
            foreach (XmlElement taskPathElement in taskPathNodeList)
            {
                if (taskPathElement.InnerText.Equals(oldTaskPath))
                {
                    taskPathElement.InnerText = newTaskPath;
                    xmlDoc.Save(xmlFile);
                }
            }
        }

        // 删除taskList(任务列表)的一个task(任务项)信息到配置文件
        public static void DeleteTaskFromTaskList(string xmlFile, string taskPath)
        {
            XmlDocument xmlDoc = new XmlDocument();//新建XML文件  
            xmlDoc.Load(xmlFile);//加载XML文件  

            XmlElement root = xmlDoc.DocumentElement;

            XmlNodeList taskPathNodeList = root.GetElementsByTagName("task-path");
            foreach (XmlElement taskPathElement in taskPathNodeList)
            {
                if (taskPathElement.InnerText.Equals(taskPath))
                {
                    XmlNode taskNode = taskPathElement.ParentNode;
                    root.RemoveChild(taskNode);

                    xmlDoc.Save(xmlFile);
                }
            }

            
        }



        /// CompletedTaskList
        // 添加taskList信息，并保存到xml
        public static void AddCompletedTaskList(string xmlFile, string taskPath)
        {
            // 文件不存在
            if (!File.Exists(xmlFile))
            {
                File.Copy(Config.CompletedTaskListConfigTempletFile, xmlFile);
            }

            XmlDocument xmlDoc = new XmlDocument();//新建XML文件  
            xmlDoc.Load(xmlFile);//加载XML文件  

            XmlElement root = xmlDoc.DocumentElement;

            XmlElement taskNode = xmlDoc.CreateElement("task");
            root.AppendChild(taskNode);

            XmlElement taskPathNode = xmlDoc.CreateElement("task-path");
            taskPathNode.InnerText = taskPath;
            taskNode.AppendChild(taskPathNode);

            xmlDoc.Save(xmlFile);
        }

        // 删除CompletedTaskList(任务列表)的一个task(任务项)信息到配置文件
        public static void DeleteTaskFromCompletedTaskList(string xmlFile, string taskPath)
        {
            XmlDocument xmlDoc = new XmlDocument();//新建XML文件  
            xmlDoc.Load(xmlFile);//加载XML文件  

            XmlElement root = xmlDoc.DocumentElement;

            XmlNodeList taskPathNodeList = root.GetElementsByTagName("task-path");
            foreach (XmlElement taskPathElement in taskPathNodeList)
            {
                if (taskPathElement.InnerText.Equals(taskPath))
                {
                    XmlNode taskNode = taskPathElement.ParentNode;
                    root.RemoveChild(taskNode);

                    xmlDoc.Save(xmlFile);
                }
            }
        }


        /// DeletedTaskList
        // 添加taskList信息，并保存到xml
        public static void AddDeletedTaskList(string xmlFile, string taskPath)
        {
            // 文件不存在
            if (!File.Exists(xmlFile))
            {
                File.Copy(Config.DeletedTaskListConfigTempletFile, xmlFile);
            }

            XmlDocument xmlDoc = new XmlDocument();//新建XML文件  
            xmlDoc.Load(xmlFile);//加载XML文件  

            XmlElement root = xmlDoc.DocumentElement;

            XmlElement taskNode = xmlDoc.CreateElement("task");
            root.AppendChild(taskNode);

            XmlElement taskPathNode = xmlDoc.CreateElement("task-path");
            taskPathNode.InnerText = taskPath;
            taskNode.AppendChild(taskPathNode);

            xmlDoc.Save(xmlFile);
        }

        // 删除DeletedTaskList(任务列表)的一个task(任务项)信息到配置文件
        public static void DeleteTaskFromDeletedTaskList(string xmlFile, string taskPath)
        {
            XmlDocument xmlDoc = new XmlDocument();//新建XML文件  
            xmlDoc.Load(xmlFile);//加载XML文件  

            XmlElement root = xmlDoc.DocumentElement;

            XmlNodeList taskPathNodeList = root.GetElementsByTagName("task-path");
            foreach (XmlElement taskPathElement in taskPathNodeList)
            {
                if (taskPathElement.InnerText.Equals(taskPath))
                {
                    XmlNode taskNode = taskPathElement.ParentNode;
                    root.RemoveChild(taskNode);

                    xmlDoc.Save(xmlFile);
                }
            }
        }


    }
}
