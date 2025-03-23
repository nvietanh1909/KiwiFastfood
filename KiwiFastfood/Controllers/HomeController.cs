using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KiwiFastfood.Services;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace KiwiFastfood.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserService _userService;
        private readonly ProductService _productService = new ProductService();

        private bool _isLogin { get => Session["UserToken"] != null; }


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

        public async Task<ActionResult> Home(int page = 1, int limit = 10)
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
    }
}