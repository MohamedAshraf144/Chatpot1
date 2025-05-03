// SmartLMS.Core/Interfaces/IUserRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartLMS.Core.Entities;

namespace SmartLMS.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(string userId);
        Task<User> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetUsersByIdsAsync(IEnumerable<string> userIds);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
        Task<IEnumerable<User>> SearchUsersAsync(string searchTerm);
        Task<bool> UpdateUserStatusAsync(string userId, bool isOnline);
        Task<User> UpdateUserAsync(User user);
    }
}