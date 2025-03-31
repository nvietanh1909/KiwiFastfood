using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace KiwiFastfood.Services
{
    public class AdminService : ApiService
    {
        //*--------------------Quản lý người dùng-------------------*
        // API lấy danh sách người dùng
        public async Task<string> GetAllUsersAsync(int page, int limit)
        {
            if (string.IsNullOrEmpty(_token))
                throw new Exception("No authentication token available. Please login first.");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await _httpClient.GetAsync($"admin/users?page={page}&limit={limit}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        // API xem chi tiết người dùng
        public async Task<string> GetUsersDetail(string userID)
        {
            if (string.IsNullOrEmpty(_token))
                throw new Exception("No authentication token available. Please login first.");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await _httpClient.GetAsync($"admin/users/{userID}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        // API cập nhật người dùng
        public async Task<string> UpdateUserAsync(Dictionary<string, object> userData, string userID)
        {
            if (string.IsNullOrEmpty(_token))
                throw new Exception("No authentication token available. Please login first.");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var content = new StringContent(
                JsonConvert.SerializeObject(userData),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PutAsync($"admin/users/{userID}", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
            }

            return await response.Content.ReadAsStringAsync();
        }

        //*--------------------Quản lý đơn hàng-------------------*
        // API lấy danh sách đơn hàng
        public async Task<string> GetAllOrdersAsync(int page, int limit)
        {
            if (string.IsNullOrEmpty(_token))
                throw new Exception("No authentication token available. Please login first.");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await _httpClient.GetAsync($"admin/orders?page={page}&limit={limit}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        //API xem chi tiết đơn hàng
        public async Task<string> getOrderDetails(string id)
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }

            var response = await _httpClient.GetAsync($"orders/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        //*--------------------Quản lý món ăn-------------------*
        // API lấy danh sách món ăn 
        public async Task<string> GetAllProductsAsync(int page, int limit)
        {
            if (string.IsNullOrEmpty(_token))
                throw new Exception("No authentication token available. Please login first.");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await _httpClient.GetAsync($"products/?page={page}&limit={limit}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        //API xem chi tiết món ăn
        public async Task<string> GetProductByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }

            var response = await _httpClient.GetAsync($"products/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        //API thêm món ăn mới
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

        //API cập nhật món ăn
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

        //API xóa món ăn
        public async Task<string> DelteProductAsync(string id)
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }

            var response = await _httpClient.DeleteAsync($"products/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        //*--------------------Quản lý danh mục-------------------*
        //API lấy danh sách danh mục
        public async Task<string> GetAllCategoriesAsync()
        {
            if (string.IsNullOrEmpty(_token))
                throw new Exception("No authentication token available. Please login first.");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await _httpClient.GetAsync("categories/");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public void SetToken(string token)
        {
            base.SetToken(token);
        }
    }
}