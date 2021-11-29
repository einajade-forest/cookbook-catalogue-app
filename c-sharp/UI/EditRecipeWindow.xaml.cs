using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;

using Controller;
using Domain;

namespace UI
{
    /// <summary>
    /// Interaction logic for EditRecipeWindow.xaml
    /// </summary>
    public partial class EditRecipeWindow : Window
    {
        /// <summary>
        /// Field to instantiate a recipe for the <c>EditRecipeWindow</c> to call.
        /// </summary>
        private readonly Recipe recipe;
        /// <summary>
        /// Field to instantiate a collection of <c>tag</c> objects for the <c>EditRecipeWindow</c> class to call.
        /// </summary>
        /// <remarks>List contains <c>Tag</c> objects available to be assigned to the recipe.</remarks>
        private List<Tag> tagList;
        ///<summary>
        /// Field to instantiate a collection of <c>tag</c> objects for the <c>EditRecipeWindow</c> class to call.
        /// </summary>
        /// <remarks>List contains <c>Tag</c> objects currently assigned to the recipe.</remarks>
        private List<Tag> assignedTagList;
        /// <summary>
        /// Field to instantiate a boolean for the purposes of determining whether closing window equates to discarding all inputs and changes for the <c>EditRecipeWindow</c> class to call.
        /// </summary>
        private bool isCancel = true;

        /// <summary>
        /// Constructor for the Edit Recipe window.
        /// </summary>
        /// <param name="isbn13">ISBN-13 of the cookbook containing the selected recipe.</param>
        /// <param name="selectedRecipe">The selected <c>Recipe</c> object.</param>
        public EditRecipeWindow(string isbn13, Recipe selectedRecipe)
        {
            InitializeComponent();
            recipe = selectedRecipe;
            
            LblIsbn13.Content = isbn13;
            TBxRecipeName.Text = recipe.RecipeName;
            TBxRecipePage.Text = recipe.Page;

            LoadTagData();
        }

        /// <summary>
        /// Handler for button click to assign tag to current recipe.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnAssignTag_Click(object sender, RoutedEventArgs e)
        {
            foreach (Tag tag in LstTag.SelectedItems)
            {
                assignedTagList.Add(tag);
                tagList.Remove(tag);
            }
            LstTag.Items.Refresh();

            LstAssignedTags.ItemsSource = assignedTagList;
            LstAssignedTags.Items.Refresh();
        }

        /// <summary>
        /// Handler for button click to unassign tag from current recipe.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnRevokeTag_Click(object sender, RoutedEventArgs e)
        {
            List<Tag> verifiedTags = (List<Tag>)ViewModel.GetTags();

            foreach (Tag tag in LstAssignedTags.SelectedItems)
            {
                if (verifiedTags.Exists(x => x.TagName == tag.TagName))
                {
                    tagList.Add(tag);
                }
                else
                {
                    MessageBox.Show("Please note that " + tag.TagName + " is no longer available for future use.", "Alert");
                }
                assignedTagList.Remove(tag);
            }
            LstTag.Items.Refresh();
            LstAssignedTags.Items.Refresh();
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
        /// Handler for button click event to update the selected recipe in the cookbook's collection.
        /// </summary>
        /// <remarks>Calls method to validate input data.</remarks>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnUpdateRecipe_Click(object sender, RoutedEventArgs e)
        {
            string recipeName = TBxRecipeName.Text;
            string recipePage = TBxRecipePage.Text;

            ValidateInput(recipeName, recipePage);
        }

        /// <summary>
        /// Handler for button click event to close window without updating recipe details.
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
        /// <remarks>Checks whether window is to be cancelled or for it to closed and apply updates to the selected recipe.</remarks>
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
        /// Method to get tag data and populate the items in the two listboxes.
        /// </summary>
        private void LoadTagData()
        {
            tagList = (List<Tag>)ViewModel.GetTags();

            assignedTagList = recipe.Tags;
            LstAssignedTags.ItemsSource = assignedTagList;
            if (assignedTagList.Count > 0)
            {
                foreach (Tag tag in assignedTagList)
                {
                    Tag assignedTag = tagList.Find(x => x.TagId == tag.TagId);
                    if (tag.TagName != assignedTag.TagName)
                    {
                        tag.TagName = assignedTag.TagName;
                    }
                    tagList.Remove(assignedTag);
                }
                LstAssignedTags.Items.Refresh();
            }
            LstTag.ItemsSource = tagList;
            LstTag.Items.Refresh();
        }

        /// <summary>
        /// Method to initialise a new <c>ManageTagsWindow</c>.
        /// </summary>
        /// <remarks>ManageTagsWindow_Closed event is also initialised in this method.</remarks>
        private void NavigateToManageTagsWindow()
        {
            ManageTagsWindow window = new ManageTagsWindow(true);
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
        /// Method to verify that the required data has been provided for a recipe to be updated.
        /// </summary>
        /// <remarks>Updates are not committed to the database at this point. It will be upserted in the database if recipe is not discarded prior to inserting or updating the cookbook.</remarks>
        /// <param name="name">Name of the recipe.</param>
        /// <param name="page">Reference for the first page of the recipe in the selected cookbook.</param>
        private void ValidateInput(string name, string page)
        {
            if (name == "")
            {
                MessageBox.Show("Please enter a name for the recipe.", "Invalid recipe name");
            }
            else
            {
                if (page == "")
                {
                    MessageBox.Show("Please enter a page number for the recipe.", "Invalid page reference");
                }
                else
                {
                    Regex regex = new Regex(@"\b([0-9]+|[ivxlcIVXLC]+)\b");
                    Regex specialCharacters = new Regex(@"[-\s]");

                    if (regex.IsMatch(page) && !specialCharacters.IsMatch(page))
                    {
                        List<Tag> updatedTags = new List<Tag>();
                        if (LstAssignedTags.Items.Count != 0)
                        {
                            foreach (Tag tag in LstAssignedTags.Items)
                            {
                                updatedTags.Add(tag);
                            }
                        }
                        recipe.RecipeName = name;
                        recipe.Page = page;
                        recipe.Tags = updatedTags;

                        TBxRecipeName.Clear();
                        TBxRecipePage.Clear();

                        isCancel = false;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Please review the page reference.\nIt should be a single page number of Roman or Arabic numerals\ne.g. iv or 159", "Invalid page reference");
                    }
                }
            }
        }
    }
}
