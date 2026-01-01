using tms_cisco_mitm.Models.Soap.Management;

namespace tms_cisco_mitm.Models.Commands
{
    public class DocumentPostPayload
    {
        public List<DocumentToPost> Documents { get; set; } = new();
    }
}
