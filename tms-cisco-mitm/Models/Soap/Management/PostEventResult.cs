using System.Xml.Serialization;

namespace tms_cisco_mitm.Models.Soap.Management
{
    [XmlType("PostEventResult", Namespace = "http://www.tandberg.net/2004/11/SystemManagementService/")]
    public class PostEventResult
    {
        [XmlElement("Management")]
        public Management Management { get; set; }

        [XmlElement("HeartBeatInterval")]
        public int HeartBeatInterval { get; set; }
    }
}
