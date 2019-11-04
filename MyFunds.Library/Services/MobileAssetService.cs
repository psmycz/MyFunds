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
        readonly IMobileAssetArchiveService mobileAssetArchiveService;
        readonly IUserRepository userRepository;

        public MobileAssetService(IMobileAssetRepository mobileAssetRepository, IMobileAssetArchiveService mobileAssetArchiveService, IUserRepository userRepository, IMapper mapper) 
            : base(mapper)
        {
            this.mobileAssetRepository = mobileAssetRepository;
            this.mobileAssetArchiveService = mobileAssetArchiveService;
            this.userRepository = userRepository;
        }

        public bool MobileAssetExist(int mobileAssetId)
        {
            return mobileAssetRepository.MobileAssetExist(mobileAssetId);
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

        public MobileAssetDTO Update(MobileAssetDTO mobileAssetDTO)
        {
            if (mobileAssetDTO.Id <= 0)
                throw new ApiException("Incorrect Id");
            if (!mobileAssetRepository.MobileAssetExist(mobileAssetDTO.Id))
                throw new ApiException("Mobile asset with provided Id does not exist");
            if (!mobileAssetDTO.InUse && mobileAssetDTO.UserId != 0)
                throw new ApiException("While not in use, cannot provide userId");
            if (mobileAssetDTO.UserId != 0 && !userRepository.UserExist(mobileAssetDTO.UserId))
                throw new ApiException("User with provided Id does not exist");
            if (mobileAssetDTO.Price <= 0)
                throw new ApiException("Invalid Price");
            if (DateTime.Compare(mobileAssetDTO.PurchaseDate, mobileAssetDTO.WarrantyEndDate) >= 0)
                throw new ApiException("Purchase Date must be earlier than Warranty End Date");
            if (!HasPropertyUpdated(mobileAssetDTO.Id, mobileAssetDTO))
                throw new ApiException("None of the properties have changed");


            var mobileAssetArchiveDTO = mobileAssetArchiveService.AddArchive(mobileAssetDTO.Id);


            var newMobileAsset = mapper.Map<MobileAsset>(mobileAssetDTO);
            var updatedMobileAsset = mobileAssetRepository.Update(newMobileAsset);
            mobileAssetRepository.Save();


            // to get missing data
            userRepository.GetById(updatedMobileAsset.UserId);


            mobileAssetDTO = mapper.Map<MobileAssetDTO>(updatedMobileAsset);
            mobileAssetDTO.PreviousMobileAsset = mobileAssetArchiveDTO;

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

        bool HasPropertyUpdated(int updatedMobileAssetId, MobileAssetDTO mobileAssetDTO)
        {
            if (updatedMobileAssetId <= 0)
                throw new ApiException("Incorrect Id");

            var mobileAsset = mobileAssetRepository.GetById(updatedMobileAssetId);
            bool changed = mobileAsset == null ? throw new NoDataException("No mobile asset with provided Id") : false ||
                mobileAsset.InUse != mobileAssetDTO.InUse ||
                mobileAsset.Name != mobileAssetDTO.Name ||
                mobileAsset.Price != mobileAssetDTO.Price ||
                mobileAsset.PurchaseDate != mobileAssetDTO.PurchaseDate ||
                mobileAsset.WarrantyEndDate != mobileAssetDTO.WarrantyEndDate ||
                mobileAsset.UserId != mobileAssetDTO.UserId;

            mobileAssetRepository.Detach(mobileAsset);

            return changed;
        }
    }
}
