using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

using Domain;

namespace DataAccess
{
    /// <summary>
    /// Data logic layer to access Microsoft SQL Server database.
    /// </summary>
    public class SqlDataAccess
    {
        /// <summary>
        /// Microsoft SQL Server Connection String.
        /// </summary>
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["CbCConnection_Prod"].ConnectionString;

        /// <summary>
        /// Method to delete cookbook from database.
        /// </summary>
        /// <remarks>
        /// Utilises database stored procedure to locate row with matching ISBN-13 in the Cookbook table and deletes row.
        /// </remarks>
        /// <param name="isbn13">The id of the cookbook to be removed.</param>
        public static void DeleteCookbook(string isbn13)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_DeleteCookbook", connection))
                    {
                        connection.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@isbn13", isbn13);
                        var result = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
        }

        /// <summary>
        /// Method to delete location from database.
        /// </summary>
        /// <remarks>
        /// Utilises database stored procedure to locate row with matching identifier in the ShelfLocation table and deletes row.
        /// </remarks>
        /// <param name="locationId">The id of the location to be removed.</param>
        public static void DeleteLocation(int locationId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_DeleteLocation", connection))
                    {
                        connection.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", locationId);
                        var result = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
        }

        /// <summary>
        /// Method to delete recipe from database.
        /// </summary>
        /// <remarks>
        /// Utilises database stored procedure to locate row with matching identifier in the Recipe table and deletes row.
        /// </remarks>
        /// <param name="recipeId">The id of the recipe.to be removed.</param>
        public static void DeleteRecipe(int recipeId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_DeleteRecipe", connection))
                    {
                        connection.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", recipeId);
                        var result = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
        }

        /// <summary>
        /// Method to delete recipe-tag relationships from database.
        /// </summary>
        /// <remarks>
        /// Utilises database stored procedure to locate rows with matching recipe identifier in the RecipeTag and deletes rows.
        /// </remarks>
        /// <param name="recipeId">The id of the recipe that has been removed.</param>
        public static void DeleteRecipeTags(int recipeId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_DeleteRecipeTags", connection))
                    {
                        connection.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@recipe", recipeId);
                        var result = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
        }

        /// <summary>
        /// Method to delete tag from database.
        /// </summary>
        /// <remarks>
        /// Utilises database stored procedure to locate row with matching identifier in the Tag table and deletes row.
        /// </remarks>
        /// <param name="tagId">The id of the tag to be removed.</param>
        public static void DeleteTag(int tagId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_DeleteTag", connection))
                    {
                        connection.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", tagId);
                        var result = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
        }


        /// <summary>
        /// Method to get the collection of cookbooks in database.
        /// </summary>
        /// <remarks>
        /// Utilises database stored procedure to join the Cookbook and ShelfLocation tables to select ISBN13, Title, Contributor, Shelf location ID and Shelf location name. Parameters used to create new <c>Cookbook</c> class object.
        /// </remarks>
        /// <returns>A list of cookbooks or null if exception is thrown.</returns>
        public static IEnumerable<Cookbook> GetCookbooks()
        {
            List<Cookbook> cookbooksList = new List<Cookbook>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_SelectCookbooks", connection))
                    {
                        connection.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Cookbook cookbook = new Cookbook();
                                {
                                    cookbook.Isbn13 = Convert.ToString(reader.GetValue(0));
                                    cookbook.Title = Convert.ToString(reader.GetValue(1));
                                    cookbook.Contributor = Convert.ToString(reader.GetValue(2));
                                    cookbook._locationId = Convert.ToInt32(reader.GetValue(3));
                                    cookbook.LocationName = Convert.ToString(reader.GetValue(4));

                                };
                                cookbooksList.Add(cookbook);
                            }
                        }
                    }
                    return cookbooksList;
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }

            return null;
        }

        /// <summary>
        /// Method to get the collection of locations in database.
        /// </summary>
        /// <remarks>
        /// Utilises database stored procedure to select Location ID and Name from the ShelfLocation table. Paramaters used to create new <c>ShelfLocation</c> class object.
        /// </remarks>
        /// <returns>A list of locations or null if exception is thrown.</returns>
        public static IEnumerable<ShelfLocation> GetLocations()
        {
            List<ShelfLocation> locations = new List<ShelfLocation>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_SelectLocations", connection))
                    {
                        connection.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataReader reader = cmd.ExecuteReader();
                        ShelfLocation location;

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                {
                                    int Id = Convert.ToInt32(reader.GetValue(0));
                                    string Location = Convert.ToString(reader.GetValue(1));

                                    location = new ShelfLocation(Id, Location);
                                    locations.Add(location);
                                };

                            }
                        }
                    }
                    return locations;
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
            return null;
        }

