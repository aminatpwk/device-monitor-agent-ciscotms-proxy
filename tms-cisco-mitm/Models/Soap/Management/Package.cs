using System.Xml.Serialization;

namespace tms_cisco_mitm.Models.Soap.Management
{
    [XmlType("Package", Namespace = "http://www.tandberg.net/2004/11/SystemManagementService/")]
    public class Package
    {
        [XmlElement("VersionId")]
        public string VersionId { get; set; }

        [XmlElement("URL")]
        public string URL { get; set; }

        [XmlElement("Username")]
        public string Username { get; set; }

        [XmlElement("Password")]
        public string Password { get; set; }
    }
}
