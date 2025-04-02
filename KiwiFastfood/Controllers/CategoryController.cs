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
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;
        private bool _isLogin { get => Session["UserToken"] != null; }

        public CategoryController()
        {
            _categoryService = new CategoryService();
        }

        public async Task<ActionResult> Category()
        {
            if (_isLogin)
            {
                try
                {
                    string token = Session["UserToken"].ToString();
                    _categoryService.SetToken(token);

                    var response = await _categoryService.GetAllCategoriesAsync();
                    dynamic result = JsonConvert.DeserializeObject(response);

                    ViewBag.Categories = result.data;

                    return View();
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Không thể tải danh sách danh mục: " + ex.Message;
                    return View();
                }
            }
            return RedirectToAction("Login", "User");
        }
        
    }
}