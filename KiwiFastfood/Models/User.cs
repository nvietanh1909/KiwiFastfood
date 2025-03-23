using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KiwiFastfood.Models
{
    public class User
    {
        public string Id { get; set; }  
        public string HoTen { get; set; }
        public string TaiKhoan { get; set; }
        public string Email { get; set; }
        public string DiaChiKH { get; set; }
        public string DienThoaiKH { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string Role { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}