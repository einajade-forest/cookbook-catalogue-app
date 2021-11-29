using System.Windows;

namespace UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Constructor for the <c>MainWindow</c> view.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            FrMainWindow.Content = new FunctionsPage();
        }
    }
}
