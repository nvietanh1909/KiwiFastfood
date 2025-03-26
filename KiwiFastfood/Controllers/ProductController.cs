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
        private readonly CategoryService _categoryService;
        private bool _isLogin => Session["UserToken"] != null;
        private bool _isAdmin => Session["UserRole"]?.ToString()?.ToLower() == "admin";

        public ProductController()
        {
            _productService = new ProductService();
            _categoryService = new CategoryService();
        }

        public async Task<ActionResult> Product(int page = 1, int limit = 8)
        {
            try
            {
                var response = await _productService.GetAllProductsAsync(new { page, limit });
                dynamic result = JsonConvert.DeserializeObject(response);

                if (result?.success != true || result?.data == null)
                {
                    ViewBag.ErrorMessage = "Không thể tải danh sách sản phẩm: Dữ liệu không đúng định dạng.";
                    ViewBag.Products = null;
                    ViewBag.Pagination = null;
                }
                else
                {
                    ViewBag.Products = result.data.products;
                    ViewBag.Pagination = result.data.pagination;
                }

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Không thể tải danh sách sản phẩm: " + ex.Message;
                return View();
            }
        }

        public async Task<ActionResult> Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID sản phẩm không hợp lệ.";
                return RedirectToAction("Product");
            }

            try
            {
                var response = await _productService.GetProductByIdAsync(id);
                dynamic result = JsonConvert.DeserializeObject(response);

                if (result?.success != true || result?.data == null)
                {
                    TempData["ErrorMessage"] = "Không thể tải thông tin sản phẩm: " + (result?.error ?? "Dữ liệu không đúng định dạng.");
                    return RedirectToAction("Product");
                }

                ViewBag.Product = result.data;
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Không thể tải thông tin sản phẩm: " + ex.Message;
                return RedirectToAction("Product");
            }
        }

        public async Task<ActionResult> Create()
        {
            if (!_isLogin || !_isAdmin) return RedirectToAction("Login", "User");

            try
            {
                string token = Session["UserToken"].ToString();
                _categoryService.SetToken(token);

                var response = await _categoryService.GetAllCategoriesAsync();
                dynamic result = JsonConvert.DeserializeObject(response);

                if (result?.success != true || result?.data == null)
                {
                    ViewBag.ErrorMessage = "Không thể tải danh sách danh mục: " + (result?.error ?? "Dữ liệu không đúng định dạng.");
                    ViewBag.Categories = null;
                }
                else
                {
                    ViewBag.Categories = result.data;
                }

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Không thể tải danh sách danh mục: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FormCollection form)
        {
            if (!_isLogin || !_isAdmin) return RedirectToAction("Login", "User");

            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                string token = Session["UserToken"].ToString();
                _productService.SetToken(token);

                var productData = new
                {
                    tenMon = form["tenMon"],
                    giaBan = decimal.Parse(form["giaBan"]),
                    maLoai = form["maLoai"],
                    noiDung = form["noiDung"],
                    hinhAnh = form["hinhAnh"],
                    soLuongTon = int.Parse(form["soLuongTon"])
                };

                var response = await _productService.CreateProductAsync(productData);
                dynamic result = JsonConvert.DeserializeObject(response);

                if (result?.success != true)
                {
                    TempData["ErrorMessage"] = "Không thể tạo sản phẩm: " + (result?.error ?? "Lỗi không xác định.");
                    return RedirectToAction("Create");
                }

                TempData["SuccessMessage"] = "Sản phẩm đã được tạo thành công.";
                return RedirectToAction("Product");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Không thể tạo sản phẩm: " + ex.Message;
                return RedirectToAction("Create");
            }
        }

        public async Task<ActionResult> Edit(string id)
        {
            if (!_isLogin || !_isAdmin) return RedirectToAction("Login", "User");

            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID sản phẩm không hợp lệ.";
                return RedirectToAction("Product");
            }

            try
            {
                string token = Session["UserToken"].ToString();
                _productService.SetToken(token);
                _categoryService.SetToken(token);

                var productResponse = await _productService.GetProductByIdAsync(id);
                var categoryResponse = await _categoryService.GetAllCategoriesAsync();

                dynamic productResult = JsonConvert.DeserializeObject(productResponse);
                dynamic categoryResult = JsonConvert.DeserializeObject(categoryResponse);

                if (productResult?.success != true || productResult?.data == null)
                {
                    TempData["ErrorMessage"] = "Không thể tải thông tin sản phẩm: " + (productResult?.error ?? "Dữ liệu không đúng định dạng.");
                    return RedirectToAction("Product");
                }

                if (categoryResult?.success != true || categoryResult?.data == null)
                {
                    ViewBag.ErrorMessage = "Không thể tải danh sách danh mục: " + (categoryResult?.error ?? "Dữ liệu không đúng định dạng.");
                    ViewBag.Categories = null;
                }
                else
                {
                    ViewBag.Categories = categoryResult.data;
                }

                ViewBag.Product = productResult.data;
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Không thể tải thông tin sản phẩm: " + ex.Message;
                return RedirectToAction("Product");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, FormCollection form)
        {
            if (!_isLogin || !_isAdmin) return RedirectToAction("Login", "User");

            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                string token = Session["UserToken"].ToString();
                _productService.SetToken(token);

                var productData = new
                {
                    tenMon = form["tenMon"],
                    giaBan = decimal.Parse(form["giaBan"]),
                    maLoai = form["maLoai"],
                    noiDung = form["noiDung"],
                    hinhAnh = form["hinhAnh"],
                    soLuongTon = int.Parse(form["soLuongTon"])
                };

                var response = await _productService.UpdateProductAsync(id, productData);
                dynamic result = JsonConvert.DeserializeObject(response);

                if (result?.success != true)
                {
                    TempData["ErrorMessage"] = "Không thể cập nhật sản phẩm: " + (result?.error ?? "Lỗi không xác định.");
                    return RedirectToAction("Edit", new { id });
                }

                TempData["SuccessMessage"] = "Sản phẩm đã được cập nhật thành công.";
                return RedirectToAction("Product");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Không thể cập nhật sản phẩm: " + ex.Message;
                return RedirectToAction("Edit", new { id });
            }
        }

        public async Task<ActionResult> Delete(string id)
        {
            if (!_isLogin || !_isAdmin) return RedirectToAction("Login", "User");

            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID sản phẩm không hợp lệ.";
                return RedirectToAction("Product");
            }

            try
            {
                string token = Session["UserToken"].ToString();
                _productService.SetToken(token);

                var response = await _productService.GetProductByIdAsync(id);
                dynamic result = JsonConvert.DeserializeObject(response);

                if (result?.success != true || result?.data == null)
                {
                    TempData["ErrorMessage"] = "Không thể tải thông tin sản phẩm: " + (result?.error ?? "Dữ liệu không đúng định dạng.");
                    return RedirectToAction("Product");
                }

                ViewBag.Product = result.data;
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Không thể tải thông tin sản phẩm: " + ex.Message;
                return RedirectToAction("Product");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (!_isLogin || !_isAdmin) return RedirectToAction("Login", "User");

            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID sản phẩm không hợp lệ.";
                return RedirectToAction("Product");
            }

            try
            {
                string token = Session["UserToken"].ToString();
                _productService.SetToken(token);

                var response = await _productService.DeleteProductAsync(id);
                dynamic result = JsonConvert.DeserializeObject(response);

                if (result?.success != true)
                {
                    TempData["ErrorMessage"] = "Không thể xóa sản phẩm: " + (result?.error ?? "Lỗi không xác định.");
                    return RedirectToAction("Delete", new { id });
                }

                TempData["SuccessMessage"] = "Sản phẩm đã được xóa thành công.";
                return RedirectToAction("Product");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Không thể xóa sản phẩm: " + ex.Message;
                return RedirectToAction("Delete", new { id });
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddRating(string productId, int rating, string comment)
        {
            if (!_isLogin) return RedirectToAction("Login", "User");

            if (string.IsNullOrEmpty(productId))
            {
                TempData["ErrorMessage"] = "ID sản phẩm không hợp lệ.";
                return RedirectToAction("Details", new { id = productId });
            }

            try
            {
                string token = Session["UserToken"].ToString();
                _productService.SetToken(token);

                var ratingData = new
                {
                    rating,
                    comment
                };

                var response = await _productService.AddRatingAsync(productId, ratingData);
                dynamic result = JsonConvert.DeserializeObject(response);

                if (result?.success != true)
                {
                    TempData["ErrorMessage"] = "Không thể thêm đánh giá: " + (result?.error ?? "Lỗi không xác định.");
                    return RedirectToAction("Details", new { id = productId });
                }

                TempData["SuccessMessage"] = "Đánh giá đã được thêm thành công.";
                return RedirectToAction("Details", new { id = productId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Không thể thêm đánh giá: " + ex.Message;
                return RedirectToAction("Details", new { id = productId });
            }
        }
    }
}