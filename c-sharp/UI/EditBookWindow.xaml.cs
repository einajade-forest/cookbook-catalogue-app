using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;

using Controller;
using Domain;

namespace UI
{
    /// <summary>
    /// Interaction logic for EditBookWindow.xaml
    /// </summary>
    public partial class EditBookWindow : Window
    {
        /// <summary>
        /// Field to instantiate a <c>Cookbook</c> for the <c>EditBookWindow</c> class to call.
        /// </summary>
        private readonly Cookbook cookbook;
        /// <summary>
        /// Field to instantiate a collection of <c>Recipe</c> objects that will need to be deleted from the database for the <c>EditBookWindow</c> class to call.
        /// </summary>
        private readonly List<Recipe> tempDeletedRecipes = new List<Recipe>();
        /// <summary>
        /// Field to instantiate a boolean for the purposes of determining whether closing window equates to discarding all inputs and changes for the <c>EditBookWindow</c> class to call.
        /// </summary>
        private bool isCancel = true;

        /// <summary>
        /// Constructor for the Edit Book window.
        /// </summary>
        /// <param name="selectedBook"><c>Cookbook</c> selected for editing from the <c>BookPage</c>.</param>
        public EditBookWindow(Cookbook selectedBook)
        {
            InitializeComponent();
            cookbook = selectedBook;

            TBxCookbookTitle.Text = cookbook.Title;
            TBxCookbookContributor.Text = cookbook.Contributor;
            TBxCookbookIsbn13.Text = cookbook.Isbn13;

            LoadLocationData();
            LoadRecipesData();
        }

        /// <summary>
        /// Handler for button click event to add recipe to current cookbook.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnAddRecipes_Click(object sender, RoutedEventArgs e)
        {
            NavigateToAddRecipeWindow(cookbook);
        }

        /// <summary>
        /// Handler for button click event to add recipe to current cookbook.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnEditRecipe_Click(object sender, RoutedEventArgs e)
        {
            NavigateToEditRecipeWindow((Recipe)DgrdBookRecipesList.CurrentItem);
        }

        /// <summary>
        /// Handler for button click event to delete selected recipe.
        /// </summary>
        /// <remarks>Recipe will not be deleted from the database until user commits to all changes to the cookbook.</remarks>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnDeleteRecipe_Click(object sender, RoutedEventArgs e)
        {
            Recipe selectedRecipe = (Recipe)DgrdBookRecipesList.CurrentItem;
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the recipe for " + selectedRecipe.RecipeName + "?", "Alert", MessageBoxButton.YesNo);
            if (MessageBoxResult.Yes == result)
            {
                tempDeletedRecipes.Add(selectedRecipe);
                cookbook.CookbookRecipes.Remove(selectedRecipe);
            }
            LoadRecipesData();
        }

        /// <summary>
        /// Handler for button click event to commit changes to the cookbook.
        /// </summary>
        /// <remarks>Calls method to validate input data.</remarks>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnSaveBook_Click(object sender, RoutedEventArgs e)
        {
            string title = TBxCookbookTitle.Text;
            string contributor = TBxCookbookContributor.Text;
            string isbn13 = TBxCookbookIsbn13.Text;
            ShelfLocation location = (ShelfLocation)CBxCookbookLocation.SelectedItem;

            ValidateInput(isbn13, title, contributor, location);
        }

        /// <summary>
        /// Handler for button click event to close window without updating the database.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handler for when the Close Window method is initialised.
        /// </summary>
        /// <remarks>Checks whether window is to be cancelled or for it to closed and continue saving data to the database.</remarks>
        /// <param name="sender">The object raised by the event.</param>
        /// <param name="e">Cancel Event Argument.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isCancel)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to discard all changes?", "Alert", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Method to get shelf location data and populate combobox items.
        /// </summary>
        private void LoadLocationData()
        {
            List<ShelfLocation> locationList = (List<ShelfLocation>)ViewModel.GetLocations();
            CBxCookbookLocation.ItemsSource = locationList;
            CBxCookbookLocation.SelectedItem = locationList.Find(x => x.Location == cookbook.LocationName);
        }

        /// <summary>
        /// Method to get recipe data and populate datagrid items.
        /// </summary>
        public void LoadRecipesData()
        {
            DataContext = null;
            DgrdBookRecipesList.Items.Clear();
            DataContext = cookbook.CookbookRecipes;
            foreach (Recipe recipe in cookbook.CookbookRecipes)
            {
                DgrdBookRecipesList.Items.Add(recipe);
            }
        }

        /// <summary>
        /// Method to initialise a new <c>AddRecipeWindow</c>.
        /// </summary>
        /// <remarks>
        /// Cookbook details passes to new window to allow new recipe to be added to its collection of <c>Recipe</c> objects.
        /// AddRecipeWindow_Closed event is also initialised in this method.
        /// </remarks>
        /// <param name="currentBook">The currently selected <c>Cookbook</c>.</param>
        private void NavigateToAddRecipeWindow(Cookbook currentBook)
        {
            AddRecipeWindow window = new AddRecipeWindow(currentBook);
            window.Closed += AddRecipeWindow_Closed;
            window.ShowDialog();

        }

