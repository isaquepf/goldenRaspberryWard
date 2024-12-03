namespace GoldenRaspberryAwardsApi.Application.Features;
public record GetMovieItemOutput(
    string Producer, 
    int Interval, 
    int PreviousWin, 
    int FollowingWin);