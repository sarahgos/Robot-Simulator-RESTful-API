using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    public interface IUserDataAccess
    {
        User DeleteUser(int id);
        List<User> GetAdminUsersOnly();
        List<User> GetUsers();
        User GetUserById(int id);
        User InsertUser(User user);
        User UpdateEmailAndPassword(int id, Login login);
        User UpdateUser(User updatedUser);
    }
}