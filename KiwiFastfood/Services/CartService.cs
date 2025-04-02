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
    public class CartService : ApiService
    {
        public async Task<string> GetCartAsync()
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }

            var response = await _httpClient.GetAsync("cart");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> AddItemAsync(dynamic itemData)
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }

            var content = new StringContent(JsonConvert.SerializeObject(itemData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("cart", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> UpdateItemQuantityAsync(string itemId, int quantity)
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }

            var updateData = new { quantity };
            var content = new StringContent(JsonConvert.SerializeObject(updateData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"cart/{itemId}", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> RemoveItemAsync(string itemId)
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }

            var response = await _httpClient.DeleteAsync($"cart/{itemId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> ClearCartAsync()
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }

            var response = await _httpClient.DeleteAsync("cart");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public void SetToken(string token)
        {
            base.SetToken(token);
        }
    }
}