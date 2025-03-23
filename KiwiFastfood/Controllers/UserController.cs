using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using KiwiFastfood.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KiwiFastfood.Controllers
{
    public class UserController : Controller
    {
        private readonly UserService _userService = new UserService();

        public UserController()
        {
            if(_userService == null) _userService = new UserService();
        }

        // Hiển thị trang đăng nhập 
        public ActionResult Login()
        {
            return View();
        }

        // Xử lý đăng nhập
        [HttpPost]
        public async Task<ActionResult> Login(string taiKhoan, string matKhau)
        {
            try
            {
                var response = await _userService.LoginAsync(taiKhoan, matKhau);
                dynamic result = JObject.Parse(response);

                if (result.success == true)
                {
                    Session["UserToken"] = result.data.token;
                    if (result.data.role == "admin") Session["Admin"] = true;
                    return RedirectToAction("Users", "Admin");
                }
                else
                {
                    ViewBag.ErrorMessage = "Đăng nhập thất bại! Vui lòng kiểm tra lại tài khoản hoặc mật khẩu.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi: " + ex.Message;
                return View();
            }
        }

        // Hiển thị trang đăng ký
        public ActionResult Register()
        {
            return View();
        }

        // Xử lý đăng ký
        [HttpPost]
        public async Task<ActionResult> Register(string taiKhoan, string matKhau, string email, string hoTen)
        {
            try
            {
                var userData = new
                {
                    hoTen,
                    taiKhoan,
                    matKhau,
                    email,
                };

                var response = await _userService.RegisterAsync(userData);
                dynamic result = JObject.Parse(response);

                if (result.success == true)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.ErrorMessage = "Đăng ký thất bại! " + result.message;
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi: " + ex.Message;
                return View();
            }
        }

        // Hiển thị thông tin cá nhân
        public async Task<ActionResult> Profile()
        {
            try
            {
                string token = Session["UserToken"].ToString();
                _userService.SetToken(token);

                var response = await _userService.GetUserProfileAsync();
                dynamic userProfile = JObject.Parse(response);
                ViewBag.UserProfile = userProfile.data;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi khi lấy thông tin người dùng: " + ex.Message;
                return RedirectToAction("Login");
            }
        }

        // Hiển thị trang cập nhật thông tin cá nhân
        public ActionResult EditProfile()
        {
            return View();
        }

        // Xử lý cập nhật thông tin cá nhân
        [HttpPost]
        public async Task<ActionResult> EditProfile(string email, string hoTen, string diaChi)
        {
            try
            {
                var profileData = new
                {
                    email,
                    hoTen,
                    diaChi
                };

                var response = await _userService.UpdateProfileAsync(profileData);
                dynamic result = JObject.Parse(response);

                if (result.success == true)
                {
                    return RedirectToAction("Profile");
                }
                else
                {
                    ViewBag.ErrorMessage = "Cập nhật thất bại!";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi: " + ex.Message;
                return View();
            }
        }

        // Hiển thị trang đổi mật khẩu
        public ActionResult ChangePassword()
        {
            return View();
        }

        // Xử lý đổi mật khẩu
        [HttpPost]
        public async Task<ActionResult> ChangePassword(string matKhauCu, string matKhauMoi)
        {
            try
            {
                var passwordData = new
                {
                    matKhauCu,
                    matKhauMoi
                };

                var response = await _userService.ChangePasswordAsync(passwordData);
                dynamic result = JObject.Parse(response);

                if (result.success == true)
                {
                    return RedirectToAction("Profile");
                }
                else
                {
                    ViewBag.ErrorMessage = "Đổi mật khẩu thất bại!";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi: " + ex.Message;
                return View();
            }
        }

        // Đăng xuất
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
