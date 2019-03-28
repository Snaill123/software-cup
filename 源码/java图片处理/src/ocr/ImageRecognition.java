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
	public static String IMAGE_DIR = null; // ͼƬ�ļ�λ��
	private static String PREPROCESSED_IMAGE_DIR = null; // Ԥ�����ͼƬ�洢λ��
	private static String EXCEL_DIR = null; // ʶ����excel�ļ��洢λ��
	
	public static LinkedList<EnterpriseInfo> highConfidenceList = new LinkedList<EnterpriseInfo>();
	public static LinkedList<EnterpriseInfo> lowConfidenceList = new LinkedList<EnterpriseInfo>();
	
	public static StringBuffer strBuffer = new StringBuffer();	// �洢ͼƬ������Ϣ,�̰߳�ȫ�Ŀɱ��ַ�����
	public static LinkedList<File> resultImgList = new LinkedList<File>();  //��¼��Ԥ����õ�ͼƬ�ļ�
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
		
		System.out.println("������������");
		
		// ��ȡ��ʼ���ļ�����ʼ������
		String initFile = args[0];
		
		try{
			init(initFile);
		}catch (SAXException e1) {
			e1.printStackTrace();
			System.out.println("java��ȡ��ʼ���ļ�ʧ�ܣ�");
			System.exit(0);
		} catch (IOException e1) {
			e1.printStackTrace();
			System.out.println("java��ȡ��ʼ���ļ�ʧ�ܣ�");
			System.exit(0);
		}
		
		System.out.println("�����ļ��Ѷ�ȡ��");
		
	    // ��ȡ��ʼʱ��
		long startTime = System.currentTimeMillis();
		
		// ԭʼͼƬ�洢λ��
		File srcImageDir = new File(ImageRecognition.IMAGE_DIR);
		
		// Ԥ����������ͼƬ�洢λ��
		File resultImageDir = new File(ImageRecognition.PREPROCESSED_IMAGE_DIR);
		if (!resultImageDir.exists())
		{
			resultImageDir.mkdirs();
		}
		
		// ����5���̣߳�����һ���߳�Ԥ����ͼƬ�������ʶ��ͼƬʶ��Ԥ������ͼƬ
		ImageRecognition.recognizeImageThreadCount = 4;
		Thread[] threads = new Thread[ImageRecognition.recognizeImageThreadCount+1];
		
		// ����ͼƬԤ�����̴߳���ͼƬ
		threads[0] = new Thread(new PreprocessingImageThread(srcImageDir, resultImageDir));
		threads[0].setName("PreprocessingImgThread");
		threads[0].start();
		
		// ����4���߳�ʶ��ͼƬ����
		for (int i = 1; i < threads.length; i++)
		{
			threads[i] = new Thread(new RecognizeImageThread());
			threads[i].setName("RecognizeImageThread" + i);
			threads[i].start();
		}
		
		System.out.println("ʶ������Ѵ�����");
		
		// ��������c#�Ŀ���
		ImageRecognition imageRecognition = new ImageRecognition();
		StatusSynchronizer.threads = threads;
		StatusSynchronizer.mainThread = imageRecognition;
		Thread daemon = new Thread(new StatusSynchronizer());  //������̨����
		daemon.setDaemon(true); // Must call before start() 
		daemon.start(); // run �������գ�c#�������ź�
		
				
		
		// �ȴ������߳�ִ����ɣ�������StatusSynchronizer�̣߳�
		for (int i = 0; i < threads.length; i++)
		{
			try {
				threads[i].join();
			} catch (InterruptedException e) {
				e.printStackTrace();
			}
		}
		
		// ��ʶ�����ĸ�ʽ����Ϣд��excel�ļ�
		saveToExcel();		
		
		// ��ӡʶ�������ͼƬ������Ϣ
		System.out.println(ImageRecognition.strBuffer.toString());
		
		long endTime = System.currentTimeMillis();    //��ȡ����ʱ��
		System.out.println("��������ʱ�䣺" + (endTime - startTime)/1000 + "s");    //�����������ʱ��

	}
/*
	// ��������
	private static void exit(ImageRecognition imageRecognition) {
		StatusSynchronizer.toExit(imageRecognition);
	}

	// ���ѳ��������̣߳�
	private static void allNotify(ImageRecognition imageRecognition) {
		StatusSynchronizer.toNotifyAll(imageRecognition);
	}
	
	// �����̵߳ȴ�
	private static void allWait() {
		synchronized (StatusSynchronizer.LOCK) {
			StatusSynchronizer.signal = Signal.WAIT;
		}
	}
*/
	// ��ȡ��ʼ���ļ�����ʼ������
	private static void init(String initFile) throws SAXException, IOException
	{
		HashMap<String, String> initParams = Initiator.getInitParams(initFile);
		IMAGE_DIR = initParams.get("image_dir");
		PREPROCESSED_IMAGE_DIR = initParams.get("preprocessed_image_dir");
		EXCEL_DIR = initParams.get("excel_dir");
	}

	// ��ʶ�����ĸ�ʽ����Ϣд��excel�ļ�
	private static void saveToExcel()
	{
		File excelDir = new File(ImageRecognition.EXCEL_DIR);
		if (!excelDir.exists())
		{
			excelDir.mkdirs();
		}
		
		String excelFile1 = ImageRecognition.EXCEL_DIR + "\\high_confidence_excel.xls";	//�����excel�ļ���
		String excelFile2 = ImageRecognition.EXCEL_DIR + "\\low_confidence_excel.xls";	//�����excel�ļ���

		String sheetName = "Ӫҵִ����Ϣ";	//�����excel�ļ���������   
		String[] tableHeader = { "��ҵע���","��ҵ����", "��ҵ�������Ŷ�", "��ҵע������Ŷ�", "ͼƬ" };	//excel������ı���
		
		ExcelTool excelTool = new ExcelTool();
		// д�����Ŷȸߵ���Ϣ
		excelTool.writeData(excelFile1, sheetName, tableHeader, highConfidenceList);
		// д�����Ŷȵ͵���Ϣ
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
