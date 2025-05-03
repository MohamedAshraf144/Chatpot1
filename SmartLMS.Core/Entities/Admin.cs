using System;

namespace SmartLMS.Core.Entities
{
    public class Admin
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public string Phone { get; set; }
        public string MobileId { get; set; }
        public string EmailVerified { get; set; }
        public DateTime CreatedAt { get; set; }

        // العلاقات
        public User User { get; set; }
    }
}