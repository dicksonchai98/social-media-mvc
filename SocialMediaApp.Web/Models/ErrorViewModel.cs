namespace SocialMediaApp.Web.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public int? StatusCode { get; set; }
    public string Title { get; set; } = "An error occurred";
    public string Message { get; set; } = "An error occurred while processing your request. Please try again later.";

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
