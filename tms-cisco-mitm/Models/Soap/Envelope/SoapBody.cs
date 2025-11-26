using System.Xml.Serialization;
using tms_cisco_mitm.Models.Soap.Management;

namespace tms_cisco_mitm.Models.Soap.Envelope
{
    [XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class SoapBody
    {
        [XmlElement(ElementName = "PostEventResponse")]
        public PostEventResponse PostEventResponse { get; set; }
    }
}
