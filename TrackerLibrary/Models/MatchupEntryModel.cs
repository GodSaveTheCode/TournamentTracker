namespace TrackerLibrary.Models
{   /// <summary>
    /// Represents one team in the matchup.
    /// </summary>
    public class MatchupEntryModel
    {
        /// <summary>
        /// The unique identifier for the matchup entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Represents one particular team in the matchup.
        /// </summary>
        public TeamModel TeamCompeting { get; set; }

        /// <summary>
        /// The ID from the database that will be used to identify competing team.
        /// </summary>
        public int TeamCompetingId { get; set; }

        /// <summary>
        /// Represents the score of this particular team.
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Represents the matchup that this team came from as the winner.
        /// </summary>
        public MatchupModel ParentMatchup { get; set; }

        /// <summary>
        /// The ID from the database that will be used to identify parent matchup.
        /// </summary>
        public int ParentMatchId { get; set; }
    }
}