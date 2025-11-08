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
using ProjectCostingDLL;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using ProjectMatrixDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for EmployeeProjectLaborReport.xaml
    /// </summary>
    public partial class EmployeeProjectLaborReport : Page
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ProjectCostingClass TheProjectCostingClass = new ProjectCostingClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();

        //setting up the data
        FindProjectMatrixByAssignedProjectIDDataSet TheFindProjectMatrixByAssignedProjectIDDataSet = new FindProjectMatrixByAssignedProjectIDDataSet();
        FindProjectMatrixByCustomerProjectIDDataSet TheFindProjectMatrixByCustomerProjectIDDataSet = new FindProjectMatrixByCustomerProjectIDDataSet();
        FindProjectTaskCostsDataSet TheFindProjectTaskCostsDataSet = new FindProjectTaskCostsDataSet();


        public EmployeeProjectLaborReport()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            string strAssignedProjectID;
            int intRecordsReturned;
            decimal decTotalCost = 0;
            decimal decTotalHours = 0;
            int intCounter;
            int intNumberOfRecords;
            double douTotalCost;

            try
            {
                dgrResults.Visibility = Visibility.Visible;
                strAssignedProjectID = txtAssignedProjectID.Text;
                if (strAssignedProjectID == "")
                {
                    TheMessagesClass.ErrorMessage("The Assigned Project ID Was Not Entered");
                    return;
                }

                TheFindProjectMatrixByCustomerProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByCustomerProjectID(strAssignedProjectID);

                intRecordsReturned = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID.Rows.Count;

                if (intRecordsReturned < 1)
                {
                    TheFindProjectMatrixByAssignedProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByAssignedProjectID(strAssignedProjectID);

                    intRecordsReturned = TheFindProjectMatrixByAssignedProjectIDDataSet.FindProjectMatrixByAssignedProjectID.Rows.Count;

                    if (intRecordsReturned < 1)
                    {
                        TheMessagesClass.ErrorMessage("The Project Was Not Found");
                        return;
                    }
                    else
                    {
                        strAssignedProjectID = TheFindProjectMatrixByAssignedProjectIDDataSet.FindProjectMatrixByAssignedProjectID[0].CustomerAssignedID;
                    }

                }

                TheFindProjectTaskCostsDataSet = TheProjectCostingClass.FindProjectTasksCosts(strAssignedProjectID);

                intNumberOfRecords = TheFindProjectTaskCostsDataSet.FindProjectTaskCosts.Rows.Count - 1;

                if (intNumberOfRecords > -1)
                {
                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        decTotalCost += TheFindProjectTaskCostsDataSet.FindProjectTaskCosts[intCounter].LaborCost;
                        decTotalHours += TheFindProjectTaskCostsDataSet.FindProjectTaskCosts[intCounter].TotalHours;
                    }
                }

                dgrResults.ItemsSource = TheFindProjectTaskCostsDataSet.FindProjectTaskCosts;

                douTotalCost = Convert.ToDouble(decTotalCost);

                decTotalCost = Convert.ToDecimal(Math.Round(douTotalCost, 2));

                txtTotalCost.Text = Convert.ToString(decTotalCost);
                txtTotalHours.Text = Convert.ToString(decTotalHours);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Employee Project Labor Report // Find Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void ResetControls()
        {
            txtAssignedProjectID.Text = "";
            txtTotalCost.Text = "";
            txtTotalHours.Text = "";
            dgrResults.Visibility = Visibility.Hidden;
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
                intRowNumberOfRecords = TheFindProjectTaskCostsDataSet.FindProjectTaskCosts.Rows.Count;
                intColumnNumberOfRecords = TheFindProjectTaskCostsDataSet.FindProjectTaskCosts.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindProjectTaskCostsDataSet.FindProjectTaskCosts.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheFindProjectTaskCostsDataSet.FindProjectTaskCosts.Rows[intRowCounter][intColumnCounter].ToString();

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Employee Project Labor Report // Export To Excel " + ex.Message);

                MessageBox.Show(ex.ToString());
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }

        }
    }
    
}
