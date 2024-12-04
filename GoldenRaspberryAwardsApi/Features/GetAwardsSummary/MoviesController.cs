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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProducersAwardsIntervals()
        {
            try
            {
                GetAwardsSummaryResponse awardsSummary = await _getMovieRatingUseCase.ExecuteAsync();
                return Ok(awardsSummary);
            }
            catch (Exception error)
            {
                return BadRequest("An error occurs, try again later.");
            }
        }
    }
}