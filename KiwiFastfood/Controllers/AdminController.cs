using KiwiFastfood.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KiwiFastfood.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;
        private readonly ProductService _productService = new ProductService();
        private readonly CategoryService _categoryService = new CategoryService();

        private bool _isLogin => Session["UserToken"] != null;
        private bool _isAdmin => Session["Admin"] != null;

        public AdminController()
        {
            _adminService = new AdminService();
        }

        public async Task<ActionResult> User(int page = 1, int limit = 10)
        {
            if (!_isLogin) return RedirectToAction("Login", "User");

            string token = Session["UserToken"].ToString();
            _adminService.SetToken(token);

            var response = await _adminService.GetAllUsersAsync(page, limit);
            var users = JsonConvert.DeserializeObject<dynamic>(response);
            return View(users);
        }

        public async Task<ActionResult> Order(int page = 1, int limit = 20)
        {
            if (!_isLogin) return RedirectToAction("Login", "User");

            string token = Session["UserToken"].ToString();
            _adminService.SetToken(token);
            var response = await _adminService.GetAllOrdersAsync(page, limit);
            var order = JsonConvert.DeserializeObject<dynamic>(response);
            return View(order);
        }

        public async Task<ActionResult> DetailUser(string id)
        {
            if (!_isLogin) return RedirectToAction("Login", "User");

            try
            {
                string token = Session["UserToken"].ToString();
                _adminService.SetToken(token);

                var response = await _adminService.GetAllUsersAsync(1, 1);
                var users = JsonConvert.DeserializeObject<dynamic>(response);

                var user = ((JArray)users.data.users)
                    .FirstOrDefault(u => u["id"].ToString() == id);

                if (user == null)
                {
                    return HttpNotFound();
                }

                ViewBag.User = user;
                ViewBag.Id = id;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi: " + ex.Message;
                return View();
            }
        }

        public async Task<ActionResult> EditUser(string id)
        {
            if (!_isLogin) return RedirectToAction("Login", "User");

            try
            {
                string token = Session["UserToken"].ToString();
                _adminService.SetToken(token);

                // Lấy thông tin user từ API
                var response = await _adminService.GetAllUsersAsync(1, 1); 
                var users = JsonConvert.DeserializeObject<dynamic>(response);

                // Tìm user cần chỉnh sửa
                var user = ((JArray)users.data.users)
                    .FirstOrDefault(u => u["id"].ToString() == id);

                if (user == null)
                {
                    return HttpNotFound();
                }

                ViewBag.User = user;
                ViewBag.UserId = id;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditUser(string id, string hoTen = null, string email = null,
         string dienThoai = null, string diaChi = null, string ngaySinh = null)
        {
            if (!_isLogin) return RedirectToAction("Login", "User");

            try
            {
                string token = Session["UserToken"].ToString();
                _adminService.SetToken(token);

                // Chỉ thêm các trường có giá trị mới vào userData
                var userData = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(hoTen)) userData.Add("hoTen", hoTen);
                if (!string.IsNullOrEmpty(email)) userData.Add("email", email);
                if (!string.IsNullOrEmpty(dienThoai)) userData.Add("dienThoaiKH", dienThoai);
                if (!string.IsNullOrEmpty(diaChi)) userData.Add("diaChiKH", diaChi);
                if (!string.IsNullOrEmpty(ngaySinh)) userData.Add("ngaySinh", ngaySinh);

                // Nếu không có trường nào được cập nhật
                if (!userData.Any())
                {
                    ViewBag.ErrorMessage = "Vui lòng nhập thông tin cần cập nhật";
                    return View();
                }

                var response = await _adminService.UpdateUserAsync(userData, id);
                dynamic result = JObject.Parse(response);

                if (result.success == true)
                {
                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                    return RedirectToAction("Users");
                }
                else
                {
                    ViewBag.ErrorMessage = "Cập nhật thất bại: " + (result.message ?? "Không xác định");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi: " + ex.Message;
                return View();
            }
        }

        public async Task<ActionResult> CreateProduct()
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
        public async Task<ActionResult> CreateProduct(FormCollection form)
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
                _categoryService.SetToken(token);
                var responseCategories = await _categoryService.GetAllCategoriesAsync();
                dynamic resultCategories = JsonConvert.DeserializeObject(responseCategories);
                var maLoai = form["maLoai"];
                string id = null;

                foreach (var item in resultCategories.data)
                {
                    if (item.tenLoai == maLoai)
                    {
                        id = item._id;
                        break;
                    }
                }

                var productData = new
                {
                    tenMon = form["tenMon"],
                    giaBan = decimal.Parse(form["giaBan"]),
                    maLoai = id,
                    noiDung = form["noiDung"],
                    hinhAnh = form["anhDD"],
                    soLuongTon = int.Parse(form["soLuongTon"])
                };

                var response = await _productService.CreateProductAsync(productData);
                dynamic result = JsonConvert.DeserializeObject(response);


                if (result?.success != true)
                {
                    TempData["ErrorMessage"] = "Không thể tạo sản phẩm: " + (result?.error ?? "Lỗi không xác định.");
                    return RedirectToAction("Home", "Home");
                }

                TempData["SuccessMessage"] = "Sản phẩm đã được tạo thành công.";
                return RedirectToAction("Home", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Không thể tạo sản phẩm: " + ex.Message;
                return RedirectToAction("Home", "Home");
            }
        }
    }
}