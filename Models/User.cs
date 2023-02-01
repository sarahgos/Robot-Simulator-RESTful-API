using System;
using System.Collections.Generic;

namespace robot_controller_api.Models
{
    // This entity type class was automatically generated

    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? Description { get; set; }
        public string? Role { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public User()
        {
        }

        public User(int id, string email, string firstname, string lastname, string passwordhash, string? description, string? role, DateTime createddate, DateTime modifieddate )
        {
            Id = id;
            Email = email;
            FirstName = firstname;
            LastName = lastname;
            PasswordHash = passwordhash;
            Description = description;
            Role = role;
            CreatedDate = createddate;
            ModifiedDate = modifieddate;
        }


    }
}
