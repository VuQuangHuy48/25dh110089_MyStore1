using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _25dh110089_MyStore.Models;

namespace _25dh110089_MyStore.Controllers
{
    public class CartController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // Lấy giỏ hàng từ Session
        private List<CartItem> GetCart()
        {
            var cart = Session["Cart"] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                Session["Cart"] = cart;
            }
            return cart;
        }

        // GET: Cart
        public ActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        // Thêm vào giỏ hàng
        public ActionResult Add(int id, int quantity = 1)
        {
            var product = db.Products.Find(id);
            if (product != null)
            {
                var cart = GetCart();
                var item = cart.FirstOrDefault(x => x.ProductID == id);
                if (item != null)
                {
                    item.Quantity += quantity;
                }
                else
                {
                    cart.Add(new CartItem
                    {
                        ProductID = product.ProductID,
                        ProductName = product.ProductName,
                        ProductPrice = product.ProductPrice,
                        ProductImage = product.ProductImage,
                        Quantity = quantity
                    });
                }
                Session["Cart"] = cart;
            }
            return RedirectToAction("Index");
        }

        // Cập nhật số lượng
        [HttpPost]
        public ActionResult Update(int id, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductID == id);
            if (item != null)
            {
                if (quantity > 0) item.Quantity = quantity;
                else cart.Remove(item); // Nếu số lượng = 0 thì xóa
            }
            Session["Cart"] = cart;
            return RedirectToAction("Index");
        }

        // Xóa 1 sản phẩm khỏi giỏ
        public ActionResult Remove(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductID == id);
            if (item != null) cart.Remove(item);
            Session["Cart"] = cart;
            return RedirectToAction("Index");
        }

        // Xóa toàn bộ giỏ
        public ActionResult Clear()
        {
            Session["Cart"] = null;
            return RedirectToAction("Index");
        }

        // Trang Thanh toán
        public ActionResult Checkout()
        {
            if (Session["CustomerUser"] == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/Cart/Checkout" });
            }
            return View();
        }

        [HttpPost]
        public ActionResult Checkout(string address)
        {
            var cart = GetCart();
            if (cart.Count == 0) return RedirectToAction("Index");

            if (Session["CustomerUser"] != null)
            {
                var username = Session["CustomerUser"].ToString();
                var customer = db.Customers.FirstOrDefault(c => c.Username == username);

                if (customer != null)
                {
                    // Tạo đơn hàng
                    var order = new Order
                    {
                        CustomerID = customer.CustomerID,
                        OrderDate = DateTime.Now,
                        TotalAmount = cart.Sum(x => x.Quantity * x.ProductPrice),
                        PaymentStatus = "Đã thanh toán",
                        AddressDelivery = address,
                        Username = username
                    };
                    db.Orders.Add(order);
                    db.SaveChanges();

                    // Tạo chi tiết đơn hàng
                    foreach (var item in cart)
                    {
                        var detail = new OrderDetail
                        {
                            OrderID = order.OrderID,
                            ProductID = item.ProductID,
                            Quantity = item.Quantity,
                            UnitPrice = item.ProductPrice
                        };
                        db.OrderDetails.Add(detail);
                    }
                    db.SaveChanges();

                    Session["Cart"] = null; // Xóa giỏ hàng
                    return RedirectToAction("Success", new { id = order.OrderID });
                }
            }
            return View();
        }

        // Trang xác nhận thành công
        public ActionResult Success(int id)
        {
            var order = db.Orders.Find(id);
            return View(order);
        }
    }
}