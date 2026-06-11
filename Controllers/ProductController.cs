// Controllers/ProductController.cs
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult Detail(int id)
        {
            ViewData["ProductId"] = id;
            return View();
        }
    }
}
