using GoldenRaspberryAwardsApi.Application.Features;
using GoldenRaspberryAwardsApi.Infra;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IGetMovieRatingUseCase, GetMovieRatingUseCase>();


var connectionString = builder.Configuration["ConnectionStrings:MoviesConnection"];

builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Create database.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MovieDbContext>();
    dbContext.Database.EnsureCreated();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();


public partial class Program { }