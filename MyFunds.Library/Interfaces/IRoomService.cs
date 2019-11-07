using MyFunds.Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.Interfaces
{
    public interface IRoomService : IBaseService<IRoomService>
    {
        bool RoomExist(int roomId);

        RoomDTO GetRoomWithAssets(int roomId);
        RoomDTO GetRoom(int roomId);
        List<RoomDTO> GetAllRooms();


        RoomDTO Create(RoomDTO roomDTO);
        RoomDTO Update(RoomDTO roomDTO);
    }
}
