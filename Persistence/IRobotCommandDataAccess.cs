using Npgsql;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    public interface IRobotCommandDataAccess
    {
        RobotCommand DeleteRobotCommand(int id);
        List<RobotCommand> GetMoveCommandsOnly();
        RobotCommand GetRobotCommandById(int id);
        List<RobotCommand> GetRobotCommands();
        RobotCommand InsertRobotCommand(RobotCommand robotCommand);
        RobotCommand UpdateRobotCommand(RobotCommand updatedCommand);
    }
}