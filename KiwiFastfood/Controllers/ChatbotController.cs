using KiwiFastfood.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KiwiFastfood.Controllers
{
    public class ChatbotController : Controller
    {
        private readonly ChatbotService _chatbotService;
        private const string ChatHistoryKey = "ChatHistory";

        private bool _isLogin { get => Session["UserToken"] != null; }

        public ChatbotController()
        {
            _chatbotService = new ChatbotService();
        }

        public ActionResult Chat()
        {
            if (_isLogin)
            {
                return View();
            }
            return RedirectToAction("Login", "User");
        }

        // Gửi tin nhắn đến chatbot
        [HttpPost]
        public async Task<ActionResult> SendMessage(string message)
        {
            if (_isLogin)
            {
                if (string.IsNullOrEmpty(message))
                {
                    return Json(new { success = false, message = "Tin nhắn không được để trống!" });
                }

                try
                {
                    // Lưu tin nhắn của người dùng vào lịch sử
                    SaveMessageToHistory("user", message);

                    // Gửi tin nhắn và nhận phản hồi từ chatbot
                    string response = await _chatbotService.SendMessageAsync(message);

                    // Lưu phản hồi vào lịch sử
                    SaveMessageToHistory("bot", response);

                    return Json(new { success = true, data = response });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            return RedirectToAction("Login", "User");
        }
        
        [HttpGet]
        public ActionResult GetChatHistory()
        {
            var chatHistory = Session[ChatHistoryKey] as List<string> ?? new List<string>();
            return Json(new { success = true, data = string.Join("", chatHistory) }, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult ClearChatHistory()
        {
            Session[ChatHistoryKey] = new List<string>();
            return Json(new { success = true, message = "Lịch sử chat đã được xóa" });
        }
        
        private void SaveMessageToHistory(string sender, string message)
        {
            var chatHistory = Session[ChatHistoryKey] as List<string> ?? new List<string>();
            
            string messageHtml = sender == "user" 
                ? $"<div class=\"message user-message\"><b>Bạn:</b> {HttpUtility.HtmlEncode(message)}</div>" 
                : $"<div class=\"message bot-message\"><b>Kiwi:</b> {HttpUtility.HtmlEncode(message)}</div>";
                
            chatHistory.Add(messageHtml);
            Session[ChatHistoryKey] = chatHistory;
        }
    }
}