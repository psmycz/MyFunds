using MyFunds.Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyFunds.Library.Interfaces
{
    public interface IUserService : IBaseService<IUserService>
    {
        UserDTO GetUser(int userId);
        UserDTO GetMe(int userId, ClaimsPrincipal loggedUser);
        UserDTO GetUserWithAssets(int userId);
        UserDTO GetMeWithAssets(int userId, ClaimsPrincipal loggedUser);

        bool UserExist(int userId);
        bool UserExist(string userName);
        bool UserWithEmailExist(string email);

        List<UserDTO> GetAllUsers();
        List<UserDTO> GetAllUsersWithAssets();

    }
}
