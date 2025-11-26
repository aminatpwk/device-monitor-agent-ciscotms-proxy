using System.Xml.Serialization;

namespace tms_cisco_mitm.Models.Soap.Management
{
    [XmlType("Feedback", Namespace = "http://www.tandberg.net/2004/11/SystemManagementService/")]
    public class Feedback
    {
        [XmlElement("URL")]
        public string URL { get; set; }

        [XmlElement("Username")]
        public string Username { get; set; }

        [XmlElement("Password")]
        public string Password { get; set; }
    }
}
