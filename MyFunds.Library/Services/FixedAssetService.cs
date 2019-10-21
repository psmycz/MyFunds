using MyFunds.Data.Interfaces;
using MyFunds.Data.Models;
using MyFunds.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.Services
{
    public class FixedAssetService : IFixedAssetService
    {
        readonly IFixedAssetRepository fixedAssetRepository;

        public FixedAssetService(IFixedAssetRepository fixedAssetRepository)
        {
            this.fixedAssetRepository = fixedAssetRepository;
        }

        // TODO:
        // add auto mapper
        public List<FixedAsset> GetAll()
        {
            var allAssets = fixedAssetRepository.GetAll().ToList();

            // return DTO object to view not a model
            return allAssets;
        }
    }
}
