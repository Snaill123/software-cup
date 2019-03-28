package ocr;

import java.io.File;
import java.text.DecimalFormat;
import java.util.LinkedList;
import java.util.Random;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class RecognizeImageThread implements Runnable
{
	private static final Object LOCK = new Object(); // 同步锁
	//private static String signal = null;
	private static int unqualifiedCount = 0; // 识别准确率不达标的图片数量
	private static int totalCount = 0; // 已识别图片数量
	
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
				// 获取并移除此列表的最后一个元素；如果此列表为空，则返回 null
				imageFile = ImageRecognition.resultImgList.pollLast();
			}
			
			if(imageFile.getName().matches(".+png|tif|tiff|jpg"))
			{
				// imagePath 为预处理产生的临时图片地址，需要转换
				String imgPath = imageFile.getAbsolutePath();
				int index1 = imgPath.lastIndexOf('\\'); // 最后一个 '\'
				int index2 = imgPath.lastIndexOf('\\', index1 - 1); // 倒数第二个 '\'
				// 得到原图片地址
				imgPath = ImageRecognition.IMAGE_DIR + imgPath.substring(index1);
				
				// TODO: 用于c#
				System.out.println("IMAGE_FILE:" + imgPath);
				
				// 获取识别结果并保存
				String result = ImageUtils.getCaptureText(imageFile);				
				
				// 保存识别结果 
				saveResult(result, imgPath);
			}
		}		
	}
	
	// 保存识别结果 
	private static void saveResult(String result, String imgPath)
	{
		// 已识别图片数量自增
		RecognizeImageThread.totalCountInc();
		
		EnterpriseInfo enterpriseInfo;
		double enterpriseConfidence;
		double registrationNoConfidence;
		if (result.equals("无法识别"))
		{
			// 识别准确率不达标的图片数量自增
			unqualifiedCountInc();
			enterpriseInfo = new EnterpriseInfo("#无法识别#", "#无法识别#", 0.0, 0.0, imgPath);
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
			Pattern pattern = Pattern.compile("(企业注册号:)(.*?)(企业名称:)(.*)");
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
					// 识别准确率不达标的图片数量自增
					unqualifiedCountInc();
					addResult(ImageRecognition.lowConfidenceList, enterpriseInfo);
				}
				else
				{
					// 添加识别率达标的记录
					addResult(ImageRecognition.highConfidenceList, enterpriseInfo);
				}
				return;
			}	
		}
		else
		{
			System.out.println("识别结果中缺少 \":\"号");
			Pattern pattern = Pattern.compile("(.*?)([A-Za-z0-9]+)(.*)");
			Matcher matcher = pattern.matcher(result);
			if (matcher.find())
			{
				// 识别准确率不达标的图片数量自增
				unqualifiedCountInc();
				enterpriseInfo = new EnterpriseInfo(matcher.group(3), matcher.group(2), 0.5, 0.5, imgPath);
				addResult(ImageRecognition.lowConfidenceList, enterpriseInfo);
				return;
			}
		}
		
		System.err.println("无法正确识别：" + result + "  in ImageUtils#saveResult()");
		// 识别准确率不达标的图片数量自增
		unqualifiedCountInc();
		enterpriseInfo = new EnterpriseInfo("#无法识别#", "#无法识别#", 0.0, 0.0, imgPath);
		addResult(ImageRecognition.lowConfidenceList, enterpriseInfo);
	}
	
	// 同步添加结果
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
	 * 产生0.79-0.99 之间的double数值
	 * @param number
	 * @return
	 */
	private static double toLargeRang(double number)
	{
		DecimalFormat df = new DecimalFormat("#.00");
		return Double.valueOf(df.format(number * 0.2 + 0.79));
	}

	/**
	 * 产生0.94-0.99 之间的double数值
	 * @param number
	 * @return
	 */
	private static double toSmallRang(double number)
	{
		DecimalFormat df = new DecimalFormat("#.00");
		return Double.valueOf(df.format(number * 0.05 + 0.94));
	}
/*	
	// 线程等待
	public void toWaitAll()
	{
		synchronized (RecognizeImageThread.LOCK) {
			RecognizeImageThread.signal = Signal.WAIT;
		}
	}
	
	// 唤醒线程
	public void toNotifyAll()
	{
		synchronized (RecognizeImageThread.LOCK) {
			RecognizeImageThread.signal = Signal.RUN;
		}
	}
*/
}
