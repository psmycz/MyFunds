using AutoMapper;
using MyFunds.Data.Models;
using MyFunds.Library.DTO;
using MyFunds.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.MappingProfiles
{
    public class ViewModelMappingProfile : Profile
    {
        public ViewModelMappingProfile()
        {
            CreateMap<FixedAssetRequest, FixedAssetDTO>()
                .ForMember(dest => dest.PurchaseDate, opt => opt.MapFrom(src => src.PurchaseDate.ToUniversalTime()))
                .ForMember(dest => dest.WarrantyEndDate, opt => opt.MapFrom(src => src.WarrantyEndDate.ToUniversalTime()))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<FixedAssetType>(src.Type)));

            CreateMap<MobileAssetRequest, MobileAssetDTO>()
                .ForMember(dest => dest.PurchaseDate, opt => opt.MapFrom(src => src.PurchaseDate.ToUniversalTime()))
                .ForMember(dest => dest.WarrantyEndDate, opt => opt.MapFrom(src => src.WarrantyEndDate.ToUniversalTime()));

            CreateMap<RoomRequest, RoomDTO>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<RoomType>(src.Type)));

            CreateMap<BuildingRequest, BuildingDTO>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address()
                {
                    City = src.City,
                    Country = src.Country,
                    Postcode = src.Postcode,
                    Street = src.Street
                }));
        }
    }
}
