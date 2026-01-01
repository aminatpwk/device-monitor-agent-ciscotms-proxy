namespace tms_cisco_mitm.Models.Commands
{
    public enum CommandStatus
    {
        Pending = 1,
        Queued,
        Delivered,
        Executed,
        Failed,
        Expired
    }
}
