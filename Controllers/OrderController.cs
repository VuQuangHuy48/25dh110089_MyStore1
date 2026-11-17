using System.Linq;
using System.Web.Mvc;
using _25dh110089_MyStore.Models;

namespace _25dh110089_MyStore.Controllers
{
    public class OrderController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        public ActionResult Index()
        {
            // Nếu chưa đăng nhập thì bắt đăng nhập
            if (Session["CustomerUser"] == null)
                return RedirectToAction("Login", "Account", new { returnUrl = "/Order" });

            string username = Session["CustomerUser"].ToString();
            var orders = db.Orders.Where(o => o.Username == username).OrderByDescending(o => o.OrderDate).ToList();

            return View(orders);
        }

        public ActionResult Details(int id)
        {
            var order = db.Orders.Find(id);
            if (order == null) return HttpNotFound();

            var details = db.OrderDetails.Include("Product").Where(d => d.OrderID == id).ToList();
            ViewBag.Order = order;

            return View(details);
        }
    }
}