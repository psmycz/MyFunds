using MyFunds.Library.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Library.Interfaces
{
    public interface IBuildingService : IBaseService<IBuildingService>
    {
        bool BuildingExist(int buildingId);

        BuildingDTO GetBuildingWithAssets(int buildingId);
        BuildingDTO GetBuilding(int buildingId);
        BuildingDTO GetBuildingWithRooms(int buildingId);
        List<BuildingDTO> GetAllBuildings();
        List<BuildingDTO> GetAllBuildingsWithRooms();


        BuildingDTO Create(BuildingDTO buildingDTO);
        BuildingDTO Update(BuildingDTO buildingDTO);
    }
}
