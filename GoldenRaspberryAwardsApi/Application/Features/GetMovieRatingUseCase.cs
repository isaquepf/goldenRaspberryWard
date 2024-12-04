using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using GoldenRaspberryAwardsApi.Domain;
using GoldenRaspberryAwardsApi.Infra;
using GoldenRaspberryAwardsApi.Infra.CsvConfig;
using Microsoft.EntityFrameworkCore;

namespace GoldenRaspberryAwardsApi.Application.Features;

public class GetMovieRatingUseCase(MovieDbContext context) : IGetMovieRatingUseCase
{
    private readonly MovieDbContext _context = context;

    public async Task<GetMovieRatingOutput> ExecuteAsync()
    {
        await EnsureMoviesLoadedAsync();
        var movieRankings = await _context.Movies.ToListAsync();
        var intervals = CalculateIntervals(movieRankings.Where(p => p.IsWinner));
        return BuildOutput(intervals);
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
        using var reader = new StreamReader(@".\movielist.csv");
        using var csv = new CsvReader(reader,
            new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", Encoding = Encoding.UTF8 });
        csv.Context.RegisterClassMap<MovieRankingCsvMap>();
        return await Task.Run(() => csv.GetRecords<MovieRanking>().ToList());
    }

    private List<GetMovieItemOutput> CalculateIntervals(IEnumerable<MovieRanking> winningMovies)
    {
        var producers = winningMovies
            .SelectMany(m => m.Producers.Split(", ").Select(p => new { m.Year, Producer = p }))
            .GroupBy(p => p.Producer)
            .Where(g => g.Count() > 1)
            .Select(g => new { Producer = g.Key, Years = g.OrderBy(p => p.Year).Select(p => p.Year).ToList() })
            .ToList();
        
        var intervals = new List<GetMovieItemOutput>();

        foreach (var producer in producers)
        {
            for (var i = 0; i < producer.Years.Count - 1; i++)
            {
                intervals.Add(new GetMovieItemOutput(
                    Producer: producer.Producer,
                    Interval: producer.Years[i + 1] - producer.Years[i],
                    PreviousWin: producer.Years[i],
                    FollowingWin: producer.Years[i + 1]));
            }
        }

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