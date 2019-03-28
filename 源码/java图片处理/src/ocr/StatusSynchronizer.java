package ocr;

import java.util.ArrayList;
import java.util.Scanner;

public class StatusSynchronizer implements Runnable{

	public static final Object LOCK = new Object();
	public static String signal = Signal.RUN;
	public static Thread[] threads;
	public static Object mainThread = null;
	
	// 线程等待
	public static void toWait()
	{
		synchronized (StatusSynchronizer.LOCK) {
			try {
				StatusSynchronizer.LOCK.wait();
			} catch (InterruptedException e) {
				System.err.println("toWait 出错！");
				e.printStackTrace();
			}
		}
	}
	
	// 所有线程等待
	public static void allWait(Object mainThread) {
		if (mainThread instanceof ImageRecognition)
		{
			synchronized (StatusSynchronizer.LOCK) {
				StatusSynchronizer.signal = Signal.WAIT;
			}
		}
	}
	
	// 主线程唤醒其他线程
	public static void toNotifyAll(Object mainThread)
	{
		if (mainThread instanceof ImageRecognition)
		{
			synchronized (StatusSynchronizer.LOCK)
			{
				StatusSynchronizer.signal = Signal.RUN;
				StatusSynchronizer.LOCK.notifyAll();
			}
		}
	}
	
	// 主线程结束程序
	public static void toExit(Object mainThread)
	{
		if (mainThread instanceof ImageRecognition)
		{
			System.out.println("java 程序已退出！");
			System.exit(0);
		}
	}

	@Override
	public void run() {
		while (true)
		{
			getSignalAndReact();
			
			/*
			try {
				Thread.sleep(5000);
			} catch (InterruptedException e) {
				e.printStackTrace();
			}
			// 判断程序是否结束
			if (StatusSynchronizer.isOver())
			{
				StatusSynchronizer.toExit(StatusSynchronizer.mainThread);
			}
			*/
		}
		
	}
	
	// 程序是否运行结束
	public static boolean isOver() {
		for (Thread t : StatusSynchronizer.threads) {
			if (t.getState() != Thread.State.TERMINATED)
			{
				return false;
			}
		}
		return true;
	}
	
	// 获取信号并作出响应
	public static void getSignalAndReact()
	{
		Scanner scanner = new Scanner(System.in);
		String sign;
		sign = scanner.nextLine();
		switch(sign)
		{
		case Signal.WAIT: StatusSynchronizer.allWait(StatusSynchronizer.mainThread);break;
		case Signal.RUN: StatusSynchronizer.toNotifyAll(StatusSynchronizer.mainThread);break;
		case Signal.STOP: StatusSynchronizer.toExit(StatusSynchronizer.mainThread);break;
		}
	}
	
/*
	// 获取信号
	public static void getSignal()
	{
		Scanner scanner = new Scanner(System.in);
		String signal;
		while (!StatusSynchronizer.isOver())
		{
			signal = scanner.nextLine();
			switch(signal)
			{
			case Signal.WAIT: StatusSynchronizer.allWait(StatusSynchronizer.mainThread);break;
			case Signal.RUN: StatusSynchronizer.toNotifyAll(StatusSynchronizer.mainThread);break;
			case Signal.STOP: StatusSynchronizer.toExit(StatusSynchronizer.mainThread);break;
			}
		}
	}
*/
}
