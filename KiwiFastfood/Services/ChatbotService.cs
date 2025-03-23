using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KiwiFastfood.Services
{
    public class ChatbotService : ApiService
    {
        public async Task<string> SendMessageAsync(string message)
        {
            var data = new { message };
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("chatbot/chat", content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            
            // Parse the JSON response
            var parsedResponse = JsonConvert.DeserializeObject<dynamic>(responseString);
            
            // Extract only the message content without "AI:" prefix
            if (parsedResponse != null && parsedResponse.success == true && parsedResponse.response != null)
            {
                string botMessage = parsedResponse.response.ToString();
                
                // Remove "AI:" prefix if present
                if (botMessage.StartsWith("AI:"))
                {
                    botMessage = botMessage.Substring(3).Trim();
                }
                
                return botMessage;
            }
            
            return "Xin lỗi, tôi không thể trả lời câu hỏi này lúc này. Vui lòng thử lại sau.";
        }
    }
}
