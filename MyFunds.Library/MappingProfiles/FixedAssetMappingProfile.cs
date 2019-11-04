using AutoMapper;
using MyFunds.Data.Models;
using MyFunds.Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.MappingProfiles
{
    public class FixedAssetMappingProfile : Profile
    {
        public FixedAssetMappingProfile()
        {
            CreateMap<FixedAssetDTO, FixedAsset>();

            CreateMap<FixedAsset, FixedAssetDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => new UserDTO()
                {
                    Id = src.User.Id,
                    UserName = src.User.UserName,
                    Email = src.User.Email,
                }))
                .ForMember(dest => dest.PreviousFixedAsset, opt => opt.Ignore())
                .ForMember(dest => dest.FixedAssetArchives, opt => opt.Ignore());
        }
    }
}
