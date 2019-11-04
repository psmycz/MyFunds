using MyFunds.Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.Interfaces
{
    public interface IUserService : IBaseService<IUserService>
    {
        UserDTO GetUser(int userId);
        UserDTO GetUserWithAssets(int userId);

        bool UserExist(int userId);
        bool UserExist(string userName);
        bool UserWithEmailExist(string email);

        List<UserDTO> GetAllUsers();
        List<UserDTO> GetAllUsersWithAssets();

    }
}
