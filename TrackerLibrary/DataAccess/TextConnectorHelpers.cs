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

        public static void SaveToPrizeFile(this List<PrizeModel> prizes, string filePath)
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
                lines.Add($"{person.FirstName},{person.LastName},{person.EmailAddress},{person.CellphoneNumber}");
            }

            File.WriteAllLines(filePath, lines);
        }
    }
}
