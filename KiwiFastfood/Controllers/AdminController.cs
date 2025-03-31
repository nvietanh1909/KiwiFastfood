using KiwiFastfood.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace KiwiFastfood.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;
        private bool _isLogin { get => Session["UserToken"] != null; }
        private bool _isAdmin => Session["Admin"] != null;

        public AdminController()
        {
            _adminService = new AdminService();
        }

        //*--------------------Quản lý người dùng-------------------*
        // Xem danh sách người dùng
        public async Task<ActionResult> User(int page = 1, int limit = 10)
        {
            string token = Session["UserToken"].ToString();
            _adminService.SetToken(token);
            var response = await _adminService.GetAllUsersAsync(page, limit);
            var users = JsonConvert.DeserializeObject<dynamic>(response);
            return View(users);
        }

        // Xem chi tiết người dùng
        public async Task<ActionResult> DetailUser(string id)
        {
            try
            {
                string token = Session["UserToken"].ToString();
                _adminService.SetToken(token);

                // Lấy thông tin user từ API
                var response = await _adminService.GetUsersDetail(id);
                var result = JsonConvert.DeserializeObject<dynamic>(response);

                if (result.success == true && result.data != null)
                {
                    ViewBag.User = result.data;
                    ViewBag.Id = id;
                    return View();
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng.";
                    return RedirectToAction("User");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi: " + ex.Message;
                return View();
            }
        }

        // Chỉnh sửa người dùng
        public async Task<ActionResult> EditUser(string id)
        {
            try
            {
                string token = Session["UserToken"].ToString();
                _adminService.SetToken(token);

                // Lấy thông tin user từ API
                var response = await _adminService.GetUsersDetail(id);
                var result = JsonConvert.DeserializeObject<dynamic>(response);

                if (result.success == true && result.data != null)
                {
                    ViewBag.User = result.data;
                    ViewBag.Id = id;
                    return View();
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng.";
                    return RedirectToAction("User");
                }
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
                    return RedirectToAction("User");
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

        //*--------------------Quản lý đơn hàng-------------------*
        // Xem danh sách đơn hàng
        public async Task<ActionResult> Order(int page = 1, int limit = 20)
        {
            string token = Session["UserToken"].ToString();
            _adminService.SetToken(token);
            var response = await _adminService.GetAllOrdersAsync(page, limit);
            var orders = JsonConvert.DeserializeObject<dynamic>(response);
            return View(orders);
        }

        // Xem chi tiết đơn hàng
        public async Task<ActionResult> DetailOrder(string id)
        {
            try
            {
                string token = Session["UserToken"].ToString();
                _adminService.SetToken(token);

                // Lấy thông tin order từ API
                var response = await _adminService.getOrderDetails(id);
                var result = JsonConvert.DeserializeObject<dynamic>(response);

                if (result.success == true && result.data != null)
                {
                    ViewBag.Order = result.data;
                    ViewBag.Id = id;
                    return View();
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin đơn hàng.";
                    return RedirectToAction("Order");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi: " + ex.Message;
                return View();
            }
        }

        //*--------------------Quản lý món ăn--------------------*
        // Xem danh sách món ăn
        public async Task<ActionResult> Product(int page = 1, int limit = 10)
        {
            if (_isLogin)
            {
                try
                {
                    string token = Session["UserToken"].ToString();
                    _adminService.SetToken(token);

                    var response = await _adminService.GetAllProductsAsync(page, limit);
                    var products = JsonConvert.DeserializeObject<dynamic>(response);
                    return View(products);
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Không thể tải danh sách sản phẩm: " + ex.Message;
                    return View();
                }
            }
            return RedirectToAction("Login", "User");
        }

        // Xem chi tiết món ăn
        public async Task<ActionResult> DetailProduct(string id)
        {
            try
            {
                string token = Session["UserToken"].ToString();
                _adminService.SetToken(token);

                // Lấy thông tin product từ API
                var response = await _adminService.GetProductByIdAsync(id);
                var result = JsonConvert.DeserializeObject<dynamic>(response);

                if (result.success == true && result.data != null)
                {
                    ViewBag.Product = result.data;
                    ViewBag.Id = id;
                    return View();
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin sản phẩm.";
                    return RedirectToAction("Product");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi: " + ex.Message;
                return View();
            }
        }

        // Chỉnh sửa món ăn
        public async Task<ActionResult> EditProduct(string id)
        {
            try
            {
                string token = Session["UserToken"].ToString();
                _adminService.SetToken(token);

                // Lấy thông tin user từ API
                var response = await _adminService.GetProductByIdAsync(id);
                var result = JsonConvert.DeserializeObject<dynamic>(response);

                if (result.success == true && result.data != null)
                {
                    ViewBag.Product = result.data;
                    ViewBag.Id = id;
                    return View();
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin sản phẩm.";
                    return RedirectToAction("Product");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi: " + ex.Message;
                return View();
            }
        }


        [HttpPost]
        public async Task<ActionResult> EditProduct(string id, string tenMon = null, string giaBan = null,
        string noiDung = null, string anhDD = null, string soLuongTon = null, string soLuongBan = null)
        {
            try
            {
                string token = Session["UserToken"].ToString();
                _adminService.SetToken(token);

                // Chỉ thêm các trường có giá trị mới vào productData
                var productData = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(tenMon)) productData.Add("tenMon", tenMon);
                if (!string.IsNullOrEmpty(giaBan)) productData.Add("giaBan", giaBan);
                if (!string.IsNullOrEmpty(noiDung)) productData.Add("noiDung", noiDung);
                if (!string.IsNullOrEmpty(anhDD)) productData.Add("anhDD", anhDD);
                if (!string.IsNullOrEmpty(soLuongTon)) productData.Add("soLuongTon", soLuongTon);
                if (!string.IsNullOrEmpty(soLuongBan)) productData.Add("soLuongBan", soLuongBan);

                // Nếu không có trường nào được cập nhật
                if (!productData.Any())
                {
                    ViewBag.ErrorMessage = "Vui lòng nhập thông tin cần cập nhật";
                    return View();
                }

                var response = await _adminService.UpdateProductAsync(id, productData);
                dynamic result = JObject.Parse(response);

                if (result.success == true)
                {
                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                    return RedirectToAction("Product");
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

        // Xóa món ăn
        public async Task<ActionResult> DeleteProduct(string id)
        {
            try
            {
                string token = Session["UserToken"].ToString();
                _adminService.SetToken(token);
                var response = await _adminService.DelteProductAsync(id);
                return RedirectToAction("Product");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi: " + ex.Message;
                return View();
            }
        }
    }
}