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
            return View();
        }
    }
}