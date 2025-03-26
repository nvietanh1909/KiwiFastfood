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
    public class CartController : Controller
    {
        private readonly CartService _cartService;
        private bool _isLogin => Session["UserToken"] != null;

        public CartController()
        {
            _cartService = new CartService();
        }

        public async Task<ActionResult> Cart()
        {
            if (!_isLogin) return RedirectToAction("Login", "User");

            try
            {
                string token = Session["UserToken"].ToString();
                _cartService.SetToken(token);

                var response = await _cartService.GetCartAsync();
                dynamic result = JsonConvert.DeserializeObject(response);

                if (result?.success != true || result?.data == null)
                {
                    ViewBag.ErrorMessage = "Không thể tải giỏ hàng: Dữ liệu không đúng định dạng.";
                    ViewBag.Cart = null;
                }
                else
                {
                    ViewBag.Cart = result.data;
                }

                return View();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in Index: " + ex.Message);
                ViewBag.ErrorMessage = "Không thể tải giỏ hàng: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Add(string productId, int? quantity)
        {
            if (!_isLogin)
            {
                return RedirectToAction("Login", "User");
            }

            if (string.IsNullOrEmpty(productId))
            {
                TempData["ErrorMessage"] = "ID sản phẩm không hợp lệ.";
                return RedirectToAction("Index");
            }

            if (!quantity.HasValue || quantity < 1)
            {
                TempData["ErrorMessage"] = "Số lượng không hợp lệ. Vui lòng nhập số lượng lớn hơn 0.";
                return RedirectToAction("Index");
            }

            try
            {
                string token = Session["UserToken"].ToString();
                _cartService.SetToken(token);

                var itemData = new { product = productId, quantity = quantity.Value };
                var response = await _cartService.AddItemAsync(itemData);
                dynamic result = JsonConvert.DeserializeObject(response);

                if (result?.success != true)
                {
                    TempData["ErrorMessage"] = "Không thể thêm sản phẩm vào giỏ hàng: " + (result?.error ?? "Lỗi không xác định.");
                    return RedirectToAction("Cart");
                }

                TempData["SuccessMessage"] = "Sản phẩm đã được thêm vào giỏ hàng.";
                return RedirectToAction("Cart");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in Add: " + ex.Message);
                TempData["ErrorMessage"] = "Không thể thêm sản phẩm vào giỏ hàng: " + ex.Message;
                return RedirectToAction("Cart");
            }
        }
    }
}