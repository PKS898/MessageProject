using Microsoft.AspNetCore.Mvc;

namespace MessageProject.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(); // Возвращает Index.cshtml
        }
    }
}
