using Npgsql;
using System.Diagnostics;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    public interface IRepository
    {
        private const string CONNECTION_STRING = "Host=localhost;Username=postgres;Password=password;Database=sit331";

        /// <summary>
        /// Generic method for all data repositories.
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="sqlCommand">sql command to be executed</param>
        /// <param name="dbParams">database parameters</param>
        /// <returns>A list of entities</returns>
        public List<T> ExecuteReader<T>(string sqlCommand, NpgsqlParameter[] dbParams = null) where T : class, new()
        {
            var entities = new List<T>();
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand(sqlCommand, conn);

            if (dbParams is not null)
            {
                cmd.Parameters.AddRange(dbParams.Where(x => x.Value is not null).ToArray());
            }

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                var entity = new T();
                // Where automatic ORM happens.
                dr.MapTo(entity); 
                entities.Add(entity);
            }

            return entities;
        }
    }
}
