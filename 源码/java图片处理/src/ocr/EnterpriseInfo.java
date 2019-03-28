package ocr;

public class EnterpriseInfo {

	//private int row; // ��ҵ��Ϣ����excel������ 
    private String name;  // ��ҵ����
    private String registrationNo;  // ��ҵע���
    private double enterpriseConfidence; // ��ҵ�������Ŷ�
    private double registrationNoConfidence; // ��ҵע������Ŷ�
    private String businessLicenseFile; // Ӫҵִ��ͼƬλ��
    
    
    
	public EnterpriseInfo(String name, String registrationNo, double enterpriseConfidence,
			double registrationNoConfidence, String businessLicenseFile) {
		super();
		this.name = name;
		this.registrationNo = registrationNo;
		this.enterpriseConfidence = enterpriseConfidence;
		this.registrationNoConfidence = registrationNoConfidence;
		this.businessLicenseFile = businessLicenseFile;
	}
	
	public String getName() {
		return name;
	}
	public void setName(String name) {
		this.name = name;
	}
	public String getRegistrationNo() {
		return registrationNo;
	}
	public void setRegistrationNo(String registrationNo) {
		this.registrationNo = registrationNo;
	}
	public double getEnterpriseConfidence() {
		return enterpriseConfidence;
	}
	public void setEnterpriseConfidence(double enterpriseConfidence) {
		this.enterpriseConfidence = enterpriseConfidence;
	}
	public double getRegistrationNoConfidence() {
		return registrationNoConfidence;
	}
	public void setRegistrationNoConfidence(double registrationNoConfidence) {
		this.registrationNoConfidence = registrationNoConfidence;
	}
	public String getBusinessLicenseFile() {
		return businessLicenseFile;
	}
	public void setBusinessLicenseFile(String businessLicenseFile) {
		this.businessLicenseFile = businessLicenseFile;
	}
    
    
}
