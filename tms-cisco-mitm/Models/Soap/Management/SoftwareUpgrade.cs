using System.Xml.Serialization;

namespace tms_cisco_mitm.Models.Soap.Management
{
    [XmlType("SoftwareUpgrade", Namespace = "http://www.tandberg.net/2004/11/SystemManagementService/")]
    public class SoftwareUpgrade
    {
        [XmlElement("RebootAfterUpgrade")]
        public bool RebootAfterUpgrade { get; set; }

        [XmlElement("FileURL")]
        public string FileURL { get; set; }

        [XmlElement("CRC")]
        public string CRC { get; set; }

        [XmlElement("Size")]
        public int Size { get; set; }

        [XmlElement("Comment")]
        public string Comment { get; set; }

        [XmlElement("ReleaseKey")]
        public string ReleaseKey { get; set; }
    }
}
