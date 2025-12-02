using Microsoft.AspNetCore.Mvc;

namespace QLNHWebApp.Controllers
{
    /// <summary>
    /// Controller xử lý trang chủ - Entry point của ứng dụng
    /// Vai trò: Serve React SPA cho phía khách hàng
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)] // Không hiển thị trong Swagger (vì là MVC Controller)
    public class HomeController : Controller
    {
        /// <summary>
        /// Action Index - Route mặc định khi truy cập http://localhost:5000/
        /// Trả về file index.html (React SPA đã build)
        /// </summary>
        /// <returns>File index.html với MIME type text/html</returns>
        public IActionResult Index()
        {
            // Trả về file index.html từ wwwroot/
            // React Router sẽ handle các routes client-side (/, /menu, /cart, /booking...)
            // ~ = wwwroot folder (static files root)
            return File("~/index.html", "text/html");
        }
    }
}