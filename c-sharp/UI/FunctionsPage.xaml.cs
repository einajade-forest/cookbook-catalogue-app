using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace UI
{
    /// <summary>
    /// Interaction logic for FunctionsPage.xaml
    /// </summary>
    public partial class FunctionsPage : Page
    {
        /// <summary>
        /// Constructor for the Home page.
        /// </summary>
        public FunctionsPage()
        {
            InitializeComponent();
            TBlkSearchInfo.Text = "Discover which cookbooks may contain the recipe you're looking for." +
                "\n\n" + "Find recipes based on keywords in the recipe names or by an associated tag.";
            TBlkIndexMgtInfo.Text = "Stop overlooking recipes in your physical cookbooks." + "\n\n" + "Manage cookbooks, associated recipes, and shelf locations to generate an informative library index.";
        }

        /// <summary>
        /// Handler for button click event to navigate to the Search page.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnRecipeSearch_Click(object sender, RoutedEventArgs e)
        {
            NavigateToSearchPage();
        }

        /// <summary>
        /// Handler for button click event to view collection of <c>Cookbook</c> objects.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnBookList_Click(object sender, RoutedEventArgs e)
        {
            NavigateToBooksPage();
        }

        /// <summary>
        /// Method to initialise a new <c>SearchPage</c>.
        /// </summary>
        private void NavigateToSearchPage()
        {
            NavigationService.Navigate(new SearchPage());
        }

        /// <summary>
        /// Method to initialise a new <c>BooksPage</c>.
        /// </summary>
        private void NavigateToBooksPage()
        {
            NavigationService.Navigate(new BooksPage());
        }
    }
}
