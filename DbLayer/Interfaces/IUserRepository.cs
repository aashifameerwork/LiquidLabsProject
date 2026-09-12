using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbLayer.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserById(long id);
        Task<bool> AddUserToDbAsync(User user);
    }
}
