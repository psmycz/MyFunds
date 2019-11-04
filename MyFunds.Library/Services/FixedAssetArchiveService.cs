using AutoMapper;
using MyFunds.Data.Interfaces;
using MyFunds.Data.Models;
using MyFunds.Library.DTO;
using MyFunds.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.Services
{
    public class FixedAssetArchiveService : BaseService<FixedAssetArchiveService>, IFixedAssetArchiveService
    {
        readonly IFixedAssetRepository fixedAssetRepository;
        readonly IFixedAssetArchiveRepository fixedAssetArchiveRepository;

        public FixedAssetArchiveService(IFixedAssetRepository fixedAssetRepository, IFixedAssetArchiveRepository fixedAssetArchiveRepository, IMapper mapper)
            : base(mapper)
        {
            this.fixedAssetRepository = fixedAssetRepository;
            this.fixedAssetArchiveRepository = fixedAssetArchiveRepository;
        }


        public FixedAssetArchiveDTO AddArchive(int fixedAssetId)
        {
            var fixedAsset = fixedAssetRepository.GetById(fixedAssetId);
            var fixedAssetArchive = mapper.Map<FixedAssetArchive>(fixedAsset);

            var newfixedAssetArchive = fixedAssetArchiveRepository.Insert(fixedAssetArchive);
            fixedAssetArchiveRepository.Save();

            var fixedAssetArchiveDTO = mapper.Map<FixedAssetArchiveDTO>(newfixedAssetArchive);
            fixedAssetRepository.Detach(fixedAsset);

            return fixedAssetArchiveDTO;
        }
    }
}
