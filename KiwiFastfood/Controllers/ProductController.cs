using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using KiwiFastfood.Services;

namespace KiwiFastfood.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;

        private bool _isLogin { get => Session["UserToken"] != null; }

        public ProductController()
        {
            _productService = new ProductService();
        }

        public async Task<ActionResult> Product(int page = 1, int limit = 10)
        {
            try
            {
                if (_isLogin)
                {
                    string token = Session["UserToken"].ToString();
                    _productService.SetToken(token);

                    var response = await _productService.GetAllProductsAsync(new { page, limit });
                    dynamic result = JsonConvert.DeserializeObject(response);

                    ViewBag.Products = result.data.products;
                    ViewBag.Pagination = result.data.pagination;

                    return View();
                }
                return RedirectToAction("Login", "User");
               
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Không thể tải danh sách sản phẩm: " + ex.Message;
                return View();
            }
        }

        // GET: Product/Details/5 - Hiển thị chi tiết sản phẩm
        public async Task<ActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            try
            {
                // Gọi ProductService để lấy chi tiết sản phẩm
                var response = await _productService.GetProductByIdAsync(id);
                dynamic product = JsonConvert.DeserializeObject(response);

                // Truyền dữ liệu sang View
                return View(product.data);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Không thể tải thông tin sản phẩm: " + ex.Message;
                return View();
            }
        }

        // GET: Product/Create - Hiển thị form tạo sản phẩm mới
        public ActionResult Create()
        {
            return View();
        }

        // POST: Product/Create - Xử lý tạo sản phẩm mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FormCollection form)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                // Lấy dữ liệu từ form
                var productData = new
                {
                    tenMon = form["tenMon"],
                    giaBan = decimal.Parse(form["giaBan"]),
                    maLoai = form["maLoai"]
                };

                // Gọi ProductService để tạo sản phẩm
                var response = await _productService.CreateProductAsync(productData);
                dynamic result = JsonConvert.DeserializeObject(response);

                // Redirect về trang danh sách nếu thành công
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Không thể tạo sản phẩm: " + ex.Message;
                return View();
            }
        }

        // GET: Product/Edit/5 - Hiển thị form chỉnh sửa sản phẩm
        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            try
            {
                // Gọi ProductService để lấy thông tin sản phẩm
                var response = await _productService.GetProductByIdAsync(id);
                dynamic product = JsonConvert.DeserializeObject(response);

                // Truyền dữ liệu sang View
                return View(product.data);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Không thể tải thông tin sản phẩm: " + ex.Message;
                return View();
            }
        }

        // POST: Product/Edit/5 - Xử lý cập nhật sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, FormCollection form)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                // Lấy dữ liệu từ form
                var productData = new
                {
                    tenMon = form["tenMon"],
                    giaBan = decimal.Parse(form["giaBan"]),
                    maLoai = form["maLoai"]
                };

                // Gọi ProductService để cập nhật sản phẩm
                var response = await _productService.UpdateProductAsync(id, productData);
                dynamic result = JsonConvert.DeserializeObject(response);

                // Redirect về trang danh sách nếu thành công
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Không thể cập nhật sản phẩm: " + ex.Message;
                return View();
            }
        }

        public async Task<ActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            try
            {
                var response = await _productService.GetProductByIdAsync(id);
                dynamic product = JsonConvert.DeserializeObject(response);

                return View(product.data);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Không thể tải thông tin sản phẩm: " + ex.Message;
                return View();
            }
        }
    }
}