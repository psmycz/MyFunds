using AutoMapper;
using MyFunds.Data.Interfaces;
using MyFunds.Library.DTO;
using MyFunds.Library.Exceptions;
using MyFunds.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        public bool UserExist(int userId)
        {
            return userRepository.Exist(u => u.Id == userId);
        }

        public bool UserExist(string userName)
        {
            return userRepository.Exist(u => u.UserName == userName);
        }

        public bool UserWithEmailExist(string email)
        {
            return userRepository.Exist(u => u.Email == email);
        }

        public List<UserDTO> GetAllUsers()
        {
            var users = userRepository.GetAll().Select(u => new UserDTO 
            {
                Id = u.Id, 
                UserName = u.UserName, 
                Email = u.Email 
            }).ToList();

            return users ?? throw new NoDataException("No registered users");
        }

        public List<UserDTO> GetAllUsersWithAssets()
        {
            var users = userRepository.GetAll();

            return mapper.Map<List<UserDTO>>(users ?? throw new NoDataException("No registered users"));
        }

        public UserDTO GetUser(int userId)
        {
            if (userId <= 0)
                throw new ApiException("Incorrect Id");

            var user = userRepository.GetById(userId);

            return user == null ? throw new NoDataException("No user with provided Id") : new UserDTO { Id = user.Id, Email = user.Email, UserName = user.UserName };
        }

        public UserDTO GetMe(int userId, ClaimsPrincipal loggedUser = null)
        {
            var userDTO = GetUser(userId);

            if (loggedUser != null)
            {
                userDTO.IsAdmin = loggedUser.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
            }

            return userDTO;
        }

        public UserDTO GetUserWithAssets(int userId)
        {
            if (userId <= 0)
                throw new ApiException("Incorrect Id");

            var user = userRepository.GetById(userId);

            return user == null ? throw new NoDataException("No user with provided Id") : mapper.Map<UserDTO>(user);
        }

        public UserDTO GetMeWithAssets(int userId, ClaimsPrincipal loggedUser = null)
        {
            var userDTO = GetUserWithAssets(userId);

            if (loggedUser != null)
            {
                userDTO.IsAdmin = loggedUser.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
            }

            return userDTO;
        }



    }
}
