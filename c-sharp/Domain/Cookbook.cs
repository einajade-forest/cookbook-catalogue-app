using System.Collections.Generic;

namespace Domain
{
    /// <summary>
    /// Object class representing a cookbook.
    /// </summary>
    public class Cookbook
    {
        /// <summary>
        /// Field representing the collection of <c>Recipe</c> objects associated with the cookbook.
        /// </summary>
        private List<Recipe> _recipes;
        /// <summary>
        /// Field representing the identifier of the <c>Location</c> associated with the cookbook.
        /// </summary>
        public int _locationId;

        /// <summary>
        /// Gets or sets the thirteen digit book identifier of the cookbook.
        /// </summary>
        public string Isbn13 { get; set; }
        /// <summary>
        /// Gets or sets the title of the cookbook.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Gets or sets the author(s) or organisation(s) responsible for producing the cookbook.
        /// </summary>
        public string Contributor { get; set; }
        /// <summary>
        /// Gets or sets the name of the location at which the cookbook is shelved.
        /// </summary>
        public string LocationName { get; set; }
        /// <summary>
        /// Gets or sets the collection of <c>Recipe</c> objects associated with the cookbook.
        /// </summary>
        public List<Recipe> CookbookRecipes
        {
            get { return _recipes ?? (_recipes = new List<Recipe>()); }
            set => _recipes = value;
        }

        /// <summary>
        /// Constructor for <c>Cookbook</c> object.
        /// </summary>
        public Cookbook() { }

        /// <summary>
        /// Method to add a recipe to the cookbook's collection of <c>Recipe</c>.
        /// </summary>
        /// <param name="recipe"><c>Recipe</c> object.</param>
        /// <returns>A collection of <c>Recipe</c> objects.</returns>
        public List<Recipe> AddCookbookRecipes(Recipe recipe)
        {
            CookbookRecipes.Add(recipe);
            return CookbookRecipes;
        }
    }
}