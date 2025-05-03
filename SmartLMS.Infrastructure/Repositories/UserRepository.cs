using Microsoft.EntityFrameworkCore;
using SmartLMS.Core.Entities;
using SmartLMS.Core.Interfaces;
using SmartLMS.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLMS.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<IEnumerable<User>> GetUsersByIdsAsync(IEnumerable<string> userIds)
        {
            return await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            // تنفيذ هذه الطريقة يعتمد على تنفيذك لنظام الأدوار
            // هذا تنفيذ مبسط يفترض أن Role هو خاصية في كيان User
            return await _context.Users
                .Where(u => u.Role == role) // قد تحتاج إلى تعديل هذا حسب نموذج البيانات الخاص بك
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<User>();

            return await _context.Users
                .Where(u => u.Name.Contains(searchTerm) ||
                            u.Email.Contains(searchTerm) ||
                            u.Phone.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<bool> UpdateUserStatusAsync(string userId, bool isOnline)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            user.IsOnline = isOnline;
            if (!isOnline)
                user.LastSeen = System.DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}