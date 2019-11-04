using MyFunds.Data.DatabaseContexts;
using MyFunds.Data.Interfaces;
using MyFunds.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Data.Repositories
{
    public class FixedAssetRepository : BaseRepository<FixedAsset>, IFixedAssetRepository
    {
        public FixedAssetRepository(MyFundsDbContext context)
           : base(context)
        { 
        }
    

        public bool FixedAssetExist(int fixedAssetId)
        {
            return Table.Any(fa => fa.Id == fixedAssetId);
        }
    }
}
