package ocr;

public class EnterpriseInfo {

	//private int row; // 企业信息所在excel表格的行 
    private String name;  // 企业名称
    private String registrationNo;  // 企业注册号
    private double enterpriseConfidence; // 企业名称置信度
    private double registrationNoConfidence; // 企业注册号置信度
    private String businessLicenseFile; // 营业执照图片位置
    
    
    
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
