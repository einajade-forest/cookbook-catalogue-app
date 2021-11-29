using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;

using Controller;
using Domain;

namespace UI
{
    /// <summary>
    /// Interaction logic for AddBookWindow.xaml
    /// </summary>
    public partial class AddBookWindow : Window
    {
        /// <summary>
        /// Field to instantiate a cookbook for the <c>AddBookWindow</c> class to call.
        /// </summary>
        private readonly Cookbook placeholder;
        /// <summary>
        /// Field to instantiate a boolean for the purposes of determining whether closing window equates to discarding all inputs and changes for the <c>AddBookWindow</c> class to call.
        /// </summary>
        private bool isCancel = true;

        /// <summary>
        /// Constructor for the Add Book window.
        /// </summary>
        public AddBookWindow()
        {
            InitializeComponent();
            placeholder = new Cookbook();
            LoadLocationData();
        }

        /// <summary>
        /// Handler for button click event to add recipe to current cookbook.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnAddRecipes_Click(object sender, RoutedEventArgs e)
        {
            placeholder.Isbn13 = TBxCookbookIsbn13.Text;
            NavigateToAddRecipeWindow(placeholder);
        }

        /// <summary>
        /// Handler for button click event to edit selected recipe.
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
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnDeleteRecipe_Click(object sender, RoutedEventArgs e)
        {
            Recipe selectedRecipe = (Recipe)DgrdBookRecipesList.CurrentItem;
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the recipe for " + selectedRecipe.RecipeName + "?", "Alert", MessageBoxButton.YesNo);
            if (MessageBoxResult.Yes == result)
            {
                placeholder.CookbookRecipes.Remove(selectedRecipe);
            }
            LoadRecipesData();
        }

        /// <summary>
        /// Handler for button click event to create a new cookbook.
        /// </summary>
        /// <remarks>Calls method to validate input data.</remarks>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnAddBook_Click(object sender, RoutedEventArgs e)
        {
            string isbn13 = TBxCookbookIsbn13.Text;
            string title = TBxCookbookTitle.Text;
            string contributor = TBxCookbookContributor.Text;
            ShelfLocation location = (ShelfLocation)CBxCookbookLocation.SelectedItem;

            ValidateInput(isbn13, title, contributor, location);
        }

        /// <summary>
        /// Handler for button click event to close window without saving anything to the database.
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
                MessageBoxResult result = MessageBox.Show("Are you sure you want to discard this new cookbook (and recipes)?", "Alert", MessageBoxButton.YesNo);
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
        }

        /// <summary>
        /// Method to get recipe data and populate datagrid items.
        /// </summary>
        private void LoadRecipesData()
        {
            DataContext = null;
            DgrdBookRecipesList.Items.Clear();
            DataContext = placeholder;
            foreach (Recipe recipe in placeholder.CookbookRecipes)
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
        /// <param name="placeholder">The current <c>Cookbook</c>.</param>
        private void NavigateToAddRecipeWindow(Cookbook placeholder)
        {
            AddRecipeWindow window = new AddRecipeWindow(placeholder);
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
        /// Method to initialise loading or re-loading of recipes data after <c>AddRecipeWindow</c> is closed.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event argument.</param>
        private void AddRecipeWindow_Closed(object sender, EventArgs e)
        {
            LoadRecipesData();
        }

        /// <summary>
        /// Method to initialise loading or re-loading of recipes data after <c>EditRecipeWindow</c> is closed.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event Argument.</param>
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
        /// </remarks>
        /// <param name="isbn13">Unique thirteen digit identifier for the cookbook.</param>
        /// <param name="title">Title of the cookbook.</param>
        /// <param name="contributor">Author(s) or organisation(s) responsible for producing the cookbook.</param>
        /// <param name="location">The selected <c>ShelfLocation</c> object.</param>
        private void ValidateInput(string isbn13, string title, string contributor, ShelfLocation location)
        {
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
                    Regex regex = new Regex("^[0-9]{13}");
                    if (regex.IsMatch(isbn13))
                    {
                        List<Cookbook> existingCookbooks = (List<Cookbook>)ViewModel.GetCookbooks();
                        bool hasIsbn13 = existingCookbooks.Exists(c => c.Isbn13 == isbn13);

                        if (hasIsbn13 == true)
                        {
                            MessageBox.Show("This ISBN-13 has already been associated with another cookbook. Please review.", "Invalid ISBN-13");
                        }
                        else
                        {
                            if (location == null)
                            {
                                MessageBox.Show("Please assign a shelf location for this cookbook.", "Invalid shelf location");
                            }
                            else
                            {
                                isCancel = false;
                                ViewModel.InsertCookbook(isbn13, title, contributor, location, placeholder.CookbookRecipes);                                
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