        /// <summary>
        /// Method to get the identifier of the most recently created recipe within the database.
        /// </summary>
        /// <remarks>
        /// Utilises database stored procedure to select the maximum integer in the RecipeId column in the Recipe table.
        /// </remarks>
        /// <returns>An integer representing the unique identifier of recipe or null if exception is thrown.</returns>
        public static int GetLastRecipeId()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    int newRecipeId;

                    using (SqlCommand cmd = new SqlCommand("sp_SelectMaxRecipeId", connection))
                    {
                        connection.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        newRecipeId = Convert.ToInt32(cmd.ExecuteScalar());

                        return newRecipeId;
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
            return 0;
        }

        /// <summary>
        /// Method to get the collection of recipes of a given cookbook within the database.
        /// </summary>
        /// <remarks>
        /// Utilises database stored procedure to join the Recipe, Cookbook and ShelfLocation tables to select the Recipe ID, Recipe Name, Page, Shelf Location ID and Shelf Location Name. Parameters are used to create new <c>Recipe</c> class object.
        /// </remarks>
        /// <param name="isbn13">The unique identifier of the cookbook containing the collection of recipes.</param>
        /// <returns>A list of recipes associated with a single cookbook or null if exception is thrown.</returns>
        public static IEnumerable<Recipe> GetRecipesByCookbook(string isbn13)
        {
            List<Recipe> results = new List<Recipe>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_SelectRecipesByCookbook", connection))
                    {
                        connection.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@isbn13", isbn13);

                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int id = Convert.ToInt32(reader.GetValue(0));
                                string name = Convert.ToString(reader.GetValue(1));
                                string page = Convert.ToString(reader.GetValue(2));
                                string title = Convert.ToString(reader.GetValue(3));
                                string location = Convert.ToString(reader.GetValue(4));

                                Recipe recipe = new Recipe(id, name, page, title, location);
                                results.Add(recipe);
                            }
                        }
                    }
                    return results;
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
            return null;
        }

        /// <summary>
        /// Method to get a recipe from the database based on a known recipe identifier.
        /// </summary>
        /// <remarks>
        /// Utilises database stored procedure to join the Recipe, Cookbook and ShelfLocation tables to select the Recipe ID, Recipe Name, Page, Shelf Location ID and Shelf Location Name. Parameters are used to create new <c>Recipe</c> class object.
        /// </remarks>
        /// <param name="id">The unique recipe identifier.</param>
        /// <returns>A recipe object or null if exception is thrown.</returns>
        public static Recipe GetRecipeById(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_SelectRecipeById", connection))
                    {
                        connection.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id);

                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int recipeId = Convert.ToInt32(reader.GetValue(0));
                                string name = Convert.ToString(reader.GetValue(1));
                                string page = Convert.ToString(reader.GetValue(2));
                                string title = Convert.ToString(reader.GetValue(3));
                                string location = Convert.ToString(reader.GetValue(4));

                                Recipe recipe = new Recipe(id, name, page, title, location);
                                return recipe;
                            }
                        }
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
            return null;
        }

        /// <summary>
        /// Method to get the collection of recipes containing a chosen keyword from the database.
        /// </summary>
        /// <remarks>
        /// Utilises database stored procedure to join the Recipe, Cookbook and ShelfLocation tables to select the Recipe ID, Recipe Name, Page, Shelf Location ID and Shelf Location Name. Parameters are used to create new <c>Recipe</c> class object.
        /// </remarks>
        /// <param name="keyword">String of text to be used as a search team.</param>
        /// <returns>A list of integers representing recipes with a name containing the provided keyword or null if exceptionw as thown.</returns>
        public static IEnumerable<Recipe> GetRecipesByKeyword(string keyword)
        {
            List<Recipe> results = new List<Recipe>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_SelectRecipesByKeyword", connection))
                    {
                        connection.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@keyword", keyword);

                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int id = Convert.ToInt32(reader.GetValue(0));
                                string name = Convert.ToString(reader.GetValue(1));
                                string page = Convert.ToString(reader.GetValue(2));
                                string title = Convert.ToString(reader.GetValue(3));
                                string location = Convert.ToString(reader.GetValue(4));

                                Recipe recipe = new Recipe(id, name, page, title, location);
                                results.Add(recipe);
                            }
                        }
                    }
                    return results;
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
            return null;
        }

        /// <summary>
        /// Method to get the collection of recipe identifiers associated with given tag from the database.
        /// </summary>
        /// <remarks>
        /// Utilises database stored procedure to select the Recipe ID from the RecipeTag rows where the Tag ID matched. 
        /// </remarks>
        /// <param name="tagId">The unique identifier of tag.</param>
        /// <returns>A list of recipe identifiers associated with a given tag or null id an exception is thrown.</returns>
        public static List<int> GetRecipesByTag(int tagId)
        {
            try
            {
                List<int> listRecipeId = new List<int>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_SelectRecipesByTag", connection))
                    {
                        connection.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@tag", tagId);

                        SqlDataReader reader = cmd.ExecuteReader();

                        int recipeId;
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                recipeId = Convert.ToInt32(reader.GetValue(0));
                                listRecipeId.Add(recipeId);
                            }
                        }
                    }
                    return listRecipeId;
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
            return null;
        }

        /// <summary>
        /// Method to get the collection tags from the database.
        /// </summary>
        /// <remarks>
        /// Utilises database stored procedure to select Tag ID and Name from the Tag table. Parameters are used to create new <c>Tag</c> class objects.
        /// </remarks>
        /// <returns>A list of tags or null if exception was thrown.</returns>
        public static IEnumerable<Tag> GetTags()
        {
            List<Tag> tags = new List<Tag>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_SelectTags", connection))
                    {
                        connection.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataReader reader = cmd.ExecuteReader();
                        Tag tag;

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                {
                                    int Id = Convert.ToInt32(reader.GetValue(0));
                                    string Name = Convert.ToString(reader.GetValue(1));

                                    tag = new Tag(Id, Name);
                                    tags.Add(tag);
                                };
                            }
                        }
                    }
                    return tags;
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
            return null;
        }

        /// <summary>
        /// Method to get the collection of tags associated with a given recipe from the database.
        /// </summary>
        /// <remarks>
        /// Utilises database stored procedure to join the RecipeTag and Tag tables and select the Tag ID and Name. Parameters are used to create a new <c>Tag</c> class object.
        /// </remarks>
        /// <param name="recipeId">The unique identifier of the given recipe.</param>
        /// <returns>A list of tags associated with a given recipe or null if exception is thrown.</returns>
        public static IEnumerable<Tag> GetTagsByRecipe(int recipeId)
        {
            try
            {
                List<Tag> recipeTags = new List<Tag>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_SelectTagsByRecipe", connection))
                    {
                        connection.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@recipeId", recipeId);

                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int tagId = Convert.ToInt32(reader.GetValue(0));
                                string tagName = Convert.ToString(reader.GetValue(1));

                                Tag tag = new Tag(tagId, tagName);
                                recipeTags.Add(tag);
                            }
                        }
                    }
                    return recipeTags;
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
            return null;
        }


        /// <summary>
        /// Method to insert new cookbook into the database.
        /// </summary>
        /// <remarks>
        /// Utilises database stored procedure to insert a new row in the Cookbook table.
        /// </remarks>
        /// <param name="isbn13">The thirteen digit, unique identifier of the cookbook.</param>
        /// <param name="title">The title of the cookbook.</param>
        /// <param name="contributor">>Name of author(s) or organisation(s) responsible for producing the cookbook.</param>
        /// <param name="location">The ID of the location where the cookbook is currently shelved.</param>
        public static void InsertCookbook(string isbn13, string title, string contributor, int location)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_InsertCookbook", connection))
                    {
                        connection.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@isbn13", isbn13);
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@contributor", contributor);
                        cmd.Parameters.AddWithValue("@location", location);
                        var result = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
        }

        /// <summary>
        /// Method to insert a new recipe-tag relationship into the database.
        /// </summary>
        /// <remarks>
        /// Utilises database stored procedure to create new row in the RecipeTag table.
        /// </remarks>
        /// <param name="tagId">The id of the assiged tag.</param>
        /// <param name="recipeId">The id of the associated recipe.</param>
        public static void InsertRecipeTag(int tagId, int recipeId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_InsertRecipeTag", connection))
                    {
                        connection.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@tag", tagId);
                        cmd.Parameters.AddWithValue("@recipe", recipeId);
                        var result = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
        }


        /// <summary>
        /// Method to update existing cookbook in the database when ISBN-13 is unchanged.
        /// </summary>
        /// <remarks>
        /// Utilises database stored procedure to update the Title, COntributor and SHelf Location Reference in the Cookbook table.
        /// </remarks>
        /// <param name="isbn13">The thirteen digit, unique identifier of the cookbook.</param>
        /// <param name="title">The title of the cookbook.</param>
        /// <param name="contributor">>Name of author(s) or organisation(s) responsible for producing the cookbook.</param>
        /// <param name="location">The ID of the location where the cookbook is currently shelved.</param>
        public static void UpdateCookbook(string isbn13, string title, string contributor, int location)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateCookbook", connection))
                    {
                        connection.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@isbn13", isbn13);
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@contributor", contributor);
                        cmd.Parameters.AddWithValue("@location", location);
                        var result = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
        }

        /// <summary>
        /// Method to insert new or update existing location in database.
        /// </summary>
        /// <remarks>
        /// Utilises database stored procedure to insert new or update exisiting row in the ShelfLocation table. Identifier is used as a condition in the if/else statement to determine appropriate execution.
        /// </remarks>
        /// <param name="id">The location identifier. If new location awaiting assignment of identifier, id=0.</param>
        /// <param name="name">The location name.</param>
        public static void UpsertLocation(int id, string name)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_UpsertLocation", connection))
                    {
                        connection.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@location", name);
                        var result = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
        }

        /// <summary>
        /// Method to insert or update existing recipe in database.
        /// </summary>
        /// <remarks>
        /// Utilises database stored procedure to insert new or update exisiting row in the Recipe table. Identifier is used as a condition in the if/else statement to determine appropriate execution.
        /// </remarks>
        /// <param name="id">The recipe identifier. If new recipe awaiting assignment of identifier, id=0.</param>
        /// <param name="name">The name of recipe.</param>
        /// <param name="page">The page reference for recipe.</param>
        /// <param name="isbn13">The thirteen digit identifier of the cookbook that contains the recipe.</param>
        public static void UpsertRecipe(int id, string name, string page, string isbn13)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_UpsertRecipe", connection))
                    {
                        connection.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@page", page);
                        cmd.Parameters.AddWithValue("@isbn13", isbn13); 
                        var result = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
        }

        /// <summary>
        /// Method to insert new or update existing tag in database.
        /// </summary>
        /// <remarks>
        /// Utilises database stored procedure to insert new or update exisiting row in the Tag table. Identifier is used as a condition in the if/else statement to determine appropriate execution.
        /// </remarks>
        /// <param name="id">The tag identifier. If new tag awaiting assignment of identifier, id=0.</param>
        /// <param name="name">The tag name.</param>
        public static void UpsertTag(int id, string name)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_UpsertTag", connection))
                    {
                        connection.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@tag", name);
                        var result = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
        }
    }
}
