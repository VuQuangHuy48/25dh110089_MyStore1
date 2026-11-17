using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _25dh110089_MyStore.Models;

namespace _25dh110089_MyStore.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();
        private const int PAGE_SIZE = 5; // Số sản phẩm trên 1 trang

        // GET: Admin/Products (Có tìm kiếm, sắp xếp, phân trang)
        public ActionResult Index(string keyword, string sort, int page = 1)
        {
            var products = db.Products.Include(p => p.Category).AsQueryable();

            // 1. Tìm kiếm theo tên hoặc mô tả
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Trim();
                products = products.Where(p => p.ProductName.Contains(keyword) || p.ProductDescription.Contains(keyword));
            }

            // 2. Sắp xếp
            switch (sort)
            {
                case "name_asc": products = products.OrderBy(p => p.ProductName); break;
                case "name_desc": products = products.OrderByDescending(p => p.ProductName); break;
                case "price_asc": products = products.OrderBy(p => p.ProductPrice); break;
                case "price_desc": products = products.OrderByDescending(p => p.ProductPrice); break;
                default: products = products.OrderBy(p => p.ProductID); break;
            }

            // 3. Phân trang
            int totalItems = products.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / PAGE_SIZE);
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var pagedProducts = products.Skip((page - 1) * PAGE_SIZE).Take(PAGE_SIZE).ToList();

            // Truyền dữ liệu sang View
            ViewBag.Keyword = keyword;
            ViewBag.Sort = sort;
            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(pagedProducts);
        }

        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Product product = db.Products.Include(p => p.Category).FirstOrDefault(p => p.ProductID == id);
            if (product == null) return HttpNotFound();
            return View(product);
        }

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Admin/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,CategoryID,ProductName,ProductDescription,ProductPrice,ProductImage")] Product product, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                // Xử lý upload ảnh
                if (upload != null && upload.ContentLength > 0)
                {
                    string fileName = System.IO.Path.GetFileName(upload.FileName);
                    string folder = Server.MapPath("~/Content/Images/");
                    if (!System.IO.Directory.Exists(folder)) System.IO.Directory.CreateDirectory(folder);
                    upload.SaveAs(folder + fileName);
                    product.ProductImage = fileName;
                }
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Product product = db.Products.Find(id);
            if (product == null) return HttpNotFound();
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,CategoryID,ProductName,ProductDescription,ProductPrice,ProductImage")] Product product, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                // Xử lý upload ảnh (nếu có chọn ảnh mới)
                if (upload != null && upload.ContentLength > 0)
                {
                    string fileName = System.IO.Path.GetFileName(upload.FileName);
                    string folder = Server.MapPath("~/Content/Images/");
                    if (!System.IO.Directory.Exists(folder)) System.IO.Directory.CreateDirectory(folder);
                    upload.SaveAs(folder + fileName);
                    product.ProductImage = fileName;
                }
                db.Entry(product).State = EntityState.Modified;

                // Nếu không chọn ảnh mới thì giữ nguyên ảnh cũ (tránh bị null)
                if (upload == null || upload.ContentLength == 0)
                {
                    db.Entry(product).Property(x => x.ProductImage).IsModified = false;
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Product product = db.Products.Include(p => p.Category).FirstOrDefault(p => p.ProductID == id);
            if (product == null) return HttpNotFound();
            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}