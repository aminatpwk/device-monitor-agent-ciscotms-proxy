namespace tms_cisco_mitm.Models.Commands
{
    public enum CommandType
    {
        ConfigurationUpdate = 1,
        SoftwareUpgrade,
        FileDownload,
        SipCredentialUpdate,
        GatekeeperUpdate,
        DocumentPost,
        CalendarUpdate,
        Reboot
    }
}
