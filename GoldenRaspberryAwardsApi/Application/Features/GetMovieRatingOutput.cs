namespace GoldenRaspberryAwardsApi.Application.Features;

public record GetMovieRatingOutput(IReadOnlyList<GetMovieItemOutput> Min, IReadOnlyList<GetMovieItemOutput> Max);