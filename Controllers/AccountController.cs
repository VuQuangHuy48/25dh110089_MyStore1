using System.Linq;
using System.Web.Mvc;
using _25dh110089_MyStore.Models;

namespace _25dh110089_MyStore.Controllers
{
    public class AccountController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Đăng nhập
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password, string returnUrl)
        {
            var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null && user.UserRole == "Customer")
            {
                Session["CustomerUser"] = user.Username;

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Tài khoản hoặc mật khẩu không đúng!";
            return View();
        }

        // GET: Đăng ký
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string username, string password, string fullName, string phone, string email, string address)
        {
            // Kiểm tra tài khoản đã tồn tại chưa
            if (db.Users.Any(u => u.Username == username))
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại!";
                return View();
            }

            // 1. Tạo tài khoản User
            var newUser = new User
            {
                Username = username,
                Password = password,
                UserRole = "Customer"
            };
            db.Users.Add(newUser);

            // 2. Tạo thông tin Customer liên kết
            var newCustomer = new Customer
            {
                CustomerName = fullName,
                CustomerPhone = phone,
                CustomerEmail = email,
                CustomerAddress = address,
                Username = username
            };
            db.Customers.Add(newCustomer);

            db.SaveChanges();

            // Đăng nhập luôn sau khi đăng ký
            Session["CustomerUser"] = username;
            return RedirectToAction("Index", "Home");
        }

        // Đăng xuất
        public ActionResult Logout()
        {
            Session["CustomerUser"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}