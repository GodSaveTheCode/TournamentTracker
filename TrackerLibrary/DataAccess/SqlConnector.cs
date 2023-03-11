using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;
using Dapper;
using System.Data;

namespace TrackerLibrary.DataAccess
{
    public class SqlConnector : IDataConnection
    {
        private const string db = "Tournaments";
        public void CreatePrize(PrizeModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.GetConnectionString(db)))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@PlaceNumber", model.PlaceNumber);
                parameters.Add("@PlaceName", model.PlaceName);
                parameters.Add("@PrizeAmount", model.PrizeAmount);
                parameters.Add("@PrizePercentage", model.PrizePercentage);
                parameters.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spPrizes_Insert", parameters, commandType: CommandType.StoredProcedure);

                model.Id = parameters.Get<int>("@id");
            }
        }



        public void CreatePerson(PersonModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.GetConnectionString(db)))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@FirstName", model.FirstName);
                parameters.Add("@LastName", model.LastName);
                parameters.Add("@EmailAddress", model.EmailAddress);
                parameters.Add("@CellphoneNumber", model.CellphoneNumber);
                parameters.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spPeople_Insert", parameters, commandType: CommandType.StoredProcedure);

                model.Id = parameters.Get<int>("@id");

            }
        }

        public List<PersonModel> GetPerson_All()
        {
            List<PersonModel> output;
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.GetConnectionString(db)))
            {
                output = connection.Query<PersonModel>("dbo.spPeople_GetAll").ToList();
            }
            return output;
        }

        public void CreateTeam(TeamModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.GetConnectionString(db)))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@TeamName", model.TeamName);
                parameters.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTeams_Insert", parameters, commandType: CommandType.StoredProcedure);

                model.Id = parameters.Get<int>("@id");

                foreach (PersonModel person in model.TeamMembers)
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@TeamId", model.Id);
                    parameters.Add("@PersonId", person.Id);
                    connection.Execute("dbo.spTeamMembers_Insert", parameters, commandType: CommandType.StoredProcedure);
                }
                
            }
        }

        public List<TeamModel> GetTeam_All()
        {
            List<TeamModel> output;
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.GetConnectionString(db)))
            {
                output = connection.Query<TeamModel>("dbo.spTeam_GetAll").ToList();

                DynamicParameters parameters;
                
                foreach (TeamModel team in output)
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@TeamId", team.Id);
                    team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam", parameters, commandType:CommandType.StoredProcedure).ToList();
                }
            }
            return output;
        }

        public void CreateTournament(TournamentModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.GetConnectionString(db)))
            {
                SaveTournament(model, connection);
                SaveTournamentPrizes(model, connection);
                SaveTournamentEntries(model, connection);
                SaveTournamentRounds(model, connection);
                TournamentLogic.UpdateTournamentResults(model);
            }
        }

        private void SaveTournamentRounds(TournamentModel model, IDbConnection connection)
        {
            foreach (List<MatchupModel> round in model.Rounds)
            {
                foreach (MatchupModel match in round)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@TournamentId", model.Id);
                    parameters.Add("@MatchupRound", match.MatchupRound);
                    parameters.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                    connection.Execute("dbo.spMatchups_Insert", parameters, commandType: CommandType.StoredProcedure);

                    match.Id = parameters.Get<int>("@id");

                    foreach (MatchupEntryModel entry in match.Entries) 
                    {
                        parameters = new DynamicParameters();

                        parameters.Add("@MatchupId", match.Id);
                        if (entry.ParentMatchup == null)
                        {
                            parameters.Add("@ParentMatchId", null);
                        }
                        else
                        {
                            parameters.Add("@ParentMatchId", entry.ParentMatchup.Id);
                        }

                        if (entry.TeamCompeting == null)
                        {
                            parameters.Add("@TeamCompeting", null);
                        }
                        else
                        {
                            parameters.Add("@TeamCompeting", entry.TeamCompeting.Id);
                        }
        
                        parameters.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                        connection.Execute("dbo.spMatchupEntries_Insert", parameters, commandType: CommandType.StoredProcedure);
                    }
                }
            }
               

        }

        private void SaveTournament(TournamentModel model, IDbConnection connection) //private?
        {
            var parameters = new DynamicParameters();
            parameters.Add("@TournamentName", model.TournamentName);
            parameters.Add("@EntryFee", model.EntryFee);
            parameters.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

            connection.Execute("dbo.spTournaments_Insert", parameters, commandType: CommandType.StoredProcedure);

            model.Id = parameters.Get<int>("@id");
        }

        private void SaveTournamentPrizes(TournamentModel model, IDbConnection connection)
        {
            foreach (PrizeModel prize in model.Prizes)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@TournamentId", model.Id);
                parameters.Add("@PrizeId", prize.Id);
                parameters.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTournamentPrizes_Insert", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        private void SaveTournamentEntries(TournamentModel model, IDbConnection connection)
        {
            foreach (TeamModel team in model.EnteredTeams)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@TournamentId", model.Id);
                parameters.Add("@TeamId", team.Id);
                parameters.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spTournamentEntries_Insert", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public List<TournamentModel> GetTournament_All()
        {
            // id,TournamentName,EntryFee,Prizes,Teams,Rounds
            List<TournamentModel> output;
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.GetConnectionString(db)))
            {
                output = connection.Query<TournamentModel>("dbo.spTournament_GetAll").ToList(); 
                DynamicParameters parameters;

                foreach (TournamentModel tournament in output)
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@TournamentId", tournament.Id);
                    tournament.Prizes = connection.Query<PrizeModel>("dbo.spPrizes_GetByTournament", parameters, commandType: CommandType.StoredProcedure).ToList();

                    tournament.EnteredTeams = connection.Query<TeamModel>("dbo.spTeams_GetByTournament", parameters, commandType: CommandType.StoredProcedure).ToList();
                    foreach (TeamModel team in tournament.EnteredTeams)
                    {
                        parameters = new DynamicParameters();
                        parameters.Add("@TeamId", team.Id);
                        team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam", parameters, commandType: CommandType.StoredProcedure).ToList();
                    }

                    parameters = new DynamicParameters();
                    parameters.Add("@TournamentId", tournament.Id);
                    List<MatchupModel> allMatchups = connection.Query<MatchupModel>("dbo.spMatchups_GetByTournament", parameters, commandType: CommandType.StoredProcedure).ToList();
                   
                    foreach (MatchupModel matchup in allMatchups)
                    {
                        if (matchup.WinnerId > 0)
                        {
                            matchup.Winner = tournament.EnteredTeams.Where(t => t.Id == matchup.WinnerId).First();
                        }
                        parameters = new DynamicParameters();
                        parameters.Add("@MatchupId", matchup.Id);
                        matchup.Entries = connection.Query<MatchupEntryModel>("dbo.spMatchupEntries_GetByMatchup", parameters, commandType: CommandType.StoredProcedure).ToList();
                        foreach (MatchupEntryModel entry in matchup.Entries)
                        {
                            if (entry.TeamCompetingId > 0)
                            {
                                entry.TeamCompeting = tournament.EnteredTeams.Where(t => t.Id == entry.TeamCompetingId).First();
                            }
                            if (entry.ParentMatchId > 0)
                            {
                                entry.ParentMatchup = allMatchups.Where(m => m.Id == entry.ParentMatchId).First();
                            }
                        }
                    }

                    List<MatchupModel> round = new List<MatchupModel>();
                    int matchupRound = 1;
                    foreach (MatchupModel matchup in allMatchups)
                    {
                        if (matchup.MatchupRound > matchupRound)
                        {
                            tournament.Rounds.Add(round);
                            round = new List<MatchupModel>();
                            matchupRound++;
                        }
                        round.Add(matchup);
                    }
                    tournament.Rounds.Add(round);

                }

            }
            return output;
        }

        public void UpdateMatchup(MatchupModel matchup) 
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.GetConnectionString(db)))
            {
                var parameters = new DynamicParameters();
                if (matchup.Winner != null)
                {
                    parameters.Add("@id", matchup.Id);
                    parameters.Add("@WinnerId", matchup.Winner.Id);
                    connection.Execute("dbo.spMatchup_Update", parameters, commandType: CommandType.StoredProcedure);
                }

                foreach (MatchupEntryModel entry in matchup.Entries)
                {
                    if (entry.TeamCompeting != null)
                    {
                        parameters = new DynamicParameters();
                        parameters.Add("@id", entry.Id);
                        parameters.Add("@TeamCompetingId", entry.TeamCompeting.Id);
                        parameters.Add("@Score", entry.Score);
                        connection.Execute("dbo.spMatchupEntry_Update", parameters, commandType: CommandType.StoredProcedure); 
                    }
                }

            }
        }
    }
}
