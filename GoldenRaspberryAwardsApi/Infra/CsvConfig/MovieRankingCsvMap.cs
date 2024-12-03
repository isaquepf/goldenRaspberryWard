using CsvHelper.Configuration;
using GoldenRaspberryAwardsApi.Domain;

namespace GoldenRaspberryAwardsApi.Infra.CsvConfig;

public class MovieRankingCsvMap : ClassMap<MovieRanking>
{
    public MovieRankingCsvMap()
    {
        Map(m => m.Year).Name("year");
        Map(m => m.Title).Name("title");
        Map(m => m.Studios).Name("studios");
        Map(m => m.Producers).Name("producers");
        Map(m => m.Winner).Name("winner");
    }
}