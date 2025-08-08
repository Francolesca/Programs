using Starbucks.Api.Extensions;
using Starbucks.Persistence;
using Starbucks.Application;
using Core.Mappy.Interfaces;
using Core.Mappy.Extensions;
using Core.Mappy;
using Starbucks.Domain;
using Starbucks.Application.Categories.DTOs;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddSingleton<IMapper, Mapper>();


var app = builder.Build();

var mapper = app.Services.GetRequiredService<IMapper>();
mapper.RegisterMappings(typeof(CategoryMappingProfile).Assembly);

await app.ApplyMigration(environment);

app.MapControllers();

app.Run();
