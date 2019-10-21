using MyFunds.Data.DatabaseContexts;
using MyFunds.Data.Interfaces;
using MyFunds.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Data.Repositories
{
    public class MobileAssetArchiveRepository : BaseRepository<MobileAssetArchive>, IMobileAssetArchiveRepository
    {
        public MobileAssetArchiveRepository(MyFundsDbContext context)
           : base(context)
        {
        }



    }
}
