using Concepts.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concepts.Services.Users.Interface
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsers();
        Task<UserDto> GetById(int id);
        Task<bool> CreateUser(UserDto user);
        Task<bool> UpdateUser(UserDto user);
        Task<bool> DeleteUser(int id);
    }
}
