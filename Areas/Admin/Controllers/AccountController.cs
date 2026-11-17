using System.Linq;
using System.Web.Mvc;
using _25dh110089_MyStore.Models;

namespace _25dh110089_MyStore.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Account/Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null && user.UserRole == "Admin")
            {
                // Lưu Session
                Session["AdminUser"] = user.Username;
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            ViewBag.Error = "Tài khoản hoặc mật khẩu không đúng, hoặc không có quyền Admin!";
            return View();
        }

        public ActionResult Logout()
        {
            Session["AdminUser"] = null;
            return RedirectToAction("Login", "Account", new { area = "Admin" });
        }
    }
}