using KiwiFastfood.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;

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

        // Xem danh sách danh mục
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

        //Xem chi tiết danh mục
        public async Task<ActionResult> DetailCategory(string id)
        {
            if (_isLogin)
            {
                try
                {
                    string token = Session["UserToken"].ToString();
                    _categoryService.SetToken(token);

                    // Lấy thông tin category từ API
                    var response = await _categoryService.GetCategoryByIdAsync(id);
                    var result = JsonConvert.DeserializeObject<dynamic>(response);

                    if (result.success == true && result.data != null)
                    {
                        ViewBag.Category = result.data;
                        ViewBag.Id = id;
                        return View();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng.";
                        return RedirectToAction("Category");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Lỗi: " + ex.Message;
                    return View();
                }
            }
            return RedirectToAction("Login", "User");
        }
    }
}