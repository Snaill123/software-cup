package ocr;

import java.io.FileOutputStream;
import java.io.FileWriter;
import java.io.IOException;
import java.io.ObjectOutputStream;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import jxl.*;   
import jxl.write.*;   
import java.io.*;
import java.util.*; 

public class Test {

	public static void main(String[] args) {
		// TODO �Զ����ɵķ������
		//File f = new File("");
		//System.out.println(f.getAbsolutePath().endsWith("OCR|R"));
//		try {
//			Scanner scanner = new Scanner(System.in);
//		} catch (IOException e) {
//			// TODO �Զ����ɵ� catch ��
//			e.printStackTrace();
//		}
//		String str = "";
//		System.out.println(str.trim() == "");
		
		
//		String str = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAABLAAAAH0CAYAAAAt0NQSAACAAElEQVR42uydD4hd1bX/h2GYvzeJ";
//		String newStr = str.replaceFirst("^data:image/png;base64,", "");
//		System.out.println(str);
//		System.out.println(newStr);
	/*	
		List<String> list = new ArrayList<String>();
		
		for (String string : list) {
			System.out.println(string);
		}
		System.out.println("string");
*/
		/*String str = "��ҵע���\0:\091310000052965752F��ҵ����\0:\0����������(�ϴ����۹�˾";
		Pattern pattern = Pattern.compile("(��ҵע���:)(.*?)(��ҵ����:)(.*)");
		Matcher matcher = pattern.matcher(str);

		if (matcher.find())
		{
			System.out.println("group0:" + matcher.group(0) + "  group1:" + matcher.group(1) + "  group2:" + matcher.group(2) + "  group3:" + matcher.group(3) + "  group4:" + matcher.group(4));
		}
		else
		{
			System.out.println("δ����ƥ�䴮");
		}*/
		
		File f = new File("E:\\eclipse-workspace\\TesseractOCR\\image\\preprocessed-images1\\1.png");
		System.out.println(f.getName());
		System.out.println(f.getClass());
	}

}
  

class Excel   
{   
	public static void main(String[] args)    
	{   

		String targetfile = "E:\\eclipse-workspace\\TesseractOCR\\result\\out.xlsx";	//�����excel�ļ���   
		String worksheet = "Ӫҵִ����Ϣ";	//�����excel�ļ���������   
		String[] title = {"��ҵ����","��ҵע���"};	//excel������ı���   

		WritableWorkbook workbook;   
		try 
		{   
			//������д���Excel������,�������ɵ��ļ���tomcat/bin��   
			//workbook = Workbook.createWorkbook(new File("output.xls"));    
			System.out.println("begin");   

			OutputStream os=new FileOutputStream(targetfile);    
			workbook=Workbook.createWorkbook(os);    

			WritableSheet sheet = workbook.createSheet(worksheet, 0); //��ӵ�һ��������   
			//WritableSheet sheet1 = workbook.createSheet("MySheet1", 1); //����ӵڶ�������   
			/* 
jxl.write.Label label = new jxl.write.Label(0, 2, "A label record"); //put a label in cell A3, Label(column,row) 
sheet.addCell(label);   
			 */ 

			jxl.write.Label label;   
			for (int i=0; i<title.length; i++)
			{   
				//Label(�к�,�к� ,���� )   
				label = new jxl.write.Label(i, 0, title[i]); //put the title in row1    
				sheet.addCell(label);    
			}   

			//������ӵĶ�����ȵ����þ�����ͨ���������ο���   

			//�������   
			jxl.write.Number number = new jxl.write.Number(3, 4, 3.14159); //put the number 3.14159 in cell D5   
			sheet.addCell(number);   

			//��Ӵ�������Formatting�Ķ���    
			jxl.write.WritableFont wf = new jxl.write.WritableFont(WritableFont.TIMES,10,WritableFont.BOLD,true);    
			jxl.write.WritableCellFormat wcfF = new jxl.write.WritableCellFormat(wf);    
			jxl.write.Label labelCF = new jxl.write.Label(4,4,"�ı�",wcfF);    
			sheet.addCell(labelCF);    

			//��Ӵ���������ɫ,��������ɫ Formatting�Ķ���    
			jxl.write.WritableFont wfc = new jxl.write.WritableFont(WritableFont.ARIAL,10,WritableFont.BOLD,false,jxl.format.UnderlineStyle.NO_UNDERLINE,jxl.format.Colour.RED);    
			jxl.write.WritableCellFormat wcfFC = new jxl.write.WritableCellFormat(wfc);    
			wcfFC.setBackground(jxl.format.Colour.BLUE);   
			jxl.write.Label labelCFC = new jxl.write.Label(1,5,"����ɫ",wcfFC);    
			sheet.addCell(labelCFC);    

			//��Ӵ���formatting��Number����    
			jxl.write.NumberFormat nf = new jxl.write.NumberFormat("#.##");    
			jxl.write.WritableCellFormat wcfN = new jxl.write.WritableCellFormat(nf);    
			jxl.write.Number labelNF = new jxl.write.Number(1,1,3.1415926,wcfN);    
			sheet.addCell(labelNF);    

			//3.���Boolean����    
			jxl.write.Boolean labelB = new jxl.write.Boolean(0,2,false);    
			sheet.addCell(labelB);    

			//4.���DateTime����    
			jxl.write.DateTime labelDT = new jxl.write.DateTime(0,3,new java.util.Date());    
			sheet.addCell(labelDT);    

			//��Ӵ���formatting��DateFormat����    
			jxl.write.DateFormat df = new jxl.write.DateFormat("ddMMyyyyhh:mm:ss");    
			jxl.write.WritableCellFormat wcfDF = new jxl.write.WritableCellFormat(df);    
			jxl.write.DateTime labelDTF = new jxl.write.DateTime(1,3,new java.util.Date(),wcfDF);    
			sheet.addCell(labelDTF);    

			//�ͱ���Ԫ��   
			//sheet.mergeCells(int col1,int row1,int col2,int row2);//���Ͻǵ����½�   
			sheet.mergeCells(4,5,8,10);//���Ͻǵ����½�   
			wfc = new jxl.write.WritableFont(WritableFont.ARIAL,40,WritableFont.BOLD,false,jxl.format.UnderlineStyle.NO_UNDERLINE,jxl.format.Colour.GREEN);    
			jxl.write.WritableCellFormat wchB = new jxl.write.WritableCellFormat(wfc);    
			wchB.setAlignment(jxl.format.Alignment.CENTRE);   
			labelCFC = new jxl.write.Label(4,5,"��Ԫ�ϲ�",wchB);    
			sheet.addCell(labelCFC); //   

			//���ñ߿�   
			jxl.write.WritableCellFormat wcsB = new jxl.write.WritableCellFormat();    
			wcsB.setBorder(jxl.format.Border.ALL,jxl.format.BorderLineStyle.THICK);   
			labelCFC = new jxl.write.Label(0,6,"�߿�����",wcsB);    
			sheet.addCell(labelCFC);    
			workbook.write();    
			workbook.close();   
		}catch(Exception e)    
		{    
			e.printStackTrace();    
		}    
		System.out.println("end");   
		Runtime r=Runtime.getRuntime();    
		Process p=null;    
		//String cmd[]={"notepad","exec.java"};    
		String cmd[]={"C:\\Program Files\\Microsoft Office\\Office\\EXCEL.EXE","out.xls"};    
		try{    
			p=r.exec(cmd);    
		}    
		catch(Exception e){    
			System.out.println("error executing: "+cmd[0]);    
		}   

	}   
}