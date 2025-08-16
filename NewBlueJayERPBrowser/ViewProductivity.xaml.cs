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
using EmployeeProjectAssignmentDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for ViewProductivity.xaml
    /// </summary>
    public partial class ViewProductivity : Window
    {
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();

        FindProjectProductivityDataSet TheFindProjectProductivityDataSet = new FindProjectProductivityDataSet();

        public ViewProductivity()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TheFindProjectProductivityDataSet = TheEmployeeProjectAssignmentClass.FindProjectProductivity(MainWindow.gintProjectID);

            dgrResults.ItemsSource = TheFindProjectProductivityDataSet.FindProjectProductivity;
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
