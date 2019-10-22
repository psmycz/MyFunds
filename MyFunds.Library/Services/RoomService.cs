using AutoMapper;
using MyFunds.Data.Interfaces;
using MyFunds.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.Services
{
    public class RoomService : BaseService<RoomService>, IRoomService
    {
        readonly IRoomRepository roomRepository;

        public RoomService(IRoomRepository roomRepository, IMapper mapper)
            : base(mapper)
        {
            this.roomRepository = roomRepository;
        }

    }
}
