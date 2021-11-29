using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using Controller;
using Domain;

namespace UI
{
    /// <summary>
    /// Interaction logic for SearchPage.xaml
    /// </summary>
    public partial class SearchPage : Page
    {
        /// <summary>
        /// Constructor for Search page.
        /// </summary>
        public SearchPage()
        {
            InitializeComponent();
            LoadTagData();
        }

        /// <summary>
        /// Handler for button click event to display results of keyword search.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnSearchRecipe_Click(object sender, RoutedEventArgs e)
        {
            DisplaySearchResults(TBxSearchRecipeKeyword.Text);
            ClearSearchParameters();
        }

        /// <summary>
        /// Handler for button click event to display results of recipes by tag search.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnDisplayTaggedList_Click(object sender, RoutedEventArgs e)
        {
            if(CBxSelectTag.SelectedItem == null)
            {
                MessageBox.Show("Please select a tag for collection of recipes to be displayed.", "Invalid selection");
            }
            else
            {
                DisplaySearchResults((Tag)CBxSelectTag.SelectedItem);
                ClearSearchParameters();
            }
        }

        /// <summary>
        /// Handler for button click to manage tags.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnManageTags_Click(object sender, RoutedEventArgs e)
        {
            NavigateToManageTagsWindow();
        }

        /// <summary>
        /// Method to open a new <c>RecipeResultsPage</c>.
        /// </summary>
        /// <param name="keyword">Text to be used for search.</param>
        private void DisplaySearchResults(string keyword)
        {
            NavigationService.Navigate(new RecipeResultsPage(keyword));
        }

        /// <summary>
        /// Method to open a new <c>RecipesByTagPage</c>.
        /// </summary>
        /// <param name="tag">Selected tag to be used for search.</param>
        private void DisplaySearchResults(Tag tag)
        {
            NavigationService.Navigate(new RecipesByTagPage(tag));
        }

        /// <summary>
        /// Handler for button click event to return to home page.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FunctionsPage());
        }

        /// <summary>
        /// Method to get tag data and populate the combobox.
        /// </summary>
        private void LoadTagData()
        {
            List<Tag> tagList = (List<Tag>)ViewModel.GetTags();
            CBxSelectTag.ItemsSource = tagList;
        }

        /// <summary>
        /// Method to initialise a new <c>ManageTagsWindow</c>.
        /// </summary>
        /// <remarks>ManageTagsWindow_Closed event is also initialised in this method.</remarks>
        private void NavigateToManageTagsWindow()
        {
            ManageTagsWindow window = new ManageTagsWindow(false);
            window.Closed += ManageTagsWindow_Closed;
            window.ShowDialog();
        }

        /// <summary>
        /// Method to initialise re-loading of available and assigned tags after <c>ManageTagsWindow</c> is closed.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event argument.</param>
        private void ManageTagsWindow_Closed(object sender, EventArgs e)
        {
            LoadTagData();
        }

        /// <summary>
        /// Method to revert search parameters to default values.
        /// </summary>
        private void ClearSearchParameters()
        {
            TBxSearchRecipeKeyword.Text = "";
            CBxSelectTag.SelectedItem = null;
        }
    }
}
