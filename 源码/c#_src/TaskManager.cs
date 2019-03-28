using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OcrTesseract
{
    /// <summary>
    /// 任务管理器。用于管理同一类任务，创建进程执行任务，并管理任务（开始、暂停、结束任务）
    /// </summary>
    class TaskManager
    {

        private List<JavaProcess> processList = new List<JavaProcess>();  // 任务列表

        // 构造函数
        public TaskManager()
        {

        }

        // 新建进程并启动
        public void NewProcess(JavaProcess javaProcess)
        {
            if (javaProcess != null)
            {
                // 添加进程
                this.processList.Add(javaProcess);
                // 启动进程
                this.NewThread(javaProcess);
            }

        }

        // 创建线程启动进程
        private void NewThread(JavaProcess javaProcess)
        {
            Thread thread = new Thread(this.ExecProcess);
            thread.Start(javaProcess);
        }

        // 启动线程
        private void ExecProcess(object process)
        {
            JavaProcess javaProcess = (JavaProcess)process;
            javaProcess.Start();
            this.ReadProcessStream(javaProcess);

            ///////
            Console.WriteLine("TaskManager ReadProcessStream End");
        }

        // 从线程读取数据
        public void ReadProcessStream(JavaProcess javaProcess)
        {
            javaProcess.ReadProcessStream();
        }

        // 向线程写入数据
        public void WriteProcessStream(JavaProcess javaProcess, string writeData)
        {
            javaProcess.WriteProcessStream(writeData);
        }

        // 暂停任务项对应的进程
        public void WaitProcess(TaskItem task)
        {
            JavaProcess javaProcess = this.GetProcess(task);
            if (javaProcess != null)
            {
                javaProcess.Wait();
            }
        }

        // 运行任务项对应的进程
        public void RunProcess(TaskItem task)
        {
            JavaProcess javaProcess = this.GetProcess(task);
            if (javaProcess != null)
            {
                javaProcess.Run();
            }
        }

        // 结束任务项对应的进程
        public void StopProcess(TaskItem task)
        {
            JavaProcess javaProcess = this.GetProcess(task);
            if (javaProcess != null)
            {
                javaProcess.Stop();
            }
        }

        // 判断任务项对应得进程是否结束
        public bool ProcessIsOver(TaskItem task)
        {
            JavaProcess javaProcess = this.GetProcess(task);
            if (javaProcess != null)
            {
                return javaProcess.IsOver();
            }
            return false;
        }

        // 获取任务项对应的进程
        private JavaProcess GetProcess(TaskItem task)
        {
            foreach (JavaProcess javaProcess in this.processList)
            {
                if (javaProcess.taskItem == task)
                {
                    return javaProcess;
                }
            }
            return null;
        }

    }
}
