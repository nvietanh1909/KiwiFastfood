using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System.Text;
using System.Web.UI.WebControls;


namespace KiwiFastfood.Services
{
    public class PaymentService : ApiService
    {
            public async Task<string> CreateVNPayPaymentAsync(int amount, string orderId, string orderInfo, string returnUrl)
            {
                var data = new { amount, orderId, orderInfo, returnUrl };
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("payments/create_payment_url", content);
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(responseData);

                return result.paymentUrl;
            }

            public void SetToken(string token)
            {
                base.SetToken(token);
            }
    }
}