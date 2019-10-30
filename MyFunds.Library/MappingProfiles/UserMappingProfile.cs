using AutoMapper;
using MyFunds.Data.Models;
using MyFunds.Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.MappingProfiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.FixedAssets, opt => opt.MapFrom(src => src.FixedAssets.Select(fa => new FixedAssetDTO()
                {
                    Id = fa.Id,
                    InUse = fa.InUse,
                    Name = fa.Name,
                    Price = fa.Price,
                    PurchaseDate = fa.PurchaseDate,
                    RoomId = fa.RoomId,
                    Room = new RoomDTO()
                    {
                        Id = fa.Room.Id,
                        Area = fa.Room.Area,
                        Building = fa.Room.Building,
                        BuildingId = fa.Room.BuildingId,
                        Floor = fa.Room.Floor,
                        Type = fa.Room.Type
                    },
                    Type = fa.Type,
                    UserId = fa.UserId,
                    WarrantyEndDate = fa.WarrantyEndDate
                })))
                .ForMember(dest => dest.MobileAssets, opt => opt.MapFrom(src => src.MobileAssets.Select(ma => new MobileAssetDTO() 
                {
                    Id = ma.Id,
                    InUse = ma.InUse,
                    WarrantyEndDate = ma.WarrantyEndDate,
                    UserId = ma.UserId,
                    Name = ma.Name,
                    Price = ma.Price,
                    PurchaseDate = ma.PurchaseDate
                })));
        }
    }
}
