using GoldenRaspberryAwardsApi.Application.Features;
using GoldenRaspberryAwardsApi.Features.GetWorstMovie.Transport;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoldenRaspberryAwardsApi.Features.GetAwardsSummary
{
    [Route("api/[controller]/v1")]
    [ApiController()]
    public class MoviesController(IGetMovieRatingUseCase getMovieRatingUseCase) : ControllerBase
    {
        private readonly IGetMovieRatingUseCase _getMovieRatingUseCase = getMovieRatingUseCase;
        
        [HttpGet("/movies/awards-winners-intervals")]
        public async Task<IActionResult> GetProducersAwardsIntervals()
        {
            GetAwardsSummaryResponse awardsSummary = await _getMovieRatingUseCase.ExecuteAsync();
            return Ok(awardsSummary);
        }
    }
}