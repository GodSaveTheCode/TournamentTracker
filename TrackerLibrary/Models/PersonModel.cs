namespace TrackerLibrary.Models
{   /// <summary>
    /// Represents a person.
    /// </summary>
    public class PersonModel
    {
        /// <summary>
        /// Unique identifier of the person.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// First name of the person.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Second name of the person.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Email address of the person.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Phone number of the person.
        /// </summary>
        public string CellphoneNumber { get; set; }
    }
}