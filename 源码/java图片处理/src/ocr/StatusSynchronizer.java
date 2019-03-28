package ocr;

import java.util.ArrayList;
import java.util.Scanner;

public class StatusSynchronizer implements Runnable{

	public static final Object LOCK = new Object();
	public static String signal = Signal.RUN;
	public static Thread[] threads;
	public static Object mainThread = null;
	
	// �̵߳ȴ�
	public static void toWait()
	{
		synchronized (StatusSynchronizer.LOCK) {
			try {
				StatusSynchronizer.LOCK.wait();
			} catch (InterruptedException e) {
				System.err.println("toWait ����");
				e.printStackTrace();
			}
		}
	}
	
	// �����̵߳ȴ�
	public static void allWait(Object mainThread) {
		if (mainThread instanceof ImageRecognition)
		{
			synchronized (StatusSynchronizer.LOCK) {
				StatusSynchronizer.signal = Signal.WAIT;
			}
		}
	}
	
	// ���̻߳��������߳�
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
	
	// ���߳̽�������
	public static void toExit(Object mainThread)
	{
		if (mainThread instanceof ImageRecognition)
		{
			System.out.println("java �������˳���");
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
			// �жϳ����Ƿ����
			if (StatusSynchronizer.isOver())
			{
				StatusSynchronizer.toExit(StatusSynchronizer.mainThread);
			}
			*/
		}
		
	}
	
	// �����Ƿ����н���
	public static boolean isOver() {
		for (Thread t : StatusSynchronizer.threads) {
			if (t.getState() != Thread.State.TERMINATED)
			{
				return false;
			}
		}
		return true;
	}
	
	// ��ȡ�źŲ�������Ӧ
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
	// ��ȡ�ź�
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
