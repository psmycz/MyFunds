using AutoMapper;
using MyFunds.Data.Interfaces;
using MyFunds.Data.Models;
using MyFunds.Library.DTO;
using MyFunds.Library.Exceptions;
using MyFunds.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.Services
{
    public class BuildingService : BaseService<BuildingService>, IBuildingService
    {
        readonly IBuildingRepository buildingRepository;
        readonly IAddressRepository addressRepository;

        public BuildingService(IBuildingRepository buildingRepository, IAddressRepository addressRepository, IMapper mapper)
            : base(mapper)
        {
            this.buildingRepository = buildingRepository;
            this.addressRepository = addressRepository;
        }


        public bool BuildingExist(int buildingId)
        {
            return buildingRepository.Exist(b => b.Id == buildingId);
        }

        public BuildingDTO GetBuilding(int buildingId)
        {
            if (buildingId <= 0)
                throw new ApiException("Incorrect Id");

            var building = buildingRepository.GetById(buildingId);
            var buildingDTO = mapper.Map<BuildingDTO>(building ?? throw new NoDataException("No building with provided Id"));

            return buildingDTO;
        }
        
        public List<BuildingDTO> GetAllBuildings()
        {
            var buildings = mapper.Map<List<BuildingDTO>>(buildingRepository.GetAll());

            return buildings ?? throw new NoDataException("No existing buildings");
        }


        public BuildingDTO GetBuildingWithAssets(int buildingId)
        {
            if (buildingId <= 0)
                throw new ApiException("Incorrect Id");

            var building = buildingRepository.GetById(buildingId);
            var buildingDTO = mapper.Map<BuildingDTO>(building ?? throw new NoDataException("No mobile asset with provided Id"));
            
            var allFixedAssets = building.Rooms?.SelectMany(r => r.FixedAssets).ToList();
            buildingDTO.FixedAssets = mapper.Map<List<FixedAssetDTO>>(allFixedAssets);


            return buildingDTO;
        }

        // TODO: check if building with address already exists
        public BuildingDTO Create(BuildingDTO buildingDTO)
        {
            if (buildingDTO.Id != 0)
                throw new ApiException("Cannot declare Id while creating new Building");

            var newBuilding = buildingRepository.Insert(new Building());
            buildingDTO.Address.BuildingId = newBuilding.Id;
            
            var address = addressRepository.Insert(buildingDTO.Address);
            buildingRepository.Save();


            buildingDTO = mapper.Map<BuildingDTO>(newBuilding);

            return buildingDTO;
        }

        // TODO: check if building with address already exists
        public BuildingDTO Update(BuildingDTO buildingDTO)
        {
            if (buildingDTO.Id <= 0)
                throw new ApiException("Incorrect Id");
            if (!buildingRepository.Exist(b => b.Id == buildingDTO.Id))
                throw new ApiException("Building with provided Id does not exist");

            var building = buildingRepository.GetById(buildingDTO.Id);
            buildingDTO.Address.Id = building.Address.Id;
            buildingDTO.Address.BuildingId = building.Id;

            if (!HasPropertyUpdated(building.Address.Id, buildingDTO.Address, addressRepository))
                throw new ApiException("None of the properties have changed");

            addressRepository.Update(buildingDTO.Address);
            addressRepository.Save();

            buildingDTO = mapper.Map<BuildingDTO>(building);

            return buildingDTO;
        }
    }
}
