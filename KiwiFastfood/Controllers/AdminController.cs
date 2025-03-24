using KiwiFastfood.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KiwiFastfood.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;

        public AdminController()
        {
            _adminService = new AdminService();
        }

        // Xem danh sách người dùng
        public async Task<ActionResult> User(int page = 1, int limit = 10)
        {
            string token = Session["UserToken"].ToString();
            _adminService.SetToken(token);
            var response = await _adminService.GetAllUsersAsync(page, limit);
            var users = JsonConvert.DeserializeObject<dynamic>(response);
            return View(users);
        }

        // Xem danh sách đơn hàng
        public async Task<ActionResult> Order(int page = 1, int limit = 20)
        {
            string token = Session["UserToken"].ToString();
            _adminService.SetToken(token);
            var response = await _adminService.GetAllOrdersAsync(page, limit);
            var orders = JsonConvert.DeserializeObject<dynamic>(response);
            return View(orders);
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
    }
}