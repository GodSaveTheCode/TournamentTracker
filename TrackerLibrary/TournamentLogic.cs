using System;
using System.Collections.Generic;
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
