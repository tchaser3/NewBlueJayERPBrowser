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
using NewEventLogDLL;
using ProductionProjectDLL;
using EmployeeDateEntryDLL;
using DateSearchDLL;
using Microsoft.Win32;
using NewEmployeeDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for OverdueProjectReport.xaml
    /// </summary>
    public partial class OverdueProjectReport : Page
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ProductionProjectClass TheProductionProjectClass = new ProductionProjectClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();

        FindOverdueOpenProductionProjectsDataSet TheFindOverDueProductionProjectsDataSet = new FindOverdueOpenProductionProjectsDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        OverdueProjectReportDataSet TheOverdueProjectReportDataSet = new OverdueProjectReportDataSet();

        public OverdueProjectReport()
        {
            InitializeComponent();
        }
        private void ResetControls()
        {
            DateTime datTransactionDate = DateTime.Now;
            int intCounter;
            int intNumberOfRecords;
            int intWarehouseID;

            try
            {
                datTransactionDate = TheDateSearchClass.AddingDays(datTransactionDate, 3);
                TheOverdueProjectReportDataSet.overdueprojectreport.Rows.Clear();

                TheFindOverDueProductionProjectsDataSet = TheProductionProjectClass.FindOverdueProductionProjects(datTransactionDate);

                intNumberOfRecords = TheFindOverDueProductionProjectsDataSet.FindOverdueOpenProductionProjects.Rows.Count;

                if (intNumberOfRecords > 0)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        intWarehouseID = TheFindOverDueProductionProjectsDataSet.FindOverdueOpenProductionProjects[intCounter].AssignedOfficeID;

                        TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intWarehouseID);

                        OverdueProjectReportDataSet.overdueprojectreportRow NewProjectRow = TheOverdueProjectReportDataSet.overdueprojectreport.NewoverdueprojectreportRow();

                        NewProjectRow.AssignedOffice = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;
                        NewProjectRow.AssignedProjectID = TheFindOverDueProductionProjectsDataSet.FindOverdueOpenProductionProjects[intCounter].AssignedProjectID;
                        NewProjectRow.Customer = TheFindOverDueProductionProjectsDataSet.FindOverdueOpenProductionProjects[intCounter].Customer;
                        NewProjectRow.CustomerAssignedID = TheFindOverDueProductionProjectsDataSet.FindOverdueOpenProductionProjects[intCounter].CustomerAssignedID;
                        NewProjectRow.DateReceived = TheFindOverDueProductionProjectsDataSet.FindOverdueOpenProductionProjects[intCounter].DateReceived;
                        NewProjectRow.ECDDate = TheFindOverDueProductionProjectsDataSet.FindOverdueOpenProductionProjects[intCounter].ECDDate;
                        NewProjectRow.ProjectName = TheFindOverDueProductionProjectsDataSet.FindOverdueOpenProductionProjects[intCounter].ProjectName;
                        NewProjectRow.Status = TheFindOverDueProductionProjectsDataSet.FindOverdueOpenProductionProjects[intCounter].WorkOrderStatus;

                        TheOverdueProjectReportDataSet.overdueprojectreport.Rows.Add(NewProjectRow);
                    }
                }

                dgrOverdueProjects.ItemsSource = TheOverdueProjectReportDataSet.overdueprojectreport;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Overdue Project Report // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void expExportToExcel_Expanded(object sender, RoutedEventArgs e)
        {
            int intRowCounter;
            int intRowNumberOfRecords;
            int intColumnCounter;
            int intColumnNumberOfRecords;

            // Creating a Excel object. 
            Microsoft.Office.Interop.Excel._Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            try
            {
                expExportToExcel.IsExpanded = false;

                worksheet = workbook.ActiveSheet;

                worksheet.Name = "OpenOrders";

                int cellRowIndex = 1;
                int cellColumnIndex = 1;
                intRowNumberOfRecords = TheOverdueProjectReportDataSet.overdueprojectreport.Rows.Count;
                intColumnNumberOfRecords = TheOverdueProjectReportDataSet.overdueprojectreport.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheOverdueProjectReportDataSet.overdueprojectreport.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheOverdueProjectReportDataSet.overdueprojectreport.Rows[intRowCounter][intColumnCounter].ToString();

                        cellColumnIndex++;
                    }
                    cellColumnIndex = 1;
                    cellRowIndex++;
                }

                //Getting the location and file name of the excel to save from user. 
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                saveDialog.FilterIndex = 1;

                saveDialog.ShowDialog();

                workbook.SaveAs(saveDialog.FileName);
                MessageBox.Show("Export Successful");

            }
            catch (System.Exception ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Overdue Project Report // Export To Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }

        private void expResetWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expResetWindow.IsExpanded = false; 
            ResetControls();
        }
    }
}
