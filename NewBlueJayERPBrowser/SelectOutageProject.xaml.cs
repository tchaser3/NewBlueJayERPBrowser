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
using OutageProjectDLL;
using NewEventLogDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for SelectOutageProject.xaml
    /// </summary>
    public partial class SelectOutageProject : Page
    {
        OutageProjectClass TheOutageProjectClass = new OutageProjectClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();   
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        FindOutageProjectsDataSet TheFindOutageProjectsDataSet = new FindOutageProjectsDataSet();
        

        public SelectOutageProject()
        {
            InitializeComponent();
        }
        private void ResetControls()
        {
            try
            {
                TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP Browser // Select Outage Project");
                TheFindOutageProjectsDataSet = TheOutageProjectClass.FindOutageProjects();

                dgrResults.ItemsSource = TheFindOutageProjectsDataSet.FindOutageProjects;
            }
            catch (Exception ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Select Outage Project // Reset Controls " + ex.ToString());
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Select Outage Project // Reset Controls " + ex.ToString());
                TheMessagesClass.ErrorMessage(ex.ToString());
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void dgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell ProjectID;
            string strProjectID;

            try
            {
                //setting local variable
                dataGrid = dgrResults;

                if(dataGrid.SelectedIndex > -1)
                {
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    ProjectID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strProjectID = ((TextBlock)ProjectID.Content).Text;

                    //find the record
                    MainWindow.gintProjectID = Convert.ToInt32(strProjectID);

                    EditOutageProject EditOutageProject = new EditOutageProject();
                    EditOutageProject.ShowDialog();

                    ResetControls();
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Select Outage Project // Data Grid Selection " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Select Outage // Data Grid Selection " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
