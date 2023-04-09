using System;
using System.IO;

class Program {
    static void Main(string[] args) {

        string setupFile = "setup.csv";
        string setupFileContent = File.ReadAllText(setupFile);

        string leagueName = setupFileContent.Split(',')[0];
        int positionsToChampionsLeague = int.Parse(setupFileContent.Split(',')[1]);
        int positionsToEuropeLeague = int.Parse(setupFileContent.Split(',')[2]);
        int positionsToConferenceLeague = int.Parse(setupFileContent.Split(',')[3]);
        int positionsToUpperLeague = int.Parse(setupFileContent.Split(',')[4]);
        int positionsToLowerLeague = int.Parse(setupFileContent.Split(',')[5]);

        Console.WriteLine("League Name: {0}", leagueName);
        Console.WriteLine("Positions to Champions League: {0}", positionsToChampionsLeague);
        Console.WriteLine("Positions to Europe League: {0}", positionsToEuropeLeague);
        Console.WriteLine("Positions to Conference League: {0}", positionsToConferenceLeague);
        Console.WriteLine("Positions to Upper League: {0}", positionsToUpperLeague);
        Console.WriteLine("Positions to Lower League: {0}", positionsToLowerLeague);

        string teamsFile = "teams.csv";
        List<Team> teams = new List<Team>();

        using (StreamReader reader = new StreamReader(teamsFile)) {
            while (!reader.EndOfStream) {
                string? line = reader.ReadLine();
                string[] fields = line?.Split(',') ?? new string[0];
                Team team = new Team(fields[0], fields[1], fields[2]);
                teams.Add(team);
            }
        }

        int numberOfRounds = 32;
        for (int i = 1; i <= numberOfRounds; i++)
        {
        string roundFilePath = Path.Combine("rounds", $"round{i}.csv");
        string[] roundLines = File.ReadAllLines(roundFilePath);


            foreach (string line in roundLines)
            {
                string[] values = line.Split(',');
                
                string homeTeamAbbriviation = values[0];
                int homeTeamScore = int.Parse(values[2]);
                string awayTeamAbbreviation = values[1];
                int awayTeamScore = int.Parse(values[3]);

                Team? homeTeam = teams.Find(t => t.Abbreviation == homeTeamAbbriviation);
                Team? awayTeam = teams.Find(t => t.Abbreviation == awayTeamAbbreviation);

                if(awayTeam != null && homeTeam != null)
                    {
                    homeTeam.GamesPlayed++;
                    homeTeam.GoalsFor += homeTeamScore;
                    homeTeam.GoalsAgainst += awayTeamScore;
                    
                    awayTeam.GamesPlayed++;
                    awayTeam.GoalsFor += awayTeamScore;
                    awayTeam.GoalsAgainst += homeTeamScore;
                    
                    if (homeTeamScore > awayTeamScore)
                    {
                        homeTeam.GamesWon++;
                        homeTeam.Points += 3;
                        homeTeam.AddToStreak("W");

                        awayTeam.GamesLost++;
                        awayTeam.AddToStreak("L");
                    }
                    else if (homeTeamScore == awayTeamScore)
                    {
                        homeTeam.GamesDrawn++;
                        homeTeam.Points++;
                        homeTeam.AddToStreak("D");

                        awayTeam.GamesDrawn++;
                        awayTeam.Points++;
                        awayTeam.AddToStreak("D");
                    }
                    else
                    {
                        homeTeam.GamesLost++;
                        homeTeam.AddToStreak("L");

                        awayTeam.GamesWon++;
                        awayTeam.Points += 3;
                        awayTeam.AddToStreak("W");
                    }
                }
            }
        }


        var standings = teams.OrderByDescending(t => t.Points)
                    .ThenByDescending(t => t.GoalDifference)
                    .ThenByDescending(t => t.GoalsFor)
                    .ThenBy(t => t.GoalsAgainst)
                    .ThenBy(t => t.Name)
                    .ToList();
                    
        string streak = "-";

        Console.WriteLine("{0,-4}{1,-5}{2,-25}{3,-4}{4,-4}{5,-4}{6,-4}{7,-4}{8,-4}{9,-4}{10,-10}{11,-4}",
                    "Pos", "Rank", "Team", "Pld", "W", "D", "L", "GF", "GA", "GD", "Pts", "Streak");

        for (int i = 0; i < standings.Count; i++)
        {
            Team team = standings[i];
            int pos = i + 1;
            
            if (i > 0 && team.Points == standings[i - 1].Points && 
                team.GoalDifference == standings[i - 1].GoalDifference && 
                team.GoalsFor == standings[i - 1].GoalsFor)
            {
                pos = -1;
            }
            
            string specialMark = "";
            switch (team.Ranking)
            {
                case "W":
                    specialMark = "(W)";
                    break;
                case "C":
                    specialMark = "(C)";
                    break;
                case "P":
                    specialMark = "(P)";
                    break;
                case "R":
                    specialMark = "(R)";
                    break;
            }

            if(team.Streak.Count >4){
                List<string> lastFive = team.Streak.GetRange(team.Streak.Count - 5, 5);
                bool allSame = lastFive.All(x => x == lastFive[0]);
                if(allSame){
                    streak = lastFive[0];
                }
            }
            
            Console.WriteLine("{0,-4}{1,-5}{2,-25}{3,-4}{4,-4}{5,-4}{6,-4}{7,-4}{8,-4}{9,-4}{10,-10}{11,-4}",
                            pos, specialMark, team.Name, team.GamesPlayed, 
                            team.GamesWon, team.GamesDrawn, team.GamesLost, 
                            team.GoalsFor, team.GoalsAgainst, team.GoalDifference, 
                            team.Points, streak);
                            
            streak = "-";
        }
    }
}

