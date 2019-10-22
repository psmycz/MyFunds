using MyFunds.Data.DatabaseContexts;
using MyFunds.Data.Interfaces;
using MyFunds.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Data.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(MyFundsDbContext context)
           : base(context)
        {
        }


        public bool UserExist(int userId)
        {
            return Table.Any(u => u.Id == userId);
        }

        public bool UserExist(string username)
        {
            return Table.Any(u => u.UserName == username);
        }

        public bool UserWithEmailExist(string email)
        {
            return Table.Any(u => u.Email == email);
        }

    }
}
