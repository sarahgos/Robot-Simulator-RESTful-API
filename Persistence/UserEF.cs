using Microsoft.AspNetCore.Identity;
using Npgsql;
using robot_controller_api.Contexts;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence
{
    public class UserEF : RobotContext, IUserDataAccess
    {
        private RobotContext robotContext = new RobotContext();

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns>List of users</returns>
        public List<User> GetUsers()
        {
            var users = robotContext.Users.ToList();

            return users;
        }

        /// <summary>
        /// Gets admin users only.
        /// </summary>
        /// <returns>List of admin users</returns>
        public List<User> GetAdminUsersOnly()
        {
            var adminUsers = robotContext.Users.Where(user => user.Role == "Admin").ToList();

            return adminUsers;
        }

        /// <summary>
        /// Get a user by given id
        /// </summary>
        /// <param name="id">id of user</param>
        /// <returns>A user object</returns>
        public User GetUserById(int id)
        {
            var user = robotContext.Users.Where(user => user.Id == id).ToList();

            if (user.Count == 0)
                return null;
            else
                return user[0];
        }

        /// <summary>
        /// Inserts a new user
        /// </summary>
        /// <param name="user">New User object</param>
        /// <returns>User result</returns>
        public User InsertUser(User user)
        {
            robotContext.Users.Add(user);

            try
            {
                robotContext.SaveChanges();
            }
            catch (Exception)
            {
                user.Email = null;
                return null;
            }

            return user;
        }


        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <param name="updatedUser">User object with updated fields</param>
        /// <returns>User result</returns>
        public User UpdateUser(User updatedUser)
        {
            try
            {
                User userUpdating = robotContext.Users.Where(user => user.Id == updatedUser.Id).First();
                userUpdating.FirstName = updatedUser.FirstName;
                userUpdating.LastName = updatedUser.LastName;
                userUpdating.Description = updatedUser.Description;
                userUpdating.Role = updatedUser.Role;
                userUpdating.CreatedDate = DateTime.Now;
                userUpdating.ModifiedDate = DateTime.Now;
                robotContext.SaveChanges();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            catch (PostgresException)
            {
                updatedUser.Email = null;
                return updatedUser;
            }

            return updatedUser;
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="id">Id of user to be deleted</param>
        /// <returns>The deleted user</returns>
        public User DeleteUser(int id)
        {
            var user = robotContext.Users.Where((user) => user.Id == id).FirstOrDefault();

            if (user != null)
            {
                robotContext.Users.Remove(user);
                robotContext.SaveChanges();
            }
            else
                return null;

            return user;
        }

        public User UpdateEmailAndPassword(int id, Login updatedLogin)
        {
            User user;

            try
            {
                user = robotContext.Users.Where((user) => user.Id == id).FirstOrDefault();
                user.Email = updatedLogin.Email;
                user.PasswordHash = updatedLogin.Password;
                robotContext.SaveChanges();
            }
            catch (InvalidOperationException)
            {
                return null;
            }

            return user;
        }
    }
}
