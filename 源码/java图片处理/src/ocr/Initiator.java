package ocr;

import java.io.IOException;
import java.util.HashMap;
import java.util.HashSet;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NodeList;
import org.xml.sax.SAXException;

public class Initiator {

	private static DocumentBuilder docBuilder;
	
	static {
		try {
			init();
		} catch (ParserConfigurationException e) {
			e.printStackTrace();
			System.err.println("��ʼ��ʧ�ܣ�");
		}
	}
	
	private static void init() throws ParserConfigurationException 
	{
		DocumentBuilderFactory docFactory = DocumentBuilderFactory.newInstance();
		// û��Ч��
		// java.lang.ClassCastException: org.apache.xerces.dom.DeferredTextImpl cannot be cast to org.w3c.dom.Element
		// ���⣺�Ὣ�ո�ʶ��ΪNode�����޷�ת��ΪElement����
		 docFactory.setIgnoringElementContentWhitespace(true);
		docBuilder = docFactory.newDocumentBuilder();
	}
	
	public static HashMap<String, String> getInitParams(String initFile) throws SAXException, IOException
	{
		HashMap<String, String> initParams = new HashMap<String, String>();
		//
		System.out.println(initFile);
		
		Document docReader = docBuilder.parse(initFile);
		Element root = docReader.getDocumentElement();
		NodeList params = root.getElementsByTagName("param");
		
		int len = params.getLength();
		Element param;
		String name;
		String value;
		for(int i = 0; i < len; i++)
		{
			param = (Element) params.item(i);
			name = param.getAttributes().item(0).getNodeValue();
			value = param.getTextContent();
			initParams.put(name, value);
		}
		
		System.out.println(initParams);		
		return initParams; 
	}
	
	public static void main(String[] args) {
		
		try {
			HashMap<String, String> initParams = Initiator.getInitParams("C:\\Users\\Administrator\\Desktop\\program-params.xml");
			System.out.println(initParams);
		} catch (SAXException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

}
