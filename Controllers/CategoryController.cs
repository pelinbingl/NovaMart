// Controllers/CategoryController.cs
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult Detail(string name)
        {
            ViewData["CategoryName"] = name;
            return View("Index");
        }
    }
}
