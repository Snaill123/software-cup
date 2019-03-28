package ocr;

import java.awt.Color;
//import java.awt.Graphics;
//import java.awt.Image;
//import java.awt.Toolkit;
import java.awt.image.BufferedImage;
//import java.awt.image.CropImageFilter;
//import java.awt.image.FilteredImageSource;
//import java.awt.image.ImageFilter;
import java.io.File;
//import java.io.IOException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.text.DecimalFormat;
import java.util.Random;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import javax.imageio.ImageIO;
import com.sun.image.codec.jpeg.JPEGImageEncoder;
import com.sun.image.codec.jpeg.JPEGEncodeParam;
import com.sun.image.codec.jpeg.ImageFormatException;
import com.sun.image.codec.jpeg.JPEGCodec;
/**
 * 
 * @author 
 * ͼ������
 */
public class ImageUtils {
	
/*
	// ��ָ���������Ϳ���и�ͼƬ
	 private static void cut(BufferedImage bi, File resultImageFile,
	            int x, int y, int width, int height) {
		 try {
			 int srcWidth = bi.getWidth(); // Դͼ���
			 int srcHeight = bi.getHeight(); // Դͼ�߶�

			 Image image = bi.getScaledInstance(srcWidth, srcHeight,
					 Image.SCALE_DEFAULT);
			 // �ĸ������ֱ�Ϊͼ���������Ϳ��
			 // ��: CropImageFilter(int x,int y,int width,int height)
			 ImageFilter cropFilter = new CropImageFilter(x, y, width, height);
			 Image img = Toolkit.getDefaultToolkit().createImage(
					 new FilteredImageSource(image.getSource(),
							 cropFilter));
			 BufferedImage tag = new BufferedImage(width, height, BufferedImage.TYPE_INT_RGB);
			 Graphics g = tag.getGraphics();
			 g.drawImage(img, 0, 0, width, height, null); // �����и���ͼ
			 g.dispose();

			 // ���Ϊ�ļ�
			 ImageIO.write(tag, "png", resultImageFile);		
		 } catch (Exception e) {
			 e.printStackTrace();
		 }
	 }


	// ��ָ����������ͼƬ�Ŀ�߱����и�ͼƬ
	 public static void cutToScale(File srcImageFile, File resultImageFile,
	            int x, int y, double widthRatio, double heightRatio)
	 {
		 try {
	            // ��ȡԴͼ�� 
	            BufferedImage bi = ImageIO.read(srcImageFile);
	            int srcWidth = bi.getWidth(); // Դͼ���
	            int srcHeight = bi.getHeight(); // Դͼ�߶�
	            if(srcWidth > 0 && srcHeight > 0)
	            {
	            	int width = (int)(srcWidth * widthRatio);
	            	int height = (int)(srcHeight * heightRatio);
	            	if(width <= srcWidth && height <= srcHeight)
	            	{
	            		ImageUtils.cut(bi, resultImageFile, x, y, width, height);
	            	}
	            }
		 } catch (Exception e) {
	            e.printStackTrace();
	        }
	 }
*/
	// ����
	public static void main(String[] args) throws Exception
	{
//		ImageUtils.covertImage(new File("E:\\eclipse-workspace\\TesseractOCR\\image\\4.png"));
		
//		String str = "boo:and:foo";
//		for(String s : str.split("o"))
//			System.out.println(s);
		
		ImageUtils.grayProcess(new File("E:\\eclipse-workspace\\TesseractOCR\\image\\6.png"));
	}
	
