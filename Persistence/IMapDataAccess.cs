using Npgsql;
using robot_controller_api.Models;


namespace robot_controller_api.Persistence
{
    public interface IMapDataAccess
    {
        Map CheckCoordinates(int id, int x, int y);
        Map DeleteMap(int id);
        Map GetMapById(int id);
        List<Map> GetMaps();
        List<Map> GetSquareMapsOnly();
        Map InsertMap(Map map);
        Map UpdateMap(Map updatedMap);
    }
}