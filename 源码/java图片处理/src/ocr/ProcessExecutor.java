package ocr;


import java.io.*;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

/**
 * 示例：执行进程并返回结果
 */
public class ProcessExecutor {

    public static final int SUCCESS = 0;            // 表示程序执行成功

//    public static final String COMMAND = null;    // 要执行的语句

    public static final String SUCCESS_MESSAGE = "程序执行成功！";

    public static final String ERROR_MESSAGE = "程序执行出错：";

    /**
     * 
     * @param command 命令行命令
     * @throws Exception
     */
    public static Process execute(final String command) throws Exception
    {
    	// 执行程序
        Process process = Runtime.getRuntime().exec(command);
        return process;
    }

    /**
     * 打印进程输出
     *
     * @param process 进程
     */
/*
    public static void readProcessOutput(final Process process) {
        // 将进程的正常输出在 System.out 中打印，进程的错误输出在 System.err 中打印
        read(process.getInputStream(), System.out);	// process.getInputStream() => java.io.BufferedInputStream
       	read(process.getErrorStream(), System.err);	// => java.io.FileInputStream
    }

    // 读取输入流
    private static void read(InputStream inputStream, PrintStream out) {
        try {
            BufferedReader reader = new BufferedReader(new InputStreamReader(inputStream, "UTF-8"));
            
//            System.out.println(inputStream.getClass().getName());
            
            String line;
            while ((line = reader.readLine()) != null) {
            	line = line.replace(' ', '\0');
            	// StringBuffer 线程安全的可变字符序列
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
    // 读取程序输出
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
    				// StringBuffer 线程安全的可变字符序列
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
    	return "无法识别";
    }
    
}