	// ͨ��Java��ⰴ��С�и�ͼƬ
	public static void cropImage(File srcImageFile, int x, int y,
			 int width, int height, File resultImageFile) throws IOException
	{
		// ��ȡԴͼ�� 
		BufferedImage bufferedImage = ImageIO.read(srcImageFile);
		int srcWidth = bufferedImage.getWidth(); // Դͼ���
		int srcHeight = bufferedImage.getHeight(); // Դͼ�߶�
		if(srcWidth > 0 && srcHeight > 0)
		{
			width = srcWidth;
			if(width <= srcWidth && height <= srcHeight)
			{
				bufferedImage = bufferedImage.getSubimage(x, y, width, height);
				String fileName = srcImageFile.getName();
				String suffix = fileName.substring(fileName.lastIndexOf('.') + 1);
				ImageIO.write(bufferedImage, suffix, resultImageFile);  
			}
		}
	 }
	
	// ͨ��Java��ⰴ�����и�ͼƬ
	 public static void cropImage(File srcImageFile, int x, int y,
			 double widthRatio, double heightRatio, File resultImageFile)
	 {
		 try
		 {
			 // ��ȡԴͼ�� 
			 BufferedImage bufferedImage = ImageIO.read(srcImageFile);
			 int srcWidth = bufferedImage.getWidth(); // Դͼ���
			 int srcHeight = bufferedImage.getHeight(); // Դͼ�߶�
			 if(srcWidth > 0 && srcHeight > 0)
			 {
				 int width = (int)(srcWidth * widthRatio);
				 int height = (int)(srcHeight * heightRatio);
				 if(width <= srcWidth && height <= srcHeight)
				 {
					 bufferedImage = bufferedImage.getSubimage(x, y, width, height);
					 String fileName = srcImageFile.getName();
					 String suffix = fileName.substring(fileName.lastIndexOf('.') + 1);
					 ImageIO.write(bufferedImage, suffix, resultImageFile);  
				 }
			 }
		 }
		 catch (Exception e)
		 {
			 e.printStackTrace();
		 }
	 }
	 
	 // ͨ������������ImageMagick�����������и�ͼƬ
	 // TODO: ���Ȱ�װ�����ú�ImageMagick���������价��������
	 public static void cropImageByImageMagick(File srcImageFile, int x, int y,
			 double widthRatio, double heightRatio, File resultImageFile)
	 {
		 // �����µ�ͼƬ��С��λ�ã�widthxheight+x+y��
		 String cropSize = "";
		 String command = "";
		 try {
			// ��ȡԴͼ�� 
			 BufferedImage bufferedImage = ImageIO.read(srcImageFile);
			 int srcWidth = bufferedImage.getWidth(); // Դͼ���
			 int srcHeight = bufferedImage.getHeight(); // Դͼ�߶�
			 //
			 if(srcWidth > 0 && srcHeight > 0)
			 {
				 int width = (int)(srcWidth * widthRatio);
				 int height = (int)(srcHeight * heightRatio);
				 if(width <= srcWidth && height <= srcHeight)
				 {
					 cropSize = cropSize + width + "x" + height + "+" + x + "+" + y;
					 command = "D:\\ImageMagick-6.2.7-Q16\\convert.exe" + " " + srcImageFile.getAbsolutePath() + " " + "-crop" + " " + cropSize + " " + resultImageFile.getAbsolutePath();
					 Process process = ProcessExecutor.execute(command);
					// �ȴ�����ִ�н��������״̬
//			        System.out.println(process.waitFor());
				 }
			 }
		} catch (Exception e) {
			// TODO �Զ����ɵ� catch ��
			e.printStackTrace();
		}
	 }
 
