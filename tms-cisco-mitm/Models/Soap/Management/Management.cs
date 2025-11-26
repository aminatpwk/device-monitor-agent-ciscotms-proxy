using System.Xml;
using System.Xml.Serialization;

namespace tms_cisco_mitm.Models.Soap.Management
{
    [XmlType("Management", Namespace = "http://www.tandberg.net/2004/11/SystemManagementService/")]
    public class Management
    {
        [XmlElement("Configuration")]
        public XmlNode Configuration { get; set; }

        [XmlElement("Command")]
        public XmlNode Command { get; set; }

        [XmlElement("Calendar")]
        public XmlNode Calendar { get; set; }

        //the above properties are XmlNode because in the WSDL file, they are defined using a mixed complex type containing <s: any/> and therefore their content 
        //can vary or contain raw XML, which is exactly what XmlNode is designed to handle during serialization and deserialization.

        [XmlElement("FilesToDownload")]
        [XmlArrayItem("FileToDownload")]
        public FileToDownload[] FilesToDownload { get; set; }

        [XmlElement("SoftwareUpgrade")]
        public SoftwareUpgrade SoftwareUpgrade { get; set; }

        [XmlElement("Software")]
        public Software Software { get; set; }

        [XmlElement("DocumentsToPost")]
        [XmlArrayItem("DocumentToPost")]
        public DocumentToPost[] DocumentsToPost { get; set; }
    }
}
