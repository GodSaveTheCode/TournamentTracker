namespace TrackerLibrary
{   /// <summary>
    /// Represents what the prize is for the given place.
    /// </summary>
    public class PrizeModel
    {
        /// <summary>
        /// The numeric identifier for the place.
        /// </summary>
        public int PlaceNumber { get; set; }

        /// <summary>
        /// Represents name for the  place.
        /// </summary>
        public string PlaceName { get; set; }

        /// <summary>
        /// The fixed amount this place earns.
        /// </summary>
        public decimal PrizeAmount { get; set; }

        /// <summary>
        /// The number that represents percentage of the overall take.
        /// The percentage is a fraction of 1(so 0.5 for 50%).
        /// </summary>
        public double PrizePercentage { get; set; }
    }
}