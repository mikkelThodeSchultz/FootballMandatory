using System;
using System.IO;

class Program {
    static void Main(string[] args) {

        string setupFile = "setup.csv";
        string setupFileContent = File.ReadAllText(setupFile);
        if (setupFileContent.Split(',').Length != 6){
            throw new ArgumentException("Invalid input file format: The setup.csv file should have 6 rows.");
        }
        string[] splitSetupFileContent = setupFileContent.Split(',');
        string leagueName = splitSetupFileContent[0];

        if (int.TryParse(splitSetupFileContent[1], out int var1) && int.TryParse(splitSetupFileContent[2], out int var2) && int.TryParse(splitSetupFileContent[3], out int var3)
            && int.TryParse(splitSetupFileContent[4], out int var4) && int.TryParse(splitSetupFileContent[5], out int var5)){
            int positionsToChampionsLeague = var1;
            int positionsToEuropeLeague = var2;
            int positionsToConferenceLeague = var3;
            int positionsToUpperLeague = var4;
            int positionsToLowerLeague = var5;

            Console.WriteLine("League Name: {0}", leagueName);
            Console.WriteLine("Positions to Champions League: {0}", positionsToChampionsLeague);
            Console.WriteLine("Positions to Europe League: {0}", positionsToEuropeLeague);
            Console.WriteLine("Positions to Conference League: {0}", positionsToConferenceLeague);
            Console.WriteLine("Positions to Upper League: {0}", positionsToUpperLeague);
            Console.WriteLine("Positions to Lower League: {0}", positionsToLowerLeague);

        } else {
            throw new FormatException("The setup.csv file is not valid");
        }

        string teamsFile = "teams.csv";
        List<Team> teams = new List<Team>();

        using (StreamReader reader = new StreamReader(teamsFile)) {
            while (!reader.EndOfStream) {
                string? line = reader.ReadLine();
                string[] fields = line?.Split(',') ?? new string[0];
                if (fields.Length != 3) {
                    throw new ArgumentException("Invalid input file format: Each team should have 3 rows");
                }
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
                if (values.Length != 4) {
                    throw new ArgumentException("Invalid input file format: Each round should have 4 rows. Exception thrown in round " + i);
                }
                
                string homeTeamAbbriviation = values[0];
                string awayTeamAbbreviation = values[1];
                int homeTeamScore = -1;
                int awayTeamScore = -1;
                if (int.TryParse(values[2], out int homeTeamValue)){
                    homeTeamScore = homeTeamValue;
                } else {
                    throw new FormatException("The home team score is not valid in round: " +  i +  ". " + line);
                }
                if (int.TryParse(values[3], out int awayTeamValue)){
                awayTeamScore = awayTeamValue;
                } else {
                    throw new FormatException("The away team score is not valid in round: " +  i +  ". " + line);
                }

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

            if(team.Streak.Count > 4){
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

