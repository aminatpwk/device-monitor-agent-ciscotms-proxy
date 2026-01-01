using tms_cisco_mitm.Models.Commands;
using tms_cisco_mitm.Models.Soap.Management;

namespace tms_cisco_mitm.Services.Interface
{
    public interface ISoapResponseBuilder
    {
        string BuildPostEventResponse(List<DeviceCommand> commands, int heartbeatInterval);
        Management BuildManagementSection(List<DeviceCommand> commands);
    }
}
