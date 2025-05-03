using Microsoft.EntityFrameworkCore;
using SmartLMS.Core.Entities;
using SmartLMS.Core.Entities.Auth;
using SmartLMS.Core.Entities.Chat;

namespace SmartLMS.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // المستخدمين والمصادقة
        public DbSet<User> Users { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        // التعليم
        public DbSet<Course> Courses { get; set; }
        public DbSet<Major> Majors { get; set; }
        public DbSet<CourseContent> CourseContents { get; set; }
        public DbSet<CourseSection> CourseSections { get; set; }
        public DbSet<CourseRequirement> CourseRequirements { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<ContentProgress> ContentProgresses { get; set; }
        public DbSet<TaskProgress> TaskProgresses { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupUser> GroupUsers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<UserAccess> UserAccesses { get; set; }
        public DbSet<NewAttribute> NewAttributes { get; set; }

        // محادثات Chatbot
        public DbSet<ChatSession> ChatSessions { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        // محادثات المستخدمين
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ConversationParticipant> ConversationParticipants { get; set; }
        public DbSet<MessageReadReceipt> MessageReadReceipts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. إعدادات دقة الأرقام العشرية (كما سبق)
            modelBuilder.Entity<Course>().Property(c => c.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Instructor>().Property(i => i.Salary).HasPrecision(18, 2);
            modelBuilder.Entity<Instructor>().Property(i => i.SalaryDeduction).HasPrecision(18, 2);
            modelBuilder.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 2);

            // 2. إعدادات العلاقات لمنع مشاكل Cascade Delete
            // العلاقات مع User
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.User)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ContentProgress>()
                .HasOne(cp => cp.User)
                .WithMany(u => u.ContentProgresses)
                .HasForeignKey(cp => cp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskProgress>()
                .HasOne(tp => tp.User)
                .WithMany(u => u.TaskProgresses)
                .HasForeignKey(tp => tp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssignmentSubmission>()
                .HasOne(a => a.User)
                .WithMany(u => u.AssignmentSubmissions)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GroupUser>()
                .HasOne(gu => gu.User)
                .WithMany(u => u.Groups)
                .HasForeignKey(gu => gu.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // العلاقات مع Course
            modelBuilder.Entity<CourseContent>()
                .HasOne(cc => cc.Course)
                .WithMany(c => c.Contents)
                .HasForeignKey(cc => cc.CourseId)
                .OnDelete(DeleteBehavior.Restrict); // تغيير من Cascade إلى Restrict

            modelBuilder.Entity<CourseContent>()
                .HasOne(cc => cc.Section)
                .WithMany(s => s.Contents)
                .HasForeignKey(cc => cc.SectionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseSection>()
                .HasOne(cs => cs.Course)
                .WithMany(c => c.Sections)
                .HasForeignKey(cs => cs.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.Course)
                .WithMany(c => c.Quizzes)
                .HasForeignKey(q => q.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Course)
                .WithMany(c => c.Assignments)
                .HasForeignKey(a => a.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseRequirement>()
                .HasOne(cr => cr.Course)
                .WithMany(c => c.Requirements)
                .HasForeignKey(cr => cr.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. العلاقات المسموح لها بالحذف المتتالي (حيث لا توجد مشاكل)
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Quiz)
                .WithMany(qz => qz.Questions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Choice>()
                .HasOne(c => c.Question)
                .WithMany(q => q.Choices)
                .HasForeignKey(c => c.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AssignmentSubmission>()
                .HasOne(a => a.Assignment)
                .WithMany(a => a.Submissions)
                .HasForeignKey(a => a.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<MessageReadReceipt>()
                .HasOne(mr => mr.User)
                .WithMany(u => u.MessageReadReceipts)
                .HasForeignKey(mr => mr.UserId)
                .OnDelete(DeleteBehavior.Restrict); // ✅ منع المسارات المتعددة

            // 4. إعدادات الفهرس (كما سبق)
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Course>().HasIndex(c => c.CourseCode).IsUnique();
            modelBuilder.Entity<Enrollment>().HasIndex(e => new { e.UserId, e.CourseId }).IsUnique();
        }
    }
}