	 // ͼƬ��ʽת���Լ���������
	 public static File covertImage(File file) throws ImageFormatException, IOException{
		 //1.��ȡͼƬ
		 BufferedImage bufferedImage = ImageIO.read(file);
		 //2.����һ���հ״�С��ͬ��RGB����
		 BufferedImage newBufferedImage = new BufferedImage(bufferedImage.getWidth(),
				 bufferedImage.getHeight(), BufferedImage.TYPE_INT_RGB);
		 newBufferedImage.createGraphics().drawImage(bufferedImage, 0, 0, Color.WHITE, null);		
		 //3.�ٴ�д���ʱ����jpegͼƬ��ʽ
		 ImageIO.write(newBufferedImage, "jpg", file);

		 JPEGImageEncoder jpegEncoder = JPEGCodec.createJPEGEncoder(new FileOutputStream(file));
		 JPEGEncodeParam jpegEncodeParam = jpegEncoder.getDefaultJPEGEncodeParam(newBufferedImage);
		 jpegEncodeParam.setDensityUnit(JPEGEncodeParam.DENSITY_UNIT_DOTS_INCH);  
		 jpegEncoder.setJPEGEncodeParam(jpegEncodeParam);  
		 jpegEncodeParam.setQuality(0.75f, false);  
		 jpegEncodeParam.setXDensity(300);  
		 jpegEncodeParam.setYDensity(300);  
		 jpegEncoder.encode(newBufferedImage, jpegEncodeParam);  
		 newBufferedImage.flush();

		 return file;
	 }
	 
	 // ͼƬ�ҶȻ�
	 public static void grayProcess(File sourceImageFile) throws IOException
	 {
		 BufferedImage sourceImage  = ImageIO.read(sourceImageFile);
		 
//		 System.out.println(sourceImage.getAlphaRaster() == null);
//		 System.exit(0);
		 
		 int width = sourceImage.getWidth();
		 int height = sourceImage.getHeight();
		 BufferedImage grayImage = new BufferedImage(width, height, BufferedImage.TYPE_BYTE_GRAY);// BufferedImage.TYPE_BYTE_GRAYָ��������һ���Ҷ�ͼƬ 
		 for(int i= 0 ; i < width ; i++){  
			 for(int j = 0 ; j < height; j++){  
				 int rgb = sourceImage.getRGB(i, j);  
				 grayImage.setRGB(i, j, rgb);
				 
				 if(i == width/2 && j == height/2)
				 {
					 System.out.println(sourceImage.getRGB(i, j) + " " + grayImage.getRGB(i, j));
				 }
				 grayImage.setRGB(i, j, rgb - grayImage.getRGB(i, j));
			 }  
		 }

		 // ��ȡͼƬ��ʽ
//		 String format = sourceImageFile.getName().substring(sourceImageFile.getName().indexOf('.')+1);
		 ImageIO.write(grayImage, "jpg", new File(sourceImageFile.getAbsolutePath().substring(0, sourceImageFile.getAbsolutePath().indexOf('.'))+".jpg"));
	 }
	 
	 // ͼƬԤ����
	 public static File preprocessingImg(File sourceImgFile, File resultImgFile) throws ImageFormatException, IOException
	 {
		 // ͨ��Java��ⰴ��С�и�ͼƬ
		 ImageUtils.cropImage(sourceImgFile, 0, 0, 0, 74, resultImgFile);
		 // ͼ���ʽת��
		 resultImgFile =  ImageUtils.covertImage(resultImgFile);
		 return resultImgFile;
	 }
	 
	 // ʶ��ͼ������
	 public static String getCaptureText(File file)
	 {
		 String result = null;
		 String imgPath = file.getAbsolutePath();  

		 try {
			 String output = "stdout";  //��ֹ·�����пո�
			 String command = ImageRecognition.OCR_COMMAND + " " + "\"" + imgPath + "\"" + " " + output +" "+ ImageRecognition.LANG_OPTION + " " + ImageRecognition.OCR_LANG_DATA;

			 System.out.println(command);
			 // ��������
			 Process process = ProcessExecutor.execute(command);
			 // ��ӡ�������
			 result = ProcessExecutor.readProcessOutput(process);
			 
			 // �ȴ�����ִ�н��������״̬
			 int exitCode = process.waitFor();

			 if (exitCode == ProcessExecutor.SUCCESS) {
				 System.out.println(ProcessExecutor.SUCCESS_MESSAGE);
			 } else {
				 System.err.println(ProcessExecutor.ERROR_MESSAGE + exitCode);
			 }
		 } catch (Exception e)
		 { 
			 e.printStackTrace();  
		 }  
		 return result;  
	 }
	 
	 
}
