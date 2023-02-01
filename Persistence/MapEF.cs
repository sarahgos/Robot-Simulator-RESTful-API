using Microsoft.EntityFrameworkCore;
using Npgsql;
using robot_controller_api.Contexts;
using robot_controller_api.Models;
using static Humanizer.On;

namespace robot_controller_api.Persistence
{
    public class MapEF : RobotContext, IMapDataAccess
    {
        private RobotContext robotContext = new RobotContext();

        /// <summary>
        /// Gets all maps
        /// </summary>
        /// <returns>List of maps</returns>
        public List<Map> GetMaps()
        {
            var maps = robotContext.Maps.ToList();
            return maps;
        }

        /// <summary>
        /// Get square maps only
        /// </summary>
        /// <returns>List of square maps</returns>
        public List<Map> GetSquareMapsOnly()
        {
            var squareMaps = robotContext.Maps.Where(map => map.IsSquareMap == true).ToList();
            return squareMaps;
        }

        /// <summary>
        /// Gets a map by id
        /// </summary>
        /// <param name="id">id of map to return</param>
        /// <returns>A map with matching id</returns>
        public Map GetMapById(int id)
        {
            var map = robotContext.Maps.Where(map => map.Id == id).ToList();

            if (map.Count == 0)
                return null;
            else
                return map[0];
        }

        /// <summary>
        /// Updates a map
        /// </summary>
        /// <param name="updatedMap">Map object with updated fields</param>
        /// <returns>Map object</returns>
        public Map UpdateMap(Map updatedMap)
        {
            try
            {
                Map mapToUpdate = robotContext.Maps.Where(map => map.Id == updatedMap.Id).First();
                mapToUpdate.Name = updatedMap.Name;
                mapToUpdate.Columns = updatedMap.Columns;
                mapToUpdate.Rows = updatedMap.Rows;
                mapToUpdate.Description = updatedMap.Description;
                mapToUpdate.CreatedDate = DateTime.Now;
                mapToUpdate.ModifiedDate = DateTime.Now;
                robotContext.SaveChanges();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            catch (PostgresException)
            {
                updatedMap.Name = null;
                return updatedMap;
            }
            return updatedMap;
        }

        /// <summary>
        /// Inserts a new map
        /// </summary>
        /// <param name="map">Map to be inserted</param>
        /// <returns>Map object</returns>
        public Map InsertMap(Map map)
        {
            map.CreatedDate = DateTime.Now;
            map.ModifiedDate = DateTime.Now;

            robotContext.Maps.Add(map);   
                
            try
            {
                robotContext.SaveChanges();
            }
            catch (Exception)
            {
                map.Name = null;
                return null;
            }

            return map;
        }

        /// <summary>
        /// Checks to see if given coordinates are on a map
        /// </summary>
        /// <param name="id">id of map to check</param>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <returns>Map object</returns>
        public Map CheckCoordinates(int id, int x, int y)
        {
            var result = robotContext.Maps.Where(map => map.Id == id).ToList();
                
            if (result.Count == 0)
                return null;
            else
                return result[0];
        }

        /// <summary>
        /// Deletes a map
        /// </summary>
        /// <param name="id">id of map to delete</param>
        /// <returns>Map object</returns>
        public Map DeleteMap(int id)
        {
            var map = robotContext.Maps.Where((map) => map.Id == id).FirstOrDefault();

            if (map != null)
            {
                robotContext.Maps.Remove(map);
                robotContext.SaveChanges();
            }
            else
                return null;

            return map;
        }
    }
}
