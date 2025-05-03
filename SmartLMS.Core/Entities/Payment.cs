using System;
using System.ComponentModel.DataAnnotations;

namespace SmartLMS.Core.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int CourseId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string Method { get; set; }
        public string TransactionId { get; set; }
        public string StatementId { get; set; }
        public DateTime PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // العلاقات
        public User User { get; set; }
        public Course Course { get; set; }
        public NewAttribute NewAttribute { get; set; }
    }
}
