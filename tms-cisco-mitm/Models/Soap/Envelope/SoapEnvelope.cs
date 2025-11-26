using System.Xml.Serialization;

namespace tms_cisco_mitm.Models.Soap.Envelope
{
    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class SoapEnvelope
    {
        public SoapBody Body { get; set; }

        public XmlSerializerNamespaces Namespaces
        {
            get
            {
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add("env", "http://schemas.xmlsoap.org/soap/envelope/");
                namespaces.Add("", "http://www.tandberg.net/2004/11/SystemManagementService/");
                return namespaces;
            }
        }
    }
}
