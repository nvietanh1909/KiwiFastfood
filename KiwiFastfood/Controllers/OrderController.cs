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
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;
        private readonly CartService _cartService;
        private bool _isLogin => Session["UserToken"] != null;
        private bool _isAdmin => Session["UserRole"]?.ToString()?.ToLower() == "admin";

        public OrderController()
        {
            _orderService = new OrderService();
            _cartService = new CartService();
        }

        public async Task<ActionResult> Order()
        {
            if (!_isLogin) return RedirectToAction("Login", "User");

            try
            {
                string token = Session["UserToken"].ToString();
                _orderService.SetToken(token);

                var response = await _orderService.GetUserOrdersAsync();
                dynamic result = JsonConvert.DeserializeObject(response);

                if (result?.success != true || result?.data == null)
                {
                    ViewBag.ErrorMessage = "Không thể tải đơn hàng: Dữ liệu không đúng định dạng.";
                    ViewBag.Orders = null;
                }
                else
                {
                    ViewBag.Orders = result.data;
                }

                return View();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in Index: " + ex.Message);
                ViewBag.ErrorMessage = "Không thể tải đơn hàng: " + ex.Message;
                return View();
            }
        }

        public async Task<ActionResult> Detail(string id)
        {
            if (!_isLogin) return RedirectToAction("Login", "User");
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID đơn hàng không hợp lệ.";
                return RedirectToAction("Index");
            }

            try
            {
                string token = Session["UserToken"].ToString();
                _orderService.SetToken(token);

                var response = await _orderService.GetOrderByIdAsync(id);
                dynamic result = JsonConvert.DeserializeObject(response);

                if (result?.success != true || result?.data == null)
                {
                    TempData["ErrorMessage"] = "Không thể tải chi tiết đơn hàng: " + (result?.error ?? "Dữ liệu không đúng định dạng.");
                    return RedirectToAction("Index");
                }

                ViewBag.Order = result.data.items;
                return View();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in Details: " + ex.Message);
                TempData["ErrorMessage"] = "Không thể tải chi tiết đơn hàng: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        public async Task<ActionResult> Create()
        {
            if (!_isLogin) return RedirectToAction("Login", "User");

            try
            {
                string token = Session["UserToken"].ToString();
                _cartService.SetToken(token);

                var response = await _cartService.GetCartAsync();
                dynamic result = JsonConvert.DeserializeObject(response);

                if (result?.success != true || result?.data == null || result?.data.items == null || result?.data.items.Count == 0)
                {
                    TempData["ErrorMessage"] = "Không thể tạo đơn hàng: Giỏ hàng trống.";
                    return RedirectToAction("Cart", "Cart");
                }

                ViewBag.Cart = result.data;
                return View();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in Create: " + ex.Message);
                TempData["ErrorMessage"] = "Không thể tạo đơn hàng: " + ex.Message;
                return RedirectToAction("Cart", "Cart");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(FormCollection collection)
        {
            if (!_isLogin) return RedirectToAction("Login", "User");

            try
            {
                string token = Session["UserToken"].ToString();
                _orderService.SetToken(token);

                var orderData = new
                {
                    shippingAddress = new
                    {
                        street = collection["shippingAddress"],
                        city = collection["city"] ?? "Thành phố Hồ Chí Minh",
                        state = collection["state"] ?? "Không có",
                        zipCode = collection["zipCode"] ?? "70000",
                        country = collection["country"] ?? "Việt Nam"
                    },
                    phoneNumber = collection["phoneNumber"],
                    paymentMethod = collection["paymentMethod"],
                    notes = collection["notes"]
                };

                var response = await _orderService.CreateOrderFromCartAsync(orderData);
                dynamic result = JsonConvert.DeserializeObject(response);

                if (result?.success != true)
                {
                    TempData["ErrorMessage"] = "Không thể tạo đơn hàng: " + (result?.error ?? "Lỗi không xác định.");
                    return RedirectToAction("Create");
                }

                if (collection["paymentMethod"] == "online_payment")
                {
                    _orderService.SetToken(token);
                    var _response = await _orderService.GetUserOrdersAsync();
                    dynamic _result = JsonConvert.DeserializeObject(response);

                    var amount = (int)_result.data.totalPrice;
                    var orderId = (string)_result.data._id;
                    var orderInfo = $"VNPay.{orderId}";

                    return RedirectToAction("CreateVNPayPayment", "Payment", new { amount, orderId, orderInfo });
                }

                TempData["SuccessMessage"] = "Đơn hàng đã được tạo thành công.";
                return RedirectToAction("Home", "Home");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in Create POST: " + ex.Message);
                TempData["ErrorMessage"] = "Không thể tạo đơn hàng: " + ex.Message;
                return RedirectToAction("Create");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Cancel(string id)
        {
            if (!_isLogin) return RedirectToAction("Login", "User");
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID đơn hàng không hợp lệ.";
                return RedirectToAction("Index");
            }

            try
            {
                string token = Session["UserToken"].ToString();
                _orderService.SetToken(token);

                var response = await _orderService.CancelOrderAsync(id);
                dynamic result = JsonConvert.DeserializeObject(response);

                if (result?.success != true)
                {
                    TempData["ErrorMessage"] = "Không thể hủy đơn hàng: " + (result?.error ?? "Lỗi không xác định.");
                    return RedirectToAction("Details", new { id });
                }

                TempData["SuccessMessage"] = "Đơn hàng đã được hủy thành công.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in Cancel: " + ex.Message);
                TempData["ErrorMessage"] = "Không thể hủy đơn hàng: " + ex.Message;
                return RedirectToAction("Details", new { id });
            }
        }

        [HttpGet]
        public async Task<ActionResult> Admin()
        {
            if (!_isLogin || !_isAdmin) return RedirectToAction("Login", "User");

            try
            {
                string token = Session["UserToken"].ToString();
                _orderService.SetToken(token);

                int page = Request.QueryString["page"] != null ? Convert.ToInt32(Request.QueryString["page"]) : 1;
                int limit = Request.QueryString["limit"] != null ? Convert.ToInt32(Request.QueryString["limit"]) : 10;
                
                var filter = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(Request.QueryString["status"]))
                {
                    filter.Add("status", Request.QueryString["status"]);
                }
                if (!string.IsNullOrEmpty(Request.QueryString["paymentStatus"]))
                {
                    filter.Add("paymentStatus", Request.QueryString["paymentStatus"]);
                }

                var queryParams = new { page, limit, filter };
                var response = await _orderService.GetAllOrdersAsync(queryParams);
                dynamic result = JsonConvert.DeserializeObject(response);

                if (result?.success != true || result?.data == null)
                {
                    ViewBag.ErrorMessage = "Không thể tải danh sách đơn hàng: Dữ liệu không đúng định dạng.";
                    ViewBag.Orders = null;
                    ViewBag.Pagination = null;
                }
                else
                {
                    ViewBag.Orders = result.data.orders;
                    ViewBag.Pagination = result.data.pagination;
                }

                return View();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in Admin: " + ex.Message);
                ViewBag.ErrorMessage = "Không thể tải danh sách đơn hàng: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateStatus(string id, string status, string paymentStatus)
        {
            if (!_isLogin || !_isAdmin) return RedirectToAction("Login", "User");
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID đơn hàng không hợp lệ.";
                return RedirectToAction("Admin");
            }

            try
            {
                string token = Session["UserToken"].ToString();
                _orderService.SetToken(token);

                var statusData = new
                {
                    status,
                    paymentStatus
                };

                var response = await _orderService.UpdateOrderStatusAsync(id, statusData);
                dynamic result = JsonConvert.DeserializeObject(response);

                if (result?.success != true)
                {
                    TempData["ErrorMessage"] = "Không thể cập nhật trạng thái đơn hàng: " + (result?.error ?? "Lỗi không xác định.");
                    return RedirectToAction("Admin");
                }

                TempData["SuccessMessage"] = "Trạng thái đơn hàng đã được cập nhật thành công.";
                return RedirectToAction("Admin");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in UpdateStatus: " + ex.Message);
                TempData["ErrorMessage"] = "Không thể cập nhật trạng thái đơn hàng: " + ex.Message;
                return RedirectToAction("Admin");
            }
        }

        [HttpPost]
        public async Task<ActionResult> CompleteOrder(string id)
        {
            if (!_isLogin || !_isAdmin) return RedirectToAction("Login", "User");
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID đơn hàng không hợp lệ.";
                return RedirectToAction("Admin");
            }

            try
            {
                string token = Session["UserToken"].ToString();
                _orderService.SetToken(token);

                var response = await _orderService.CompleteOrderAsync(id);
                dynamic result = JsonConvert.DeserializeObject(response);

                if (result?.success != true)
                {
                    TempData["ErrorMessage"] = "Không thể hoàn tất đơn hàng: " + (result?.error ?? "Lỗi không xác định.");
                    return RedirectToAction("Admin");
                }

                TempData["SuccessMessage"] = "Đơn hàng đã được hoàn tất thành công.";
                return RedirectToAction("Admin");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in CompleteOrder: " + ex.Message);
                TempData["ErrorMessage"] = "Không thể hoàn tất đơn hàng: " + ex.Message;
                return RedirectToAction("Admin");
            }
        }

        [HttpPost]
        public async Task<ActionResult> UndoLastOperation()
        {
            if (!_isLogin || !_isAdmin) return RedirectToAction("Login", "User");

            try
            {
                string token = Session["UserToken"].ToString();
                _orderService.SetToken(token);

                var response = await _orderService.UndoLastOperationAsync();
                dynamic result = JsonConvert.DeserializeObject(response);

                if (result?.success != true)
                {
                    TempData["ErrorMessage"] = "Không thể hoàn tác thao tác: " + (result?.error ?? "Lỗi không xác định.");
                    return RedirectToAction("Admin");
                }

                TempData["SuccessMessage"] = "Hoàn tác thao tác thành công.";
                return RedirectToAction("Admin");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in UndoLastOperation: " + ex.Message);
                TempData["ErrorMessage"] = "Không thể hoàn tác thao tác: " + ex.Message;
                return RedirectToAction("Admin");
            }
        }

        [HttpPost]
        public async Task<ActionResult> QuickOrder()
        {
            if (!_isLogin) return RedirectToAction("Login", "User");

            try
            {
                string token = Session["UserToken"].ToString();
                _orderService.SetToken(token);
                
                // Lấy thông tin người dùng từ session nếu có
                string userPhone = Session["UserPhone"]?.ToString();
                
                // Get cart items to include in order
                _cartService.SetToken(token);
                var cartResponse = await _cartService.GetCartAsync();
                dynamic cartResult = JsonConvert.DeserializeObject(cartResponse);
                
                if (cartResult?.success != true || cartResult?.data == null || cartResult?.data.items == null || cartResult?.data.items.Count == 0)
                {
                    TempData["ErrorMessage"] = "Không thể tạo đơn hàng: Giỏ hàng trống.";
                    return RedirectToAction("Cart", "Cart");
                }
                
                // Extract items from cart
                var items = new List<object>();
                foreach (var item in cartResult.data.items)
                {
                    items.Add(new
                    {
                        maMon = item.productId,
                        soLuong = item.quantity
                    });
                }
                
                // Tạo dữ liệu đơn hàng với giá trị mặc định và định dạng đúng
                var orderData = new
                {
                    items,
                    shippingAddress = new
                    {
                        street = "Giao hàng trực tiếp tại cửa hàng",
                        city = "Thành phố Hồ Chí Minh",
                        state = "Không có",
                        zipCode = "70000",
                        country = "Việt Nam"
                    },
                    phoneNumber = userPhone ?? "Sẽ liên hệ sau",
                    paymentMethod = "cash",
                    notes = "Đơn hàng nhanh từ giỏ hàng"
                };

                var response = await _orderService.CreateOrderFromCartAsync(orderData);
                dynamic result = JsonConvert.DeserializeObject(response);

                if (result?.success != true)
                {
                    TempData["ErrorMessage"] = "Không thể tạo đơn hàng: " + (result?.error ?? "Lỗi không xác định.");
                    return RedirectToAction("Cart", "Cart");
                }

                TempData["SuccessMessage"] = "Đơn hàng đã được tạo thành công. Chúng tôi sẽ liên hệ để xác nhận đơn hàng của bạn.";
                return RedirectToAction("Detail", new { id = result.data._id });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in QuickOrder: " + ex.Message);
                TempData["ErrorMessage"] = "Không thể tạo đơn hàng: " + ex.Message;
                return RedirectToAction("Cart", "Cart");
            }
        }

        public ActionResult OrderSuccess()
        {
            return View();
        }
    }
}