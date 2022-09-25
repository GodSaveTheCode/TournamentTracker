using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{   /// <summary>
    /// Represents one team in the tournament.
    /// </summary>
    public class TeamModel
    {
        /// <summary>
        /// The unique identifier for the team
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Members of this team.
        /// </summary>
        public List<PersonModel> TeamMembers { get; set; } = new List<PersonModel>();

        /// <summary>
        /// Name of the team.
        /// </summary>
        public string TeamName { get; set; }

    }
}
