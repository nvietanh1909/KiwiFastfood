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
    public class UserService : ApiService
    {
        public async Task<string> RegisterAsync(dynamic userData)
        {
            var content = new StringContent(JsonConvert.SerializeObject(userData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("users/register", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> LoginAsync(string taiKhoan, string matKhau)
        {
            var data = new { taiKhoan, matKhau };
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("users/login", content);
            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(responseData);
            _token = result.data.token;
            SetToken(_token);
            return responseData;
        }

        public async Task<string> GetUserProfileAsync()
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }
            var response = await _httpClient.GetAsync("users/profile");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> UpdateProfileAsync(dynamic profileData)
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }
            var content = new StringContent(JsonConvert.SerializeObject(profileData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("users/profile", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> ChangePasswordAsync(dynamic passwordData)
        {
            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("No authentication token available. Please login first.");
            }
            var content = new StringContent(JsonConvert.SerializeObject(passwordData), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("users/change-password", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}