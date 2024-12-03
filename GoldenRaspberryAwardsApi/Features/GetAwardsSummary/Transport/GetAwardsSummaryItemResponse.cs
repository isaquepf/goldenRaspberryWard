namespace GoldenRaspberryAwardsApi.Features.GetWorstMovie.Transport;
public record GetAwardsSummaryItemResponse(
    string Producer, 
    int Interval, 
    int PreviousWin, 
    int FollowingWin);