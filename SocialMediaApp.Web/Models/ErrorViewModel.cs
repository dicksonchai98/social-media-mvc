namespace SocialMediaApp.Web.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public int? StatusCode { get; set; }
    public string Title { get; set; } = "發生錯誤";
    public string Message { get; set; } = "系統處理您的請求時發生錯誤，請稍後再試。";

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
