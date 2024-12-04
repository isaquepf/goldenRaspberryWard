using System.Net;
using GoldenRaspberryAwardsApi.Infra;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Web.CodeGeneration.Design;

namespace GoldenRaspberryAwards.Tests
{
     public class MoviesControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        
        public MoviesControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove o DbContext original e adiciona um em memória
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<MovieDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<MovieDbContext>(options =>
                    {
                        options.UseSqlite("./Sample.db");
                    });
                });
            }).CreateClient();
        }
        
        [Fact]
        public async Task GetIntervals_ReturnsOk_WhenDataExists()
        {
            //Act :
            var response = await _client.GetAsync("/api/movies/intervals");

            // Assert:
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}