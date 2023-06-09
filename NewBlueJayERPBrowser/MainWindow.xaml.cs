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

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            fraMainWindow.Content = new AddEmployee();
        }

        private void btnEditEmployee_Click(object sender, RoutedEventArgs e)
        {
            fraMainWindow.Content = new EditEmployee();
        }

        private void expClose_Expanded(object sender, RoutedEventArgs e)
        {
            expClose.IsExpanded = false;
            TheMessagesClass.CloseTheProgram();
        }
    }
}
