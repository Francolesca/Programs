using Starbucks.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddPersistence(builder.Configuration);

var app = builder.Build();

app.MapControllers();

app.Run();
