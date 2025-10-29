using Microsoft.AspNetCore.Mvc;

namespace QLNHWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Redirect to React app
            return File("~/index.html", "text/html");
        }
    }
}