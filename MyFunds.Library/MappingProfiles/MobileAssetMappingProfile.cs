using AutoMapper;
using MyFunds.Data.Models;
using MyFunds.Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.MappingProfiles
{
    public class MobileAssetMappingProfile : Profile
    {
        public MobileAssetMappingProfile()
        {
            CreateMap<MobileAssetDTO, MobileAsset>();

            CreateMap<MobileAsset, MobileAssetDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => new UserDTO() 
                {
                    Id = src.User.Id,
                    Email = src.User.Email,
                    UserName = src.User.UserName
                }))
                .ForMember(dest => dest.PreviousMobileAsset, opt => opt.Ignore())
                .ForMember(dest => dest.MobileAssetArchives, opt => opt.Ignore());
        }
    }
}
