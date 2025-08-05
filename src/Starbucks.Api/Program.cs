using Starbucks.Api.Extensions;
using Starbucks.Persistence;
using Starbucks.Application;
var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

await app.ApplyMigration(environment);

app.MapControllers();

app.Run();
