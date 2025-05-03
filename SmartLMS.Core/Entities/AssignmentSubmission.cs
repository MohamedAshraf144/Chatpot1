using System;

namespace SmartLMS.Core.Entities
{
    public class AssignmentSubmission
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public string UserId { get; set; }
        public string SubmissionFile { get; set; }
        public string Note { get; set; }
        public int? Score { get; set; }
        public bool IsPassed { get; set; }
        public string Status { get; set; }
        public DateTime SubmittedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // العلاقات
        public Assignment Assignment { get; set; }
        public User User { get; set; }
    }
}