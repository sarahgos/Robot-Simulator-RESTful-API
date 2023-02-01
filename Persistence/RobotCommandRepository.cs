using Npgsql;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    public class RobotCommandRepository : IRobotCommandDataAccess, IRepository
    {
        private IRepository _repo => this;

        /// <summary>
        /// Gets all robot commands
        /// </summary>
        /// <returns>List of robot commands</returns>
        public List<RobotCommand> GetRobotCommands()
        {
            var commands = _repo.ExecuteReader<RobotCommand>("SELECT * FROM robot_command");
            return commands;
        }

        /// <summary>
        /// Gets move robot commands only.
        /// </summary>
        /// <returns>List of move commands</returns>
        public List<RobotCommand> GetMoveCommandsOnly()
        {
            var moveCommands = _repo.ExecuteReader<RobotCommand>("SELECT * FROM robot_command WHERE is_move_command = true");
            return moveCommands;
        }

        /// <summary>
        /// Get a robot command by given id
        /// </summary>
        /// <param name="id">id of robot command</param>
        /// <returns>A robot command object</returns>
        public RobotCommand GetRobotCommandById(int id)
        {
            var command = _repo.ExecuteReader<RobotCommand>("SELECT * FROM robot_command WHERE id = " + id);

            if (command.Count == 0)
                return null;
            else
                return command[0];
        }

        /// <summary>
        /// Updates a robot command
        /// </summary>
        /// <param name="updatedCommand">RobotCommand object with updated fields</param>
        /// <returns>RobotCommand result</returns>
        public RobotCommand UpdateRobotCommand(RobotCommand updatedCommand)
        {
            RobotCommand result = new RobotCommand();

            var sqlParams = new NpgsqlParameter[]{
                new("id", updatedCommand.Id),
                new("name", updatedCommand.Name),
                new("description", updatedCommand.Description ?? (object)DBNull.Value),
                new("is_move_command", updatedCommand.IsMoveCommand)
            };

            try
            {
                result = _repo.ExecuteReader<RobotCommand>("UPDATE robot_command SET name = @name, description = @description, " +
                    "is_move_command = @is_move_command, modified_date = current_timestamp WHERE id = @id RETURNING *; ", sqlParams).Single();

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
        /// Inserts a new robot commands
        /// </summary>
        /// <param name="robotCommand">New RobotCommand object</param>
        /// <returns>Robot command result</returns>
        public RobotCommand InsertRobotCommand(RobotCommand robotCommand)
        {
            RobotCommand result = new RobotCommand();

            var sqlParams = new NpgsqlParameter[]{
                new("name", robotCommand.Name),
                new("description", robotCommand.Description ?? (object)DBNull.Value),
                new("is_move_command", robotCommand.IsMoveCommand)
            };

            try
            {
                result = _repo.ExecuteReader<RobotCommand>("INSERT INTO robot_command (name, description, is_move_command, created_date, modified_date) " +
                    "VALUES (@name, @description, @is_move_command, current_timestamp, current_timestamp) RETURNING *; ", sqlParams).Single();
            }
            catch (PostgresException)
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// Deletes a robot command
        /// </summary>
        /// <param name="id">Id of command to be deleted</param>
        /// <returns>The deleted robot command</returns>
        public RobotCommand DeleteRobotCommand(int id)
        {
            var result = _repo.ExecuteReader<RobotCommand>("DELETE FROM robot_command WHERE id = " + id + " RETURNING *; ");

            if (result.Count == 0)
                return null;
            else
                return result[0];
        }
    }
}



