using AutoMapper;
using Concepts.Domain;
using Concepts.Domain.DTOs;
using Concepts.Repository;
using Concepts.Repository.UsersRepo;
using Concepts.Services.Helpers;
using Concepts.Services.Users.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Concepts.Services.Users
{
    public class UserService : IUserService
    {
        private UserRepository _userRepository;
        private IMapper _mapper;
        private AmazonSQS _sqs;
        public UserService(UserRepository userRepository, IMapper mapper, AmazonSQS sqs)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _sqs = sqs;
        }

        public async Task<List<UserDto>> GetAllUsers()
        {
            var result = await _userRepository.GetAll();
            return _mapper.Map<List<UserDto>>(result);
        }

        public async Task<UserDto> GetById(int id)
        {
            var result = await _userRepository.GetById(id);
            return _mapper.Map<UserDto>(result);
        }

        public async Task<bool> CreateUser(UserDto user)
        {
            var newUser = _mapper.Map<UserDto, User>(user);
            var result = await _userRepository.Create(newUser);

            ResponseData response = new()
            {
                RequestType = RequestType.POST,
                Message = result != null ? "User created successfully!" : "Error to create user!",
                UserData = _mapper.Map<UserDto>(result),
                IsValid = result != null
            };
            await _sqs.SendMessage(response);

            return result != null;
        }

        public async Task<bool> UpdateUser(UserDto user)
        {
            var findedUser = await _userRepository.GetById(user.Id);
            if (findedUser is null) throw new NullReferenceException();

            findedUser.Name = user.Name;
            findedUser.Age = user.Age;
            findedUser.Adress = user.Adress;
            findedUser.City = user.City;
            findedUser.Username = user.Username;
            findedUser.Password = user.Password;

            var result = await _userRepository.Update(findedUser);

            ResponseData response = new()
            {
                IsValid = result != null,
                Message = result != null ? "User updated successfully!" : "Error to update user!",
                RequestType = RequestType.PUT,
                UserData = _mapper.Map<User, UserDto>(result),
            };

            await _sqs.SendMessage(response);
            return result != null;
        }

        public async Task<bool> DeleteUser(int id)
        {
            var user = await GetById(id);
            var result = await _userRepository.Delete(id);
            ResponseData response = new()
            {
                IsValid = result,
                RequestType = RequestType.DELETE,
                EntityId = id,
                Message = result ? "User deleted successfully!" : "Error to delete user!",
                UserData = user
            };
            await _sqs.SendMessage(response);
            return result;
        }
    }
}

