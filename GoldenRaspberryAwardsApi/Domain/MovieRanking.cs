using System.ComponentModel.DataAnnotations.Schema;

namespace GoldenRaspberryAwardsApi.Domain;

public class MovieRanking
{
    public MovieRanking()
    {
        
    }
    public int Id { get; set; }
    public int Year { get; set; }
    public string Title { get; set; }
    public string Studios { get; set; } 
    public string Producers { get; set; } 
    public string Winner { get; set; }


    public bool IsWinner => Winner == "yes";

    
}


