package ocr;

//import java.io.BufferedReader;
import java.io.File;
import java.io.IOException;
import java.util.HashMap;
//import java.io.FileInputStream;
//import java.io.FileReader;
//import java.io.IOException;
//import java.io.InputStreamReader;
import java.util.LinkedList;
import java.util.Scanner;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.xml.sax.SAXException;

public class ImageRecognition {
	public static final String LANG_OPTION = "-l";
	public static final String OCR_LANG_DATA = "chi_sim";
	public static final String OCR_COMMAND = "tesseract";
	public static String IMAGE_DIR = null; // 图片文件位置
	private static String PREPROCESSED_IMAGE_DIR = null; // 预处理后图片存储位置
	private static String EXCEL_DIR = null; // 识别结果excel文件存储位置
	
	public static LinkedList<EnterpriseInfo> highConfidenceList = new LinkedList<EnterpriseInfo>();
	public static LinkedList<EnterpriseInfo> lowConfidenceList = new LinkedList<EnterpriseInfo>();
	
	public static StringBuffer strBuffer = new StringBuffer();	// 存储图片文字信息,线程安全的可变字符序列
	public static LinkedList<File> resultImgList = new LinkedList<File>();  //记录已预处理好的图片文件
	public static boolean preprocessImageIsCompleted = false;
	public static int recognizeImageThreadCount;

/*
	public ImageRecognition(int recognizeImageThreadCount)
	{
		this.recognizeImageThreadCount = recognizeImageThreadCount;
	}
*/
	public static void main(String[] args)
	{
		
		System.out.println("程序已启动！");
		
		// 获取初始化文件，初始化程序
		String initFile = args[0];
		
		try{
			init(initFile);
		}catch (SAXException e1) {
			e1.printStackTrace();
			System.out.println("java读取初始化文件失败！");
			System.exit(0);
		} catch (IOException e1) {
			e1.printStackTrace();
			System.out.println("java读取初始化文件失败！");
			System.exit(0);
		}
		
		System.out.println("配置文件已读取！");
		
	    // 获取开始时间
		long startTime = System.currentTimeMillis();
		
		// 原始图片存储位置
		File srcImageDir = new File(ImageRecognition.IMAGE_DIR);
		
		// 预处理后产生的图片存储位置
		File resultImageDir = new File(ImageRecognition.PREPROCESSED_IMAGE_DIR);
		if (!resultImageDir.exists())
		{
			resultImageDir.mkdirs();
		}
		
		// 创建5个线程，其中一个线程预处理图片，其余的识别图片识别预处理后的图片
		ImageRecognition.recognizeImageThreadCount = 4;
		Thread[] threads = new Thread[ImageRecognition.recognizeImageThreadCount+1];
		
		// 创建图片预处理线程处理图片
		threads[0] = new Thread(new PreprocessingImageThread(srcImageDir, resultImageDir));
		threads[0].setName("PreprocessingImgThread");
		threads[0].start();
		
		// 创建4个线程识别图片文字
		for (int i = 1; i < threads.length; i++)
		{
			threads[i] = new Thread(new RecognizeImageThread());
			threads[i].setName("RecognizeImageThread" + i);
			threads[i].start();
		}
		
		System.out.println("识别进程已创建！");
		
		// 接收来自c#的控制
		ImageRecognition imageRecognition = new ImageRecognition();
		StatusSynchronizer.threads = threads;
		StatusSynchronizer.mainThread = imageRecognition;
		Thread daemon = new Thread(new StatusSynchronizer());  //创建后台进程
		daemon.setDaemon(true); // Must call before start() 
		daemon.start(); // run 方法接收（c#）控制信号
		
				
		
		// 等待所有线程执行完成（不包含StatusSynchronizer线程）
		for (int i = 0; i < threads.length; i++)
		{
			try {
				threads[i].join();
			} catch (InterruptedException e) {
				e.printStackTrace();
			}
		}
		
		// 将识别结果的格式化信息写入excel文件
		saveToExcel();		
		
		// 打印识别的所有图片文字信息
		System.out.println(ImageRecognition.strBuffer.toString());
		
		long endTime = System.currentTimeMillis();    //获取结束时间
		System.out.println("程序运行时间：" + (endTime - startTime)/1000 + "s");    //输出程序运行时间

	}
/*
	// 结束程序
	private static void exit(ImageRecognition imageRecognition) {
		StatusSynchronizer.toExit(imageRecognition);
	}

	// 唤醒程序（所有线程）
	private static void allNotify(ImageRecognition imageRecognition) {
		StatusSynchronizer.toNotifyAll(imageRecognition);
	}
	
	// 所有线程等待
	private static void allWait() {
		synchronized (StatusSynchronizer.LOCK) {
			StatusSynchronizer.signal = Signal.WAIT;
		}
	}
*/
	// 获取初始化文件，初始化程序
	private static void init(String initFile) throws SAXException, IOException
	{
		HashMap<String, String> initParams = Initiator.getInitParams(initFile);
		IMAGE_DIR = initParams.get("image_dir");
		PREPROCESSED_IMAGE_DIR = initParams.get("preprocessed_image_dir");
		EXCEL_DIR = initParams.get("excel_dir");
	}

	// 将识别结果的格式化信息写入excel文件
	private static void saveToExcel()
	{
		File excelDir = new File(ImageRecognition.EXCEL_DIR);
		if (!excelDir.exists())
		{
			excelDir.mkdirs();
		}
		
		String excelFile1 = ImageRecognition.EXCEL_DIR + "\\high_confidence_excel.xls";	//输出的excel文件名
		String excelFile2 = ImageRecognition.EXCEL_DIR + "\\low_confidence_excel.xls";	//输出的excel文件名

		String sheetName = "营业执照信息";	//输出的excel文件工作表名   
		String[] tableHeader = { "企业注册号","企业名称", "企业名称置信度", "企业注册号置信度", "图片" };	//excel工作表的标题
		
		ExcelTool excelTool = new ExcelTool();
		// 写入置信度高的信息
		excelTool.writeData(excelFile1, sheetName, tableHeader, highConfidenceList);
		// 写入置信度低的信息
		excelTool.writeData(excelFile2, sheetName, tableHeader, lowConfidenceList);
	}
	
    
/*    
    private String readFileWithEncode(File file, String encoding) throws IOException {  
        String content = "";   
        BufferedReader reader = new BufferedReader(new InputStreamReader(  
                new FileInputStream(file), encoding));  
        String line = null;  
        while ((line = reader.readLine()) != null) {  
            content += line + "\n";  
        }  
        reader.close();  
        return content;  
    }
*/
}
