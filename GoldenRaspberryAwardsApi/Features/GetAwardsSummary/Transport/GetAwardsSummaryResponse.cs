using GoldenRaspberryAwardsApi.Application.Features;
using System.Linq;
namespace GoldenRaspberryAwardsApi.Features.GetWorstMovie.Transport;

public record GetAwardsSummaryResponse(
    IReadOnlyList<GetAwardsSummaryItemResponse> Min,
    IReadOnlyList<GetAwardsSummaryItemResponse> Max)
{
    public static implicit operator GetAwardsSummaryResponse(GetMovieRatingOutput output)
    {
        if (output == null)
            return new GetAwardsSummaryResponse(Min: [], Max: []);
        
        var min = output.Max.Select(p => new GetAwardsSummaryItemResponse(
            Producer: p.Producer,
            Interval: p.Interval,
            PreviousWin: p.PreviousWin,
            FollowingWin: p.FollowingWin
        ));
        

        var max = output.Max.Select(p => new GetAwardsSummaryItemResponse(
            Producer: p.Producer,
            Interval: p.Interval,
            PreviousWin: p.PreviousWin,
            FollowingWin: p.FollowingWin
        ));
        
        return  new GetAwardsSummaryResponse(Min: [..min], Max: [..max]);
    }
};