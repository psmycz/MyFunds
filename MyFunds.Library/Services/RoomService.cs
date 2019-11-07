using AutoMapper;
using MyFunds.Data.Interfaces;
using MyFunds.Data.Models;
using MyFunds.Library.DTO;
using MyFunds.Library.Exceptions;
using MyFunds.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyFunds.Library.Services
{
    public class RoomService : BaseService<RoomService>, IRoomService
    {
        readonly IRoomRepository roomRepository;
        readonly IBuildingRepository buildingRepository;

        public RoomService(IRoomRepository roomRepository, IBuildingRepository buildingRepository, IMapper mapper)
            : base(mapper)
        {
            this.roomRepository = roomRepository;
            this.buildingRepository = buildingRepository;
        }

        public bool RoomExist(int roomId)
        {
            return roomRepository.Exist(r => r.Id == roomId);
        }


        public RoomDTO GetRoom(int roomId)
        {
            if (roomId <= 0)
                throw new ApiException("Incorrect Id");

            var room = roomRepository.GetById(roomId);
            var roomDTO = mapper.Map<RoomDTO>(room ?? throw new NoDataException("No room with provided Id"));

            return roomDTO;
        }

        public List<RoomDTO> GetAllRooms()
        {
            var rooms = mapper.Map<List<RoomDTO>>(roomRepository.GetAll());

            return rooms ?? throw new NoDataException("No existing rooms");
        }

        public RoomDTO GetRoomWithAssets(int roomId)
        {
            if (roomId <= 0)
                throw new ApiException("Incorrect Id");

            var room = roomRepository.GetById(roomId);
            var roomDTO = mapper.Map<RoomDTO>(room ?? throw new NoDataException("No room with provided Id"));

            // do not need repeated room property in every fixed asset - remove select if needed for some reason
            roomDTO.FixedAssets = mapper.Map<List<FixedAssetDTO>>(room.FixedAssets.Select(fa => { fa.Room = null; return fa; }));


            return roomDTO;
        }

        public RoomDTO Create(RoomDTO roomDTO)
        {
            if (roomDTO.Id != 0)
                throw new ApiException("Cannot declare Id while creating new Room");
            if (roomDTO.Area <= 0)
                throw new ApiException("Area must have positive value");
            if (roomDTO.BuildingId <= 0) 
                throw new ApiException("Field building Id is required");
            if (roomDTO.BuildingId != 0 && !buildingRepository.Exist(b => b.Id == roomDTO.BuildingId))
                throw new ApiException("Building with provided Id does not exist");

            var room = mapper.Map<Room>(roomDTO);

            var newRoom = roomRepository.Insert(room);
            roomRepository.Save();

            // to get missing data 
            buildingRepository.GetById(newRoom.BuildingId);

            roomDTO = mapper.Map<RoomDTO>(newRoom);

            return roomDTO;
        }

        // not tested
        public RoomDTO Update(RoomDTO roomDTO)
        {
            if(roomDTO.Id <= 0)
                throw new ApiException("Incorrect room Id");
            if(!this.RoomExist(roomDTO.Id))
                throw new ApiException("Room with provided Id does not exist");
            if (roomDTO.Area <= 0)
                throw new ApiException("Area must have positive value");
            if (roomDTO.BuildingId <= 0)
                throw new ApiException("Field building Id is required");
            if (roomDTO.BuildingId != 0 && !buildingRepository.Exist(b => b.Id == roomDTO.BuildingId))
                throw new ApiException("Building with provided Id does not exist");
            if (!HasPropertyUpdated(roomDTO.Id, roomDTO, roomRepository))
                throw new ApiException("None of the properties have changed");


            var room = mapper.Map<Room>(roomDTO);

            var updatedRoom = roomRepository.Update(room);
            roomRepository.Save();

            // to get missing data 
            buildingRepository.GetById(updatedRoom.BuildingId);

            roomDTO = mapper.Map<RoomDTO>(updatedRoom);

            return roomDTO;
        }

        
    }
}
