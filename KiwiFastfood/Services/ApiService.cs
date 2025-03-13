using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http.Headers;

namespace KiwiFastfood.Services
{
    public class ApiService
    {   
        protected readonly HttpClient _httpClient;
        protected string _token;

        public ApiService()
        {
            _httpClient = new HttpClient();
            string apiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            if (string.IsNullOrEmpty(apiBaseUrl))
            {
                throw new Exception("ApiBaseUrl is not configured in Web.config.");
            }
            _httpClient.BaseAddress = new Uri(apiBaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected void SetToken(string token)
        {
            _token = token;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }

        protected void ClearToken()
        {
            _token = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}