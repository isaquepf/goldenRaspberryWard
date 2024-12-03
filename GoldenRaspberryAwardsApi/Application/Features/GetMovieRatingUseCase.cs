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
        IEnumerable<MovieRanking> movieRankings = null;
        var movies = await _context.Movies.CountAsync();

        if (movies == 0)
        {
            movieRankings = GetMovieRankings();
            _context.Movies.AddRange(movieRankings);
            _context.SaveChanges();
        }

        movieRankings = _context.Movies.ToList();

        var winningMovies = movieRankings.Where(p => p.IsWinner);
        
        var producers = winningMovies.SelectMany(m => m.Producers.Split(", ").Select(p => new { m.Year, Producer = p }))
            .GroupBy(p => p.Producer).Where(g => g.Count() > 1).Select(g =>
                new { Producer = g.Key, Years = g.OrderBy(p => p.Year).Select(p => p.Year).ToList() }).ToList();
        var intervals = new List<GetMovieItemOutput>();
        
        foreach (var producer in producers)
        {
            for (var i = 0; i < producer.Years.Count - 1; i++)
            {
                intervals.Add(new GetMovieItemOutput(producer.Producer, producer.Years[i + 1] - producer.Years[i],
                    producer.Years[i], producer.Years[i + 1]));
            }
        }

        var maxIntervalValue = intervals.Max(x => x.Interval);
        var minIntervalValue = intervals.Min(x => x.Interval);
        var max = intervals.Where(i => i.Interval == maxIntervalValue).ToList();
        var min = intervals.Where(i => i.Interval == minIntervalValue).ToList();

        return new GetMovieRatingOutput(Min: [..min], Max: [..max]);
    }

    private static IEnumerable<MovieRanking> GetMovieRankings()
    {
        using var reader = new StreamReader(@".\movielist.csv");
        using (var csv = new CsvReader(reader,
                   new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", Encoding = Encoding.UTF8 }))
        {
            csv.Context.RegisterClassMap<MovieRankingCsvMap>();
            var movieRankings = csv.GetRecords<MovieRanking>().ToList();
            return movieRankings;
        }
    }
}