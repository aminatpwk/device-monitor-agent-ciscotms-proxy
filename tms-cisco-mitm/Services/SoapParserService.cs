using System.Xml;
using System.Xml.Serialization;
using tms_cisco_mitm.Models;
using tms_cisco_mitm.Models.Soap.Management;
using tms_cisco_mitm.Services.Interface;

namespace tms_cisco_mitm.Services
{
    public class SoapParserService : ISoapParserService
    {
        private readonly ILogger<SoapParserService> _logger;
        private readonly XmlSerializer _postEventSerializer;

        public SoapParserService(ILogger<SoapParserService> logger)
        {
            _logger = logger;
            _postEventSerializer = new XmlSerializer(typeof(PostEvent));
        }

        public async Task<ParsedSoapMessage?> ParseAsync(string rawXml)
        {
            if (string.IsNullOrWhiteSpace(rawXml))
            {
                _logger.LogWarning("Empty XML provided for parsing");
                return null;
            }

            try
            {
                return await Task.Run(() => ParseSoapMessage(rawXml));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse SOAP message");
                return null;
            }
        }

        private ParsedSoapMessage? ParseSoapMessage(string rawXml)
        {
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(rawXml);
                var namespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
                namespaceManager.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
                namespaceManager.AddNamespace("tms", "http://www.tandberg.net/2004/11/SystemManagementService/");
                var postEventNode = xmlDoc.SelectSingleNode("//soap:Body/tms:PostEvent", namespaceManager)
                                    ?? xmlDoc.SelectSingleNode("//PostEvent"); 

                if (postEventNode == null)
                {
                    _logger.LogWarning("PostEvent element not found in SOAP message");
                    return null;
                }

                PostEvent? postEvent = null;
                using (var reader = new StringReader(postEventNode.OuterXml))
                {
                    postEvent = _postEventSerializer.Deserialize(reader) as PostEvent;
                }

                if (postEvent == null)
                {
                    _logger.LogWarning("Failed to deserialize PostEvent");
                    return null;
                }

                var parsed = new ParsedSoapMessage
                {
                    PostEvent = postEvent,
                    Event = postEvent.Event,
                    Identification = postEvent.Identification,
                    ParsedAt = DateTime.UtcNow
                };

                _logger.LogInformation(
                    "Parsed PostEvent - Device: {SystemName}, IP: {IpAddress}, Serial: {SerialNumber}, Event: {Event}",
                    postEvent.Identification?.SystemName ?? "unknown",
                    postEvent.Identification?.IPAddress ?? "unknown",
                    postEvent.Identification?.SerialNumber ?? "unknown",
                    postEvent.Event ?? "unknown"
                );

                return parsed;
            }
            catch (XmlException ex)
            {
                _logger.LogError(ex, "XML parsing error");
                return null;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Deserialization error");
                return null;
            }
        }

    }
}
