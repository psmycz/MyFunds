using AutoMapper;
using MyFunds.Data.Models;
using MyFunds.Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.MappingProfiles
{
    public class FixedAssetMappingArchiveProfile : Profile
    {
        public FixedAssetMappingArchiveProfile()
        {
            CreateMap<FixedAsset, FixedAssetArchive>()
                .ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FixedAssetId, opt => opt.MapFrom(src => src.Id));

            CreateMap<FixedAssetArchive, FixedAssetArchiveDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => new UserDTO()
                {
                    Id = src.User.Id,
                    UserName = src.User.UserName,
                    Email = src.User.Email,
                }))
                .ForMember(dest => dest.FixedAsset, opt => opt.Ignore()); 

        }
    }
}
