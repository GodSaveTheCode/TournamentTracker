using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary
{   /// <summary>
    /// Represents one team in the tournament.
    /// </summary>
    public class TeamModel
    {
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
