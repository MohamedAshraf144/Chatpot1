using System;

namespace SmartLMS.Core.Entities
{
    public class NewAttribute
    {
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
        public DateTime CreatedAt { get; set; }

        // العلاقات
        public Payment Payment { get; set; }
    }
}