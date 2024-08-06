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
using NewEventLogDLL;
using ProductionProjectDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for ViewProjectDocumentation.xaml
    /// </summary>
    public partial class ViewProjectDocumentation : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ProductionProjectClass TheProductionProjectClass = new ProductionProjectClass();

        //setting up the data
        FindProductionProjectDocumentationByProjectIDDataSet TheFindProductionProjectDocumentationByProjectIDDataSet = new FindProductionProjectDocumentationByProjectIDDataSet();

        public ViewProjectDocumentation()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TheFindProductionProjectDocumentationByProjectIDDataSet = TheProductionProjectClass.FindProductionProjectDocumentationByProjectID(MainWindow.gintProjectID);

            dgrDocuments.ItemsSource = TheFindProductionProjectDocumentationByProjectIDDataSet.FindProductionProjectDocumentationByProjectID;
        }
        private void dgrDocuments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell DocumentPath;
            string strDocumentPath;

            try
            {
                if (dgrDocuments.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgrDocuments;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    DocumentPath = (DataGridCell)dataGrid.Columns[3].GetCellContent(selectedRow).Parent;
                    strDocumentPath = ((TextBlock)DocumentPath.Content).Text;

                    System.Diagnostics.Process.Start(strDocumentPath);
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Project Management Sheet // View Project Documentation // Documentation Grid View Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
