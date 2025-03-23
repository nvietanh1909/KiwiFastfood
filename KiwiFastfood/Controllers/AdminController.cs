using KiwiFastfood.Services;
using Newtonsoft.Json;
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

        public AdminController()
        {
            _adminService = new AdminService();
        }

        public async Task<ActionResult> User(int page = 1, int limit = 10)
        {
            string token = Session["UserToken"].ToString();
            _adminService.SetToken(token);
            var response = await _adminService.GetAllUsersAsync(page, limit);
            var users = JsonConvert.DeserializeObject<dynamic>(response);
            return View(users);
        }
    }
}