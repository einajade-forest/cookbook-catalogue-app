﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using Controller;
using Domain;

namespace UI
{
    /// <summary>
    /// Interaction logic for RecipeResultsPage.xaml
    /// </summary>
    public partial class RecipeResultsPage : Page
    {
        /// <summary>
        /// Constructor for the Search by Keyword Recipe Results page.
        /// </summary>
        /// <param name="keyword">String representing term used to filter search results.</param>
        public RecipeResultsPage(string keyword)
        {
            InitializeComponent();
            LblSearchedTerm.Content = keyword;

            List<Recipe> results = (List<Recipe>)ViewModel.GetRecipesByKeyword(keyword);
            foreach (Recipe recipe in results)
            {
                DgrdSearchResults.Items.Add(recipe);
            }

            if (DgrdSearchResults.Items.Count == 0)
            {
                TBxNoResults.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Handler for button click event to try another search.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Routed Event Argument.</param>
        private void BtnNewSearch_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                NavigationService.Navigate(new SearchPage());
            }
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
    }
}
