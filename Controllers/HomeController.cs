using Microsoft.AspNetCore.Mvc;

namespace LocadoraVeiculosApi.Controllers;

public class HomeController : Controller
{
    [HttpGet("/")]
    public IActionResult Index()
    {
        return View();
    }
}
