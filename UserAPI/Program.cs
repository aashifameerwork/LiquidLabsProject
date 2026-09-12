using DbLayer.Interfaces;
using DbLayer.Repositories;
using ServiceLayer.Interfaces;
using ServiceLayer.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();


var connectionString = builder.Configuration.GetConnectionString("AashifConn");

if(string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("DB Connection string is not found.");
}

builder.Services.AddScoped<IUserRepository>(provider => new UserRepository(connectionString));

builder.Services.AddHttpClient<IUserService, UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

Console.WriteLine("=====================================");
Console.WriteLine("Liquid Labs User API - Aashif Ameer");
Console.WriteLine("=====================================");
Console.WriteLine("End Point Information:\n");
Console.WriteLine("To get all users     : [GET] {url}/api/users");
Console.WriteLine("To get a user by ID  : [GET] {url}/api/users/{id}");
Console.WriteLine("-------------------------------------");

app.Run();
