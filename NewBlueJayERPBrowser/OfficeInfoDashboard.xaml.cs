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
using DepartmentDLL;
using NewEmployeeDLL;
using NewEventLogDLL;
using ProductionProjectDLL;
using DateSearchDLL;
using EmployeeDateEntryDLL;
using Microsoft.Win32;
using System.Windows.Threading;
using ProjectMatrixDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for OfficeInfoDashboard.xaml
    /// </summary>
    public partial class OfficeInfoDashboard : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ProductionProjectClass TheProductionProjectClass = new ProductionProjectClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();

        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindSortedCustomerLinesDataSet TheFindSortedCustomerLinesDataSet = new FindSortedCustomerLinesDataSet();
        FindOpenOfficeBusinessLineDataSet TheFindOpenOfficeBusinessLineDataSet = new FindOpenOfficeBusinessLineDataSet();
        FindOverdueOfficeBusinesLinesDataSet TheFindOverdueOfficeBusinessLinesDataSet = new FindOverdueOfficeBusinesLinesDataSet();
        FindOpenBusinessLineProjectsForOfficeDataSet TheFindOpenBusinessLineProjectsForOfficeDataSet = new FindOpenBusinessLineProjectsForOfficeDataSet();
        FindOverdueBusinessLineProjectsByOfficeDataSet TheFindOverdueBusinessLinebyOfficeDataSet = new FindOverdueBusinessLineProjectsByOfficeDataSet();
        FindOpenOfficeBusinessLineProjectListDataSet TheFindOpenOfficeBusinessLineProjectListDataSet = new FindOpenOfficeBusinessLineProjectListDataSet();
        FindOverdueOfficeBusinessLineProjectListDataSet TheFindOverdueOfficeBusinessLineProjectListDataSet = new FindOverdueOfficeBusinessLineProjectListDataSet();

        public OfficeInfoDashboard()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            int intCounter;
            int intNumberOfRecords;

            try
            {
                TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();
                cboSelectOffice.Items.Clear();
                cboSelectOffice.Items.Add("Select Office");

                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count;

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectOffice.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectOffice.SelectedIndex = 0;

                cboSelectBusinessLine.Items.Clear();
                cboSelectBusinessLine.Items.Add("Select Business Line");
                TheFindSortedCustomerLinesDataSet = TheDepartmentClass.FindSortedCustomerLines();

                intNumberOfRecords = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines.Rows.Count;

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectBusinessLine.Items.Add(TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intCounter].Department);
                }

                cboSelectBusinessLine.SelectedIndex = 0;
                cboSelectBusinessLine.IsEnabled = false;
                TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Office Info Dashboard");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Office Info Dashboard // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectOffice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectOffice.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    MainWindow.gintWarehouseID = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;

                    cboSelectBusinessLine.IsEnabled = true;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Office Info Dashboard // Select Office Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectBusinessLine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedID;

            try
            {
                intSelectedID = cboSelectBusinessLine.SelectedIndex - 1;

                if (intSelectedID > -1)
                {
                    MainWindow.gintDepartmentID = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intSelectedID].DepartmentID;

                    GetProjectCount();
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Office Info Dashboard " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void GetProjectCount()
        {
            DateTime datTransactionDate = DateTime.Now;
            int intRecordsReturned;

            datTransactionDate = TheDateSearchClass.AddingDays(datTransactionDate, 3);

            TheFindOpenOfficeBusinessLineDataSet = TheProductionProjectClass.FindOpenOfficeBusinessLine(MainWindow.gintWarehouseID, MainWindow.gintDepartmentID);

            intRecordsReturned = TheFindOpenOfficeBusinessLineDataSet.FindOpenOfficeBusinessLineProjects.Rows.Count;

            if (intRecordsReturned > 0)
            {
                txtTotalOpenForOffice.Text = Convert.ToString(TheFindOpenOfficeBusinessLineDataSet.FindOpenOfficeBusinessLineProjects[0].TotalCount);
            }
            else
            {
                txtTotalOpenForOffice.Text = "0";
            }

            TheFindOverdueOfficeBusinessLinesDataSet = TheProductionProjectClass.FindOverdueOfficeBusinessLines(MainWindow.gintWarehouseID, MainWindow.gintDepartmentID, datTransactionDate);

            intRecordsReturned = TheFindOverdueOfficeBusinessLinesDataSet.FindOverdueOfficeBusinessLines.Rows.Count;

            if (intRecordsReturned > 0)
            {
                txtTotalOverdueProjects.Text = TheFindOverdueOfficeBusinessLinesDataSet.FindOverdueOfficeBusinessLines[0].TotalCount.ToString();
            }
            else
            {
                txtTotalOverdueProjects.Text = "0";
            }

            TheFindOpenBusinessLineProjectsForOfficeDataSet = TheProductionProjectClass.FindOpenBusinessLineProjectsForOffice(MainWindow.gintWarehouseID, MainWindow.gintDepartmentID);

            dgrOpenProjects.ItemsSource = TheFindOpenBusinessLineProjectsForOfficeDataSet.FindOpenBusinessLineProjectsForOffice;

            TheFindOverdueBusinessLinebyOfficeDataSet = TheProductionProjectClass.FindOverdueBusinessLineProjectsByOffice(MainWindow.gintWarehouseID, MainWindow.gintDepartmentID, datTransactionDate);

            dgrOverdueProjects.ItemsSource = TheFindOverdueBusinessLinebyOfficeDataSet.FindOverdueBusinessLineProjectsByOffice;

            TheFindOpenOfficeBusinessLineProjectListDataSet = TheProjectMatrixClass.FindOpenOfficeBusinessLineProjectList(MainWindow.gintWarehouseID, MainWindow.gintDepartmentID);

            dgrOpenProjectsList.ItemsSource = TheFindOpenOfficeBusinessLineProjectListDataSet.FindOpenOfficeBusinessLineProjectList;

            TheFindOverdueOfficeBusinessLineProjectListDataSet = TheProjectMatrixClass.FindOverdueOfficeBusinessLineProjectList(MainWindow.gintWarehouseID, MainWindow.gintDepartmentID, datTransactionDate);

            dgrOverdueProjectsList.ItemsSource = TheFindOverdueOfficeBusinessLineProjectListDataSet.FindOverdueOfficeBusinessLineProjectList;

        }

        private void dgrOpenProjectsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell ProjectID;
            string strProjectID;

            try
            {
                if (dgrOpenProjectsList.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgrOpenProjectsList;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    ProjectID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strProjectID = ((TextBlock)ProjectID.Content).Text;


                    //find the record
                    MainWindow.gintProjectID = Convert.ToInt32(strProjectID);

                    EditOutageProject EditOutageProject = new EditOutageProject();
                    EditOutageProject.ShowDialog();
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Office Info Dashboard // Open Projects List Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void dgrOverdueProjectsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell ProjectID;
            string strProjectID;

            try
            {
                if (dgrOverdueProjectsList.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgrOverdueProjectsList;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    ProjectID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strProjectID = ((TextBlock)ProjectID.Content).Text;


                    //find the record
                    MainWindow.gintProjectID = Convert.ToInt32(strProjectID);

                    EditOutageProject EditOutageProject = new EditOutageProject();
                    EditOutageProject.ShowDialog();
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Office Info Dashboard // Overdue Projects List Grid Selection " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
