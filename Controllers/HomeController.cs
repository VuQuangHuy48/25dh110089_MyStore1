using System.Linq;
using System.Web.Mvc;
using _25dh110089_MyStore.Models;

namespace _25dh110089_MyStore.Controllers
{
    public class HomeController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        public ActionResult Index()
        {
            var products = db.Products.OrderByDescending(p => p.ProductID).Take(8).ToList();
            return View(products);
        }
    }
}