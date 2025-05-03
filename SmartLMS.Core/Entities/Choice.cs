using System;

namespace SmartLMS.Core.Entities
{
    public class Choice
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public string ChoiceText { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // العلاقات
        public Question Question { get; set; }
    }
}