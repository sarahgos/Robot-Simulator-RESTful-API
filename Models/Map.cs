using System;
using System.Collections.Generic;

namespace robot_controller_api.Models
{
    // This entity type class was automatically generated

    public class Map
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool? IsSquareMap { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }

        public Map()
        {
        }

        public Map(int id, string name, string? description, bool? issquaremap, DateTime createddate, DateTime modifieddate, int columns, int rows )
        {
            Id = id;
            Name = name;
            Description = description;
            IsSquareMap = issquaremap;
            CreatedDate = createddate;
            ModifiedDate = modifieddate;
            Columns = columns;
            Rows = rows;
        }


    }
}
