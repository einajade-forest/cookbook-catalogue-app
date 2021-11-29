namespace Domain
{
    /// <summary>
    /// Object class representing a tag.
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// Field representing the unique identifier of the tag.
        /// </summary>
        private int _id;

        /// <summary>
        /// Gets or sets the unique identifier of the tag.
        /// </summary>
        public int TagId { get => TagId = _id; set => _id = value; }
        /// <summary>
        /// Gets or sets the name of the tag.
        /// </summary>
        public string TagName { get; set; }
        /// <summary>
        /// Gets or sets the number of <c>Recipe</c> objects associated with the tag.
        /// </summary>
        public int RecipeCount { get; set; }

        /// <summary>
        /// Constructor for <c>Tag</c> object.
        /// </summary>
        /// <param name="id">Unique identifier of the tag.</param>
        /// <param name="name">Name of tag.</param>
        public Tag (int id, string name)
        {
            _id = id;
            TagName = name;
        }

    }
}