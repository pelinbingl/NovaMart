// Controllers/HomeController.cs
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
