using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KiwiFastfood.Services;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace KiwiFastfood.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserService _userService;

        public HomeController()
        {
            _userService = new UserService();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public async Task<ActionResult> Home()
        {
            try
            {
                // Kiểm tra trạng thái đăng nhập
                if (Session["UserToken"] == null)
                {
                    return RedirectToAction("Login", "User");
                }

                // Lấy token từ session và gán cho UserService
                string token = Session["UserToken"].ToString();
                _userService.SetToken(token);

                // Lấy thông tin người dùng
                var response = await _userService.GetUserProfileAsync();
                dynamic userProfile = JObject.Parse(response);

                // Truyền thông tin người dùng sang view
                ViewBag.UserProfile = userProfile.data;
                
                return View();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                ViewBag.ErrorMessage = "Không thể lấy thông tin người dùng: " + ex.Message;
                return View();
            }
        }
    }
}