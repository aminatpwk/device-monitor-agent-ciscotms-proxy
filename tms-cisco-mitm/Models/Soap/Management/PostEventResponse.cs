using System.Xml.Serialization;

namespace tms_cisco_mitm.Models.Soap.Management
{
    [XmlType("PostEventResponse", Namespace = "http://www.tandberg.net/2004/11/SystemManagementService/")]
    public class PostEventResponse
    {
        [XmlElement("PostEventResult")]
        public PostEventResult PostEventResult { get; set; }
    }
}