        /// <summary>
        /// Method to initialise a new <c>EditRecipeWindow</c>.
        /// </summary>
        /// <remarks>
        /// Recipe details passes to new window to allow values to be updated.
        /// EditRecipeWindow_Closed event is also initialised in this method.
        /// </remarks>
        /// <param name="selectedRecipe">The selected <c>Recipe</c> object.</param>
        private void NavigateToEditRecipeWindow(Recipe selectedRecipe)
        {
            EditRecipeWindow window = new EditRecipeWindow(TBxCookbookIsbn13.Text, selectedRecipe);
            window.Closed += EditRecipeWindow_Closed;
            window.ShowDialog();
        }

        /// <summary>
        /// Method to initialise re-loading of recipes data after <c>AddRecipeWindow</c> is closed.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event argument.</param>
        private void AddRecipeWindow_Closed(object sender, EventArgs e)
        {
            LoadRecipesData();
        }

        /// <summary>
        /// Method to initialise re-loading of recipes data after <c>EditRecipeWindow</c> is closed.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event argument.</param>
        private void EditRecipeWindow_Closed(object sender, EventArgs e)
        {
            LoadRecipesData();
        }

        /// <summary>
        /// Method to verify that the appropriate data has been provided for a cookbook to be inserted into the database.
        /// </summary>
        /// <remarks>
        /// If any field is empty, user will be prompted to input a new value and event cancelled.
        /// If the entered ISBN-13 does not adhere to the thirteen digit constraint, user will be prompted to input a new value and event cancelled.
        ///If ISBN-13 value has changed, user will be required to confirm that change was intentional and new value does not match any other ISBN-13 value in the database.
        /// </remarks>
        /// <param name="isbn13">Unique thirteen digit identifier for the cookbook.</param>
        /// <param name="title">Title of the cookbook.</param>
        /// <param name="contributor">Author(s) or organisation(s) responsible for producing the cookbook.</param>
        /// <param name="location">The selected <c>ShelfLocation</c> object.</param>
        private void ValidateInput(string isbn13, string title, string contributor, ShelfLocation location)
        {
            string originalIsbn13 = cookbook.Isbn13;

            if (title == "")
            {
                MessageBox.Show("Please enter a title.", "Invalid Title");
            }
            else
            {
                if (contributor == "")
                {
                    MessageBox.Show("Please enter an author/authors or the organisation responsible for producing the cookbook.", "Invalid contributor(s)");
                }
                else
                {
                    if (isbn13 == "")
                    {
                        MessageBox.Show("Please enter an ISBN-13.", "Invalid ISBN-13");
                    }
                    else
                    {
                        Regex regex = new Regex("^[0-9]{13}");
                        if (regex.IsMatch(isbn13))
                        {
                            if ( isbn13 != originalIsbn13)
                            {
                                MessageBoxResult msgResult = MessageBox.Show("You have altered the cookbook's ISBN-13. This property does not typically change.\nAre you making a deliberate correction?","Alert",MessageBoxButton.YesNo);
                                if(msgResult == MessageBoxResult.Yes)
                                {
                                    List<Cookbook> existingCookbooks = (List<Cookbook>)ViewModel.GetCookbooks();
                                    existingCookbooks.Remove(cookbook);
                                    bool hasIsbn13 = existingCookbooks.Exists(c => c.Isbn13 == isbn13);

                                    if (hasIsbn13)
                                    {
                                        MessageBox.Show("This ISBN-13 has already been associated with another cookbook. Please first edit the other cookbook before applying this change.", "Invalid ISBN-13");
                                    }
                                    else
                                    {
                                        if (location == null)
                                        {
                                            MessageBox.Show("Please assign a shelf location for this cookbook.", "Invalid shelf location");
                                        }
                                        else
                                        {
                                            ViewModel.UpdateCookbook(originalIsbn13, isbn13, title, contributor, location, cookbook.CookbookRecipes);
                                            isCancel = false;
                                            Close();
                                        }
                                    }
                                }
                                if (msgResult == MessageBoxResult.No)
                                {
                                    MessageBox.Show("ISBN-13 will be reverted back to last saved value.", "Alert");
                                    isbn13 = originalIsbn13;

                                    if (location == null)
                                    {
                                        MessageBox.Show("Please assign a shelf location for this cookbook.", "Invalid shelf location");
                                    }
                                    else
                                    {
                                        ViewModel.UpdateCookbook(originalIsbn13, isbn13, title, contributor, location, cookbook.CookbookRecipes);
                                        isCancel = false;
                                        Close();
                                    }
                                }
                            }
                            else
                            {
                                if (location == null)
                                {
                                    MessageBox.Show("Please assign a shelf location for this cookbook.", "Invalid shelf location");
                                }
                                else
                                {
                                    foreach (Recipe deletedRecipe in tempDeletedRecipes)
                                    {
                                        ViewModel.DeleteRecipe(deletedRecipe);
                                    }
                                    ViewModel.UpdateCookbook(originalIsbn13, isbn13, title, contributor, location, cookbook.CookbookRecipes);
                                    isCancel = false;
                                    Close();
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("The cookbook identifier should have 13 digits with no spaces or special characters.\nPlease review.", "Invalid ISBN-13");
                        }
                    }
                }
            }
        }
    }
}
