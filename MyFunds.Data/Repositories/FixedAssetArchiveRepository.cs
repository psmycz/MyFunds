using MyFunds.Data.DatabaseContexts;
using MyFunds.Data.Interfaces;
using MyFunds.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Data.Repositories
{
    public class FixedAssetArchiveRepository : BaseRepository<FixedAssetArchive>, IFixedAssetArchiveRepository
    {
        public FixedAssetArchiveRepository(MyFundsDbContext context)
           : base(context)
        {
        }


    
    }
}
