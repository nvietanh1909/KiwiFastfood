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
    public class CategoryService : ApiService
    {
        public async Task<string> CreateCategoryAsync(dynamic categoryData)
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }

            var content = new StringContent(JsonConvert.SerializeObject(categoryData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("categories", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetAllCategoriesAsync()
        {
            var response = await _httpClient.GetAsync("categories");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetCategoryByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"categories/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> UpdateCategoryAsync(string id, dynamic categoryData)
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }

            var content = new StringContent(JsonConvert.SerializeObject(categoryData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"categories/{id}", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> DeleteCategoryAsync(string id)
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }

            var response = await _httpClient.DeleteAsync($"categories/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetProductsByCategoryAsync(string categoryId)
        {
            var response = await _httpClient.GetAsync($"categories/{categoryId}/products");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public void SetToken(string token)
        {
            base.SetToken(token);
        }
    }
}