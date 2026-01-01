namespace tms_cisco_mitm.Models.Requests
{
    public class UpdateSipCredentialsRequest
    {
        public string ServerUri { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? RequestedBy { get; set; }
    }
}
