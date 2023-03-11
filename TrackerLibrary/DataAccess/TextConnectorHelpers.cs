using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess.Helpers
{
    public static class TextConnectorHelpers
    {
        public static string GetFullPath(this string fileName)
        {
            return $"{ ConfigurationManager.AppSettings["filePath"] }\\{ fileName }";
        }

        public static List<string> LoadFile(this string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new List<string>();
            }
           return File.ReadAllLines(filePath).ToList();
        }

        public static List<PersonModel> ConvertToPersonModels(this List<string> lines) 
        {
            List<PersonModel> people = new List<PersonModel>();
            PersonModel person;
            string[] values;
            foreach (string line in lines)
            {
                values = line.Split(',');
                person = new PersonModel();
                person.Id = int.Parse(values[0]);
                person.FirstName = values[1];
                person.LastName = values[2];
                person.EmailAddress = values[3];
                person.CellphoneNumber = values[4];

                people.Add(person);
            }

            return people;
        }

        public static List<PrizeModel> ConvertToPrizeModels(this List<string> lines)
        {
            List<PrizeModel> prizes = new List<PrizeModel>();
            PrizeModel prize;
            string[] values;
            foreach (string line in lines)
            {
                values = line.Split(',');
                prize = new PrizeModel();
                prize.Id = int.Parse(values[0]);
                prize.PlaceNumber = int.Parse(values[1]);
                prize.PlaceName = values[2];
                prize.PrizeAmount = decimal.Parse(values[3]);
                prize.PrizePercentage = double.Parse(values[4]);

                prizes.Add(prize);
            }

            return prizes;
        }

        public static List<TeamModel> ConvertToTeamModels(this List<string> lines)
        {
            List<TeamModel> teams = new List<TeamModel>();
            List<PersonModel> people = GlobalConfig.PeopleFile.GetFullPath().LoadFile().ConvertToPersonModels();
            TeamModel team;
            string[] values;
            foreach (var line in lines)
            {
                values = line.Split(',');
                team = new TeamModel();
                team.Id = int.Parse(values[0]);
                team.TeamName = values[1];
                string[] ids = values[2].Split('|');
                foreach (var id in ids)
                {
                    team.TeamMembers.Add(people.Where(person => person.Id == int.Parse(id)).First());
                }
                teams.Add(team);
            }
            return teams;
        }

        public static List<TournamentModel> ConvertToTournamentModels(this List<string> lines)
        {
            List<TournamentModel> tournaments = new List<TournamentModel>();
            List<TeamModel> teams = GlobalConfig.TeamsFile.GetFullPath().LoadFile().ConvertToTeamModels();
            List<PrizeModel> prizes = GlobalConfig.PrizesFile.GetFullPath().LoadFile().ConvertToPrizeModels();
            List<MatchupModel> matchups = GlobalConfig.MatchupsFile.GetFullPath().LoadFile().ConvertToMatchupModels();

            TournamentModel tournament;
            // id,TournamentName,EntryFee,id|id|id,id|id|id,id^id^id|id^id^id|id^id^id
            string[] values;
            foreach (var line in lines)
            {
                tournament = new TournamentModel();
                values = line.Split(',');
                tournament.Id = int.Parse(values[0]);
                tournament.TournamentName = values[1];
                tournament.EntryFee = decimal.Parse(values[2]);
                string[] teamIds = values[3].Split('|');
                foreach (var id in teamIds)
                {
                    tournament.EnteredTeams.Add(teams.Where(team => team.Id == int.Parse(id)).First());
                }

                if (values[4].Length > 0)
                {
                    string[] prizeIds = values[4].Split('|');
                    foreach (var id in prizeIds)
                    {
                        tournament.Prizes.Add(prizes.Where(prize => prize.Id == int.Parse(id)).First());
                    } 
                }

               
                string[] rounds = values[5].Split('|');  
                foreach (var round in rounds)
                {
                    List<MatchupModel> matches = new List<MatchupModel>();
                    string[] matchupsIds = round.Split('^');
                    foreach (string id in matchupsIds)
                    {
                        MatchupModel matchup = matchups.Where(m => m.Id == int.Parse(id)).First();
                        matches.Add(matchup);
                    }
                    tournament.Rounds.Add(matches);
                }

                
                tournaments.Add(tournament);
            }

            return tournaments;
        }

        public static List<MatchupEntryModel> ConvertToMatchupEntryModels(this List<string> lines)
        {
            // matchupEntries   id, id(team_competing), score, id(parent matchup)
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();
            MatchupEntryModel matchupEntry;
            string[] values;
            foreach (string line in lines)
            {
                values = line.Split(',');
                matchupEntry = new MatchupEntryModel();
                matchupEntry.Id = int.Parse(values[0]);
                if (int.TryParse(values[1], out int teamId))
                {
                    matchupEntry.TeamCompeting = LookupTeamById(teamId);
                }
                else
                {
                    matchupEntry.TeamCompeting = null;
                }
                matchupEntry.Score = double.Parse(values[2]);
                if (int.TryParse(values[3], out int parentId))
                {
                    matchupEntry.ParentMatchup = LookupMatchupById(parentId);
                }
                else
                {
                    matchupEntry.ParentMatchup = null;
                }
                output.Add(matchupEntry);
            }
            return output;
        }

        public static List<MatchupModel> ConvertToMatchupModels(this List<string> lines)
        {
            // matchups    id, id|id(entries ids), id(winner), matchupround

            List<MatchupModel> matchups = new List<MatchupModel>();
           // List<MatchupEntryModel> matchupEntries = GlobalConfig.MatchupEntriesFile.GetFullPath().LoadFile().ConvertToMatchupEntryModels();
            MatchupModel matchup;
            string[] values;
            foreach (string line in lines)
            {
                values = line.Split(',');
                matchup = new MatchupModel();
                matchup.Id = int.Parse(values[0]);
                matchup.Entries = ConvertStringToMatchupEntryModels(values[1]);
                if (int.TryParse(values[2], out int winnerId))
                {
                    matchup.Winner = LookupTeamById(winnerId);
                }
                else
                {
                    matchup.Winner = null;
                }
                matchup.MatchupRound = int.Parse(values[3]);
                matchups.Add(matchup);
            }
            return matchups;

        }

        public static TeamModel LookupTeamById(int id)
        {
            List<string> teams = GlobalConfig.TeamsFile.GetFullPath().LoadFile();
            foreach (string team in teams)
            {
                string[] values = team.Split(',');
                if (values[0] == id.ToString())
                {
                    List<string> matchingTeams = new List<string>();
                    matchingTeams.Add(team);
                    return matchingTeams.ConvertToTeamModels().First();
                }

            }

            return null;
        }

        public static MatchupModel LookupMatchupById(int id)
        {
            List<string> matchups = GlobalConfig.MatchupsFile.GetFullPath().LoadFile();
            foreach (string matchup in matchups)
            {
                string[] values = matchup.Split(',');
                if (values[0] == id.ToString())
                {
                    List<string> matchingMatchups = new List<string>();
                    matchingMatchups.Add(matchup);
                    return matchingMatchups.ConvertToMatchupModels().First();
                }
            }
            return null;
        }

        public static List<MatchupEntryModel> ConvertStringToMatchupEntryModels(string input)
        {
            List<string> matchingEntries = new List<string>();
            List<string> entries = GlobalConfig.MatchupEntriesFile.GetFullPath().LoadFile();
            string[] ids = input.Split('|');
            foreach (string id in ids)
            {
                matchingEntries.Add(entries.Where(s => s.Split(',')[0] == id).First());
            }

            return matchingEntries.ConvertToMatchupEntryModels();
        }

        public static void SaveToPrizesFile(this List<PrizeModel> prizes)
        {
            List<string> lines = new List<string>();
            foreach (var prize in prizes)
            {
                lines.Add($"{prize.Id},{prize.PlaceNumber},{prize.PlaceName},{prize.PrizeAmount},{prize.PrizePercentage}");
            }

            File.WriteAllLines(GlobalConfig.PrizesFile.GetFullPath(), lines);
        }

        public static void SaveToPeopleFile(this List<PersonModel> people)
        {
            List<string> lines = new List<string>();
            foreach (var person in people)
            {
                lines.Add($"{person.Id},{person.FirstName},{person.LastName},{person.EmailAddress},{person.CellphoneNumber}");
            }

            File.WriteAllLines(GlobalConfig.PeopleFile.GetFullPath(), lines);
        }

        public static void SaveToTeamsFile(this List<TeamModel> teams)
        {
            List<string> lines = new List<string>();
            foreach (TeamModel team in teams)
            {
                lines.Add($"{team.Id},{team.TeamName},{ConvertPeopleListToString(team.TeamMembers)}");  
            }

            File.WriteAllLines(GlobalConfig.TeamsFile.GetFullPath(), lines);
        }

        public static void SaveMatchupToFile(this MatchupModel matchup)
        {
            List<MatchupModel> matchups = GlobalConfig.MatchupsFile.GetFullPath().LoadFile().ConvertToMatchupModels();
            int newMatchId = 1;
            if (matchups.Count > 0)
            {
                newMatchId = matchups.OrderByDescending(match => match.Id).First().Id + 1;
            }

            matchup.Id = newMatchId;
            matchups.Add(matchup);

            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                entry.SaveEntryToFile();
            }

            List<string> lines = new List<string>();
            foreach (MatchupModel match in matchups)
            {
                string winner = "";
                if (match.Winner != null)
                {
                    winner = $"{match.Winner}";
                }
                lines.Add($"{match.Id},{ConvertMatchupEntryListToString(match.Entries)},{winner},{match.MatchupRound}");
            }
            // matchups    id, id|id|id|id(entries ids), id(winner), matchupround
            File.WriteAllLines(GlobalConfig.MatchupsFile.GetFullPath(), lines);
        }

        public static void UpdateMatchupToFile(this MatchupModel matchup)
        {
            List<MatchupModel> matchups = GlobalConfig.MatchupsFile.GetFullPath().LoadFile().ConvertToMatchupModels();
            MatchupModel matchupToRemove = new MatchupModel();
            foreach (MatchupModel m in matchups)
            {
                if (m.Id == matchup.Id)
                {
                    matchupToRemove = m;
                }
            }
            matchups.Remove(matchupToRemove);
            matchups.Add(matchup);

            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                entry.UpdateEntryToFile();
            }


            List<string> lines = new List<string>();
            foreach (MatchupModel match in matchups)
            {
                string winner = "";
                if (match.Winner != null)
                {
                    winner = $"{match.Winner.Id}";
                }
                lines.Add($"{match.Id},{ConvertMatchupEntryListToString(match.Entries)},{winner},{match.MatchupRound}");
            }
            File.WriteAllLines(GlobalConfig.MatchupsFile.GetFullPath(), lines);
        }

        public static void UpdateEntryToFile(this MatchupEntryModel entry)
        {
            List<MatchupEntryModel> matchupEntries = GlobalConfig.MatchupEntriesFile.GetFullPath().LoadFile().ConvertToMatchupEntryModels();

            MatchupEntryModel entryToRemove = new MatchupEntryModel();
            foreach (MatchupEntryModel me in matchupEntries)
            {
                if (me.Id == entry.Id)
                {
                    entryToRemove = me;
                }
            }
            matchupEntries.Remove(entryToRemove);
            matchupEntries.Add(entry);

            List<string> lines = new List<string>();
            foreach (MatchupEntryModel me in matchupEntries)
            {
                string parent = "";
                if (me.ParentMatchup != null)
                {
                    parent = $"{me.ParentMatchup.Id}";
                }
                string teamCompeting = "";
                if (me.TeamCompeting != null)
                {
                    teamCompeting = me.TeamCompeting.Id.ToString();
                }
                lines.Add($"{me.Id},{teamCompeting},{me.Score},{parent}");
            }
            File.WriteAllLines(GlobalConfig.MatchupEntriesFile.GetFullPath(), lines);
        }

        public static void SaveEntryToFile(this MatchupEntryModel entry)
        {
            List<MatchupEntryModel> matchupEntries = GlobalConfig.MatchupEntriesFile.GetFullPath().LoadFile().ConvertToMatchupEntryModels();
            int newEntryId = 1;
            if (matchupEntries.Count > 0)
            {
                newEntryId = matchupEntries.OrderByDescending(e => e.Id).First().Id + 1;
            }

            entry.Id = newEntryId;

            matchupEntries.Add(entry);
            List<string> lines = new List<string>();
            foreach (MatchupEntryModel me in matchupEntries)
            {
                string parent = ""; 
                if (me.ParentMatchup != null)
                {
                    parent = $"{me.ParentMatchup.Id}";
                }
                string teamCompeting = ""; 
                if (me.TeamCompeting != null)
                {
                    teamCompeting = me.TeamCompeting.Id.ToString();
                }
                lines.Add($"{me.Id},{teamCompeting},{me.Score},{parent}");
            }
            File.WriteAllLines(GlobalConfig.MatchupEntriesFile.GetFullPath(), lines);
            // matchupEntries   id, id(team_competing), score, id(parent matchup)

        }



        public static void SaveToTournamentsFile(this List<TournamentModel> tournaments)
        {
            List<string> lines = new List<string>();
            // id,TournamentName,EntryFee,id|id|id,id|id|id,id^id^id|id^id^id|id^id^id
            foreach (TournamentModel tm in tournaments)
            {
                lines.Add($"{ tm.Id },{ tm.TournamentName },{ tm.EntryFee },{ ConvertTeamListToString(tm.EnteredTeams) },{ ConvertPrizeListToString(tm.Prizes) },{ ConvertRoundListToString(tm.Rounds) }");
            }

            File.WriteAllLines(GlobalConfig.TournamentsFile.GetFullPath(), lines);
        }

        public static void SaveRoundsToFile(this TournamentModel tournament)
        {
            foreach (List<MatchupModel> round in tournament.Rounds)
            {
                foreach (MatchupModel matchup in round)
                {
                    matchup.SaveMatchupToFile();
                }
            }
        }

        private static string ConvertPeopleListToString(List<PersonModel> people)
        {
            if (people.Count < 1)
            {
                return "";
            }
            string peopleIds = "";
            foreach (PersonModel person in people)
            {
                peopleIds += $"{person.Id}|";
            }
            peopleIds = peopleIds.Substring(0, peopleIds.Length - 1);
            return peopleIds;
        }

        private static string ConvertTeamListToString(List<TeamModel> teams)
        {
            if (teams.Count < 1)
            {
                return "";
            }
            string teamsIds = "";

            foreach (TeamModel team in teams)
            {
                teamsIds += $"{team.Id}|";
            }

            teamsIds = teamsIds.Substring(0, teamsIds.Length - 1);
            return teamsIds;
        }

        private static string ConvertPrizeListToString(List<PrizeModel> prizes)
        {
            if (prizes.Count < 1)
            {
                return "";
            }
            string prizesIds = "";

            foreach (PrizeModel prize in prizes)
            {
                prizesIds += $"{prize.Id}|";
            }

            prizesIds = prizesIds.Substring(0, prizesIds.Length - 1);
            return prizesIds;
        }

        private static string ConvertRoundListToString(List<List<MatchupModel>> rounds)
        {
            if (rounds.Count < 1)
            {
                return "";
            }
            string roundsIds = "";

            foreach (List<MatchupModel> round in rounds)
            {
                roundsIds += $"{ConvertMatchupListToString(round)}|";
            }

            roundsIds = roundsIds.Substring(0, roundsIds.Length - 1);
            return roundsIds;
        }

        private static string ConvertMatchupListToString(List<MatchupModel> round)
        {
            if (round.Count < 1)
            {
                return "";
            }
            string roundIds = "";

            foreach (MatchupModel matchup in round)
            {
                roundIds += $"{matchup.Id}^";
            }
            roundIds = roundIds.Substring(0, roundIds.Length - 1);
            return roundIds;
        }

        private static string ConvertMatchupEntryListToString(List<MatchupEntryModel> matchupEntries)
        {
            if (matchupEntries.Count < 1)
            {
                return "";
            }
            string entriesIds = "";

            foreach (MatchupEntryModel entry in matchupEntries)
            {
                entriesIds += $"{entry.Id}|";
            }

            entriesIds = entriesIds.Substring(0, entriesIds.Length - 1);
            return entriesIds;

        }

    }
}
