using Microsoft.AspNetCore.Mvc;

namespace QLNHWebApp.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Redirect to React app
            return File("~/index.html", "text/html");
        }
    }
}