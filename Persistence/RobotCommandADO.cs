using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Windows.Input;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    public class RobotCommandADO : IRobotCommandDataAccess
    {
        private const string CONNECTION_STRING = "Host=localhost;Username=postgres;Password=password;Database=sit331";

        /// <summary>
        /// Method to get robot commands from database.
        /// </summary>
        /// <returns>List of robot commands</returns>
        public List<RobotCommand> GetRobotCommands()
        {
            var robotCommands = new List<RobotCommand>();
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT * FROM robot_command", conn);
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                int id = (int)dr["id"];
                string name = (string)dr["name"];
                string? description = dr["description"] as string;
                bool isMoveCommand = (bool)dr["is_move_command"];
                DateTime createdDate = (DateTime)dr["created_date"];
                DateTime modifiedDate = (DateTime)dr["modified_date"];

                RobotCommand robotCommand = new RobotCommand(id, name, description, isMoveCommand, createdDate, modifiedDate );

                robotCommands.Add(robotCommand);
            }

            return robotCommands;
        }

        /// <summary>
        /// Gets the robot commands with isMoveCommand = true
        /// </summary>
        /// <returns>A list of move commands</returns>
        public List<RobotCommand> GetMoveCommandsOnly()
        {
            var robotCommands = new List<RobotCommand>();
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT * FROM robot_command WHERE is_move_command = true", conn);
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                int id = (int)dr["id"];
                string name = (string)dr["name"];
                string? description = dr["description"] as string;
                bool isMoveCommand = (bool)dr["is_move_command"];
                DateTime createdDate = (DateTime)dr["created_date"];
                DateTime modifiedDate = (DateTime)dr["modified_date"];

                RobotCommand robotCommand = new RobotCommand(id, name, description, isMoveCommand, createdDate, modifiedDate);

                robotCommands.Add(robotCommand);
            }

            return robotCommands;
        }

        /// <summary>
        /// Finds robot command in the database by id
        /// </summary>
        /// <param name="id">id of the database row</param>
        /// <returns>Robot command by id</returns>
        public RobotCommand GetRobotCommandById(int id)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            string name = null;
            string? description = "";
            bool isMoveCommand = false;
            DateTime createdDate = DateTime.Now;
            DateTime modifiedDate = DateTime.Now;

            using var cmd = new NpgsqlCommand("SELECT * FROM robot_command WHERE id = " + id, conn);
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                id = (int)dr["id"];
                name = (string)dr["name"];
                description = dr["description"] as string;
                isMoveCommand = (bool)dr["is_move_command"];
                createdDate = (DateTime)dr["created_date"];
                modifiedDate = (DateTime)dr["modified_date"];
            }

            RobotCommand robotCommand = new RobotCommand(id, name, description, isMoveCommand, createdDate, modifiedDate);

            return robotCommand;
        }

        /// <summary>
        /// Updates a robot command in the database.
        /// </summary>
        /// <param name="id">id of database row</param>
        /// <param name="updatedCommand">robot command object with updated properties</param>
        /// <returns>Boolean successful/unsuccessful</returns>
        public RobotCommand UpdateRobotCommand(RobotCommand updatedCommand)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            string sql = "";
            string result;
            int rows;

            /// Drop the unique name constraint for updating an entry.
            string drop_unique_constraint = "ALTER TABLE robot_command DROP CONSTRAINT unique_name_rb";
            using var dropConstraint = new NpgsqlCommand(drop_unique_constraint, conn);
            dropConstraint.ExecuteNonQuery();

            /// If there is no description included in robot command.

                if (updatedCommand.Description != null)
                {
                    sql = "UPDATE robot_command SET name = $1, is_move_command = $2, modified_date = $3, description = $4 WHERE id = $5 RETURNING *;";

                    using var cmd = new NpgsqlCommand(sql, conn)
                    {
                        Parameters =
                        {
                            new() { Value = updatedCommand.Name },
                            new() { Value = updatedCommand.IsMoveCommand },
                            new() { Value = DateTime.Now },
                            new() { Value = updatedCommand.Description },
                            new() { Value = updatedCommand.Id }
                        }
                    };

                    rows = cmd.ExecuteNonQuery();
                }
                else
                {

                    sql = "UPDATE robot_command SET name = $1, is_move_command = $2, modified_date = $3 WHERE id = $5 RETURNING *;";

                    using var cmd = new NpgsqlCommand(sql, conn)
                    {
                        Parameters =
                        {
                            new() { Value = updatedCommand.Name },
                            new() { Value = updatedCommand.IsMoveCommand },
                            new() { Value = DateTime.Now },
                            new() { Value = updatedCommand.Id }
                        }
                    };

                    rows = cmd.ExecuteNonQuery();
                }

            /// Add the unique name constraint back on.
            string add_unique_constraint = "ALTER TABLE robot_command ADD CONSTRAINT unique_name_rb UNIQUE (name)";
            using var addConstraint = new NpgsqlCommand(add_unique_constraint, conn);
            addConstraint.ExecuteNonQuery();

            if (rows == 0)
                return null;

            return updatedCommand;
        }

        /// <summary>
        /// Inserts a new robot command into the database.
        /// </summary>
        /// <param name="robotCommand">Robot command object</param>
        /// <returns>Boolean successful/unsuccessful</returns>
        public RobotCommand InsertRobotCommand(RobotCommand robotCommand)
        {
            using var conn = new NpgsqlConnection(CONNECTION_STRING);
            conn.Open();

            string sql = "";
            int rows;

            try
            {
                if (robotCommand.Description != null)
                {
                    sql = "INSERT INTO robot_command (name, description, is_move_command, created_date, modified_date) VALUES ($1, $2, $3, $4, $5) RETURNING *;";

                    using var cmd = new NpgsqlCommand(sql, conn)
                    {
                        Parameters =
                        {
                            new() { Value = robotCommand.Name },
                            new() { Value = robotCommand.Description },
                            new() { Value = robotCommand.IsMoveCommand },
                            new() { Value = DateTime.Now },
                            new() { Value = DateTime.Now },
                        }
                    };

                    rows = cmd.ExecuteNonQuery();
                }
                else
                {

                    sql = "INSERT INTO robot_command (name, is_move_command, created_date, modified_date) VALUES ($1, $2, $3, $4) RETURNING *;";

                    using var cmd = new NpgsqlCommand(sql, conn)
                    {
                        Parameters =
                        {
                            new() { Value = robotCommand.Name },
                            new() { Value = robotCommand.IsMoveCommand },
                            new() { Value = DateTime.Now },
                            new() { Value = DateTime.Now },
                        }
                    };
                    rows = cmd.ExecuteNonQuery();
                }
            }
            catch (PostgresException)
            {
                return null;
            }

            if (rows == 0)
                return null;

            return robotCommand;
        }

        /// <summary>
        /// Delete a robotcommand from the database
        /// </summary>
        /// <param name="id">id of database row</param>
        /// <returns>success or null if </returns>
        public RobotCommand DeleteRobotCommand(int id)
        {
            RobotCommand robotCommand = GetRobotCommandById(id);

            if (robotCommand != null)
            {
                using var conn = new NpgsqlConnection(CONNECTION_STRING);
                conn.Open();

                string sql = "DELETE FROM robot_command WHERE id = " + id;
                using var cmd = new NpgsqlCommand(sql, conn);
                var result = cmd.ExecuteNonQuery();
            }

            return robotCommand;
        }
    }
}
