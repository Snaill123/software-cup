using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OcrTesseract
{
    class JavaProcess
    {
        public TaskItem taskItem = null;  // 绑定的任务

        public string cmd { get; set; }  //运行同意类任务的命令
        private string commonArgs { get; set; }  // 命令公用（都含）的参数
        private string customArgs { get; set; }  // 每个任务自定义的参数

        private Process process = null;
        private StreamReader stdOut = null;
        private StreamReader stdError = null;
        private StreamWriter stdIn = null;

        public StreamReader StdOut { get => stdOut; }
        public StreamReader StdError { get => stdError; }
        public StreamWriter StdIn { get => stdIn; }

        public JavaProcess(string cmd, string commonArgs, string customArgs, TaskItem taskItem)
        {
            this.cmd = cmd;
            this.commonArgs = commonArgs;
            this.customArgs = customArgs;
            this.taskItem = taskItem;

            // 配置运行环境
            this.ConfigEnviroment();
        }

        public JavaProcess(string customArgs, TaskItem taskItem)
        {
            // 初始化cmdh和common
            this.cmd = "java";
            this.commonArgs = " -jar " + Config.JarFile;
            this.customArgs = " " + customArgs;
            //
            Console.WriteLine(commonArgs);
            Console.WriteLine(customArgs);

            this.taskItem = taskItem;

            // 配置运行环境
            this.ConfigEnviroment();
        }

        // 配置运行环境
        private void ConfigEnviroment()
        {
            if (this.process == null)
            {
                // 创建进程
                this.process = new Process();
                // 配置执行环境
                process.StartInfo.FileName = this.cmd;
                process.StartInfo.Arguments = this.commonArgs + this.customArgs;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardError = true;

            }
        }

        // 启动进程
        public void Start()
        {
            this.taskItem.Status = "正在识别";
            this.process.Start();
        }

        // 暂停进程
        public void Wait()
        {
            this.taskItem.Status = "暂停";
            this.WriteProcessStream(Signal.WAIT);
        }

        // 运行进程
        public void Run()
        {
            this.taskItem.Status = "正在识别";
            this.WriteProcessStream(Signal.RUN);
        }

        // 结束进程
        public void Stop()
        {
            this.WriteProcessStream(Signal.STOP);
        }

        // 判断进程是否结束
        public bool IsOver()
        {
            return this.process.HasExited;
        }

        // 从线程读取数据
        public void ReadProcessStream()
        {
            Console.WriteLine("//////////ReadProcessStream Start/////////");
            if (this.stdOut == null || this.stdError == null)
            {
                // 获取流
                this.stdOut = this.process.StandardOutput;
                this.stdError = this.process.StandardError;
            }

            string strLine = null;
            int index;
            string imageFile;
            string imageName;
            while ((strLine = stdOut.ReadLine()) != null)
            {
                Console.WriteLine("Java 程序输出(out)：" + strLine);

                index = strLine.IndexOf("IMAGE_FILE:");
                if (index != -1)
                {
                    imageFile = strLine.Replace("IMAGE_FILE:", "");
                    imageName = Path.GetFileName(imageFile);

                    this.taskItem.pictureFinishedCount++;
                    this.taskItem.CompletionRate = "" + this.taskItem.pictureFinishedCount + "/" + this.taskItem.pictureCount;
                    this.taskItem.Status = "正在识别: " + imageName;
                    // 问题：当达到100%时进度条不满（可能是因为速度太快没触发Step值自增）
                    this.taskItem.progress = (int)(((double)(this.taskItem.pictureFinishedCount)) / this.taskItem.pictureCount * 100);

                    //
                    Console.WriteLine(this.taskItem.progress + "%");
                }
            }

            while ((strLine = stdError.ReadLine()) != null)
            {
                Console.WriteLine("Java 程序输出(err)：" + strLine);
            }

            if (this.taskItem.pictureFinishedCount == this.taskItem.pictureCount)
            {
                this.taskItem.Status = "已完成";
            }
            //
            Console.WriteLine("//////////ReadProcessStream End/////////");
            //this.process.WaitForExit();
        }

        // 向线程写入数据
        public void WriteProcessStream(string writeData)
        {
            if (this.stdIn == null)
            {
                // 获取流
                this.stdIn = this.process.StandardInput;
            }
            this.stdIn.WriteLine(writeData);
        }

    }
}
