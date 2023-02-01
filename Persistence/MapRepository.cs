using Npgsql;
using static Npgsql.Replication.PgOutput.Messages.RelationMessage;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    public class MapRepository : IMapDataAccess, IRepository
    {
        private IRepository _repo => this;

        /// <summary>
        /// Gets all maps
        /// </summary>
        /// <returns>List of maps</returns>
        public List<Map> GetMaps()
        {
            var maps = _repo.ExecuteReader<Map>("SELECT * FROM map");
            return maps;
        }

        /// <summary>
        /// Get square maps only
        /// </summary>
        /// <returns>List of square maps</returns>
        public List<Map> GetSquareMapsOnly()
        {
            var squareMaps = _repo.ExecuteReader<Map>("SELECT * FROM map WHERE is_square_map = true");
            return squareMaps;
        }

        /// <summary>
        /// Gets a map by id
        /// </summary>
        /// <param name="id">id of map to return</param>
        /// <returns>A map with matching id</returns>
        public Map GetMapById(int id)
        {
            var map = _repo.ExecuteReader<Map>("SELECT * FROM map WHERE id = " + id);

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
            Map result = new Map();

            var sqlParams = new NpgsqlParameter[]{
                new("id", updatedMap.Id),
                new("name", updatedMap.Name),
                new("description", updatedMap.Description ?? (object)DBNull.Value),
                new("columns", updatedMap.Columns),
                new("rows", updatedMap.Rows)
            };

            try
            {
                result = _repo.ExecuteReader<Map>("UPDATE map SET name = @name, description = @description, " +
                    "modified_date = current_timestamp, columns = @columns, rows = @rows WHERE id = @id RETURNING *; ", sqlParams).Single();

                Console.WriteLine(result.Name);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            catch (PostgresException)
            {
                return result;
            }

            return result;
        }

        /// <summary>
        /// Inserts a new map
        /// </summary>
        /// <param name="map">Map to be inserted</param>
        /// <returns>Map object</returns>
        public Map InsertMap(Map map)
        {
            Map result = new Map();

            var sqlParams = new NpgsqlParameter[]{
                new("name", map.Name),
                new("description", map.Description ?? (object)DBNull.Value),
                new("is_square_map", map.IsSquareMap),
                new("columns", map.Columns),
                new("rows", map.Rows)
            };

            try
            {
                result = _repo.ExecuteReader<Map>("INSERT INTO map (name, description, created_date, modified_date, columns, rows) " +
                    "VALUES (@name, @description, current_timestamp, current_timestamp, @columns, @rows) RETURNING *; ", sqlParams).Single();
            }
            catch (PostgresException)
            {
                return null;
            }

            return result;
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
            var result = _repo.ExecuteReader<Map>("SELECT * FROM map WHERE id = " + id);

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
            var result = _repo.ExecuteReader<Map>("DELETE FROM map WHERE id = " + id + " RETURNING *; ");

            if (result.Count == 0)
                return null;
            else
                return result[0];
        }
    }
}
