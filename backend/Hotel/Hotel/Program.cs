using Hotel.API.Extensions;
using Hotel.Application.Services;
using Hotel.DataAccess.DBAdapter;
using Hotel.DataAccess.Repositories;
using Hotel.DataAccess.Security;
using Hotel.Domain.Abstractions.Repositories;
using Hotel.Domain.Abstractions.Security;
using Hotel.Domain.Abstractions.Services;
using Hotel.Domain.Abstractions.Services.AdministratorServices;
using Hotel.Domain.Abstractions.Services.ClientServices;
using Hotel.Domain.Abstractions.Services.ManagerServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
DotNetEnv.Env.Load();
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Configuration.AddEnvironmentVariables();
var dbPassword = builder.Configuration["DB_PASSWORD"];

var jwtSecretKey = builder.Configuration["JWT_SECRET"];
builder.Services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
builder.Services.PostConfigure<JwtOptions>(options =>
{

    options.SecretKey = jwtSecretKey;

});
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<HotelDbContext>(options =>
{
    options.UseNpgsql($"{configuration.GetConnectionString(nameof(HotelDbContext))}Password={dbPassword};");
});
builder.Services.AddScoped<IBookingRepository,BookingRepository>();
builder.Services.AddScoped<IClientRepository,ClientRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IRentRepository, RentRepository>();
builder.Services.AddScoped<IRoomRepository,RoomRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISystemInitializer, SystemInitializeService>();
builder.Services.AddScoped<IAuthenticationClientService, AuthenticationService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IClientBookingService,BookingService>();
builder.Services.AddScoped<IManagerBookingService,BookingService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IManagerClientService, ClientService>();
builder.Services.AddScoped<IEmployeeService,EmployeeService>();
builder.Services.AddScoped<IClientRentHistoryService,RentHistoryService>();
builder.Services.AddScoped<IManagerRentService,RentService>();
builder.Services.AddScoped<IAdminRoomService,RoomService>();
builder.Services.AddScoped<IClientRoomService, RoomService>();
builder.Services.AddScoped<IManagerRoomService, RoomService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IAdminImageService, ImageService>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddJwtAuthentication(configuration,jwtSecretKey);
builder.Services.AddControllers();
var app = builder.Build();
using(var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<ISystemInitializer>();
    if(!await initializer.IsSystemNeedInitializeAsync()) await initializer.InitializeSystemAsync(); 
   
}
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors(policy => policy.AllowAnyOrigin()           
.WithOrigins("http://localhost:5173")
.AllowAnyMethod()
.AllowAnyHeader()
.AllowCredentials());
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
