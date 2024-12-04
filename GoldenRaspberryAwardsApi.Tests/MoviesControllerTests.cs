using System.Data.Common;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GoldenRaspberryAwards.Tests
{
    public class MoviesControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        [Fact]
        public async Task GetIntervals_ReturnsOk_WhenDataExists()
        {
            await using var application = new ApplicationServices();

            using var client = application.CreateClient();

            client.BaseAddress = new Uri("https://localhost:7077");
            
            //Act :
            var response = await client.GetAsync("/movies/awards-winners-intervals");

            // Assert:
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}

internal class ApplicationServices : WebApplicationFactory<Program>
{
    private readonly string _environment;

    public ApplicationServices(string environment = "Development")
    {
        _environment = environment;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(_environment);

        // Add mock/test services to the builder here
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<DbConnection>(container =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();
                return connection;
            });
        });

        return base.CreateHost(builder);
    }
}