using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using GoldenRaspberryAwardsApi.Domain;
using GoldenRaspberryAwardsApi.Infra;
using GoldenRaspberryAwardsApi.Infra.CsvConfig;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using MoreLinq.Extensions;

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

            var movies = movieRankings.Where(p => p.IsWinner);

            var intervals = CalculateIntervals(movies);

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
        var producers = winningMovies
            .SelectMany(m =>
                m.Producers.Split([", ", " and "], StringSplitOptions.None).Select(p => new { m.Year, Producer = p }))
            .GroupBy(p => p.Producer)
            .Select(g => new
            {
                Producer = g.Key.Trim(),
                Years = g.Select(x => x.Year).OrderBy(y => y).ToList()
            }).Distinct();

        var intervals = new List<GetMovieItemOutput>();
        
        foreach (var producer in producers)
        {
            for (int i = 1; i < producer.Years.Count; i++)
            {
                var previousWin = producer.Years[i - 1];
                var followingWin = producer.Years[i];
                var interval = new GetMovieItemOutput(
                    Producer: producer.Producer,
                    Interval: followingWin - previousWin,
                    PreviousWin: previousWin,
                    FollowingWin: followingWin
                );
                intervals.Add(interval);
            }
        }
        
        var minInterval = intervals.OrderBy(p => p.Interval).FirstOrDefault();
        var maxInterval = intervals.OrderByDescending(p => p.Interval).FirstOrDefault();

        var result = new List<GetMovieItemOutput>();
        
        if (minInterval != null) 
            result.Add(minInterval);
        
        if (maxInterval != null && maxInterval != minInterval) 
            result.Add(maxInterval);

        return result;
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