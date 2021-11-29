using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Controller;
using Domain;

namespace UI
{
    /// <summary>
    /// Interaction logic for ManageLocationsWindow.xaml
    /// </summary>
    public partial class ManageLocationsWindow : Window
    {
        /// <summary>
        /// Field to instantiate a collection of shelf locations for the <c>ManageLocationsWindow</c> class to call.
        /// </summary>
        private List<ShelfLocation> locationList;
        /// <summary>
        /// Field to instantiate a shelf location for the <c>ManageLocationsWindow</c> class to call.
        /// </summary>
        private ShelfLocation selectedLocation;

        /// <summary>
        /// Constructor for the Manage Locations window.
        /// </summary>
        public ManageLocationsWindow()
        {
            InitializeComponent();
            LoadData();
        }

        /// <summary>
        /// Handler for button click event to add or update shelf location.
        /// </summary>
        /// <remarks>Location name is sent to be validated.</remarks>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnSaveLocation_Click(object sender, RoutedEventArgs e)
        {
            ValidateInput(TBxLocationName.Text);
        }

        /// <summary>
        /// Handler for button click event to remove shelf location.
        /// </summary>
        /// <remarks>
        /// Checks whether there are any cookbooks associated with the location. User is alerted if the number of associated cookbooks is greater than zero and event is cancelled.
        /// User to confirm deletion.</remarks>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnDeleteLocation_Click(object sender, RoutedEventArgs e)
        {
            selectedLocation = (ShelfLocation)DgrdLocationsList.CurrentItem;

            if(selectedLocation.BookCount == 0)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the " + selectedLocation.Location + " location?", "Alert", MessageBoxButton.YesNo);
                if (MessageBoxResult.Yes == result)
                {
                    ViewModel.DeleteLocation(selectedLocation);
                }
                ClearSelection();
                LoadData();
            }
            else
            {
                MessageBox.Show("Cannot delete selected location as there are cookbooks associated with it.\nPlease update location of these books first.", "Alert");
            }

        }

        /// <summary>
        /// Handler for button click event to close window.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Method to get location data and populate datagrid.
        /// </summary>
        private void LoadData()
        {
            DgrdLocationsList.Items.Clear();
            locationList = (List<ShelfLocation>)ViewModel.GetLocations();
            foreach (ShelfLocation location in locationList)
            {
                DgrdLocationsList.Items.Add(location);
            }
        }

        /// <summary>
        /// Method to verify that the appropriate data has been provided for shelf location to be upserted in the database.
        /// </summary>
        /// <remarks>
        /// If new location name is not provided, user will be prompted to input a new value and event cancelled.
        /// If new location name already exists in the database, user will be prompted to input a new value and event cancelled.
        /// </remarks>
        /// <param name="name">Name of shelf location.</param>
        private void ValidateInput(string name)
        {
            if(name != "< Enter new location name >")
            {
                name = name.ToUpper();

                if (locationList.Exists(x => x.Location == name))
                {
                    MessageBox.Show("This location already exists. Please try another name.", "Invalid location");
                    TBxLocationName.Text = "< Enter new location name >";
                    TBxLocationName.FontStyle = FontStyles.Italic;
                    TBxLocationName.Foreground = Brushes.Gray;
                    TBxLocationName.Focus();
                }
                else
                {
                    Regex regex = new Regex(@"^[a-zA-Z]");
                    if (regex.IsMatch(name))
                    {
                        int id;
                        string origLocation = LblSelectedLocation.Content.ToString();
                        if (origLocation != "< Select location from table (if req'd) >")
                        {
                            id = selectedLocation._id;
                        } else
                        {
                            id = 0;
                        }

                        ViewModel.UpsertLocation(id, name);
                        ClearSelection();
                        TBxLocationName.Text = "< Enter new location name >";
                        TBxLocationName.FontStyle = FontStyles.Italic;
                        TBxLocationName.Foreground = Brushes.Gray;
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Please review the entered text. Location did not start with an alphabetic character.", "Invalid location");
                        TBxLocationName.Focus();
                    }

                }
            }
            else
            {
                MessageBox.Show("Please enter a new location name.","Alert");
                TBxLocationName.Focus();
            }          
        }

        /// <summary>
        /// Handler for text change event to revert location name textbox style to default.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Text Changed Event Argument.</param>
        private void TBxLocationName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(((TextBox)sender).Foreground == Brushes.Gray)
            {
                ((TextBox)sender).FontStyle = FontStyles.Normal;
                ((TextBox)sender).Foreground = Brushes.Black;
            }
        }

        /// <summary>
        /// Handler for selection change event to revert selected location name textbox style to default.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Selection Changed Event Argument.</param>
        private void DgrdLocationsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgrdLocationsList.SelectedItem != null)
            {
                selectedLocation = (ShelfLocation)DgrdLocationsList.SelectedItem;

                LblSelectedLocation.FontStyle = FontStyles.Normal;
                LblSelectedLocation.FontWeight = FontWeights.Bold;
                LblSelectedLocation.Foreground = Brushes.Black;
                LblSelectedLocation.Content = selectedLocation.Location;
                LblUpdateTo.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Handler for button click event to clear datagrid selection and revert textboxes to the default.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnClearSelected_Click(object sender, RoutedEventArgs e)
        {
            ClearSelection();
        }

        /// <summary>
        /// Method to clear datagrid selection and revert text in textboxes to default values and styling.
        /// </summary>
        private void ClearSelection()
        {
            DgrdLocationsList.SelectedItem = null;
            LblSelectedLocation.FontStyle = FontStyles.Italic;
            LblSelectedLocation.FontWeight = FontWeights.Normal;
            LblSelectedLocation.Foreground = Brushes.Gray;
            LblSelectedLocation.Content = "< Select location from table (if req'd) >";
            LblUpdateTo.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Handler for when location name textbox is in focus.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void TBxLocationName_GotFocus(object sender, RoutedEventArgs e)
        {
            TextSelectAll();
        }

        /// <summary>
        /// Handler for mouse double click event associated with the location name textbox.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Mouse Button Event Argment.</param>
        private void TBxLocationName_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextSelectAll();
        }

        /// <summary>
        /// Method to select all text within location name textbox.
        /// </summary>
        private void TextSelectAll()
        {
            TBxLocationName.SelectAll();
        }
    }
}
