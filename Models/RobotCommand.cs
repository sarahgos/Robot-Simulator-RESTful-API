using System;
using System.Collections.Generic;

namespace robot_controller_api.Models
{
    // This entity type class was automatically generated

    public class RobotCommand
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsMoveCommand { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public RobotCommand()
        {
        }

        public RobotCommand(int id, string name, string? description, bool ismovecommand, DateTime createddate, DateTime modifieddate )
        {
            Id = id;
            Name = name;
            Description = description;
            IsMoveCommand = ismovecommand;
            CreatedDate = createddate;
            ModifiedDate = modifieddate;
        }


    }
}
