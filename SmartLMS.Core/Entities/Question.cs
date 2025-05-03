using System;
using System.Collections.Generic;

namespace SmartLMS.Core.Entities
{
    public class Question
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public int Points { get; set; }
        public int QuestionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // العلاقات
        public Quiz Quiz { get; set; }
        public ICollection<Choice> Choices { get; set; }
    }
}