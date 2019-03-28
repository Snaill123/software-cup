package ocr;

import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.OutputStream;
import java.util.LinkedList;
import java.util.List;

import jdk.nashorn.internal.objects.annotations.SpecializedFunction.LinkLogic;
import jxl.Workbook;
import jxl.write.WritableFont;
import jxl.write.WritableSheet;
import jxl.write.WritableWorkbook;
import jxl.write.WriteException;
import jxl.write.biff.RowsExceededException;

public class ExcelTool {
	
	public static void main(String[] args)
	{
		ExcelTool excelTool = new ExcelTool();
		
		String excelFile = "E:\\eclipse-workspace\\TesseractOCR\\result\\out.xls";	//输出的excel文件名   
		String sheetName = "营业执照信息";	//输出的excel文件工作表名   
		String[] tableHeader = { "企业名称","企业注册号", "企业名称置信度", "企业注册号置信度", "图片" };	//excel工作表的标题
		
		LinkedList<EnterpriseInfo> enterpriseInfoList = new LinkedList<EnterpriseInfo>();
		EnterpriseInfo info1 = new EnterpriseInfo("福娃集团有限公司", "914210236162401000", 0.32, 0.89, "E:\\\\test1");
		EnterpriseInfo info2 = new EnterpriseInfo("盛千生活服务专营店", "914210236162401000", 0.66, 0.96, "E:\\\\test2");
		for (int i = 0; i < 500; i++)
		{
			if (i % 2 == 0)
			{
				enterpriseInfoList.add(info1);
			}
			else
			{
				enterpriseInfoList.add(info2);
			}
		}
		excelTool.writeData(excelFile, sheetName, tableHeader, enterpriseInfoList);
	}

	private WritableWorkbook workbook = null;   
	
	// 将格式化数据写入excel
	public void writeData(String excelFile, String sheetName, String[] tableHeader, LinkedList<EnterpriseInfo> data)
	{
		System.out.println("begin");
		
		if (sheetName == null || sheetName == "")
		{
			sheetName = "Sheet1";
		}

		OutputStream os;
		try {
			os = new FileOutputStream(excelFile);
			workbook=Workbook.createWorkbook(os);
		} catch (FileNotFoundException e) {
			System.err.println("无法创建文件：" + excelFile);
			e.printStackTrace();
		} catch (IOException e) {
			System.err.println("创建WorkBook失败");
			e.printStackTrace();
		}
		
		//添加第一个工作表
		WritableSheet sheet = workbook.createSheet(sheetName, 0);
		
		// 写入表头
		jxl.write.Label label;   
		for (int i=0; i<tableHeader.length; i++)
		{   
			//Label(列号,行号 ,内容 )   
			label = new jxl.write.Label(i, 0, tableHeader[i]); //put the title in row1    
			try {
				sheet.addCell(label);
			} catch (RowsExceededException e) {
				System.err.println("写入表头失败：RowsExceededException");
				e.printStackTrace();
			} catch (WriteException e) {
				System.err.println("写入表头失败：WriteException");
				e.printStackTrace();
			}    
		}
		
		// 写入数据
		jxl.write.Label str; // 字符串数据
		jxl.write.Number number;  // 数字
		EnterpriseInfo enterpriseInfo;	// 企业信息
		int dataCount = data.size();
		for (int row = 0; row < dataCount; row++)
		{			
			try {
				enterpriseInfo = data.get(row);
				// 向第  row+1 行，写入数据
				// 写入企业注册号
				str = new jxl.write.Label(0, row + 1, enterpriseInfo.getRegistrationNo());
				sheet.addCell(str);
				// 写入企业名称
				str = new jxl.write.Label(1, row + 1, enterpriseInfo.getName());
				sheet.addCell(str);				
				// 写入企业名称置信度
				number = new jxl.write.Number(2, row + 1, enterpriseInfo.getEnterpriseConfidence());
				sheet.addCell(number);
				// 写入企业注册号置信度
				number = new jxl.write.Number(3, row + 1, enterpriseInfo.getRegistrationNoConfidence());
				sheet.addCell(number);
				// 写入工商图片地址
				str = new jxl.write.Label(4, row + 1, enterpriseInfo.getBusinessLicenseFile());
				sheet.addCell(str);
			} catch (RowsExceededException e) {
				System.err.println("写入内容失败：RowsExceededException");
				e.printStackTrace();
			} catch (WriteException e) {
				System.err.println("写入内容失败：WriteException");
				e.printStackTrace();
			}
		}
		
		try {
			// 最后一行写入结束标志
			jxl.write.Label endMark = endMark = new jxl.write.Label(0, dataCount + 1, "#结束#");
			sheet.addCell(endMark);
		} catch (RowsExceededException e) {
			System.err.println("写入结束标志失败：RowsExceededException");
			e.printStackTrace();
		} catch (WriteException e) {
			System.err.println("写入结束标志失败：WriteException");
			e.printStackTrace();
		}
		
		try {
			workbook.write();
			workbook.close();
			System.out.println("end");  
		} catch (IOException e) {
			System.err.println("workbook写入excel失败：WriteException");
			e.printStackTrace();
		} catch (WriteException e) {
			System.err.println("关闭workbook失败：WriteException");
			e.printStackTrace();
		}    
		 
	}


