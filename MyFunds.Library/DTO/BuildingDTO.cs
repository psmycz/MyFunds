using AutoMapper.Configuration.Annotations;
using MyFunds.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.DTO
{
    public class BuildingDTO
    {
        public int Id { get; set; }
        

        public Address Address { get; set; }
        public List<RoomDTO> Rooms { get; set; }
        [Ignore]
        public List<FixedAssetDTO> FixedAssets { get; set; }
    }
}
