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
    public class ProductService : ApiService
    {
        public async Task<string> GetAllProductsAsync(dynamic queryParams = null)
        {
            var queryString = queryParams != null
                ? "?" + string.Join("&", ((object)queryParams).GetType().GetProperties()
                    .Select(p => $"{p.Name}={Uri.EscapeDataString(p.GetValue(queryParams)?.ToString() ?? "")}"))
                : "";

            var response = await _httpClient.GetAsync($"products{queryString}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetProductByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"products/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> CreateProductAsync(dynamic productData)
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }

            var content = new StringContent(JsonConvert.SerializeObject(productData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("products", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> UpdateProductAsync(string id, dynamic productData)
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }

            var content = new StringContent(JsonConvert.SerializeObject(productData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"products/{id}", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> AddRatingAsync(string productId, dynamic ratingData)
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }

            var content = new StringContent(JsonConvert.SerializeObject(ratingData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"products/{productId}/ratings", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public void SetToken(string token)
        {
            base.SetToken(token);
        }
    }
}