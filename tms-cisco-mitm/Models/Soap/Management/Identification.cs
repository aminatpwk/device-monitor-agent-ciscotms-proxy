using System.Xml.Serialization;

namespace tms_cisco_mitm.Models.Soap.Management
{
    [XmlType("Identification", Namespace = "http://www.tandberg.net/2004/11/SystemManagementService/")]
    public class Identification
    {
        [XmlElement("SerialNumber")]
        public string SerialNumber { get; set; }

        [XmlElement("MACAddress")]
        public string MACAddress { get; set; }

        [XmlElement("IPAddress")]
        public string IPAddress { get; set; }

        [XmlElement("IPAddressV6")]
        public string IPAddressV6 { get; set; }

        [XmlElement("NATAddress")]
        public string NATAddress { get; set; }

        [XmlElement("SystemName")]
        public string SystemName { get; set; }

        [XmlElement("SWVersion")]
        public string SWVersion { get; set; }

        [XmlElement("ProductType")]
        public string ProductType { get; set; }

        [XmlElement("ProductId")]
        public string ProductId { get; set; }

        [XmlElement("SystemType")]
        public string SystemType { get; set; }
    }
}
