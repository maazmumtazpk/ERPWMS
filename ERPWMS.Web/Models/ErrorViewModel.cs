namespace ERPWMS .Web.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string? Message { get; set; }
        public string? ErrorCode { get; set; }
        public ErrorViewModel() { }
        public ErrorViewModel(string message) { Message = message; }
    }
}