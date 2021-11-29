using System.Collections.Generic;
using System.Linq;

using DataAccess;
using Domain;

namespace Controller
{
    /// <summary>
    /// Controller class for the application
    /// </summary>
    public class ViewModel
    {
        /// <summary>
        /// Method to return a collection of cookbooks via <c>SqlDataAccess</c> and the associated recipe references.
        /// </summary>
        /// <remarks>
        /// Method typically called when initialising the <c>BooksPage</c>. Also called in <c>AddBookWindow</c> and <c>EditBookWindow</c> in the event that the ISBN-13 requires updating to check for duplication.
        /// </remarks>
        /// <returns>A collection of <c>Cookbook</c>.</returns>
        public static IEnumerable<Cookbook> GetCookbooks()
        {
            var cookbooks = SqlDataAccess.GetCookbooks();

            foreach (Cookbook cookbook in cookbooks)
            {
                List<Recipe> recipes = (List<Recipe>)SqlDataAccess.GetRecipesByCookbook(cookbook.Isbn13);

                foreach (Recipe recipe in recipes)
                {
                    cookbook.AddCookbookRecipes(recipe);
                    recipe.Tags = (List<Tag>)SqlDataAccess.GetTagsByRecipe(recipe.RecipeId);
                }
            }
            return cookbooks;
        }

        /// <summary>
        /// Method to return a collection of locations via <c>SqlDataAccess</c>.
        /// </summary>
        /// <remarks>
        /// Method called when initialising the <c>ManageLocationsWindow</c>, <c>AddBookWindow</c> and <c>EditBookWindow</c>. Method includes the count of cookbooks per location to be displayed in <c>ManageLocationsWindow</c>.
        /// </remarks>
        /// <returns>A collection of <c>ShelfLocation</c>.</returns>
        public static IEnumerable<ShelfLocation> GetLocations()
        {
            var locations = (List<ShelfLocation>)SqlDataAccess.GetLocations();
            var cookbooks = SqlDataAccess.GetCookbooks();

            var groupedCookbooks = cookbooks.GroupBy(c => c._locationId)
                                            .Select(g => new
                                            {
                                                Location = g.Key,
                                                Count = g.Count()
                                            })
                                            .OrderBy(x => x.Location);

            foreach (var loc in groupedCookbooks)
            {
                locations.Find(x => x._id == loc.Location).BookCount = loc.Count;
            }
            return locations;
        }

        /// <summary>
        /// Method to get collection of recipes containing a keyword in the title via <c>SqlDataAccess</c>.
        /// </summary>
        /// <remarks>Method called when the "Search" button is clicked on the <c>SearchPage</c>.</remarks>
        /// <param name="keyword">User entered string.</param>
        /// <returns>A collection of <c>Recipe</c> by keyword.</returns>
        public static IEnumerable<Recipe> GetRecipesByKeyword(string keyword)
        {
            List<Recipe> results = (List<Recipe>)SqlDataAccess.GetRecipesByKeyword(keyword);
            return results;
        }
        
        /// <summary>
        /// Method to get collection of recipes with a given tag via <c>SqlDataAccess</c>.
        /// </summary>
        /// <remarks>Method called when the "Display Recipes" button is clicked on the <c>SearchPage</c>.</remarks>
        /// <param name="tag">Tag object selected from combobox.</param>
        /// <returns>A collection of <c>Recipe</c> by keyword.</returns>
        public static IEnumerable<Recipe> GetRecipesByTag(Tag tag)
        {
            List<Recipe> results = new List<Recipe>();
            List<int> listRecipeId = SqlDataAccess.GetRecipesByTag(tag.TagId);
            foreach (int id in listRecipeId)
            {
                var recipe = SqlDataAccess.GetRecipeById(id);
                results.Add(recipe);
            }
            return results;
        }

        /// <summary>
        /// Method to return a collection of recipe tags via <c>SqlDataAccess</c>.
        /// </summary>
        /// <remarks>
        /// Method called when initialising the <c>ManageTagsWindow</c>, <c>AddRecipeWindow</c> and <c>EditRecipeWindow</c> and for validating collection after changes to source and assigned tag lists. Method also includes the count of recipes per tag to be displayed in <c>ManageTagsWindow</c>.
        /// </remarks>
        /// <returns>A collection of <c>Tag</c>.</returns>
        public static IEnumerable<Tag> GetTags()
        {
            var tags = SqlDataAccess.GetTags();
            foreach (Tag tag in tags)
            {
                List<int> group = SqlDataAccess.GetRecipesByTag(tag.TagId);
                tag.RecipeCount = group.Count;
            }
            return tags;
        }


        /// <summary>
        /// Method to delete cookbook and the associated recipe information via <c>SqlDataAccess</c>.
        /// </summary>
        /// <remarks>Method deletes information on the cookbook selected on <c>BookPage</c>, associated recipes and recipe tags from the database.</remarks>
        /// <param name="isbn13">Primary key of selected cookbook.</param>
        /// <param name="recipes">Collection of recipes assigned to the selected cookbook.</param>
        public static void DeleteCookbook(string isbn13, List<Recipe> recipes)
        {
            foreach (Recipe recipe in recipes)
            {
                if (recipe.RecipeId > 0)
                {
                    SqlDataAccess.DeleteRecipeTags(recipe.RecipeId);
                }
                SqlDataAccess.DeleteRecipe(recipe.RecipeId);
            }
            SqlDataAccess.DeleteCookbook(isbn13);
        }

