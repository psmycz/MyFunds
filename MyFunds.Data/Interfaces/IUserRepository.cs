using MyFunds.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Data.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        bool UserExist(int userId);
        bool UserExist(string username);
        bool UserWithEmailExist(string email);
    }
}
