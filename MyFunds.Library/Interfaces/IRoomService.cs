using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.Interfaces
{
    public interface IRoomService : IBaseService<IRoomService>
    {
        bool RoomExist(int roomId);
    }
}
