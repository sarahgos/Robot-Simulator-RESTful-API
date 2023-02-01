using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;
using robot_controller_api.Contexts;
using robot_controller_api.Models;
using static Humanizer.On;

namespace robot_controller_api.Persistence
{
    public class RobotCommandEF : RobotContext, IRobotCommandDataAccess
    {
        private RobotContext robotContext = new RobotContext();

        /// <summary>
        /// Gets all robot commands
        /// </summary>
        /// <returns>List of robot commands</returns>
        public List<RobotCommand> GetRobotCommands()
        {
            var commands = robotContext.RobotCommands.ToList();

            return commands;
        }

        /// <summary>
        /// Gets move robot commands only.
        /// </summary>
        /// <returns>List of move commands</returns>
        public List<RobotCommand> GetMoveCommandsOnly()
        {
            var moveCommands = robotContext.RobotCommands.Where(command => command.IsMoveCommand).ToList();

            return moveCommands;
        }

        /// <summary>
        /// Get a robot command by given id
        /// </summary>
        /// <param name="id">id of robot command</param>
        /// <returns>A robot command object</returns>
        public RobotCommand GetRobotCommandById(int id)
        {
            var command = robotContext.RobotCommands.Where(command => command.Id == id).ToList();

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
            try
            {
                RobotCommand robotCommandUpdating = robotContext.RobotCommands.Where(map => map.Id == updatedCommand.Id).First();
                robotCommandUpdating.Name = updatedCommand.Name;
                robotCommandUpdating.IsMoveCommand = updatedCommand.IsMoveCommand;
                robotCommandUpdating.Description = updatedCommand.Description;
                robotCommandUpdating.CreatedDate = DateTime.Now;
                robotCommandUpdating.ModifiedDate = DateTime.Now;
                robotContext.SaveChanges();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            catch (PostgresException)
            {
                updatedCommand.Name = null;
                return updatedCommand;
            }

            return updatedCommand;
        }

        /// <summary>
        /// Inserts a new robot commands
        /// </summary>
        /// <param name="robotCommand">New RobotCommand object</param>
        /// <returns>Robot command result</returns>
        public RobotCommand InsertRobotCommand(RobotCommand robotCommand)
        {
            robotCommand.CreatedDate = DateTime.Now;
            robotCommand.ModifiedDate = DateTime.Now;

            robotContext.RobotCommands.Add(robotCommand);

            try
            {
                robotContext.SaveChanges();
            }
            catch (Exception)
            {
                robotCommand.Name = null;
                return null;
            }

            return robotCommand;
        }

        /// <summary>
        /// Deletes a robot command
        /// </summary>
        /// <param name="id">Id of command to be deleted</param>
        /// <returns>The deleted robot command</returns>
        public RobotCommand DeleteRobotCommand(int id)
        {
            var command = robotContext.RobotCommands.Where((command) => command.Id == id).FirstOrDefault();

            if (command != null)
            {
                robotContext.RobotCommands.Remove(command);
                robotContext.SaveChanges();
            }
            else
                return null;

            return command;
        }
    }
}
