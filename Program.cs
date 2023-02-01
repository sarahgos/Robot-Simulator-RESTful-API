using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using robot_controller_api.Authentication;
using robot_controller_api.Persistence;
using System.Reflection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Robot Controller API",
        Description = "New backend service that provides resources for the Moon robot simulator.",
        Contact = new OpenApiContact
        {
            Name = "Sarah Gosling",
            Email = "sgosling@deakin.edu.au"
        },
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions,
    BasicAuthenticationHandler>("BasicAuthentication", default);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Admin", "User"));
    options.AddPolicy("HeadAdminOnly", policy => policy.RequireClaim(ClaimTypes.Email, "sarah@gmail.com"));
    options.AddPolicy("CEOOnly", policy => policy.RequireClaim("name", "Sarah Gosling"));
});

builder.Services.AddScoped<IRobotCommandDataAccess, RobotCommandEF>();
builder.Services.AddScoped<IMapDataAccess, MapEF>();
builder.Services.AddScoped<IUserDataAccess, UserEF>();
//builder.Services.AddScoped<IRobotCommandDataAccess, RobotCommandRepository>();
//builder.Services.AddScoped<IMapDataAccess, MapRepository>();
//builder.Services.AddScoped<IRobotCommandDataAccess, RobotCommandADO>();
//builder.Services.AddScoped<IMapDataAccess, MapADO>();



var app = builder.Build();

app.UseStaticFiles();

app.UseSwagger();

app.UseSwaggerUI(setup => setup.InjectStylesheet("/styles/theme-deakin.css"));

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
