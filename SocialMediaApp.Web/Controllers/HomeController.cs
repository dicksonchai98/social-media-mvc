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
            400 => ("Bad request", "Please verify your input and try again."),
            401 => ("Unauthorized", "Please sign in before accessing this feature."),
            403 => ("Forbidden", "You do not have permission to access this resource."),
            404 => ("Page not found", "The page you requested does not exist or has been removed."),
            _ => ("Service unavailable", "An error occurred while processing your request. Please try again later.")
        };

        return model;
    }
}
