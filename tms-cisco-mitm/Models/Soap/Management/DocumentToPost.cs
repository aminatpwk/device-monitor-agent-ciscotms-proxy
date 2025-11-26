using System.Xml.Serialization;

namespace tms_cisco_mitm.Models.Soap.Management
{
    [XmlType("DocumentToPost", Namespace = "http://www.tandberg.net/2004/11/SystemManagementService/")]
    public class DocumentToPost
    {
        [XmlElement("Location")]
        public string Location { get; set; }

        //the below property may be removed if any minimal api is listening on the standard postdocument url
        [XmlElement("URL")]
        public string URL { get; set; }
    }
}
