namespace GoldenRaspberryAwardsApi.Application.Features;

public interface IGetMovieRatingUseCase
{
    Task<GetMovieRatingOutput> ExecuteAsync();
}