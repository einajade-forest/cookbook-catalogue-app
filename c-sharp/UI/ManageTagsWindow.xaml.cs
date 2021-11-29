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
    /// Interaction logic for ManageTagsWindow.xaml
    /// </summary>
    public partial class ManageTagsWindow : Window
    {
        /// <summary>
        /// Field to instantiate a collection of tags for the <c>ManageTagsWindow</c> class to call.
        /// </summary>
        private List<Tag> tagList;
        /// <summary>
        /// Field to instantiate a tag for the <c>ManageTagsWindow</c> class to call.
        /// </summary>
        private Tag selectedTag;
        /// <summary>
        /// Field to instantiate a string representing the original name of the selected <c>Tag</c> object for the <c>ManageTagsWindow</c> class to call.
        /// </summary>
        private string origTagName;
        /// <summary>
        /// Field to instantiate a boolean for the purposes of determining whether the delete function is to be disabled.
        /// </summary>
        private bool openFromRecipe = false;

        /// <summary>
        /// Constructor for the Manage tags window.
        /// </summary>
        /// <param name="fromRecipeWindow">Boolean value advising whether the new window has been initiated from a recipe management window.</param>
        public ManageTagsWindow(bool fromRecipeWindow)
        {
            openFromRecipe = fromRecipeWindow;
            InitializeComponent();
            if(openFromRecipe == true)
            {
                TBlkDeleteMsg.Text = "*** Note:  Tag deletion is currently disabled.\nTo delete unwanted tags, please launch the Manage Tags window via the Search or Cookbooks pages.";
                TBlkDeleteMsg.Foreground = Brushes.Red;
                TBlkDeleteMsg.FontStyle = FontStyles.Italic;
                TBlkNote.Visibility = Visibility.Hidden;
            }
            LoadData(openFromRecipe);
        }

        /// <summary>
        /// Handler for button click event to add or update tag.
        /// </summary>
        /// <remarks>Tag name is sent to be validated.</remarks>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnSaveTag_Click(object sender, RoutedEventArgs e)
        {
            ValidateInput(TBxTagName.Text);
        }

        /// <summary>
        /// Handler for button click event to remove tag.
        /// </summary>
        /// <remarks>
        /// Checks whether there are any recipes associated with the tag. User is alerted if the number of associated recipes is greater than zero and event is cancelled.
        /// User to confirm deletion.</remarks>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnDeleteTag_Click(object sender, RoutedEventArgs e)
        {
            selectedTag = (Tag)DgrdTagsList.CurrentItem;
            if (selectedTag.RecipeCount == 0)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the " + selectedTag.TagName + " tag?", "Alert", MessageBoxButton.YesNo);
                if (MessageBoxResult.Yes == result)
                {
                    ViewModel.DeleteTag(selectedTag);
                }
                ClearSelection();
                LoadData(openFromRecipe);
            }
            else
            {
                MessageBox.Show("Cannot delete selected tag as there are recipes associated with it.\nPlease update tags of these books first.", "Alert");
                ClearSelection();
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
        /// Method to get tag data and populate datagrid.
        /// </summary>
        /// <param name="deleteDisabled">Boolean value to advise if delete function should be disabled</param>
        private void LoadData(bool deleteDisabled)
        {
            DgrdTagsList.Items.Clear();
            tagList = (List<Tag>)ViewModel.GetTags();
            foreach (Tag tag in tagList)
            {
                DgrdTagsList.Items.Add(tag);
            }

            if(deleteDisabled == true)
            {
                    DgrdTagsList.Columns[1].Visibility = Visibility.Hidden;
                    DgrdTagsList.Columns[2].Visibility = Visibility.Hidden;
                DgrdTagsList.Columns[0].Width = 294;
            }
        }

        /// <summary>
        /// Method to verify that the appropriate data has been provided for tag to be upserted in the database.
        /// </summary>
        /// <remarks>
        /// If new tag name is not provided, user will be prompted to input a new value and event cancelled.
        /// If new tag name already exists in the database, user will be prompted to input a new value and event cancelled.
        /// </remarks>
        /// <param name="name">Name of tag.</param>
        private void ValidateInput(string name)
        {
            if (name != "< Enter new tag name >")
            {
                name = name.ToUpper();

                if (tagList.Exists(x => x.TagName == name))
                {
                    MessageBox.Show("This tag already exists. Please try another name.", "Invalid tag");
                    TBxTagName.Text = "< Enter new tag name >";
                    TBxTagName.FontStyle = FontStyles.Italic;
                    TBxTagName.Foreground = Brushes.Gray;
                    TBxTagName.Focus();
                }
                else
                {
                    Regex regex = new Regex(@"^[a-zA-Z]");
                    if (regex.IsMatch(name))
                    {
                        int id;
                        origTagName = LblSelectedTag.Content.ToString();
                        if (origTagName != "< Select tag from table (if req'd) >")
                        {
                            id = selectedTag.TagId;
                        }
                        else
                        {
                            id = 0;
                        }

                        ViewModel.UpsertTag(id, name);
                        ClearSelection();
                        TBxTagName.Text = "< Enter new tag name >";
                        TBxTagName.FontStyle = FontStyles.Italic;
                        TBxTagName.Foreground = Brushes.Gray;
                        LoadData(openFromRecipe);
                    }
                    else
                    {
                        MessageBox.Show("Please review the entered text. Tag did not start with an alphabetic character.", "Invalid tag");
                        TBxTagName.Focus();
                    }

                }
            }
            else
            {
                MessageBox.Show("Please enter a new tag name.", "Alert");
                TBxTagName.Focus();
            }                
        }

        /// <summary>
        /// Handler for text change event to revert tag name textbox style to default.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Text Changed Event Argument.</param>
        private void TBxTagName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).Foreground == Brushes.Gray)
            {
                ((TextBox)sender).FontStyle = FontStyles.Normal;
                ((TextBox)sender).Foreground = Brushes.Black;
            }
        }

        /// <summary>
        /// Handler for selection change event to revert selected tag name textbox style to default.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Selection Changed Event Argument.</param>
        private void DgrdTagsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgrdTagsList.SelectedItem != null)
            {
                selectedTag = (Tag)DgrdTagsList.SelectedItem;

                LblSelectedTag.FontStyle = FontStyles.Normal;
                LblSelectedTag.FontWeight = FontWeights.Bold;
                LblSelectedTag.Foreground = Brushes.Black;
                LblSelectedTag.Content = selectedTag.TagName;
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
            DgrdTagsList.SelectedItem = null;
            LblSelectedTag.FontStyle = FontStyles.Italic;
            LblSelectedTag.FontWeight = FontWeights.Normal;
            LblSelectedTag.Foreground = Brushes.Gray;
            LblSelectedTag.Content = "< Select tag from table (if req'd) >";
            LblUpdateTo.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Handler for when tag name textbox is in focus.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void TBxTagName_GotFocus(object sender, RoutedEventArgs e)
        {
            TextSelectAll();
        }

        /// <summary>
        /// Handler for mouse double click event associated with the tag name textbox.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Mouse Button Event Argment.</param>
        private void TBxTagName_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextSelectAll();
        }

        /// <summary>
        /// Method to select all text within tag name textbox.
        /// </summary>
        private void TextSelectAll()
        {
            TBxTagName.SelectAll();
        }
    }
}
