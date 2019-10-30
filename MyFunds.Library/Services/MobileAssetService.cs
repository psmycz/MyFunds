using AutoMapper;
using MyFunds.Data.Interfaces;
using MyFunds.Data.Models;
using MyFunds.Library.DTO;
using MyFunds.Library.Exceptions;
using MyFunds.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.Services
{
    public class MobileAssetService : BaseService<MobileAssetService>, IMobileAssetService
    {
        readonly IMobileAssetRepository mobileAssetRepository;
        readonly IUserRepository userRepository;

        public MobileAssetService(IMobileAssetRepository mobileAssetRepository, IUserRepository userRepository, IMapper mapper) 
            : base(mapper)
        {
            this.mobileAssetRepository = mobileAssetRepository;
            this.userRepository = userRepository;
        }

        public MobileAssetDTO Create(MobileAssetDTO mobileAssetDTO)
        {
            if (mobileAssetDTO.InUse && mobileAssetDTO.UserId == 0)
                throw new ApiException("Must provide UserId when InUse");
            if (!mobileAssetDTO.InUse && mobileAssetDTO.UserId != 0)
                throw new ApiException("While not in use, cannot provide userId");
            if (mobileAssetDTO.UserId != 0 && !userRepository.UserExist(mobileAssetDTO.UserId))
                throw new ApiException("User with provided Id does not exist");
            if (mobileAssetDTO.Price <= 0)
                throw new ApiException("Invalid Price");
            if (DateTime.Compare(mobileAssetDTO.PurchaseDate, mobileAssetDTO.WarrantyEndDate) >= 0)
                throw new ApiException("Purchase Date must be earlier than Warranty End Date");

            var mobileAsset = mapper.Map<MobileAsset>(mobileAssetDTO);

            var newMobileAsset = mobileAssetRepository.Insert(mobileAsset);
            mobileAssetRepository.Save();

            // to load missing data
            userRepository.GetById(newMobileAsset.UserId);
            mobileAssetDTO = mapper.Map<MobileAssetDTO>(newMobileAsset);

            return mobileAssetDTO;
        }

        public MobileAssetDTO GetMobileAsset(int mobileAssetId)
        {
            if (mobileAssetId <= 0)
                throw new ApiException("Incorrect Id");

            var mobileAsset = mobileAssetRepository.GetById(mobileAssetId);
            var mobileAssetDTO = mapper.Map<MobileAssetDTO>(mobileAsset ?? throw new NoDataException("No mobile asset with provided Id"));
            
            return mobileAssetDTO;
        }

        public List<MobileAssetDTO> GetAllMobileAssets()
        {
            var allAssets = mobileAssetRepository.GetAll();
            var allAssetsDTO = mapper.Map<List<MobileAssetDTO>>(allAssets ?? throw new NoDataException("No registered mobile assets"));

            return allAssetsDTO;
        }
    }
}