        /// <summary>
        /// Method to delete location information via <c>SqlDataAccess.</c>
        /// </summary>
        /// <remarks>Method deletes location selected on <c>ManageLocationsWindow</c> from the database.</remarks>
        /// <param name="location">Location object selected by user.</param>
        public static void DeleteLocation(ShelfLocation location)
        {
            SqlDataAccess.DeleteLocation(location._id);
        }

        /// <summary>
        /// Method to delete recipe and associated recipe tag information via <c>SqlDataAccess</c>.
        /// </summary>
        /// <remarks>Method is called when a collection of recipes to be deleted is not null at the point of clicking the "Save Changes" button on <c>EditCookbookWindow</c>.</remarks>
        /// <param name="recipe">Recipe object to be deleted.</param>
        public static void DeleteRecipe(Recipe recipe)
        {
            if (recipe.RecipeId > 0)
            {
                SqlDataAccess.DeleteRecipeTags(recipe.RecipeId);
            }
            SqlDataAccess.DeleteRecipe(recipe.RecipeId);
        }

        /// <summary>
        /// Method to delete tag information via <c>SqlDataAccess.</c>
        /// </summary>
        /// <remarks>Method is called when selected tag is deleted on <c>ManageTagsWindow</c>.</remarks>
        /// <param name="tag">Tag object selected by user.</param>
        public static void DeleteTag(Tag tag)
        {
            SqlDataAccess.DeleteTag(tag.TagId);
        }


        /// <summary>
        /// Method to insert details of a new cookbook, including associated recipes and recipe tags via <c>SqlDataAccess</c>.
        /// </summary>
        /// <remarks>Method called when the "Add Cookbook" button is clicked on <c>AddCookbookWindow</c>.</remarks>
        /// <param name="isbn13">Primary key of the cookbook.</param>
        /// <param name="title">Title of the cookbook.</param>
        /// <param name="contributor">Name of author(s) or organisation(s) responsible for producing the cookbook.</param>
        /// <param name="location">Shelf lcoation of the cookbook.</param>
        /// <param name="recipes">Collection of recipes assigned to the cookbook.</param>
        public static void InsertCookbook(string isbn13, string title, string contributor, ShelfLocation location, List<Recipe> recipes)
        {
            SqlDataAccess.InsertCookbook(isbn13, title, contributor, location._id);

            foreach (Recipe recipe in recipes)
            {
                SqlDataAccess.UpsertRecipe(recipe.RecipeId, recipe.RecipeName, recipe.Page, isbn13);
                int newId = SqlDataAccess.GetLastRecipeId();
                foreach (Tag tag in recipe.Tags)
                {
                    SqlDataAccess.InsertRecipeTag(tag.TagId, newId);
                }
            }
        }


        /// <summary>
        /// Method to update the details of an exisiting cookbook, including associated recipes and recipe tags via <c>SqlDataAccess</c>.
        /// </summary>
        /// <remarks>
        /// Method called when the "Save Changes" button is clicked on <c>EditCookbookWindow</c>. In the event that ISBN13 of an exisiting cookbook has been changed, all information associated with the cookbook is deleted and recreated.
        /// </remarks>
        /// <param name="originalIsbn13">Original thirteen digit identifier of the cookbook as currently represented in the database.</param>
        /// <param name="isbn13">Current or new thirteen digit identifier of the cookbook.</param>
        /// <param name="title">Title of the cookbook.</param>
        /// <param name="contributor">Name of author(s) or organisation(s) responsible for producing the cookbook.</param>
        /// <param name="location">Shelf lcoation of the cookbook.</param>
        /// <param name="recipes">Collection of recipes assigned to the cookbook.</param>
        public static void UpdateCookbook(string originalIsbn13, string isbn13, string title, string contributor, ShelfLocation location, List<Recipe> recipes)
        {
            if (originalIsbn13 != isbn13)
            {
                DeleteCookbook(originalIsbn13, recipes);
                foreach (Recipe recipe in recipes)
                {
                    recipe.RecipeId = 0;
                }
                InsertCookbook(isbn13, title, contributor, location, recipes);
            }
            else
            {
                SqlDataAccess.UpdateCookbook(isbn13, title, contributor, location._id);
                foreach (Recipe recipe in recipes)
                {
                    SqlDataAccess.UpsertRecipe(recipe.RecipeId, recipe.RecipeName, recipe.Page, isbn13);
                    if (recipe.RecipeId > 0)
                    {
                        SqlDataAccess.DeleteRecipeTags(recipe.RecipeId);
                    }
                    else
                    {
                        int newId = SqlDataAccess.GetLastRecipeId();
                        recipe.RecipeId = newId;
                    }

                    foreach (Tag tag in recipe.Tags)
                    {
                        SqlDataAccess.InsertRecipeTag(tag.TagId, recipe.RecipeId);
                    }
                }
            }

        }


        /// <summary>
        /// Method to insert new location or update exisiting location via <c>SqlDataAccess</c>.
        /// </summary>
        /// <remarks>Method is called when a new location name is saved in <c>ManageLocationsWindow</c>.</remarks>
        /// <param name="id">Id of location object. If new object, id=0.</param>
        /// <param name="name">Name of location.</param>
        public static void UpsertLocation(int id, string name)
        {
            SqlDataAccess.UpsertLocation(id, name);
        }

        /// <summary>
        /// Method to insert new tag or update exisiting tag via <c>SqlDataAccess</c>.
        /// </summary>
        /// <remarks>Method is called when a new tag name is saved in <c>ManageTagsWindow</c>.</remarks>
        /// <param name="id">Id of tag object. If new object, id=0.</param>
        /// <param name="name">Name of tag.</param>
        public static void UpsertTag (int id, string name)
        {
            SqlDataAccess.UpsertTag(id, name);
        }
    }
}
