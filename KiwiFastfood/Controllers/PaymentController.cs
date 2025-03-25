using KiwiFastfood.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KiwiFastfood.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PaymentService _paymentService;

        public PaymentController()
        {
            _paymentService = new PaymentService();
        }

        [HttpGet]
        public async Task<ActionResult> CreateVNPayPayment(int amount, string orderId, string orderInfo)
        {
            string token = Session["UserToken"].ToString();
            _paymentService.SetToken(token);

            var returnUrl = Request.Headers["Referer"].ToString();
            string paymentUrl = await _paymentService.CreateVNPayPaymentAsync(amount, orderId, orderInfo, returnUrl);

            if (!string.IsNullOrEmpty(paymentUrl))
            {
                return Redirect(paymentUrl);
            }

            return View("Error");
        }


        public ActionResult Success()
        {
            TempData["Message"] = "Thanh toán thành công!";
            return View();
        }

        public ActionResult Failed()
        {
            TempData["Error"] = "Thanh toán thất bại!";
            return View();
        }

        public ActionResult Error()
        {
            TempData["Error"] = "Có lỗi xảy ra trong quá trình thanh toán!";
            return View();
        }
    }
}