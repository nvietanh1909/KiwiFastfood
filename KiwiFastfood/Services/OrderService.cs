using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace KiwiFastfood.Services
{
    public class OrderService : ApiService
    {
        public async Task<string> GetAllOrdersAsync(dynamic queryParams = null)
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }

            var queryString = queryParams != null
                ? "?" + string.Join("&", ((object)queryParams).GetType().GetProperties()
                    .Select(p => $"{p.Name}={Uri.EscapeDataString(p.GetValue(queryParams)?.ToString() ?? "")}"))
                : "";

            var response = await _httpClient.GetAsync($"orders{queryString}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetUserOrdersAsync()
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }

            var response = await _httpClient.GetAsync("orders");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetOrderByIdAsync(string orderId)
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }

            var response = await _httpClient.GetAsync($"orders/{orderId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> CreateOrderAsync(dynamic orderData)
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }

            var content = new StringContent(JsonConvert.SerializeObject(orderData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("orders", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> UpdateOrderStatusAsync(string orderId, dynamic statusData)
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }

            var content = new StringContent(JsonConvert.SerializeObject(statusData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"orders/{orderId}/status", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> CompleteOrderAsync(string orderId)
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }

            var response = await _httpClient.PutAsync($"orders/{orderId}/complete", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> CancelOrderAsync(string orderId)
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }

            var response = await _httpClient.DeleteAsync($"orders/{orderId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> UndoLastOperationAsync()
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }

            var response = await _httpClient.PostAsync("orders/undo", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> CreateOrderFromCartAsync(dynamic orderData)
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }

            var content = new StringContent(JsonConvert.SerializeObject(orderData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("orders", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public void SetToken(string token)
        {
            base.SetToken(token);
        }
    }
}