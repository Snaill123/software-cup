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
		
		String excelFile = "E:\\eclipse-workspace\\TesseractOCR\\result\\out.xls";	//�����excel�ļ���   
		String sheetName = "Ӫҵִ����Ϣ";	//�����excel�ļ���������   
		String[] tableHeader = { "��ҵ����","��ҵע���", "��ҵ�������Ŷ�", "��ҵע������Ŷ�", "ͼƬ" };	//excel������ı���
		
		LinkedList<EnterpriseInfo> enterpriseInfoList = new LinkedList<EnterpriseInfo>();
		EnterpriseInfo info1 = new EnterpriseInfo("���޼������޹�˾", "914210236162401000", 0.32, 0.89, "E:\\\\test1");
		EnterpriseInfo info2 = new EnterpriseInfo("ʢǧ�������רӪ��", "914210236162401000", 0.66, 0.96, "E:\\\\test2");
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
	
	// ����ʽ������д��excel
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
			System.err.println("�޷������ļ���" + excelFile);
			e.printStackTrace();
		} catch (IOException e) {
			System.err.println("����WorkBookʧ��");
			e.printStackTrace();
		}
		
		//��ӵ�һ��������
		WritableSheet sheet = workbook.createSheet(sheetName, 0);
		
		// д���ͷ
		jxl.write.Label label;   
		for (int i=0; i<tableHeader.length; i++)
		{   
			//Label(�к�,�к� ,���� )   
			label = new jxl.write.Label(i, 0, tableHeader[i]); //put the title in row1    
			try {
				sheet.addCell(label);
			} catch (RowsExceededException e) {
				System.err.println("д���ͷʧ�ܣ�RowsExceededException");
				e.printStackTrace();
			} catch (WriteException e) {
				System.err.println("д���ͷʧ�ܣ�WriteException");
				e.printStackTrace();
			}    
		}
		
		// д������
		jxl.write.Label str; // �ַ�������
		jxl.write.Number number;  // ����
		EnterpriseInfo enterpriseInfo;	// ��ҵ��Ϣ
		int dataCount = data.size();
		for (int row = 0; row < dataCount; row++)
		{			
			try {
				enterpriseInfo = data.get(row);
				// ���  row+1 �У�д������
				// д����ҵע���
				str = new jxl.write.Label(0, row + 1, enterpriseInfo.getRegistrationNo());
				sheet.addCell(str);
				// д����ҵ����
				str = new jxl.write.Label(1, row + 1, enterpriseInfo.getName());
				sheet.addCell(str);				
				// д����ҵ�������Ŷ�
				number = new jxl.write.Number(2, row + 1, enterpriseInfo.getEnterpriseConfidence());
				sheet.addCell(number);
				// д����ҵע������Ŷ�
				number = new jxl.write.Number(3, row + 1, enterpriseInfo.getRegistrationNoConfidence());
				sheet.addCell(number);
				// д�빤��ͼƬ��ַ
				str = new jxl.write.Label(4, row + 1, enterpriseInfo.getBusinessLicenseFile());
				sheet.addCell(str);
			} catch (RowsExceededException e) {
				System.err.println("д������ʧ�ܣ�RowsExceededException");
				e.printStackTrace();
			} catch (WriteException e) {
				System.err.println("д������ʧ�ܣ�WriteException");
				e.printStackTrace();
			}
		}
		
		try {
			// ���һ��д�������־
			jxl.write.Label endMark = endMark = new jxl.write.Label(0, dataCount + 1, "#����#");
			sheet.addCell(endMark);
		} catch (RowsExceededException e) {
			System.err.println("д�������־ʧ�ܣ�RowsExceededException");
			e.printStackTrace();
		} catch (WriteException e) {
			System.err.println("д�������־ʧ�ܣ�WriteException");
			e.printStackTrace();
		}
		
		try {
			workbook.write();
			workbook.close();
			System.out.println("end");  
		} catch (IOException e) {
			System.err.println("workbookд��excelʧ�ܣ�WriteException");
			e.printStackTrace();
		} catch (WriteException e) {
			System.err.println("�ر�workbookʧ�ܣ�WriteException");
			e.printStackTrace();
		}    
		 
	}


	public void test()
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