	public void test()
	{
		String targetfile = "E:\\eclipse-workspace\\TesseractOCR\\result\\out.xlsx";	//输出的excel文件名   
		String worksheet = "营业执照信息";	//输出的excel文件工作表名   
		String[] title = {"企业名称","企业注册号"};	//excel工作表的标题   

		WritableWorkbook workbook;   
		try 
		{   
			//创建可写入的Excel工作薄,运行生成的文件在tomcat/bin下   
			//workbook = Workbook.createWorkbook(new File("output.xls"));    
			System.out.println("begin");   

			OutputStream os=new FileOutputStream(targetfile);    
			workbook=Workbook.createWorkbook(os);    

			WritableSheet sheet = workbook.createSheet(worksheet, 0); //添加第一个工作表   
			//WritableSheet sheet1 = workbook.createSheet("MySheet1", 1); //可添加第二个工作   
			/* 
jxl.write.Label label = new jxl.write.Label(0, 2, "A label record"); //put a label in cell A3, Label(column,row) 
sheet.addCell(label);   
			 */ 

			jxl.write.Label label;   
			for (int i=0; i<title.length; i++)
			{   
				//Label(列号,行号 ,内容 )   
				label = new jxl.write.Label(i, 0, title[i]); //put the title in row1    
				sheet.addCell(label);    
			}   

			//下列添加的对字体等的设置均调试通过，可作参考用   

			//添加数字   
			jxl.write.Number number = new jxl.write.Number(3, 4, 3.14159); //put the number 3.14159 in cell D5   
			sheet.addCell(number);   

			//添加带有字型Formatting的对象    
			jxl.write.WritableFont wf = new jxl.write.WritableFont(WritableFont.TIMES,10,WritableFont.BOLD,true);    
			jxl.write.WritableCellFormat wcfF = new jxl.write.WritableCellFormat(wf);    
			jxl.write.Label labelCF = new jxl.write.Label(4,4,"文本",wcfF);    
			sheet.addCell(labelCF);    

			//添加带有字体颜色,带背景颜色 Formatting的对象    
			jxl.write.WritableFont wfc = new jxl.write.WritableFont(WritableFont.ARIAL,10,WritableFont.BOLD,false,jxl.format.UnderlineStyle.NO_UNDERLINE,jxl.format.Colour.RED);    
			jxl.write.WritableCellFormat wcfFC = new jxl.write.WritableCellFormat(wfc);    
			wcfFC.setBackground(jxl.format.Colour.BLUE);   
			jxl.write.Label labelCFC = new jxl.write.Label(1,5,"带颜色",wcfFC);    
			sheet.addCell(labelCFC);    

			//添加带有formatting的Number对象    
			jxl.write.NumberFormat nf = new jxl.write.NumberFormat("#.##");    
			jxl.write.WritableCellFormat wcfN = new jxl.write.WritableCellFormat(nf);    
			jxl.write.Number labelNF = new jxl.write.Number(1,1,3.1415926,wcfN);    
			sheet.addCell(labelNF);    

			//3.添加Boolean对象    
			jxl.write.Boolean labelB = new jxl.write.Boolean(0,2,false);    
			sheet.addCell(labelB);    

			//4.添加DateTime对象    
			jxl.write.DateTime labelDT = new jxl.write.DateTime(0,3,new java.util.Date());    
			sheet.addCell(labelDT);    

			//添加带有formatting的DateFormat对象    
			jxl.write.DateFormat df = new jxl.write.DateFormat("ddMMyyyyhh:mm:ss");    
			jxl.write.WritableCellFormat wcfDF = new jxl.write.WritableCellFormat(df);    
			jxl.write.DateTime labelDTF = new jxl.write.DateTime(1,3,new java.util.Date(),wcfDF);    
			sheet.addCell(labelDTF);    

			//和宾单元格   
			//sheet.mergeCells(int col1,int row1,int col2,int row2);//左上角到右下角   
			sheet.mergeCells(4,5,8,10);//左上角到右下角   
			wfc = new jxl.write.WritableFont(WritableFont.ARIAL,40,WritableFont.BOLD,false,jxl.format.UnderlineStyle.NO_UNDERLINE,jxl.format.Colour.GREEN);    
			jxl.write.WritableCellFormat wchB = new jxl.write.WritableCellFormat(wfc);    
			wchB.setAlignment(jxl.format.Alignment.CENTRE);   
			labelCFC = new jxl.write.Label(4,5,"单元合并",wchB);    
			sheet.addCell(labelCFC); //   

			//设置边框   
			jxl.write.WritableCellFormat wcsB = new jxl.write.WritableCellFormat();    
			wcsB.setBorder(jxl.format.Border.ALL,jxl.format.BorderLineStyle.THICK);   
			labelCFC = new jxl.write.Label(0,6,"边框设置",wcsB);    
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
