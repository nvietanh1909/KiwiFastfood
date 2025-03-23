using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace KiwiFastfood.Services
{
    public class AdminService : ApiService
    {
        public async Task<string> GetAllUsersAsync(int page, int limit)
        {
            if (string.IsNullOrEmpty(_token))
                throw new Exception("No authentication token available. Please login first.");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await _httpClient.GetAsync($"admin/users?page={page}&limit={limit}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public void SetToken(string token)
        {
            base.SetToken(token);
        }
    }
}