using System;
using System.Linq;
using System.Web.Mvc;
using _25dh110089_MyStore.Models;

namespace _25dh110089_MyStore.Controllers
{
    public class ProductController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();
        private const int PAGE_SIZE = 6; // Số sản phẩm trên 1 trang

        // GET: Product (Tìm kiếm, Sắp xếp, Phân trang)
        public ActionResult Index(string keyword, int? categoryId, string sort, int page = 1)
        {
            var products = db.Products.Include("Category").AsQueryable();

            // 1. Tìm kiếm
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Trim();
                products = products.Where(p => p.ProductName.Contains(keyword));
            }

            // 2. Lọc theo danh mục (nếu có)
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryID == categoryId.Value);
            }

            // 3. Sắp xếp
            switch (sort)
            {
                case "price_asc": products = products.OrderBy(p => p.ProductPrice); break;
                case "price_desc": products = products.OrderByDescending(p => p.ProductPrice); break;
                default: products = products.OrderBy(p => p.ProductID); break;
            }

            // 4. Phân trang
            int totalItems = products.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / PAGE_SIZE);
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var pagedProducts = products.Skip((page - 1) * PAGE_SIZE).Take(PAGE_SIZE).ToList();

            // Truyền dữ liệu ra View
            ViewBag.Keyword = keyword;
            ViewBag.CategoryId = categoryId;
            ViewBag.Sort = sort;
            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Categories = db.Categories.ToList(); // Để hiển thị menu lọc danh mục

            return View(pagedProducts);
        }

        // GET: Product/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return HttpNotFound();
            Product product = db.Products.Include("Category").FirstOrDefault(p => p.ProductID == id);
            if (product == null) return HttpNotFound();
            return View(product);
        }
    }
}