using DateSearchDLL;
using EmployeeDateEntryDLL;
using NewEmployeeDLL;
using NewEventLogDLL;
using ProductionProjectDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
using ProjectTaskDLL;
using Microsoft.Win32;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for TotalFootages.xaml
    /// </summary>
    public partial class TotalFootages : Page
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        ProjectTaskClass TheProjectTaskClass = new ProjectTaskClass();

        FindTotalFootagesDataSet findTotalFootagesDataSet = new FindTotalFootagesDataSet();
        FindTotalProjectPurchaseOrdersDataSet findTotalProjectPurchaseOrdersDataSet = new FindTotalProjectPurchaseOrdersDataSet();

        bool gblnFootages;

        public TotalFootages()
        {
            InitializeComponent();
        }
        private void ResetControls()
        {
            try
            {
                gblnFootages = true;
                lblTitle.Content = "Project Total Footage For The Last 180 Days";
                findTotalFootagesDataSet = TheProjectTaskClass.FindTotalFootages();

                dgrResults.ItemsSource = findTotalFootagesDataSet.FindTotalProjectFootages;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Find Total Report // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void expResetWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expResetWindow.IsExpanded = false;
            ResetControls();
        }

        private void expExportToExcel_Expanded(object sender, RoutedEventArgs e)
        {
            if(gblnFootages == true)
            {
                ExportFootagesExcel();
            }
            else if(gblnFootages == false)
            {
                ExportPurchaseOrdersExcel();
            }
            else
            {
                TheMessagesClass.ErrorMessage("There Was An Error Determining What To Export");
            }
        }
        private void ExportFootagesExcel()
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
                intRowNumberOfRecords = findTotalFootagesDataSet.FindTotalProjectFootages.Rows.Count;
                intColumnNumberOfRecords = findTotalFootagesDataSet.FindTotalProjectFootages.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = findTotalFootagesDataSet.FindTotalProjectFootages.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = findTotalFootagesDataSet.FindTotalProjectFootages.Rows[intRowCounter][intColumnCounter].ToString();

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
        private void ExportPurchaseOrdersExcel()
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
                intRowNumberOfRecords = findTotalProjectPurchaseOrdersDataSet.FindTotalProjectPurchaseOrders.Rows.Count;
                intColumnNumberOfRecords = findTotalProjectPurchaseOrdersDataSet.FindTotalProjectPurchaseOrders.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = findTotalProjectPurchaseOrdersDataSet.FindTotalProjectPurchaseOrders.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = findTotalProjectPurchaseOrdersDataSet.FindTotalProjectPurchaseOrders.Rows[intRowCounter][intColumnCounter].ToString();

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

        private void expFindTotalPurchaseOrders_Expanded(object sender, RoutedEventArgs e)
        {
            expFindTotalPurchaseOrders.IsExpanded = false;
            lblTitle.Content = "Project Total Purchase Orders For The Last 180 Days";
            gblnFootages = false;

            findTotalProjectPurchaseOrdersDataSet = TheProjectTaskClass.FindTotalProjectPurchaseOrders();
            dgrResults.ItemsSource = findTotalProjectPurchaseOrdersDataSet.FindTotalProjectPurchaseOrders;
        }
    }
}
