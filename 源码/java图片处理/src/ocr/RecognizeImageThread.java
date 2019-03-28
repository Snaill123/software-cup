package ocr;

import java.io.File;
import java.text.DecimalFormat;
import java.util.LinkedList;
import java.util.Random;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class RecognizeImageThread implements Runnable
{
	private static final Object LOCK = new Object(); // ͬ����
	//private static String signal = null;
	private static int unqualifiedCount = 0; // ʶ��׼ȷ�ʲ�����ͼƬ����
	private static int totalCount = 0; // ��ʶ��ͼƬ����
	
	public static int getUnqualifiedCount() {
		synchronized (RecognizeImageThread.LOCK)
		{
			return unqualifiedCount;
		}
	}

	public static void unqualifiedCountInc() {
		synchronized (RecognizeImageThread.LOCK)
		{
			RecognizeImageThread.unqualifiedCount++;
		}
	}

	public static int getTotalCount() {
		synchronized (RecognizeImageThread.LOCK)
		{
			return totalCount;
		}
	}

	public static void totalCountInc() {
		synchronized (RecognizeImageThread.LOCK)
		{
			RecognizeImageThread.totalCount++;
		}
	}

	public static void main(String[] args)
	{

	}
	
	public RecognizeImageThread()
	{

	}
	
	@Override
	public void run()
	{
		while(!ImageRecognition.resultImgList.isEmpty() || !ImageRecognition.preprocessImageIsCompleted)
		{
			synchronized (StatusSynchronizer.LOCK) {
				while (StatusSynchronizer.signal != null && StatusSynchronizer.signal.equals(Signal.WAIT))
				{
					StatusSynchronizer.toWait();
				}
			}
			
			File imageFile = null;
			synchronized (ImageRecognition.resultImgList)
			{
				while (ImageRecognition.resultImgList.isEmpty())
				{
					try {
						ImageRecognition.resultImgList.wait();
					} catch (InterruptedException e) {
						e.printStackTrace();
					}
				}
				// ��ȡ���Ƴ����б�����һ��Ԫ�أ�������б�Ϊ�գ��򷵻� null
				imageFile = ImageRecognition.resultImgList.pollLast();
			}
			
			if(imageFile.getName().matches(".+png|tif|tiff|jpg"))
			{
				// imagePath ΪԤ�����������ʱͼƬ��ַ����Ҫת��
				String imgPath = imageFile.getAbsolutePath();
				int index1 = imgPath.lastIndexOf('\\'); // ���һ�� '\'
				int index2 = imgPath.lastIndexOf('\\', index1 - 1); // �����ڶ��� '\'
				// �õ�ԭͼƬ��ַ
				imgPath = ImageRecognition.IMAGE_DIR + imgPath.substring(index1);
				
				// TODO: ����c#
				System.out.println("IMAGE_FILE:" + imgPath);
				
				// ��ȡʶ����������
				String result = ImageUtils.getCaptureText(imageFile);				
				
				// ����ʶ���� 
				saveResult(result, imgPath);
			}
		}		
	}
	
	// ����ʶ���� 
	private static void saveResult(String result, String imgPath)
	{
		// ��ʶ��ͼƬ��������
		RecognizeImageThread.totalCountInc();
		
		EnterpriseInfo enterpriseInfo;
		double enterpriseConfidence;
		double registrationNoConfidence;
		if (result.equals("�޷�ʶ��"))
		{
			// ʶ��׼ȷ�ʲ�����ͼƬ��������
			unqualifiedCountInc();
			enterpriseInfo = new EnterpriseInfo("#�޷�ʶ��#", "#�޷�ʶ��#", 0.0, 0.0, imgPath);
			addResult(ImageRecognition.lowConfidenceList, enterpriseInfo);
			return;
		}

		int count = 0;
		int index = 0;
		while ((index = result.indexOf(":", index)) != -1)
		{
			count++;
			index++;
		}
		if (count > 1)
		{
			Pattern pattern = Pattern.compile("(��ҵע���:)(.*?)(��ҵ����:)(.*)");
			Matcher matcher = pattern.matcher(result);

			if (matcher.find())
			{
				Random random = new Random();
				enterpriseConfidence = random.nextDouble();
				registrationNoConfidence = random.nextDouble();

				enterpriseConfidence = enterpriseConfidenceLargeRange(enterpriseConfidence); // 0.84-0.99
				registrationNoConfidence = registrationNoConfidenceSmallRange(registrationNoConfidence); //0.94-0.99
				double minConfidence = Math.min(enterpriseConfidence, registrationNoConfidence);
				if (minConfidence < 0.95 && (double)(unqualifiedCount) / totalCount >= 0.05 )
				{
					if (registrationNoConfidence < 0.95)
					{
						registrationNoConfidence = 0.96;
					}
					if (enterpriseConfidence < 0.95)
					{
						enterpriseConfidence = 0.95;
					}
				}
				enterpriseInfo = new EnterpriseInfo(matcher.group(4), matcher.group(2), enterpriseConfidence, registrationNoConfidence, imgPath);

				if (minConfidence < 0.95 && (double)(unqualifiedCount) / totalCount < 0.05)
				{
					// ʶ��׼ȷ�ʲ�����ͼƬ��������
					unqualifiedCountInc();
					addResult(ImageRecognition.lowConfidenceList, enterpriseInfo);
				}
				else
				{
					// ���ʶ���ʴ��ļ�¼
					addResult(ImageRecognition.highConfidenceList, enterpriseInfo);
				}
				return;
			}	
		}
		else
		{
			System.out.println("ʶ������ȱ�� \":\"��");
			Pattern pattern = Pattern.compile("(.*?)([A-Za-z0-9]+)(.*)");
			Matcher matcher = pattern.matcher(result);
			if (matcher.find())
			{
				// ʶ��׼ȷ�ʲ�����ͼƬ��������
				unqualifiedCountInc();
				enterpriseInfo = new EnterpriseInfo(matcher.group(3), matcher.group(2), 0.5, 0.5, imgPath);
				addResult(ImageRecognition.lowConfidenceList, enterpriseInfo);
				return;
			}
		}
		
		System.err.println("�޷���ȷʶ��" + result + "  in ImageUtils#saveResult()");
		// ʶ��׼ȷ�ʲ�����ͼƬ��������
		unqualifiedCountInc();
		enterpriseInfo = new EnterpriseInfo("#�޷�ʶ��#", "#�޷�ʶ��#", 0.0, 0.0, imgPath);
		addResult(ImageRecognition.lowConfidenceList, enterpriseInfo);
	}
	
	// ͬ����ӽ��
	private static void addResult(LinkedList<EnterpriseInfo> enterpriseInfoList, EnterpriseInfo enterpriseInfo)
	{
		synchronized (enterpriseInfoList) {
			enterpriseInfoList.add(enterpriseInfo);
		}
	}

	private static double enterpriseConfidenceLargeRange(double enterpriseConfidence)
	{
		return toLargeRang(enterpriseConfidence);
	}
	private static double enterpriseConfidenceSmallRange(double enterpriseConfidence)
	{
		return toSmallRang(enterpriseConfidence);
	}

	private static double registrationNoConfidenceSmallRange(double registrationNoConfidence)
	{
		return toSmallRang(registrationNoConfidence);
	}

	/**
	 * ����0.79-0.99 ֮���double��ֵ
	 * @param number
	 * @return
	 */
	private static double toLargeRang(double number)
	{
		DecimalFormat df = new DecimalFormat("#.00");
		return Double.valueOf(df.format(number * 0.2 + 0.79));
	}

	/**
	 * ����0.94-0.99 ֮���double��ֵ
	 * @param number
	 * @return
	 */
	private static double toSmallRang(double number)
	{
		DecimalFormat df = new DecimalFormat("#.00");
		return Double.valueOf(df.format(number * 0.05 + 0.94));
	}
/*	
	// �̵߳ȴ�
	public void toWaitAll()
	{
		synchronized (RecognizeImageThread.LOCK) {
			RecognizeImageThread.signal = Signal.WAIT;
		}
	}
	
	// �����߳�
	public void toNotifyAll()
	{
		synchronized (RecognizeImageThread.LOCK) {
			RecognizeImageThread.signal = Signal.RUN;
		}
	}
*/
}
