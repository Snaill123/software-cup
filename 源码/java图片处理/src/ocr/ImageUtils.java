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
 * 图像处理工具
 */
public class ImageUtils {
	
/*
	// 按指定起点坐标和宽高切割图片
	 private static void cut(BufferedImage bi, File resultImageFile,
	            int x, int y, int width, int height) {
		 try {
			 int srcWidth = bi.getWidth(); // 源图宽度
			 int srcHeight = bi.getHeight(); // 源图高度

			 Image image = bi.getScaledInstance(srcWidth, srcHeight,
					 Image.SCALE_DEFAULT);
			 // 四个参数分别为图像起点坐标和宽高
			 // 即: CropImageFilter(int x,int y,int width,int height)
			 ImageFilter cropFilter = new CropImageFilter(x, y, width, height);
			 Image img = Toolkit.getDefaultToolkit().createImage(
					 new FilteredImageSource(image.getSource(),
							 cropFilter));
			 BufferedImage tag = new BufferedImage(width, height, BufferedImage.TYPE_INT_RGB);
			 Graphics g = tag.getGraphics();
			 g.drawImage(img, 0, 0, width, height, null); // 绘制切割后的图
			 g.dispose();

			 // 输出为文件
			 ImageIO.write(tag, "png", resultImageFile);		
		 } catch (Exception e) {
			 e.printStackTrace();
		 }
	 }


	// 按指定起点坐标和图片的宽高比例切割图片
	 public static void cutToScale(File srcImageFile, File resultImageFile,
	            int x, int y, double widthRatio, double heightRatio)
	 {
		 try {
	            // 读取源图像 
	            BufferedImage bi = ImageIO.read(srcImageFile);
	            int srcWidth = bi.getWidth(); // 源图宽度
	            int srcHeight = bi.getHeight(); // 源图高度
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
	// 测试
	public static void main(String[] args) throws Exception
	{
//		ImageUtils.covertImage(new File("E:\\eclipse-workspace\\TesseractOCR\\image\\4.png"));
		
//		String str = "boo:and:foo";
//		for(String s : str.split("o"))
//			System.out.println(s);
		
		ImageUtils.grayProcess(new File("E:\\eclipse-workspace\\TesseractOCR\\image\\6.png"));
	}
	
	// 通过Java类库按大小切割图片
	public static void cropImage(File srcImageFile, int x, int y,
			 int width, int height, File resultImageFile) throws IOException
	{
		// 读取源图像 
		BufferedImage bufferedImage = ImageIO.read(srcImageFile);
		int srcWidth = bufferedImage.getWidth(); // 源图宽度
		int srcHeight = bufferedImage.getHeight(); // 源图高度
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
	
	// 通过Java类库按比例切割图片
	 public static void cropImage(File srcImageFile, int x, int y,
			 double widthRatio, double heightRatio, File resultImageFile)
	 {
		 try
		 {
			 // 读取源图像 
			 BufferedImage bufferedImage = ImageIO.read(srcImageFile);
			 int srcWidth = bufferedImage.getWidth(); // 源图宽度
			 int srcHeight = bufferedImage.getHeight(); // 源图高度
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
	 
	 // 通过第三方程序（ImageMagick）来按比例切割图片
	 // TODO: 需先安装并配置好ImageMagick（需配置其环境变量）
	 public static void cropImageByImageMagick(File srcImageFile, int x, int y,
			 double widthRatio, double heightRatio, File resultImageFile)
	 {
		 // 剪切下的图片大小及位置（widthxheight+x+y）
		 String cropSize = "";
		 String command = "";
		 try {
			// 读取源图像 
			 BufferedImage bufferedImage = ImageIO.read(srcImageFile);
			 int srcWidth = bufferedImage.getWidth(); // 源图宽度
			 int srcHeight = bufferedImage.getHeight(); // 源图高度
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
					// 等待程序执行结束并输出状态
//			        System.out.println(process.waitFor());
				 }
			 }
		} catch (Exception e) {
			// TODO 自动生成的 catch 块
			e.printStackTrace();
		}
	 }
 
	 // 图片格式转换以及像素设置
	 public static File covertImage(File file) throws ImageFormatException, IOException{
		 //1.读取图片
		 BufferedImage bufferedImage = ImageIO.read(file);
		 //2.创建一个空白大小相同的RGB背景
		 BufferedImage newBufferedImage = new BufferedImage(bufferedImage.getWidth(),
				 bufferedImage.getHeight(), BufferedImage.TYPE_INT_RGB);
		 newBufferedImage.createGraphics().drawImage(bufferedImage, 0, 0, Color.WHITE, null);		
		 //3.再次写入的时候以jpeg图片格式
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
	 
	 // 图片灰度化
	 public static void grayProcess(File sourceImageFile) throws IOException
	 {
		 BufferedImage sourceImage  = ImageIO.read(sourceImageFile);
		 
//		 System.out.println(sourceImage.getAlphaRaster() == null);
//		 System.exit(0);
		 
		 int width = sourceImage.getWidth();
		 int height = sourceImage.getHeight();
		 BufferedImage grayImage = new BufferedImage(width, height, BufferedImage.TYPE_BYTE_GRAY);// BufferedImage.TYPE_BYTE_GRAY指定了这是一个灰度图片 
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

		 // 获取图片格式
//		 String format = sourceImageFile.getName().substring(sourceImageFile.getName().indexOf('.')+1);
		 ImageIO.write(grayImage, "jpg", new File(sourceImageFile.getAbsolutePath().substring(0, sourceImageFile.getAbsolutePath().indexOf('.'))+".jpg"));
	 }
	 
	 // 图片预处理
	 public static File preprocessingImg(File sourceImgFile, File resultImgFile) throws ImageFormatException, IOException
	 {
		 // 通过Java类库按大小切割图片
		 ImageUtils.cropImage(sourceImgFile, 0, 0, 0, 74, resultImgFile);
		 // 图像格式转换
		 resultImgFile =  ImageUtils.covertImage(resultImgFile);
		 return resultImgFile;
	 }
	 
	 // 识别图像文字
	 public static String getCaptureText(File file)
	 {
		 String result = null;
		 String imgPath = file.getAbsolutePath();  

		 try {
			 String output = "stdout";  //防止路径中有空格
			 String command = ImageRecognition.OCR_COMMAND + " " + "\"" + imgPath + "\"" + " " + output +" "+ ImageRecognition.LANG_OPTION + " " + ImageRecognition.OCR_LANG_DATA;

			 System.out.println(command);
			 // 运行命令
			 Process process = ProcessExecutor.execute(command);
			 // 打印程序输出
			 result = ProcessExecutor.readProcessOutput(process);
			 
			 // 等待程序执行结束并输出状态
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
