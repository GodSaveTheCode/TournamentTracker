using System.Collections.Generic;

namespace TrackerLibrary.Models
{   /// <summary>
    /// Represents a match between two teams.
    /// </summary>
    public class MatchupModel
    {
        /// <summary>
        /// Represents the teams that compete with each other in this match.
        /// </summary>
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();

        /// <summary>
        /// The winner of the match.
        /// </summary>
        public TeamModel Winner { get; set; }

        /// <summary>
        /// Represents which round this match is part of.
        /// </summary>
        public int MatchupRound { get; set; }
    }
}