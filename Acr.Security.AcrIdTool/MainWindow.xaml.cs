using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Acr.Security.AcrIdTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainModel _model;

        public MainWindow()
        {
            InitializeComponent();

            _model = new MainModel();

            DataContext = _model;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var result = await _model.TestConnectionAsync();

            if (result)
            {
                MessageBox.Show(this, "Success", "Connection test", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show(this, "Failed", "Connection test", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
