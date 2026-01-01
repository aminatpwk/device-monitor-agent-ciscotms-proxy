using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using tms_cisco_mitm.Models.Commands;
using tms_cisco_mitm.Models.Requests;
using tms_cisco_mitm.Services.Interface;

namespace tms_cisco_mitm.Controllers
{
    [ApiController]
    [Route("api/commands")]
    public class CommandController : ControllerBase
    {
        private readonly ICommandService _commandService;
        private readonly IDeviceRegistry _deviceRegistry;
        private readonly ILogger<CommandController> _logger;

        public CommandController(ICommandService commandService, IDeviceRegistry deviceRegistry, ILogger<CommandController> logger)
        {
            _commandService = commandService;
            _deviceRegistry = deviceRegistry;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitCommand([FromBody] SubmitCommandRequest request)
        {
            try
            {
                var device = await _deviceRegistry.GetDeviceAsync(request.DeviceSerialNumber);
                if (device == null)
                {
                    return NotFound(new { error = $"Device {request.DeviceSerialNumber} not found" });
                }

                if (!device.IsActive)
                {
                    return BadRequest(new { error = $"Device {request.DeviceSerialNumber} is not active" });
                }

                var command = new DeviceCommand
                {
                    DeviceSerialNumber = request.DeviceSerialNumber,
                    DeviceMacAddress = device.MacAddress,
                    Type = request.Type,
                    Priority = request.Priority,
                    PayloadJson = JsonSerializer.Serialize(request.Payload),
                    RequestedBy = request.RequestedBy,
                    ExpiresAt = request.ExpiresAt
                };

                var commandId = await _commandService.QueueCommandAsync(command);

                return Ok(new
                {
                    commandId,
                    status = "queued",
                    message = $"Command queued for device {request.DeviceSerialNumber}",
                    estimatedDelivery = $"Next heartbeat (within {device.HeartbeatInterval} seconds)"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting command for device {Serial}", request.DeviceSerialNumber);
                return StatusCode(500, new { error = "Failed to submit command" });
            }
        }

        [HttpGet("{commandId}")]
        public async Task<IActionResult> GetCommand(string commandId)
        {
            var command = await _commandService.GetCommandAsync(commandId);
            if (command == null)
            {
                return NotFound(new { error = "Command not found" });
            }

            return Ok(new
            {
                commandId = command.CommandId,
                deviceSerialNumber = command.DeviceSerialNumber,
                type = command.Type.ToString(),
                status = command.Status.ToString(),
                priority = command.Priority.ToString(),
                createdAt = command.CreatedAt,
                deliveredAt = command.DeliveredAt,
                executedAt = command.ExecutedAt,
                expiresAt = command.ExpiresAt,
                deliveryAttempts = command.DeliveryAttempts,
                errorMessage = command.ErrorMessage
            });
        }

        [HttpGet("device/{serialNumber}")]
        public async Task<IActionResult> GetDeviceCommands(string serialNumber)
        {
            var commands = await _commandService.GetDeviceCommandsAsync(serialNumber);

            return Ok(new
            {
                deviceSerialNumber = serialNumber,
                totalCommands = commands.Count,
                commands = commands.Select(c => new
                {
                    commandId = c.CommandId,
                    type = c.Type.ToString(),
                    status = c.Status.ToString(),
                    priority = c.Priority.ToString(),
                    createdAt = c.CreatedAt,
                    deliveredAt = c.DeliveredAt,
                    executedAt = c.ExecutedAt
                })
            });
        }

        [HttpPost("{serialNumber}/sip-credentials")]
        public async Task<IActionResult> UpdateSipCredentials(string serialNumber, [FromBody] UpdateSipCredentialsRequest request)
        {
            var payload = new ConfigurationUpdatePayload
            {
                Sip = new SipConfiguration
                {
                    ServerUri = request.ServerUri,
                    Username = request.Username,
                    Password = request.Password,
                    DisplayName = request.DisplayName
                }
            };

            var submitRequest = new SubmitCommandRequest
            {
                DeviceSerialNumber = serialNumber,
                Type = CommandType.ConfigurationUpdate,
                Priority = CommandPriority.High,
                Payload = payload,
                RequestedBy = request.RequestedBy
            };

            return await SubmitCommand(submitRequest);
        }

        [HttpPost("{serialNumber}/upgrade")]
        public async Task<IActionResult> TriggerUpgrade(string serialNumber, [FromBody] TriggerUpgradeRequest request)
        {
            var payload = new SoftwareUpgradePayload
            {
                FileUrl = request.FileUrl,
                TargetVersion = request.TargetVersion,
                RebootAfterUpgrade = request.RebootAfterUpgrade,
                CRC = request.CRC,
                Size = request.Size
            };

            var submitRequest = new SubmitCommandRequest
            {
                DeviceSerialNumber = serialNumber,
                Type = CommandType.SoftwareUpgrade,
                Priority = request.Priority ?? CommandPriority.Normal,
                Payload = payload,
                RequestedBy = request.RequestedBy
            };

            return await SubmitCommand(submitRequest);
        }

        [HttpGet("devices")]
        public async Task<IActionResult> GetDevices()
        {
            var devices = await _deviceRegistry.GetActiveDevicesAsync();

            return Ok(new
            {
                totalDevices = devices.Count,
                devices = devices.Select(d => new
                {
                    serialNumber = d.SerialNumber,
                    systemName = d.SystemName,
                    productType = d.ProductType,
                    softwareVersion = d.SoftwareVersion,
                    ipAddress = d.IpAddress,
                    lastContactAt = d.LastContactAt,
                    heartbeatInterval = d.HeartbeatInterval,
                    pendingCommandCount = d.PendingCommandCount
                })
            });
        }
    }
}
