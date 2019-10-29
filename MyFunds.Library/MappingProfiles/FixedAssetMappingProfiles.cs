using AutoMapper;
using MyFunds.Data.Models;
using MyFunds.Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.MappingProfiles
{
    public class FixedAssetMappingProfiles : Profile
    {
        public FixedAssetMappingProfiles()
        {
            CreateMap<FixedAssetDTO, FixedAsset>();
        }
    }
}
