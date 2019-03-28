package ocr;


import java.io.*;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

/**
 * ʾ����ִ�н��̲����ؽ��
 */
public class ProcessExecutor {

    public static final int SUCCESS = 0;            // ��ʾ����ִ�гɹ�

//    public static final String COMMAND = null;    // Ҫִ�е����

    public static final String SUCCESS_MESSAGE = "����ִ�гɹ���";

    public static final String ERROR_MESSAGE = "����ִ�г���";

    /**
     * 
     * @param command ����������
     * @throws Exception
     */
    public static Process execute(final String command) throws Exception
    {
    	// ִ�г���
        Process process = Runtime.getRuntime().exec(command);
        return process;
    }

    /**
     * ��ӡ�������
     *
     * @param process ����
     */
/*
    public static void readProcessOutput(final Process process) {
        // �����̵���������� System.out �д�ӡ�����̵Ĵ�������� System.err �д�ӡ
        read(process.getInputStream(), System.out);	// process.getInputStream() => java.io.BufferedInputStream
       	read(process.getErrorStream(), System.err);	// => java.io.FileInputStream
    }

    // ��ȡ������
    private static void read(InputStream inputStream, PrintStream out) {
        try {
            BufferedReader reader = new BufferedReader(new InputStreamReader(inputStream, "UTF-8"));
            
//            System.out.println(inputStream.getClass().getName());
            
            String line;
            while ((line = reader.readLine()) != null) {
            	line = line.replace(' ', '\0');
            	// StringBuffer �̰߳�ȫ�Ŀɱ��ַ�����
            	ImageRecognition.strBuffer.append(line);
                out.print(line);
            }
            System.out.println("\n--------------------------------");

        } catch (IOException e) {
            e.printStackTrace();
        } finally {

            try {
                inputStream.close();
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }
*/
    // ��ȡ�������
    public static String readProcessOutput(final Process process)
    {
    	try {
    		BufferedReader reader = new BufferedReader(new InputStreamReader(process.getInputStream(), "UTF-8"));

    		String line = null;
    		boolean flag = false;
    		String result = "";
    		
    		while ((line = reader.readLine()) != null)
    		{
    			if (!line.trim().isEmpty())
    			{
    				line = line.replace(" ", "");    			
    				// StringBuffer �̰߳�ȫ�Ŀɱ��ַ�����
    				//ImageRecognition.strBuffer.append(line);
    				result += line;
    				flag = true;
    			}    			
    		}

    		if (flag)
    		{
    			synchronized (ImageRecognition.strBuffer)
    			{
    				ImageRecognition.strBuffer.append(result).append('\n');
    			}
    			return result;
    		}
    		
    		reader = new BufferedReader(new InputStreamReader(process.getErrorStream(), "UTF-8"));
    		while ((line = reader.readLine()) != null)
    		{
    			System.err.println(line);
    		}
    	} catch (IOException e) {
    		e.printStackTrace();
    	} finally {
    		try {
    			process.getInputStream().close();
    			process.getErrorStream().close();
    		} catch (IOException e) {
    			e.printStackTrace();
    		}
    	}
    	return "�޷�ʶ��";
    }
    
}