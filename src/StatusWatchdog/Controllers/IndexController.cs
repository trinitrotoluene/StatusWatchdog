using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

[Route("/")]
public class IndexController : Controller
{
    private readonly IWebHostEnvironment _env;

    public IndexController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return File("~/index.html", "text/html");
    }
}
