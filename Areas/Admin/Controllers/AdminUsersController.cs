using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using _25dh110089_MyStore.Models;

namespace _25dh110089_MyStore.Areas.Admin.Controllers
{
    public class AdminUsersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();
        private const int PAGE_SIZE = 10;

        // Chỉ hiển thị các tài khoản có Role là Admin
        public ActionResult Index(string keyword, int page = 1)
        {
            var users = db.Users.Where(u => u.UserRole == "Admin").AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Trim();
                users = users.Where(u => u.Username.Contains(keyword));
            }

            int totalItems = users.Count();
            int totalPages = (int)System.Math.Ceiling((double)totalItems / PAGE_SIZE);
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var pagedUsers = users.OrderBy(u => u.Username).Skip((page - 1) * PAGE_SIZE).Take(PAGE_SIZE).ToList();

            ViewBag.Keyword = keyword;
            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(pagedUsers);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Username,Password,UserRole")] User user)
        {
            if (ModelState.IsValid)
            {
                // Mặc định tạo mới ở đây là Admin
                user.UserRole = "Admin";
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        public ActionResult Edit(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            User user = db.Users.Find(id);
            if (user == null) return HttpNotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Username,Password,UserRole")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        public ActionResult Delete(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            User user = db.Users.Find(id);
            if (user == null) return HttpNotFound();
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}