namespace Domain
{
    /// <summary>
    /// Object class representing a shelf location.
    /// </summary>
    public class ShelfLocation
    {
        /// <summary>
        /// Field representing the unique identifier for the shelf location.
        /// </summary>
        public int _id;

        /// <summary>
        /// Gets or sets the name of the shelf location.
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// Gets or sets the number of cookbooks assigned to the shelf location.
        /// </summary>
        public int BookCount { get; set; }

        /// <summary>
        /// Constructor for the <c>ShelfLocation</c> object.
        /// </summary>
        /// <param name="id">Unique identifier for the shelf location.</param>
        /// <param name="location">Name of the shelf location.</param>
        public ShelfLocation(int id, string location)
        {
            _id = id;
            Location = location;
        }
    }
}
