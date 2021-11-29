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
    /// Interaction logic for BooksPage.xaml
    /// </summary>
    public partial class BooksPage : Page
    {
        /// <summary>
        /// Field to instantiate a <c>Cookbook</c> for the <c>BooksPage</c> class to call.
        /// </summary>
        private Cookbook selectedCookbook;

        /// <summary>
        /// Constructor for the Cookbooks page.
        /// </summary>
        public BooksPage()
        {
            InitializeComponent();
            LoadData();

            if(DgrdBookResults.Items.Count == 0)
            {
                TBxNoResults.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Handler for button click event to add new cookbook.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnAddBook_Click(object sender, RoutedEventArgs e)
        {
            NavigateToAddBookWindow();
        }

        /// <summary>
        /// Handler for button click event to edit selected cookbook.
        /// </summary>
        /// <remarks>Checks if a cookbook has been selected on the datagrid. User is alerted if no cookbook has been selected.</remarks>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnEditBook_Click(object sender, RoutedEventArgs e)
        {
            if (DgrdBookResults.SelectedItem == null)
            {
                MessageBox.Show("Please select the desired cookbook from the table to edit details.", "Alert");
            }
            else
            {
                NavigateToEditBookWindow();
            }
        }

        /// <summary>
        /// Handler for button click event to delete selected cookbook.
        /// </summary>
        /// <remarks>
        /// Checks if a cookbook has been selected on the datagrid. User is alerted if no cookbook has been selected.
        ///User is asked to confirm cookbook deletion.
        ///</remarks>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnDeleteBook_Click(object sender, RoutedEventArgs e)
        {
            if (DgrdBookResults.SelectedItem == null)
            {
                MessageBox.Show("Please select the desired cookbook from the table to delete.", "Alert");
            }
            else
            {
                selectedCookbook = (Cookbook)DgrdBookResults.SelectedItem;
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete " + selectedCookbook.Title + "?", "Alert", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    ViewModel.DeleteCookbook(selectedCookbook.Isbn13, selectedCookbook.CookbookRecipes);
                    LoadData();
                }
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
        /// Handler for button click to manage locations.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnManageLocations_Click(object sender, RoutedEventArgs e)
        {
            NavigateManageLocationsWindow();
        }

        /// <summary>
        /// Handler for button click to return to the home page.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FunctionsPage());
        }

        /// <summary>
        /// Method to get cookbook data to populate datagrid.
        /// </summary>
        private void LoadData()
        {
            DgrdBookResults.Items.Clear();

            List<Cookbook> bookList = (List<Cookbook>)ViewModel.GetCookbooks();
            foreach (Cookbook book in bookList)
            {
                DgrdBookResults.Items.Add(book);
            }
        }

        /// <summary>
        /// Method to initialise a new <c>AddBookWindow</c>.
        /// </summary>
        /// <remarks>AddBookWindow_Closed event is initialised in this method.</remarks>
        private void NavigateToAddBookWindow()
        {
            AddBookWindow window = new AddBookWindow();
            window.Closed += AddBookWindow_Closed;
            window.ShowDialog();
        }

        /// <summary>
        /// Method to initialise a new <c>EditBookWindow</c>.
        /// </summary>
        /// <remarks>
        /// Details of selected cookbook passes to new window to allow values to be updated.
        /// EditRecipeWindow_Closed event is also initialised in this method.
        /// </remarks>
        private void NavigateToEditBookWindow()
        {
            selectedCookbook = (Cookbook)DgrdBookResults.SelectedItem;

            EditBookWindow window = new EditBookWindow(selectedCookbook);
            window.Closed += EditBookWindow_Closed;
            window.ShowDialog();
        }

        /// <summary>
        /// Method to initialise a new <c>ManageTagsWindow</c>.
        /// </summary>
        /// <remarks>ManageTagsWindow_Closed event is also initialised in this method.</remarks>
        private void NavigateToManageTagsWindow()
        {
            ManageTagsWindow window = new ManageTagsWindow(false);
            window.ShowDialog();
        }

        /// <summary>
        /// Method to initialise a new <c>ManageLocationsWindow</c>.
        /// </summary>
        private void NavigateManageLocationsWindow()
        {
            ManageLocationsWindow window = new ManageLocationsWindow();
            window.ShowDialog();
        }

        /// <summary>
        /// Method to initialise re-loading of cookbook data after <c>AddBookWindow</c> is closed.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event argument.</param>
        private void AddBookWindow_Closed(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// Method to initialise re-loading of cookbook data after <c>EditBookWindow</c> is closed.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event argument.</param>
        private void EditBookWindow_Closed(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
