using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;
using System.IO;
using TrackerLibrary.DataAccess.Helpers;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {       

        public void CreatePerson(PersonModel model)
        {
            List<PersonModel> people = GlobalConfig.PeopleFile.GetFullPath().LoadFile().ConvertToPersonModels();
            int newId = 1;
            if (people.Count > 0)
            {
                newId = people.OrderByDescending(person => person.Id).First().Id + 1;
            }

            model.Id = newId;
            people.Add(model);
            people.SaveToPeopleFile();
        }

        public void CreatePrize(PrizeModel model)
        {
           List<PrizeModel> prizes = GlobalConfig.PrizesFile.GetFullPath().LoadFile().ConvertToPrizeModels();

            int newId = 1;
            if (prizes.Count > 0)
            {
                newId = prizes.OrderByDescending(prize => prize.Id).First().Id + 1;
            }
            model.Id = newId;
            prizes.Add(model);
            prizes.SaveToPrizesFile();
            
        }

        public void CreateTeam(TeamModel model)
        {
            List<TeamModel> teams = GlobalConfig.TeamsFile.GetFullPath().LoadFile().ConvertToTeamModels();

            int newId = 1;
            if (teams.Count > 0)
            {
                newId = teams.OrderByDescending(team => team.Id).First().Id + 1;
            }
            model.Id = newId;
            teams.Add(model);
            teams.SaveToTeamsFile();
        }

        public void CreateTournament(TournamentModel model)
        {
            List<TournamentModel> tournaments = GlobalConfig.TournamentsFile
                .GetFullPath()
                .LoadFile()
                .ConvertToTournamentModels();

            int newId = 1;
            if (tournaments.Count > 0)
            {
                newId = tournaments.OrderByDescending(tm => tm.Id).First().Id + 1;
            }

            model.Id = newId;
            model.SaveRoundsToFile();
            tournaments.Add(model);
            tournaments.SaveToTournamentsFile();
            TournamentLogic.UpdateTournamentResults(model);
        }


        public List<PersonModel> GetPerson_All()
        {
            return GlobalConfig.PeopleFile.GetFullPath().LoadFile().ConvertToPersonModels();
        }

        public List<TeamModel> GetTeam_All()
        {
            return GlobalConfig.TeamsFile.GetFullPath().LoadFile().ConvertToTeamModels();
        }

        public List<TournamentModel> GetTournament_All()
        {
            List<TournamentModel> tournaments = GlobalConfig.TournamentsFile
                .GetFullPath()
                .LoadFile()
                .ConvertToTournamentModels();
            return tournaments;
        }

        public void UpdateMatchup(MatchupModel model)
        {
            model.UpdateMatchupToFile();
        }

    }
}
