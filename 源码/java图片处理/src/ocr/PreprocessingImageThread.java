package ocr;

import java.io.File;
import java.io.IOException;

import com.sun.image.codec.jpeg.ImageFormatException;

public class PreprocessingImageThread implements Runnable{
	private File sourceImgDir = null;
	private File resultImgDir = null;
	private static String signal = null;
	
	public static void main(String[] args) {
		// TODO 自动生成的方法存根

	}
	
	public PreprocessingImageThread(File sourceImgDir, File resultImgDir)
	{
		this.sourceImgDir = sourceImgDir;
		this.resultImgDir = resultImgDir;
	}
	@Override
	public void run()
	{/*
		// 图片预处理
				for(File file : srcImageDir.listFiles())
				{
					if(file.isFile() && file.getName().endsWith("png"))
					{
						String fileName = file.getName();
						File resultImageFile = new File(resultImageDir, fileName);
						// 通过Java类库按比例切割图片
						//ImageUtils.cropImage(file, 0, 0, 0.6, 0.2, resultImageFile);
						// 通过第三方程序（ImageMagick）来按比例切割图片
						//ImageUtils.cropImageByImageMagick(file, 0, 0, 0.6, 0.2, resultImageFile);
						// 通过Java类库按大小切割图片
						ImageUtils.cropImage(file, 0, 0, 0, 74, resultImageFile);
						// 图像格式转换
						resultImageFile =  ImageUtils.covertImage(resultImageFile);
						
						if(resultImageFile == null)
						{
							System.out.println("图片格式转换失败！");
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
		
		// 图片预处理
		for(File file : sourceImgDir.listFiles())
		{
			if(file.isFile() && file.getName().endsWith("png"))
			{
				String fileName = file.getName();
				File resultImgFile = new File(resultImgDir, fileName);
				// 图像切割和格式转换
				try {
					resultImgFile =  ImageUtils.preprocessingImg(file, resultImgFile);
				} catch (ImageFormatException e) {
					e.printStackTrace();
				} catch (IOException e) {
					e.printStackTrace();
				}

				if(resultImgFile == null)
				{
					System.err.println("图片格式转换失败！");
				}
				else
				{
					synchronized (ImageRecognition.resultImgList)
					{
						if (ImageRecognition.resultImgList.isEmpty())
						{
							ImageRecognition.resultImgList.notifyAll();
						}
						// 将预处理完成的图片保存至列表最后
						ImageRecognition.resultImgList.addLast(resultImgFile);
					}
				}
			}
		}
		// 图片预处理完成
		ImageRecognition.preprocessImageIsCompleted = true;
	}
}
