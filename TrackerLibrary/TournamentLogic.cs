using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary
{
    public static class TournamentLogic
    {
        public static void CreateRounds(TournamentModel model)
        {
            List<TeamModel> randomizedTeams = RandomizeTeamList(model.EnteredTeams);
            int rounds = FindNumberOfRounds(model.EnteredTeams.Count());
            int byes = FindNumberOfByes(model.EnteredTeams.Count(), rounds);
            model.Rounds.Add(CreateFirstRound(randomizedTeams, byes));
            CreateOtherRounds(model, rounds);
        }

        public static void UpdateTournamentResults(TournamentModel tournament)
        {
            List<MatchupModel> toScore = new List<MatchupModel>();

            foreach (List<MatchupModel> round in tournament.Rounds)
            {
                foreach (MatchupModel m in round)
                {
                    if (m.Winner == null && (m.Entries.Any(entry => entry.Score != 0) || m.Entries.Count == 1))
                    {
                        toScore.Add(m);
                    }
                }
            }

            MarkWinnerInMatchups(toScore);

            AdvanceWinners(toScore, tournament);

            toScore.ForEach(matchup => GlobalConfig.Connection.UpdateMatchup(matchup));

        }

        private static void AdvanceWinners(List<MatchupModel> matchups, TournamentModel tournament)
        {
            foreach (MatchupModel matchup in matchups)
            {
                foreach (List<MatchupModel> round in tournament.Rounds)
                {
                    foreach (MatchupModel m in round) 
                    {
                        foreach (MatchupEntryModel me in m.Entries)
                        {
                            if (me.ParentMatchup != null && (me.ParentMatchup.Id == matchup.Id))
                            {
                                me.TeamCompeting = matchup.Winner;
                                GlobalConfig.Connection.UpdateMatchup(m);
                            }
                        }
                    }
                }
            }

        }

        private static void MarkWinnerInMatchups(List<MatchupModel> matchups)
        {
            string greaterWins = $"{ ConfigurationManager.AppSettings["greaterWins"] }";

            foreach (MatchupModel matchup in matchups)
            {
                if (matchup.Entries.Count == 1)
                {
                    matchup.Winner = matchup.Entries[0].TeamCompeting;
                    continue;
                }

                // 0 means false, or low score wins
                if (greaterWins == "0")
                {
                    if (matchup.Entries[0].Score < matchup.Entries[1].Score)
                    {
                        matchup.Winner = matchup.Entries[0].TeamCompeting;
                    }
                    else if(matchup.Entries[1].Score < matchup.Entries[0].Score)
                    {
                        matchup.Winner = matchup.Entries[1].TeamCompeting;
                    }
                    else
                    {
                        throw new Exception("We do not allow ties int this application.");
                    }
                }
                else
                {
                    // 1 means true, or high score wins
                    if (matchup.Entries[0].Score > matchup.Entries[1].Score)
                    {
                        matchup.Winner = matchup.Entries[0].TeamCompeting;
                    }
                    else if (matchup.Entries[1].Score > matchup.Entries[0].Score)
                    {
                        matchup.Winner = matchup.Entries[1].TeamCompeting;
                    }
                    else
                    {
                        throw new Exception("We do not allow ties int this application.");
                    }
                } 
            }
        }

        private static List<TeamModel> RandomizeTeamList(List<TeamModel> teams) 
        {
            return teams.OrderBy(team => Guid.NewGuid()).ToList(); 
        }


        private static void CreateOtherRounds(TournamentModel model, int rounds)
        {
            int round = 2;
            List<MatchupModel> previousRound = model.Rounds[0];
            List<MatchupModel> currentRound = new List<MatchupModel>();
            MatchupModel matchup = new MatchupModel();

            while (round <= rounds)
            {
                foreach (MatchupModel match in previousRound)
                {
                    matchup.Entries.Add(new MatchupEntryModel { ParentMatchup = match });
                    if (matchup.Entries.Count > 1)
                    {
                        matchup.MatchupRound = round;
                        currentRound.Add(matchup);
                        matchup = new MatchupModel();
                    }
                }

                model.Rounds.Add(currentRound);

                previousRound = currentRound;
                currentRound = new List<MatchupModel>();
                round++;
            }
        }

        private static List<MatchupModel> CreateFirstRound(List<TeamModel> teams, int byes)
        {
            List<MatchupModel> matchups = new List<MatchupModel>();
            MatchupModel match = new MatchupModel();

            foreach (var team in teams)
            {
                match.Entries.Add(new MatchupEntryModel { TeamCompeting = team});

                if (byes > 0 || match.Entries.Count > 1)
                {
                    match.MatchupRound = 1;
                    matchups.Add(match);
                    match = new MatchupModel();
                    if (byes > 0)
                    {
                        byes--;
                    }
                }
            }
            return matchups;
        }

        private static int FindNumberOfByes(int teamCount, int rounds)
        {
            int totalTeams = 1; 
            for (int i = 1; i <= rounds; i++)
            {
                totalTeams *= 2;
            }
            int byes = totalTeams - teamCount;
            return byes;
        }

        private static int FindNumberOfRounds(int teamCount)
        {
            int rounds = 1;
            int totalTeams = 2;  // total amount of teams in round
            while (teamCount > totalTeams)
            {
                rounds++;
                totalTeams *= 2;
            }
            return rounds;
        }

       
    }
}
