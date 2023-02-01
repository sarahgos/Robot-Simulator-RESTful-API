using FastMember;
using Humanizer;
using Npgsql;

namespace robot_controller_api.Persistence
{
    /// <summary>
    /// Extension methods related to data accesss layer.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// An automatic Object-Relational Mapping (ORM) method.
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="dr">SQL data reader</param>
        /// <param name="entity">entity will be object of class eg, RobotCommand/Map etc</param>
        /// <exception cref="ArgumentNullException">If entity is null</exception>
        public static void MapTo<T>(this NpgsqlDataReader dr, T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            // FastMember needs an object of a specific class to get its properties (used later to map db columns).
            var fastMember = TypeAccessor.Create(entity.GetType());

            // Creates fast type accessor to iterate through properties. entity here could be map/robotcommand.
            // HashSet has O(1) time complexity and is used here to compare column/property names (not case sensitive).
            var props = fastMember.GetMembers().Select(x => x.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Loops through all columns from SQL data reader searches for matching property name.
            for (int i = 0; i < dr.FieldCount; i++)
            {
                var prop = props.FirstOrDefault(x =>
                x.Equals(dr.GetName(i).Pascalize(), StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(prop))
                    fastMember[entity, prop] = dr.IsDBNull(i) ? null : dr.GetValue(i);
            }
        }
    }
}


