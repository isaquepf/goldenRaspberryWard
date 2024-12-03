using GoldenRaspberryAwardsApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace GoldenRaspberryAwardsApi.Infra;
public class MovieDbContext(DbContextOptions<MovieDbContext> options) : DbContext(options)
{
    public DbSet<MovieRanking> Movies { get; set; }
}




