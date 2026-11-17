using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using _25dh110089_MyStore.Models;

namespace _25dh110089_MyStore.Areas.Admin.Controllers
{
    public class OrdersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();
        private const int PAGE_SIZE = 15; // Yêu cầu: 15 đơn/trang

        // GET: Admin/Orders
        public ActionResult Index(string customerName, string status, DateTime? date, int page = 1)
        {
            var orders = db.Orders.Include(o => o.Customer).AsQueryable();

            // Tìm kiếm theo Tên khách hàng
            if (!string.IsNullOrEmpty(customerName))
            {
                orders = orders.Where(o => o.Customer.CustomerName.Contains(customerName));
            }

            // Tìm kiếm theo Trạng thái
            if (!string.IsNullOrEmpty(status))
            {
                orders = orders.Where(o => o.PaymentStatus == status);
            }

            // Tìm kiếm theo Ngày đặt
            if (date.HasValue)
            {
                orders = orders.Where(o => DbFunctions.TruncateTime(o.OrderDate) == date.Value);
            }

            // Sắp xếp mới nhất lên đầu
            orders = orders.OrderByDescending(o => o.OrderDate);

            // Phân trang
            int totalItems = orders.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / PAGE_SIZE);
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var pagedOrders = orders.Skip((page - 1) * PAGE_SIZE).Take(PAGE_SIZE).ToList();

            ViewBag.CustomerName = customerName;
            ViewBag.Status = status;
            ViewBag.Date = date?.ToString("yyyy-MM-dd");
            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;

            return View(pagedOrders);
        }

        // GET: Admin/Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var order = db.Orders.Include(o => o.Customer).FirstOrDefault(o => o.OrderID == id);
            if (order == null) return HttpNotFound();

            var details = db.OrderDetails.Include(d => d.Product).Where(d => d.OrderID == id).ToList();
            ViewBag.Order = order;
            return View(details);
        }

        // (Sẽ làm thêm phần Cập nhật trạng thái ở nhịp sau)
    }
}