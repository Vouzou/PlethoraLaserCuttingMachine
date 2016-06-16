using System.Windows;

namespace ImportJsonApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel = new MainWindowViewModel();
        }

        public MainWindowViewModel ViewModel { get; set; }

        private void MainWindow_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // HandleFileOpen
                if (ViewModel.ImportCommand.CanExecute(files[0]))
                {
                    #pragma warning disable 4014
                    ViewModel.ImportCommand.Execute(files[0]);
                    #pragma warning restore 4014
                }
            }
        }
    }
}
