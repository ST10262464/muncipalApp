using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Prog7312_App.Models;


// Author: Microsoft Docs Contributors  
// Reference: https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/actions

namespace Prog7312_App.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }


    public IActionResult Help()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
