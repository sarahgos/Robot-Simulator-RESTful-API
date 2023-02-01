using HandlebarsDotNet;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.VisualBasic;

namespace robot_controller_api
{
    public class ScaffoldingDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection services)
        {

            services.AddHandlebarsScaffolding(options =>
            {
                options.TemplateData = new Dictionary<string, object>
                {
                    { "class-comment", "// This entity type class was automatically generated" },
                    { "lines", true }
                };
            });

            services.AddHandlebarsHelpers(("lower-case-property-name", (writer, context, parameters) =>
                writer.Write($"{context["property-name"]}".ToLower())
            ));

        }
    }
}
