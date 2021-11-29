using System.Collections.Generic;

namespace Domain
{
    /// <summary>
    /// Object class representing a recipe reference.
    /// </summary>
    public class Recipe
    {
        /// <summary>
        /// Field representing the collection of <c>Tag</c> objects associated with the recipe.
        /// </summary>
        private List<Tag> _tags;
        /// <summary>
        /// Field representing the title of the cookbook wherein the recipe resides.
        /// </summary>
        private string _title;
        /// <summary>
        /// Field representing the shelf location of the cookbook wherein the recipe resides.
        /// </summary>
        private string _location;
        /// <summary>
        /// Field representing the unique identifier of the recipe.
        /// </summary>
        private int _id;

        /// <summary>
        /// Gets or sets the title of the cookbook wherein the recipe resides.
        /// </summary>
        public string CookbookTitle { get => CookbookTitle = _title; set => _title = value; }
        /// <summary>
        /// Gets or sets the shelf location of the cookbook wherein the recipe resides.
        /// </summary>
        public string Location { get => Location = _location; set => _location = value; }
        /// <summary>
        /// Gets or sets the unique identifier of the recipe.
        /// </summary>
        public int RecipeId { get => RecipeId = _id; set => _id = value; }
        /// <summary>
        /// Gets or sets the name of the recipe.
        /// </summary>
        public string RecipeName { get; set; }
        /// <summary>
        /// Gets or sets the reference for the first page of the recipe.
        /// </summary>
        public string Page { get; set; }
        /// <summary>
        /// Gets or sets the collection of <c>Tag</c> objects associated with the recipe.
        /// </summary>
        public List<Tag> Tags
        {
            get { return _tags ?? (_tags = new List<Tag>()); }
            set => _tags = value;
        }

        /// <summary>
        /// Constructor for <c>Recipe</c> object.
        /// </summary>
        /// <param name="recipeId">Unique identifier of the recipe.</param>
        /// <param name="name">Name of the recipe.</param>
        /// <param name="page">Reference representing the first page of the recipe.</param>
        /// <param name="title">Title of cookbook containing the recipe.</param>
        /// <param name="location">Shelf location of the cookbook containing the recipe.</param>
        public Recipe(int recipeId, string name, string page, string title, string location)
        {
            _id = recipeId;
            RecipeName = name;
            Page = page;
            _title = title;
            _location = location;
        }

        /// <summary>
        /// Alternate constructor for <c>Recipe</c> object.
        /// </summary>
        /// <remarks>Utilised for temporary <c>Recipe</c> objects in listed within <c>AddBookWindow</c> and <c>EditBookWindow</c> datagrids.</remarks>
        /// <param name="tempName">Name of the recipe.</param>
        /// <param name="tempPage">Reference representing the first page of the recipe.</param>
        public Recipe(string tempName, string tempPage)
        {
            RecipeName = tempName;
            Page = tempPage;
        }

        /// <summary>
        /// Method to add a tag to the recipe's collection of tags.
        /// </summary>
        /// <param name="tag"><c>Tag</c> object.</param>
        /// <returns>A collection of <c>Tag</c> objects.</returns>
        public List<Tag> AddTags( Tag tag)
        {
            Tags.Add(tag);
            return Tags;
        }
    }
}
