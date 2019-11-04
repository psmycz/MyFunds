using AutoMapper;
using MyFunds.Data.Models;
using MyFunds.Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.MappingProfiles
{
    public class MobileAssetMappingArchiveProfile : Profile
    {
        public MobileAssetMappingArchiveProfile()
        {
            CreateMap<MobileAsset, MobileAssetArchive>()
                .ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.MobileAssetId, opt => opt.MapFrom(src => src.Id));

            CreateMap<MobileAssetArchive, MobileAssetArchiveDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => new UserDTO()
                {
                    Id = src.User.Id,
                    UserName = src.User.UserName,
                    Email = src.User.Email,
                }))
                .ForMember(dest => dest.MobileAsset, opt => opt.Ignore()); 

        }
    }
}
