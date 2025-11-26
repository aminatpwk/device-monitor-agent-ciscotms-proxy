using System.Xml.Serialization;

namespace tms_cisco_mitm.Models.Soap.Management
{
    [XmlType("Software", Namespace = "http://www.tandberg.net/2004/11/SystemManagementService/")]
    public class Software
    {
        [XmlElement("ReleaseKey")]
        public string ReleaseKey { get; set; }

        [XmlElement("SessionId")]
        public string SessionId { get; set; }

        [XmlElement("Package")]
        public Package Package { get; set; }

        [XmlElement("Feedback")]
        public Feedback Feedback { get; set; }
    }
}
