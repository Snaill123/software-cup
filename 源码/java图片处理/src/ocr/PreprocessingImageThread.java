package ocr;

import java.io.File;
import java.io.IOException;

import com.sun.image.codec.jpeg.ImageFormatException;

public class PreprocessingImageThread implements Runnable{
	private File sourceImgDir = null;
	private File resultImgDir = null;
	private static String signal = null;
	
	public static void main(String[] args) {
		// TODO �Զ����ɵķ������

	}
	
	public PreprocessingImageThread(File sourceImgDir, File resultImgDir)
	{
		this.sourceImgDir = sourceImgDir;
		this.resultImgDir = resultImgDir;
	}
	@Override
	public void run()
	{/*
		// ͼƬԤ����
				for(File file : srcImageDir.listFiles())
				{
					if(file.isFile() && file.getName().endsWith("png"))
					{
						String fileName = file.getName();
						File resultImageFile = new File(resultImageDir, fileName);
						// ͨ��Java��ⰴ�����и�ͼƬ
						//ImageUtils.cropImage(file, 0, 0, 0.6, 0.2, resultImageFile);
						// ͨ������������ImageMagick�����������и�ͼƬ
						//ImageUtils.cropImageByImageMagick(file, 0, 0, 0.6, 0.2, resultImageFile);
						// ͨ��Java��ⰴ��С�и�ͼƬ
						ImageUtils.cropImage(file, 0, 0, 0, 74, resultImageFile);
						// ͼ���ʽת��
						resultImageFile =  ImageUtils.covertImage(resultImageFile);
						
						if(resultImageFile == null)
						{
							System.out.println("ͼƬ��ʽת��ʧ�ܣ�");
						}
						else
						{
							ImageRecognition.resultImgList.add(resultImageFile);
						}
					}
				}
		*/
		
		synchronized (StatusSynchronizer.LOCK) {
			while (StatusSynchronizer.signal != null && StatusSynchronizer.signal.equals(Signal.WAIT))
			{
				StatusSynchronizer.toWait();
			}
		}
		
		// ͼƬԤ����
		for(File file : sourceImgDir.listFiles())
		{
			if(file.isFile() && file.getName().endsWith("png"))
			{
				String fileName = file.getName();
				File resultImgFile = new File(resultImgDir, fileName);
				// ͼ���и�͸�ʽת��
				try {
					resultImgFile =  ImageUtils.preprocessingImg(file, resultImgFile);
				} catch (ImageFormatException e) {
					e.printStackTrace();
				} catch (IOException e) {
					e.printStackTrace();
				}

				if(resultImgFile == null)
				{
					System.err.println("ͼƬ��ʽת��ʧ�ܣ�");
				}
				else
				{
					synchronized (ImageRecognition.resultImgList)
					{
						if (ImageRecognition.resultImgList.isEmpty())
						{
							ImageRecognition.resultImgList.notifyAll();
						}
						// ��Ԥ������ɵ�ͼƬ�������б����
						ImageRecognition.resultImgList.addLast(resultImgFile);
					}
				}
			}
		}
		// ͼƬԤ�������
		ImageRecognition.preprocessImageIsCompleted = true;
	}
}
