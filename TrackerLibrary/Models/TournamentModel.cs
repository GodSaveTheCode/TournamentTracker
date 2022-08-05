using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{   /// <summary>
    /// Represents a tournament.
    /// </summary>
    public class TournamentModel
    {
        /// <summary>
        /// Name of the tournament.
        /// </summary>
        public string TournamentName { get; set; }

        /// <summary>
        /// Entry fee that tournamens charges.
        /// </summary>
        public decimal EntryFee { get; set; }

        /// <summary>
        /// Represents teams that participate in the tournament.
        /// </summary>
        public List<TeamModel> EnteredTeams { get; set; } = new List<TeamModel>();

        /// <summary>
        /// Represents prizes of the tournament.
        /// </summary>
        public List<PrizeModel> Prizes { get; set; } = new List<PrizeModel>();

        /// <summary>
        /// Represents amount of rounds in the tournament.
        /// </summary>
        public List<List<MatchupModel>> Rounds { get; set; } = new List<List<MatchupModel>>();
    }

}
