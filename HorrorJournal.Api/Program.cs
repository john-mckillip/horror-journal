using HorrorJournal.Api.Endpoints;
using HorrorJournal.Api.Repositories;
using HorrorJournal.Api.Services;
using HorrorJournal.Api.Settings;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection(MongoDbSettings.SectionName));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = builder.Configuration
        .GetSection(MongoDbSettings.SectionName)
        .Get<MongoDbSettings>()!;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddScoped<IFilmRepository, FilmRepository>();
builder.Services.AddHostedService<IndexInitializer>();
builder.Services.AddHostedService<FilmChangeStreamService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapFilmEndpoints();

await app.RunAsync();
