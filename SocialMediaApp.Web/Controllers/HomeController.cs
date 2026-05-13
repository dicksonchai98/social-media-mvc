using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Web.Models;

namespace SocialMediaApp.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View("Error", BuildErrorViewModel(500));
    }

    [Route("Home/StatusCode")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult StatusCodePage(int code)
    {
        return View("Error", BuildErrorViewModel(code));
    }

    private ErrorViewModel BuildErrorViewModel(int code)
    {
        var model = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            StatusCode = code
        };

        (model.Title, model.Message) = code switch
        {
            400 => ("請求格式錯誤", "請確認輸入內容後再試一次。"),
            401 => ("尚未登入", "請先登入後再存取此功能。"),
            403 => ("沒有權限", "您沒有權限存取這個資源。"),
            404 => ("找不到頁面", "您要找的頁面不存在或已被移除。"),
            _ => ("系統忙碌中", "系統處理您的請求時發生錯誤，請稍後再試。")
        };

        return model;
    }
}
