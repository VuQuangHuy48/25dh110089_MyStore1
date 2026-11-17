using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _25dh110089_MyStore.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            // Nếu chưa đăng nhập thì đá về trang Login
            if (Session["AdminUser"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }
    }
}