using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using _25dh110089_MyStore.Models;

namespace _25dh110089_MyStore.Controllers
{
    public class OrdersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // Hàm phụ trợ để chuyển đổi mã trạng thái sang tên hiển thị
        private string GetStatusName(int? status)
        {
            switch (status)
            {
                case 0: return "Chờ thanh toán";
                case 1: return "Đang xử lý";
                case 2: return "Đang vận chuyển";
                case 3: return "Đã giao";
                case 4: return "Đã hủy";
                default: return "Không xác định";
            }
        }

        // GET: Đơn hàng của tôi
        public ActionResult MyOrder(string searchString, string statusFilter)
        {
            int customerId = Session["CustomerID"] != null ? (int)Session["CustomerID"] : 1;
            var orders = db.Orders.Where(o => o.CustomerID == customerId);

            if (!String.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(o => o.OrderID.ToString().Contains(searchString) ||
                                           o.OrderDetails.Any(od => od.Product.ProductName.Contains(searchString)));
            }

            // 2. Xử lý Lọc theo Tab trạng thái
            if (!String.IsNullOrEmpty(statusFilter))
            {
                orders = orders.Where(o => o.PaymentStatus == statusFilter);
            }

            // Lấy dữ liệu
            var orderList = orders.OrderByDescending(o => o.OrderDate).ToList();

            var viewModelList = orderList.Select(o => new MyOrderViewModel
            {
                Order = o,
                OrderDetails = o.OrderDetails.ToList(),
                StatusName = o.PaymentStatus, // Sửa ở đây
                TotalItems = o.OrderDetails.Sum(od => od.Quantity)
            }).ToList();

            return View(viewModelList);
        }

        // Chức năng: Mua lại (Thêm 1 sản phẩm vào giỏ hàng)
        [HttpPost]
        public ActionResult BuyAgain(int productId)
        {
            // Code logic thêm vào giỏ hàng (Session["Cart"])
            // ...
            return RedirectToAction("Index", "Cart"); // Chuyển hướng sang giỏ hàng
        }
    }
}