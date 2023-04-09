public class Team
{
    public string Abbreviation { get; set; }
    public string Name { get; set; }
    public string Ranking { get; set; }
    public int GamesPlayed { get; set; }
    public int GamesWon { get; set; }
    public int GamesDrawn { get; set; }
    public int GamesLost { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int GoalDifference { get => GoalsFor - GoalsAgainst;}
    public int Points { get; set; }
    public List<string> Streak { get; set; }

    public Team(string abbreviation, string name, string ranking)
    {
        Abbreviation = abbreviation;
        Name = name;
        Ranking = ranking;
        GamesPlayed = 0;
        GamesWon = 0;
        GamesDrawn = 0;
        GamesLost = 0;
        GoalsFor = 0;
        GoalsAgainst = 0;
        Points = 0;
        Streak = new List<string>();
    }

    public void AddToStreak(string result){
        Streak.Add(result);
    }
    
    public override string ToString()
    {
    return $"{Abbreviation}: {Name} ({Ranking})";
    }

}