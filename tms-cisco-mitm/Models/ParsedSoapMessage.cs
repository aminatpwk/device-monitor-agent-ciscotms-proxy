using tms_cisco_mitm.Models.Soap.Management;

namespace tms_cisco_mitm.Models
{
    public class ParsedSoapMessage
    {
        public PostEvent? PostEvent { get; set; }
        public string? Event { get; set; }
        public Identification? Identification { get; set; }
        public DateTime ParsedAt { get; set; }
    }
}
