using System.Xml.Serialization;

namespace tms_cisco_mitm.Models.Soap.Management
{
    [XmlType("FileToDownload", Namespace = "http://www.tandberg.net/2004/11/SystemManagementService/")]
    public class FileToDownload
    {
        [XmlElement("ClientPath")]
        public string ClientPath { get; set; }

        [XmlElement("CRC")]
        public string CRC { get; set; }

        [XmlElement("Size")]
        public int Size { get; set; }

        [XmlElement("FileURL")]
        public string FileURL { get; set; }

        [XmlElement("Comment")]
        public string Comment { get; set; }
    }
}
