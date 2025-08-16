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
using System.Windows.Shapes;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for ExistingEmployees.xaml
    /// </summary>
    public partial class ExistingEmployees : Window
    {
        public ExistingEmployees()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dgrEmployees.ItemsSource = MainWindow.TheVerifyEmployeeDataSet.VerifyEmployee;
        }
        private void rdoYes_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow.gblnKeepNewEmployee = false;

            this.Close();
        }
        private void rdoNo_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow.gblnKeepNewEmployee = true;

            this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
