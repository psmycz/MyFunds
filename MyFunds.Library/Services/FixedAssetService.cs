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
        readonly IRoomRepository roomRepository;
        readonly IUserRepository userRepository;

        public FixedAssetService(IFixedAssetRepository fixedAssetRepository, IRoomRepository roomRepository, IUserRepository userRepository, IMapper mapper)
            : base (mapper)
        {
            this.fixedAssetRepository = fixedAssetRepository;
            this.roomRepository = roomRepository;
            this.userRepository = userRepository;
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



            var fixedAsset = new FixedAsset()
            {
                Name = fixedAssetDTO.Name,
                InUse = fixedAssetDTO.InUse,
                Price = fixedAssetDTO.Price,
                PurchaseDate = fixedAssetDTO.PurchaseDate,
                WarrantyEndDate = fixedAssetDTO.WarrantyEndDate,
                Type = fixedAssetDTO.Type,
                RoomId = fixedAssetDTO.RoomId,
                UserId = fixedAssetDTO.UserId
            };

            var newFixedAsset = fixedAssetRepository.Insert(fixedAsset);
            fixedAssetRepository.Save();



            var room = roomRepository.GetById(newFixedAsset.RoomId);
            var user = userRepository.GetById(newFixedAsset.UserId);

            fixedAssetDTO.Id = newFixedAsset.Id;
            fixedAssetDTO.Room = new RoomDTO()
            {
                Id = room.Id,
                BuildingId = room.BuildingId,
                Area = room.Area,
                Floor = room.Floor,
                Type = room.Type,
                Building = room.Building
            };
            fixedAssetDTO.User = new UserDTO()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
            
            return fixedAssetDTO;
        }

        // TODO:
        // add auto mapper
        public List<FixedAsset> GetAll()
        {
            var allAssets = fixedAssetRepository.GetAll().ToList();

            return allAssets;
        }
    }
}
