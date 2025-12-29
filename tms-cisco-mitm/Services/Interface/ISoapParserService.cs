using tms_cisco_mitm.Models;

namespace tms_cisco_mitm.Services.Interface
{
    public interface ISoapParserService
    {
        Task<ParsedSoapMessage?> ParseAsync(string rawXml);
    }
}
