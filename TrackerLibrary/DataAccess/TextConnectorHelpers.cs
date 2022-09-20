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

        public static List<TeamModel> ConvertToTeamModels(this List<string> lines, string peopleFile)
        {
            List<TeamModel> teams = new List<TeamModel>();
            List<PersonModel> people = peopleFile.GetFullPath().LoadFile().ConvertToPersonModels();
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

        public static void SaveToPrizesFile(this List<PrizeModel> prizes, string filePath)
        {
            List<string> lines = new List<string>();
            foreach (var prize in prizes)
            {
                lines.Add($"{prize.Id},{prize.PlaceNumber},{prize.PlaceName},{prize.PrizeAmount},{prize.PrizePercentage}");
            }

            File.WriteAllLines(filePath, lines);
        }

        public static void SaveToPeopleFile(this List<PersonModel> people, string filePath)
        {
            List<string> lines = new List<string>();
            foreach (var person in people)
            {
                lines.Add($"{person.Id},{person.FirstName},{person.LastName},{person.EmailAddress},{person.CellphoneNumber}");
            }

            File.WriteAllLines(filePath, lines);
        }

        public static void SaveToTeamsFile(this List<TeamModel> teams, string filePath)
        {
            List<string> lines = new List<string>();
            foreach (TeamModel team in teams)
            {
                lines.Add($"{team.Id},{team.TeamName},{ConvertPeopleListToString(team.TeamMembers)}");  
            }

            File.WriteAllLines(filePath, lines);
        }

        public static string ConvertPeopleListToString(List<PersonModel> people)
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

    }
}
