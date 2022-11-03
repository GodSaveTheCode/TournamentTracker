using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.DataAccess;

namespace TrackerLibrary
{
    public static class GlobalConfig
    {

        public const string PrizesFile = "Prizes.txt";
        public const string PeopleFile = "People.txt";
        public const string TeamsFile = "Teams.txt";
        public const string TournamentsFile = "Tournaments.txt";
        public const string MatchupsFile = "Matchups.txt";
        public const string MatchupEntriesFile = "MatchupEntries.txt";
        public static IDataConnection Connection { get; private set; } 

        public static void InitializeConnections(DatabaseType db)
        {
            if (db == DatabaseType.SqlConnection)
            {
                Connection = new SqlConnector();
            }
            else if (db == DatabaseType.TextFileConnection)
            {
                Connection = new TextConnector();
            }
        }

        public static string GetConnectionString(string connectionName)
        {
            return ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
        }
    }
}
