using Starbucks.Api.Extensions;
using Starbucks.Persistence;
using Starbucks.Application;
using Core.Mappy.Interfaces;
using Core.Mappy.Extensions;
using Core.Mappy;
using Starbucks.Domain;
using Starbucks.Application.Categories.DTOs;
using Starbucks.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IMapper, Mapper>();


var app = builder.Build();

var mapper = app.Services.GetRequiredService<IMapper>();
mapper.RegisterMappings(typeof(CategoryMappingProfile).Assembly);

await app.ApplyMigration(environment);

app.MapControllers();
app.MapOpenApi();

app.UseSwaggerUI(option =>
{
    option.SwaggerEndpoint("/openapi/v1.json", "Documentacion de Starbucks");
});

app.UseMiddleware<ExceptionHandlingMidlleware>();
app.Run();
