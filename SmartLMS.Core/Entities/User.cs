using SmartLMS.Core.Entities.Chat;
using System;
using System.Collections.Generic;

namespace SmartLMS.Core.Entities
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string ProfileImage { get; set; }
        public bool EmailVerified { get; set; }
        public string Token { get; set; }
        public string Status { get; set; }
        public string Gender { get; set; }
        public string Region { get; set; }
        public string Phone { get; set; }
        public string Bio { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Role { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastSeen { get; set; }
        // العلاقات
        public ICollection<GroupUser> Groups { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<TaskProgress> TaskProgresses { get; set; }
        public ICollection<ContentProgress> ContentProgresses { get; set; }
        public ICollection<AssignmentSubmission> AssignmentSubmissions { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<UserAccess> Accesses { get; set; }
        public ICollection<MessageReadReceipt> MessageReadReceipts { get; set; }

    }
}