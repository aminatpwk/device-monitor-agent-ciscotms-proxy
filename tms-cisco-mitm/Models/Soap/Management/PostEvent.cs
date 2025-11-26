using System.Xml.Serialization;

namespace tms_cisco_mitm.Models.Soap.Management
{
    [XmlType("PostEvent", Namespace = "http://www.tandberg.net/2004/11/SystemManagementService/")]
    public class PostEvent
    {
        [XmlElement("Identification")]
        public Identification Identification { get; set; }

        [XmlElement("Event")]
        public string Event { get; set; }
    }
}
