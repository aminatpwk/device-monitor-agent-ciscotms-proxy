using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using tms_cisco_mitm.Models.Commands;
using tms_cisco_mitm.Models.Soap.Envelope;
using tms_cisco_mitm.Models.Soap.Management;
using tms_cisco_mitm.Services.Interface;

namespace tms_cisco_mitm.Services
{
    public class SoapResponseBuilder : ISoapResponseBuilder
    {
        private readonly ILogger<SoapResponseBuilder> _logger;

        public SoapResponseBuilder(ILogger<SoapResponseBuilder> logger)
        {
            _logger = logger;
        }

        public string BuildPostEventResponse(List<DeviceCommand> commands, int heartbeatInterval)
        {
            try
            {
                var management = commands.Any() ? BuildManagementSection(commands) : null;
                var response = new SoapEnvelope
                {
                    Body = new SoapBody
                    {
                        PostEventResponse = new PostEventResponse
                        {
                            PostEventResult = new PostEventResult
                            {
                                Management = management,
                                HeartBeatInterval = heartbeatInterval
                            }
                        }
                    }
                };

                var serializer = new XmlSerializer(typeof(SoapEnvelope));
                using var stringWriter = new StringWriter();
                using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = false,
                    Encoding = Encoding.UTF8
                });
                serializer.Serialize(xmlWriter, response, response.Namespaces);
                return stringWriter.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to build SOAP response");
                return "<?xml version=\"1.0\" encoding=\"utf-8\"?><Success/>";
            }
        }

        public Management BuildManagementSection(List<DeviceCommand> commands)
        {
            var management = new Management();

            foreach (var command in commands)
            {
                try
                {
                    switch (command.Type)
                    {
                        case CommandType.ConfigurationUpdate:
                            AppendConfigurationCommand(management, command);
                            break;

                        case CommandType.SoftwareUpgrade:
                            AppendSoftwareUpgradeCommand(management, command);
                            break;

                        case CommandType.FileDownload:
                            AppendFileDownloadCommand(management, command);
                            break;

                        case CommandType.DocumentPost:
                            AppendDocumentPostCommand(management, command);
                            break;

                        default:
                            _logger.LogWarning("Unsupported command type {Type} for command {CommandId}",
                                command.Type, command.CommandId);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to build command {CommandId} of type {Type}",
                        command.CommandId, command.Type);
                }
            }

            return management;
        }

        private void AppendConfigurationCommand(Management management, DeviceCommand command)
        {
            var payload = JsonSerializer.Deserialize<ConfigurationUpdatePayload>(command.PayloadJson);
            if (payload == null) return;

            var xmlDoc = new XmlDocument();
            var configNode = xmlDoc.CreateElement("Configuration");
            if (payload.SystemName != null)
            {
                var systemNameNode = xmlDoc.CreateElement("SystemName");
                systemNameNode.InnerText = payload.SystemName;
                configNode.AppendChild(systemNameNode);
            }
            if (payload.Sip != null)
            {
                var sipNode = BuildSipConfigurationNode(xmlDoc, payload.Sip);
                configNode.AppendChild(sipNode);
            }

            if (payload.Gatekeeper != null)
            {
                var gkNode = BuildGatekeeperConfigurationNode(xmlDoc, payload.Gatekeeper);
                configNode.AppendChild(gkNode);
            }
            if (payload.CustomSettings != null)
            {
                foreach (var setting in payload.CustomSettings)
                {
                    var settingNode = xmlDoc.CreateElement(setting.Key);
                    settingNode.InnerText = setting.Value;
                    configNode.AppendChild(settingNode);
                }
            }
            management.Configuration = configNode;
        }

        private XmlNode BuildSipConfigurationNode(XmlDocument doc, SipConfiguration sip)
        {
            var sipNode = doc.CreateElement("SIP");

            if (sip.ServerUri != null)
            {
                var serverNode = doc.CreateElement("URI");
                serverNode.InnerText = sip.ServerUri;
                sipNode.AppendChild(serverNode);
            }
            if (sip.Username != null)
            {
                var usernameNode = doc.CreateElement("Username");
                usernameNode.InnerText = sip.Username;
                sipNode.AppendChild(usernameNode);
            }
            if (sip.Password != null)
            {
                var passwordNode = doc.CreateElement("Password");
                passwordNode.InnerText = sip.Password;
                sipNode.AppendChild(passwordNode);
            }
            if (sip.DisplayName != null)
            {
                var displayNameNode = doc.CreateElement("DisplayName");
                displayNameNode.InnerText = sip.DisplayName;
                sipNode.AppendChild(displayNameNode);
            }

            return sipNode;
        }

        private XmlNode BuildGatekeeperConfigurationNode(XmlDocument doc, GatekeeperConfiguration gk)
        {
            var gatekeepNode = doc.CreateElement("Gatekeeper");

            if (gk.Address != null)
            {
                var addressNode = doc.CreateElement("Address");
                addressNode.InnerText = gk.Address;
                gatekeepNode.AppendChild(addressNode);
            }
            
            return gatekeepNode;
        }

        private void AppendSoftwareUpgradeCommand(Management management, DeviceCommand command)
        {
            var payload = JsonSerializer.Deserialize<SoftwareUpgradePayload>(command.PayloadJson);
            if (payload == null) return;
            management.SoftwareUpgrade = new SoftwareUpgrade
            {
                FileURL = payload.FileUrl,
                CRC = payload.CRC ?? string.Empty,
                Size = payload.Size,
                RebootAfterUpgrade = payload.RebootAfterUpgrade,
                ReleaseKey = payload.ReleaseKey ?? string.Empty,
                Comment = payload.Comment ?? $"Upgrade to {payload.TargetVersion}"
            };
        }

        private void AppendFileDownloadCommand(Management management, DeviceCommand command)
        {
            var payload = JsonSerializer.Deserialize<FileDownloadPayload>(command.PayloadJson);
            if (payload == null || !payload.FilesToDownload.Any()) return;
            management.FilesToDownload = payload.FilesToDownload.Select(f => new Models.Soap.Management.FileToDownload
            {
                ClientPath = f.ClientPath,
                FileURL = f.FileURL,
                CRC = f.CRC ?? string.Empty,
                Size = f.Size,
                Comment = f.Comment ?? string.Empty
            }).ToArray();
        }

        private void AppendDocumentPostCommand(Management management, DeviceCommand command)
        {
            var payload = JsonSerializer.Deserialize<DocumentPostPayload>(command.PayloadJson);
            if (payload == null || !payload.Documents.Any()) return;
            management.DocumentsToPost = payload.Documents.Select(d => new Models.Soap.Management.DocumentToPost
            {
                Location = d.Location,
                URL = d.URL ?? string.Empty
            }).ToArray();
        }
    }
}
