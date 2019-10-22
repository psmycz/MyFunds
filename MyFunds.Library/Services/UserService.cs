﻿using AutoMapper;
using MyFunds.Data.Interfaces;
using MyFunds.Library.DTO;
using MyFunds.Library.Exceptions;
using MyFunds.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.Services
{
    public class UserService : BaseService<UserService>, IUserService
    {
        readonly IUserRepository userRepository;


        public UserService(IUserRepository userRepository, IMapper mapper)
            : base(mapper)
        {
            this.userRepository = userRepository;
        }


        public bool UserWithEmailExist(string email)
        {
            return userRepository.UserWithEmailExist(email);
        }

        public List<UserDTO> GetAllUsers()
        {
            var users = userRepository.GetAll().Select(u => new UserDTO { Id = u.Id, UserName = u.UserName, Email = u.Email }).ToList();

            return users;
        }

        public List<UserDTO> GetAllUsersWithAssets()
        {
            var users = userRepository.GetAll();

            // TODO: Fix cuz doesnt work xd
            return mapper.Map<List<UserDTO>>(users);
        }

        public UserDTO GetUser(int userId)
        {
            var user = userRepository.GetById(userId);
            
            return user == null ? throw new ApiException("No user with provided Id") : new UserDTO { Id = user.Id, Email = user.Email, UserName = user.UserName };
        }

        public UserDTO GetUserWithAssets(int userId)
        {
            var user = userRepository.GetById(userId);

            return user == null ? throw new ApiException("No user with provided Id") : mapper.Map<UserDTO>(user);
        }

        public bool UserExist(int userId)
        {
            return userRepository.UserExist(userId);
        }

        public bool UserExist(string userName)
        {
            return userRepository.UserExist(userName);
        }


    }
}
