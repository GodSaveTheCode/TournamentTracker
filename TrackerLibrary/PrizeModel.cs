namespace TrackerLibrary
{   /// <summary>
    /// Represents a prize for particular place in the tournament.
    /// </summary>
    public class PrizeModel
    {
        /// <summary>
        /// Represents which place number matches the prize.
        /// </summary>
        public int PlaceNumber { get; set; }

        /// <summary>
        /// Represents name of place.
        /// </summary>
        public string PlaceName { get; set; }

        /// <summary>
        /// Represents amount of prize for particular place.
        /// </summary>
        public decimal PrizeAmount { get; set; }

        /// <summary>
        /// Represents percentage of prize for particular place.
        /// </summary>
        public double PrizePercentage { get; set; }
    }
}