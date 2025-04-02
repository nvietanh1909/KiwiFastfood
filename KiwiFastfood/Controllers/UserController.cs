using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using KiwiFastfood.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KiwiFastfood.Controllers
{
    public class UserController : Controller
    {
        private readonly UserService _userService = new UserService();
        private readonly OrderService _orderService = new OrderService();

        private bool _isLogin { get => Session["UserToken"] != null; }

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
        public async Task<ActionResult> Login(string taiKhoan, string matKhau, bool rememberMe = false)
        {
            try
            {
                var response = await _userService.LoginAsync(taiKhoan, matKhau);
                dynamic result = JObject.Parse(response);

                if (result.success == true)
                {
                    Session["UserToken"] = result.data.token;
                    
                    Session["UserInfo"] = JsonConvert.SerializeObject(result.data.user);
                    
                    if (result.data.user.role == "admin") 
                    { 
                        Session["Admin"] = result.data.user.role;
                        return RedirectToAction("User", "Admin");
                    }

                    // Tăng thời gian timeout của session lên 12 giờ
                    Session.Timeout = 720; // 12 giờ

                    // Nếu người dùng chọn "Ghi nhớ đăng nhập"
                    if (rememberMe)
                    {
                        // Tạo cookie lưu token, hết hạn sau 30 ngày
                        var authCookie = new HttpCookie("AuthToken", result.data.token)
                        {
                            Expires = DateTime.Now.AddDays(30),
                            HttpOnly = true, // Cookie chỉ được truy cập qua HTTP, không qua JavaScript
                            Secure = true // Cookie chỉ được gửi qua HTTPS
                        };
                        Response.Cookies.Add(authCookie);

                        // Tạo cookie lưu thông tin user
                        var userInfoCookie = new HttpCookie("UserInfo", JsonConvert.SerializeObject(result.data.user))
                        {
                            Expires = DateTime.Now.AddDays(30),
                            HttpOnly = true,
                            Secure = true
                        };
                        Response.Cookies.Add(userInfoCookie);
                    }

                    return RedirectToAction("Home", "Home");
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

        // Thêm phương thức kiểm tra cookie khi khởi động ứng dụng
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Nếu chưa đăng nhập và có cookie
            if (!_isLogin && Request.Cookies["AuthToken"] != null)
            {
                var authToken = Request.Cookies["AuthToken"].Value;
                var userInfo = Request.Cookies["UserInfo"]?.Value;

                // Khôi phục session từ cookie
                Session["UserToken"] = authToken;
                if (!string.IsNullOrEmpty(userInfo))
                {
                    Session["UserInfo"] = userInfo;
                    dynamic user = JObject.Parse(userInfo);
                    if (user.role == "admin")
                    {
                        Session["Admin"] = user.role;
                    }
                }
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
            if (!_isLogin) return RedirectToAction("Login", "User");

            try
            {
                string token = Session["UserToken"].ToString();
                _userService.SetToken(token);
                _orderService.SetToken(token);

                var responseOrder = await _orderService.GetUserOrdersAsync();
                dynamic result = JsonConvert.DeserializeObject(responseOrder);

                var response = await _userService.GetUserProfileAsync();
                dynamic userProfile = JObject.Parse(response);

                ViewBag.Orders = result.data;
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
            if (!_isLogin) return RedirectToAction("Login", "User");

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
            if (!_isLogin) return RedirectToAction("Login", "User");

            return View();
        }

        // Xử lý đổi mật khẩu
        [HttpPost]
        public async Task<ActionResult> ChangePassword(string matKhauCu, string matKhauMoi)
        {
            if (!_isLogin) return RedirectToAction("Login", "User");

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
            // Xóa session
            Session.Clear();

            // Xóa cookie bằng cách set thời gian hết hạn về quá khứ
            if (Request.Cookies["AuthToken"] != null)
            {
                var authCookie = new HttpCookie("AuthToken")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(authCookie);
            }

            if (Request.Cookies["UserInfo"] != null)
            {
                var userInfoCookie = new HttpCookie("UserInfo")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(userInfoCookie);
            }

            return RedirectToAction("Login");
        }
    }
}
