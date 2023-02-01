using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Windows.Input;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence

{
    public class MapADO : IMapDataAccess
    {
        private const string CONNECTION_STRING = "Host=localhost;Username=postgres;Password=password;Database=sit331";

        /// <summary>
        /// Method to get maps from database.
        /// </summary>
        /// <returns>List of maps</returns>
        public List<Map> GetMaps()
        {
            var maps = new List<Map>();
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT * FROM map", conn);
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                int id = (int)dr["id"];
                string name = (string)dr["name"];
                string? description = dr["description"] as string;
                DateTime createdDate = (DateTime)dr["created_date"];
                DateTime modifiedDate = (DateTime)dr["modified_date"];
                int columns = (int)dr["columns"];
                int rows = (int)dr["rows"];
                bool isSquareMap = (bool)dr["is_square_map"];

                Map map = new Map(id, name, description, isSquareMap, createdDate, modifiedDate, columns, rows);

                maps.Add(map);
            }

            return maps;
        }

        /// <summary>
        /// Gets the maps with isSquareMap = true
        /// </summary>
        /// <returns>A list of maps</returns>
        public List<Map> GetSquareMapsOnly()
        {
            var maps = new List<Map>();
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT * FROM map WHERE is_square_map = true", conn);
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                int id = (int)dr["id"];
                string name = (string)dr["name"];
                string? description = dr["description"] as string;
                DateTime createdDate = (DateTime)dr["created_date"];
                DateTime modifiedDate = (DateTime)dr["modified_date"];
                int columns = (int)dr["columns"];
                int rows = (int)dr["rows"];
                bool isSquareMap = (bool)dr["is_square_map"];

                Map map = new Map(id, name, description, isSquareMap, createdDate, modifiedDate, columns, rows);

                maps.Add(map);
            }

            return maps;
        }

        /// <summary>
        /// Finds map in the database by id
        /// </summary>
        /// <param name="id">id of the database row</param>
        /// <returns>map by id</returns>
        public Map GetMapById(int id)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            string name = null;
            string? description = "";
            DateTime createdDate = DateTime.Now;
            DateTime modifiedDate = DateTime.Now;
            int columns = 0;
            int rows = 0;
            bool isSquareMap = false;

            using var cmd = new NpgsqlCommand("SELECT * FROM map WHERE id = " + id, conn);

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                id = (int)dr["id"];
                name = (string)dr["name"];
                description = dr["description"] as string;
                createdDate = (DateTime)dr["created_date"];
                modifiedDate = (DateTime)dr["modified_date"];
                columns = (int)dr["columns"];
                rows = (int)dr["rows"];
                isSquareMap = (bool)dr["is_square_map"];
            }

            Map map = new Map(id, name, description, isSquareMap, createdDate, modifiedDate, columns, rows);

            return map;
        }

        /// <summary>
        /// Updates a map in the database.
        /// </summary>
        /// <param name="id">id of database row</param>
        /// <param name="updatedMap">map object with updated properties</param>
        /// <returns>string with success/error</returns>
        public Map UpdateMap(Map updatedMap)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            string sql = "";
            string result;
            int rows;

            /// Drop the unique name constraint for updating an entry.
            string drop_unique_constraint = "ALTER TABLE map DROP CONSTRAINT unique_name_map";
            using var dropConstraint = new NpgsqlCommand(drop_unique_constraint, conn);
            dropConstraint.ExecuteNonQuery();

            /// If there is no description included in robot command.
            if (updatedMap.Description != null)
            {
                sql = "UPDATE map SET name = $1, columns = $2, rows = $3, modified_date = $4, description = $5 WHERE id = $6";

                using var cmd = new NpgsqlCommand(sql, conn)
                {
                    Parameters =
                    {
                        new() { Value = updatedMap.Name },
                        new() { Value = updatedMap.Columns },
                        new() { Value = updatedMap.Rows },
                        new() { Value = DateTime.Now },
                        new() { Value = updatedMap.Description },
                        new() { Value = updatedMap.Id }
                    }
                };
                rows = cmd.ExecuteNonQuery();
            }
            else
            {
                sql = "UPDATE map SET name = $1, columns = $2, rows = $3, modified_date = $4 WHERE id = $5";

                using var cmd = new NpgsqlCommand(sql, conn)
                {
                    Parameters =
                    {
                        new() { Value = updatedMap.Name },
                        new() { Value = updatedMap.Columns },
                        new() { Value = updatedMap.Rows },
                        new() { Value = DateTime.Now },
                        new() { Value = updatedMap.Id }
                    }
                };
                rows = cmd.ExecuteNonQuery();
            }

            /// Add the unique name constraint back on.
            string add_unique_constraint = "ALTER TABLE map ADD CONSTRAINT unique_name_map UNIQUE (name)";
            using var addConstraint = new NpgsqlCommand(add_unique_constraint, conn);
            addConstraint.ExecuteNonQuery();

            if (rows == 0)
                return null;

            return updatedMap;
        }

        /// <summary>
        /// Inserts a new map into the database.
        /// </summary>
        /// <param name="robotCommand">map object</param>
        /// <returns>String with success/error</returns>
        public Map InsertMap(Map map)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            string sql = "";
            string result;
            int rows;

            try
            {
                if (map.Description != null)
                {
                    sql = "INSERT INTO map (name, description, columns, rows, created_date, modified_date) VALUES ($1, $2, $3, $4, $5, $6)";

                    using var cmd = new NpgsqlCommand(sql, conn)
                    {
                        Parameters =
                        {
                            new() { Value = map.Name },
                            new() { Value = map.Description },
                            new() { Value = map.Columns },
                            new() { Value = map.Rows },
                            new() { Value = DateTime.Now },
                            new() { Value = DateTime.Now },
                        }
                    };

                    rows = cmd.ExecuteNonQuery();
                }
                else
                {
                    sql = "INSERT INTO map (name, columns, rows, created_date, modified_date) VALUES ($1, $2, $3, $4, $5)";

                    using var cmd = new NpgsqlCommand(sql, conn)
                    {
                        Parameters =
                        {
                            new() { Value = map.Name },
                            new() { Value = map.Columns },
                            new() { Value = map.Rows },
                            new() { Value = DateTime.Now },
                            new() { Value = DateTime.Now },
                        }
                    };
                    rows = cmd.ExecuteNonQuery();
                }
            }
            catch (PostgresException)
            {
                map.Name = null;
                return map;
            }

            if (rows == 0)
                return null;

            return map;
        }

        /// <summary>
        /// Delete a map from the database
        /// </summary>
        /// <param name="id">id of database row</param>
        /// <returns>success or not found</returns>
        public Map DeleteMap(int id)
        {
            Map map = GetMapById(id);

            if (map != null)
            {
                using var conn = new NpgsqlConnection(CONNECTION_STRING);
                conn.Open();

                string sql = "DELETE FROM map WHERE id = " + id;
                using var cmd = new NpgsqlCommand(sql, conn);
                int rows = cmd.ExecuteNonQuery();
            }

            return map;
        }

        /// <summary>
        /// Checks to see if given coordinates are on the map.
        /// </summary>
        /// <param name="id">id of map object</param>
        /// <param name="x">columns coordinate</param>
        /// <param name="y">rows corrodinate</param>
        /// <returns>string success or not on map</returns>
        public Map CheckCoordinates(int id, int x, int y)
        {
            int columns = 0;
            int rows = 0;

            Map map = GetMapById(id);

            if (map != null)
            {
                using var conn = new NpgsqlConnection(CONNECTION_STRING);
                conn.Open();
                using var cmd = new NpgsqlCommand("SELECT * FROM map WHERE id = " + id, conn);
                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    columns = (int)dr["columns"];
                    rows = (int)dr["rows"];
                }
            }

            if (x > columns || x < 0 || y > rows || y < 0)
                map.Rows = -1;

            return map;
        }
    }
}
