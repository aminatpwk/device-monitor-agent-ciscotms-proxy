using tms_cisco_mitm.Models.Soap.Management;

namespace tms_cisco_mitm.Models.Commands
{
    public class FileDownloadPayload
    {
        public List<FileToDownload> FilesToDownload { get; set; } = new();
    }
}
