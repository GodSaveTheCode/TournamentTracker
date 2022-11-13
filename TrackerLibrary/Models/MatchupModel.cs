using System.Collections.Generic;

namespace TrackerLibrary.Models
{   /// <summary>
    /// Represents a match between two teams.
    /// </summary>
    public class MatchupModel
    {
        /// <summary>
        /// The unique identifier for the matchup.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Represents the teams that compete with each other in this match.
        /// </summary>
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();

        /// <summary>
        /// The winner of the match.
        /// </summary>
        public TeamModel Winner { get; set; }

        /// <summary>
        /// The ID from the database that will be used to identify the winner.
        /// </summary>
        public int WinnerId { get; set; }

        /// <summary>
        /// Represents which round this match is part of.
        /// </summary>
        public int MatchupRound { get; set; }

        public string DisplayName
        { 
            get
            {
                string output = "";
                foreach (MatchupEntryModel entry in Entries)
                {
                    if (entry.TeamCompeting != null)
                    {
                        if (output.Length == 0)
                        {
                            output = entry.TeamCompeting.TeamName;
                        }
                        else
                        {
                            output += $" vs {entry.TeamCompeting.TeamName}";
                        }
                    }
                    else
                    {
                        output = "Matchup Not Yet Determined";
                        break;
                    }

                }
                return output;
            }
        }
    }
}