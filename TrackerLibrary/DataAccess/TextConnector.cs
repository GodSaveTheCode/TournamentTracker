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
        
        public const string PrizesFile = "Prizes.txt";
        public const string PeopleFile = "People.txt";
        public const string TeamsFile = "Teams.txt";

        public PersonModel CreatePerson(PersonModel model)
        {
            List<PersonModel> people = PeopleFile.GetFullPath().LoadFile().ConvertToPersonModels();
            int newId = 1;
            if (people.Count > 0)
            {
                newId = people.OrderByDescending(person => person.Id).First().Id + 1;
            }

            model.Id = newId;
            people.Add(model);
            people.SaveToPeopleFile(PeopleFile.GetFullPath());
            return model;
        }

        public PrizeModel CreatePrize(PrizeModel model)
        {
           List<PrizeModel> prizes = PrizesFile.GetFullPath().LoadFile().ConvertToPrizeModels();

            int newId = 1;
            if (prizes.Count > 0)
            {
                newId = prizes.OrderByDescending(prize => prize.Id).First().Id + 1;
            }
            model.Id = newId;
            prizes.Add(model);
            prizes.SaveToPrizesFile(PrizesFile.GetFullPath());
            return model;
            
        }

        public TeamModel CreateTeam(TeamModel model)
        {
            List<TeamModel> teams = TeamsFile.GetFullPath().LoadFile().ConvertToTeamModels(PeopleFile);

            int newId = 1;
            if (teams.Count > 0)
            {
                newId = teams.OrderByDescending(team => team.Id).First().Id + 1;
            }
            model.Id = newId;
            teams.Add(model);
            teams.SaveToTeamsFile(TeamsFile.GetFullPath());
            return model;
        }

        public List<PersonModel> GetPerson_All()
        {
            return PeopleFile.GetFullPath().LoadFile().ConvertToPersonModels();
        }
    }
}
