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
    public class FixedAssetService : BaseService<FixedAssetService>, IFixedAssetService
    {
        readonly IFixedAssetRepository fixedAssetRepository;
        readonly IFixedAssetArchiveService fixedAssetArchiveService;
        readonly IRoomRepository roomRepository;
        readonly IUserRepository userRepository;

        public FixedAssetService(IFixedAssetRepository fixedAssetRepository, IFixedAssetArchiveService fixedAssetArchiveService, IRoomRepository roomRepository, IUserRepository userRepository, IMapper mapper)
            : base (mapper)
        {
            this.fixedAssetRepository = fixedAssetRepository;
            this.fixedAssetArchiveService = fixedAssetArchiveService;
            this.roomRepository = roomRepository;
            this.userRepository = userRepository;
        }
        public bool FixedAssetExist(int fixedAssetId)
        {
            return fixedAssetRepository.FixedAssetExist(fixedAssetId);
        }

        public FixedAssetDTO Create(FixedAssetDTO fixedAssetDTO)
        {
            if (fixedAssetDTO.InUse && (fixedAssetDTO.Type != FixedAssetType.Rentable || fixedAssetDTO.UserId == 0))
                throw new ApiException($"Type must be declared as {FixedAssetType.Rentable.ToString()} and userId must be provided");
            if (!fixedAssetDTO.InUse && fixedAssetDTO.UserId != 0)
                throw new ApiException("While not in use, cannot provide userId");
            if (fixedAssetDTO.UserId != 0 && !userRepository.UserExist(fixedAssetDTO.UserId))
                throw new ApiException("User with provided Id does not exist");
            if (fixedAssetDTO.RoomId != 0 && !roomRepository.RoomExist(fixedAssetDTO.RoomId))
                throw new ApiException("Room with provided Id does not exist");
            if(fixedAssetDTO.Price <= 0)
                throw new ApiException("Invalid Price");
            if(DateTime.Compare(fixedAssetDTO.PurchaseDate, fixedAssetDTO.WarrantyEndDate) >= 0)
                throw new ApiException("Purchase Date must be earlier than Warranty End Date");


            var fixedAsset = mapper.Map<FixedAsset>(fixedAssetDTO);

            var newFixedAsset = fixedAssetRepository.Insert(fixedAsset);
            fixedAssetRepository.Save();

            // to get missing data 
            roomRepository.GetById(newFixedAsset.RoomId);
            userRepository.GetById(newFixedAsset.UserId);

            fixedAssetDTO = mapper.Map<FixedAssetDTO>(newFixedAsset);
            
            return fixedAssetDTO;
        }
        public FixedAssetDTO Update(FixedAssetDTO fixedAssetDTO)
        {
            if (fixedAssetDTO.Id <= 0 )
                throw new ApiException("Incorrect Id");
            if (!fixedAssetRepository.FixedAssetExist(fixedAssetDTO.Id))
                throw new ApiException("Fixed asset with provided Id does not exist");
            if (fixedAssetDTO.InUse && (fixedAssetDTO.Type != FixedAssetType.Rentable || fixedAssetDTO.UserId == 0))
                throw new ApiException($"Type must be declared as {FixedAssetType.Rentable.ToString()} and userId must be provided");
            if (!fixedAssetDTO.InUse && fixedAssetDTO.UserId != 0)
                throw new ApiException("While not in use, cannot provide userId");
            if (fixedAssetDTO.UserId != 0 && !userRepository.UserExist(fixedAssetDTO.UserId))
                throw new ApiException("User with provided Id does not exist");
            if (fixedAssetDTO.RoomId != 0 && !roomRepository.RoomExist(fixedAssetDTO.RoomId))
                throw new ApiException("Room with provided Id does not exist");
            if (fixedAssetDTO.Price <= 0)
                throw new ApiException("Invalid Price");
            if (DateTime.Compare(fixedAssetDTO.PurchaseDate, fixedAssetDTO.WarrantyEndDate) >= 0)
                throw new ApiException("Purchase Date must be earlier than Warranty End Date");
            if (!HasPropertyUpdated(fixedAssetDTO.Id, fixedAssetDTO))
                throw new ApiException("None of the properties have changed");


            var fixedAssetArchiveDTO = fixedAssetArchiveService.AddArchive(fixedAssetDTO.Id);


            var newFixedAsset = mapper.Map<FixedAsset>(fixedAssetDTO);
            var updatedFixedAsset = fixedAssetRepository.Update(newFixedAsset);
            fixedAssetRepository.Save();


            // to get missing data
            roomRepository.GetById(updatedFixedAsset.RoomId);
            userRepository.GetById(updatedFixedAsset.UserId);


            fixedAssetDTO = mapper.Map<FixedAssetDTO>(updatedFixedAsset);
            fixedAssetDTO.PreviousFixedAsset = fixedAssetArchiveDTO;

            return fixedAssetDTO;
        }

        public FixedAssetDTO GetFixedAsset(int fixedAssetId)
        {
            if (fixedAssetId <= 0)
                throw new ApiException("Incorrect Id");

            var fixedAsset = fixedAssetRepository.GetById(fixedAssetId);
            
            var fixedAssetDTO = mapper.Map<FixedAssetDTO>(fixedAsset ?? throw new NoDataException("No fixed asset with provided Id"));
            
            return fixedAssetDTO;
        }

        public List<FixedAssetDTO> GetAllFixedAssets()
        {
            var allAssets = fixedAssetRepository.GetAll().ToList();

            var allAssetsDTO = mapper.Map<List<FixedAssetDTO>>(allAssets ?? throw new NoDataException("No registered mobile assets"));

            return allAssetsDTO;
        }

        bool HasPropertyUpdated(int updatedFixedAssetId, FixedAssetDTO fixedAssetDTO)
        {
            if (updatedFixedAssetId <= 0)
                throw new ApiException("Incorrect Id");

            var fixedAsset = fixedAssetRepository.GetById(updatedFixedAssetId);
            bool changed = fixedAsset == null ? throw new NoDataException("No fixed asset with provided Id") : false ||
                fixedAsset.InUse != fixedAssetDTO.InUse ||
                fixedAsset.Name != fixedAssetDTO.Name ||
                fixedAsset.Price != fixedAssetDTO.Price ||
                fixedAsset.PurchaseDate != fixedAssetDTO.PurchaseDate ||
                fixedAsset.WarrantyEndDate != fixedAssetDTO.WarrantyEndDate ||
                fixedAsset.Type != fixedAssetDTO.Type ||
                fixedAsset.RoomId != fixedAssetDTO.RoomId ||
                fixedAsset.UserId != fixedAssetDTO.UserId;

            fixedAssetRepository.Detach(fixedAsset);

            return changed;
        }
    }
}
