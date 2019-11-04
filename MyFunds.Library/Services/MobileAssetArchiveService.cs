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
    public class MobileAssetArchiveService : BaseService<MobileAssetArchiveService>, IMobileAssetArchiveService
    {
        readonly IMobileAssetRepository mobileAssetRepository;
        readonly IMobileAssetArchiveRepository mobileAssetArchiveRepository;

        public MobileAssetArchiveService(IMobileAssetRepository mobileAssetRepository, IMobileAssetArchiveRepository mobileAssetArchiveRepository, IMapper mapper)
            : base(mapper)
        {
            this.mobileAssetRepository = mobileAssetRepository;
            this.mobileAssetArchiveRepository = mobileAssetArchiveRepository;
        }

        public MobileAssetArchiveDTO AddArchive(int mobileAssetId)
        {
            var mobileAsset = mobileAssetRepository.GetById(mobileAssetId);
            var mobileAssetArchive = mapper.Map<MobileAssetArchive>(mobileAsset);

            var newmobileAssetArchive = mobileAssetArchiveRepository.Insert(mobileAssetArchive);
            mobileAssetArchiveRepository.Save();

            var fixedAssetArchiveDTO = mapper.Map<MobileAssetArchiveDTO>(newmobileAssetArchive);
            mobileAssetRepository.Detach(mobileAsset);

            return fixedAssetArchiveDTO;
        }
    }
}
