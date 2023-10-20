using Microsoft.AspNetCore.Mvc;

namespace BOOK_Shopping_WEB_App.Areas.Customer.Controllers
{
    [Area("customer")]
    public class CartController : Controller
    {
       
        public IActionResult Index()
        {
            return View();
        }
    }
}
