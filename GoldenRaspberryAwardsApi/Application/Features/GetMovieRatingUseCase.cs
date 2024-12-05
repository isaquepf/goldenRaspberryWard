using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using GoldenRaspberryAwardsApi.Domain;
using GoldenRaspberryAwardsApi.Infra;
using GoldenRaspberryAwardsApi.Infra.CsvConfig;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace GoldenRaspberryAwardsApi.Application.Features;

public class GetMovieRatingUseCase(
    MovieDbContext context,
    IWebHostEnvironment hostEnv,
    ILogger<GetMovieRatingUseCase> logger) : IGetMovieRatingUseCase
{
    private readonly MovieDbContext _context = context;

    public async Task<GetMovieRatingOutput> ExecuteAsync()
    {
        try
        {
            await EnsureMoviesLoadedAsync();
            var movieRankings = await _context.Movies.ToListAsync();
            var intervals = CalculateIntervals(movieRankings.Where(p => p.IsWinner));
            return BuildOutput(intervals);
        }
        catch (Exception error)
        {
            logger.LogError(error, "An error occurs at GetMovieRatingUseCase message: {error.Message}", error.Message);
            throw;
        }
    }

    private async Task EnsureMoviesLoadedAsync()
    {
        if (!await _context.Movies.AnyAsync())
        {
            var movieRankings = await LoadCsvMoviesAsync();
            await _context.Movies.AddRangeAsync(movieRankings);
            await _context.SaveChangesAsync();
        }
    }

    private async Task<IEnumerable<MovieRanking>> LoadCsvMoviesAsync()
    {
        var path = Path.Combine(hostEnv.ContentRootPath, "Data", "movielist.csv");

        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader,
            new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", Encoding = Encoding.UTF8 });
        csv.Context.RegisterClassMap<MovieRankingCsvMap>();
        return await Task.Run(() => csv.GetRecords<MovieRanking>().ToList());
    }

    private List<GetMovieItemOutput> CalculateIntervals(IEnumerable<MovieRanking> winningMovies)
    {
        var intervals = new List<GetMovieItemOutput>();
        var producers = winningMovies.Where(m => m.Winner == "yes")
            .SelectMany(m => m.Producers.Split(", ").Select(p => new { m.Year, Producer = p })).GroupBy(p => p.Producer)
            .Select(g => new { g.Key, Years = g.Select(x => x.Year).OrderBy(y => y).ToList() }).SelectMany(g =>
                g.Years.Zip(g.Years.Skip(1),
                    (prevYear, nextYear) => new GetMovieItemOutput(
                        Producer: g.Key,
                        Interval: nextYear - prevYear,
                        PreviousWin: prevYear,
                        FollowingWin: nextYear)));

        
        
        var minInterval = producers.OrderBy(p => p.Interval).FirstOrDefault();
        var maxInterval = producers.OrderByDescending(p => p.Interval).FirstOrDefault();
        
        if (minInterval != null) intervals.Add(minInterval);
        if (maxInterval != null) intervals.Add(maxInterval);
        return intervals;   
    }

    private GetMovieRatingOutput BuildOutput(List<GetMovieItemOutput> intervals)
    {
        var maxIntervalValue = intervals.Max(x => x.Interval);
        var minIntervalValue = intervals.Min(x => x.Interval);
        var maxIntervals = intervals.Where(i => i.Interval == maxIntervalValue).ToList();
        var minIntervals = intervals.Where(i => i.Interval == minIntervalValue).ToList();
        return new GetMovieRatingOutput(Min: minIntervals.AsReadOnly(), Max: maxIntervals.AsReadOnly());
    }
}