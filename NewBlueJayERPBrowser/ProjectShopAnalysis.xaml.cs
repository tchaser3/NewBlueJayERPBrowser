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
using ProjectMatrixDLL;
using EmployeeProjectAssignmentDLL;
using NewEventLogDLL;
using DateSearchDLL;
using EmployeeProductivityStatsDLL;
using EmployeeDateEntryDLL;
using Microsoft.Win32;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for ProjectShopAnalysis.xaml
    /// </summary>
    public partial class ProjectShopAnalysis : Page
    {
        EventLogClass TheEventLogClass = new EventLogClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EmployeeProductiivityStatsClass TheEmployeeProductivityStatsClass = new EmployeeProductiivityStatsClass();
        EmployeeDateEntryClass TheEmployeeDataEntryClass = new EmployeeDateEntryClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        //setting up the data
        FindProjectMatrixByAssignedProjectIDDataSet TheFindProjectMatrixByAssignedProjectIDDataSet = new FindProjectMatrixByAssignedProjectIDDataSet();
        FindProjectHoursAboveLimitDataSet TheFindProjectHoursAboveLimitDataSet = new FindProjectHoursAboveLimitDataSet();
        ShopViolatorDataSet TheShopViolatorDataSet = new ShopViolatorDataSet();
        FindProjectStatsDataSet TheFindProjectStatsDataSet = new FindProjectStatsDataSet();
        NormalDistributionDataSet TheNormalDistributionDataSet = new NormalDistributionDataSet();

        //setting variables
        decimal gdecTotalHours;
        decimal gdecMean;
        decimal gdecStandDeviation;
        decimal gdecVariance;
        decimal gdecUpperBound;
        decimal gdecProjectHours;
        decimal gdecProjectCost;

        public ProjectShopAnalysis()
        {
            InitializeComponent();
        }
        private void ResetControls()
        {
            //setting up the local variables
            int intCounter;
            int intNumberOfRecords;
            int intProjectID;
            DateTime datTransactionDate = DateTime.Now;
            DateTime datStartDate = DateTime.Now;
            decimal decAveragePayRate;

            try
            {
                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                TheShopViolatorDataSet.violator.Rows.Clear();

                datTransactionDate = TheDateSearchClass.RemoveTime(datTransactionDate);

                datTransactionDate = TheDateSearchClass.SubtractingDays(datTransactionDate, 31);

                TheFindProjectMatrixByAssignedProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByAssignedProjectID("SHOP");

                intProjectID = TheFindProjectMatrixByAssignedProjectIDDataSet.FindProjectMatrixByAssignedProjectID[0].ProjectID;

                datStartDate = TheDateSearchClass.SubtractingDays(datStartDate, 31);

                TheFindProjectStatsDataSet = TheEmployeeProductivityStatsClass.FindProjectStats(intProjectID);

                gdecMean = TheFindProjectStatsDataSet.FindProjectStats[0].AveHours;
                gdecStandDeviation = Convert.ToDecimal(TheFindProjectStatsDataSet.FindProjectStats[0].HoursSTDev);
                gdecVariance = Convert.ToDecimal(TheFindProjectStatsDataSet.FindProjectStats[0].HoursVariance);
                gdecTotalHours = Convert.ToDecimal(TheFindProjectStatsDataSet.FindProjectStats[0].TotalHours);
                decAveragePayRate = TheFindProjectStatsDataSet.FindProjectStats[0].AveragePayRate;

                gdecMean = Math.Round(gdecMean, 4);

                txtAverageHours.Text = Convert.ToString(gdecMean);

                gdecVariance = Math.Round(gdecVariance, 4);
                gdecStandDeviation = Math.Round(gdecStandDeviation, 4);

                gdecUpperBound = gdecMean + gdecStandDeviation;

                txtUpperBound.Text = Convert.ToString(gdecUpperBound);

                TheFindProjectHoursAboveLimitDataSet = TheEmployeeProjectAssignmentClass.FindProjectHoursAboveLimit(intProjectID, datStartDate, gdecUpperBound);

                intNumberOfRecords = TheFindProjectHoursAboveLimitDataSet.FindProjectHoursAboveLimit.Rows.Count;

                TheEmployeeDataEntryClass.InsertIntoEmployeeDateEntry(MainWindow.gintEmployeeID, "New Blue Jay ERP Browser // Project Shop Analysis");

                if (intNumberOfRecords > 0)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        ShopViolatorDataSet.violatorRow NewViolatorRow = TheShopViolatorDataSet.violator.NewviolatorRow();

                        NewViolatorRow.FirstName = TheFindProjectHoursAboveLimitDataSet.FindProjectHoursAboveLimit[intCounter].FirstName;
                        NewViolatorRow.HomeOffice = TheFindProjectHoursAboveLimitDataSet.FindProjectHoursAboveLimit[intCounter].HomeOffice;
                        NewViolatorRow.Hours = TheFindProjectHoursAboveLimitDataSet.FindProjectHoursAboveLimit[intCounter].TotalHours;
                        NewViolatorRow.LastName = TheFindProjectHoursAboveLimitDataSet.FindProjectHoursAboveLimit[intCounter].LastName;
                        NewViolatorRow.TransactionDate = TheFindProjectHoursAboveLimitDataSet.FindProjectHoursAboveLimit[intCounter].TransactionDate;

                        TheShopViolatorDataSet.violator.Rows.Add(NewViolatorRow);
                    }
                }

                dgrResults.ItemsSource = TheShopViolatorDataSet.violator;

                gdecProjectHours = TheFindProjectStatsDataSet.FindProjectStats[0].TotalHours;
                txtShopHours.Text = Convert.ToString(gdecProjectHours);
                gdecProjectCost = gdecProjectHours * decAveragePayRate;
                gdecProjectCost = Math.Round(gdecProjectCost, 4);
                txtProjectCost.Text = Convert.ToString(gdecProjectCost);

                PleaseWait.Close();
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Shop Hours Analysis // Reset Controls " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Shop Hours Analysis // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void expPrintReport_Expanded(object sender, RoutedEventArgs e)
        {
            int intCurrentRow = 0;
            int intCounter;
            int intColumns;
            int intNumberOfRecords;
            string strAverageHours;
            string strUpperBound;
            string strShopHours;
            string strProjectCost;

            try
            {
                strAverageHours = txtAverageHours.Text;
                strUpperBound = txtUpperBound.Text;
                strShopHours = txtShopHours.Text;
                strProjectCost = txtProjectCost.Text;

                expPrintReport.IsExpanded = false;
                PrintDialog pdProjectReport = new PrintDialog();

                if (pdProjectReport.ShowDialog().Value)
                {
                    FlowDocument fdProjectReport = new FlowDocument();
                    Thickness thickness = new Thickness(50, 50, 50, 50);
                    fdProjectReport.PagePadding = thickness;

                    pdProjectReport.PrintTicket.PageOrientation = System.Printing.PageOrientation.Landscape;

                    //Set Up Table Columns
                    Table ProjectReportTable = new Table();
                    fdProjectReport.Blocks.Add(ProjectReportTable);
                    ProjectReportTable.CellSpacing = 0;
                    intColumns = TheShopViolatorDataSet.violator.Columns.Count;
                    fdProjectReport.ColumnWidth = 10;
                    fdProjectReport.IsColumnWidthFlexible = false;


                    for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                    {
                        ProjectReportTable.Columns.Add(new TableColumn());
                    }
                    ProjectReportTable.RowGroups.Add(new TableRowGroup());

                    //Title row
                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    TableRow newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Project Shop Analysis and Violations "))));
                    newTableRow.Cells[0].FontSize = 25;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);

                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Average Hours Employee In Shop: " + strAverageHours + "\t\t" + "Upper Bound For Acceptable Hours: " + strUpperBound))));
                    newTableRow.Cells[0].FontSize = 16;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);

                    //ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    //intCurrentRow++;
                    //newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    //newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Upper Bound For Acceptable Hours: " + strUpperBound))));
                    //newTableRow.Cells[0].FontSize = 16;
                    //newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    //newTableRow.Cells[0].ColumnSpan = intColumns;
                    //newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    //newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);

                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Total Time in Shop: " + strShopHours + "\t\t" + "Total Cost For Time Period:" + strProjectCost))));
                    newTableRow.Cells[0].FontSize = 16;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);

                    //Header Row
                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Transaction ID"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Date"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("First Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Last Name"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Location"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Hours In Shop"))));

                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);
                    newTableRow.Cells[0].ColumnSpan = 1;
                    newTableRow.Cells[1].ColumnSpan = 1;
                    newTableRow.Cells[2].ColumnSpan = 1;
                    newTableRow.Cells[3].ColumnSpan = 1;
                    newTableRow.Cells[4].ColumnSpan = 1;

                    //Format Header Row
                    for (intCounter = 0; intCounter < intColumns; intCounter++)
                    {
                        newTableRow.Cells[intCounter].FontSize = 16;
                        newTableRow.Cells[intCounter].FontFamily = new FontFamily("Times New Roman");
                        newTableRow.Cells[intCounter].BorderBrush = Brushes.Black;
                        newTableRow.Cells[intCounter].TextAlignment = TextAlignment.Center;
                        newTableRow.Cells[intCounter].BorderThickness = new Thickness();

                    }

                    intNumberOfRecords = TheShopViolatorDataSet.violator.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheShopViolatorDataSet.violator[intReportRowCounter][intColumnCounter].ToString()))));

                            newTableRow.Cells[intColumnCounter].FontSize = 12;
                            newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                            newTableRow.Cells[intColumnCounter].BorderBrush = Brushes.LightSteelBlue;
                            newTableRow.Cells[intColumnCounter].BorderThickness = new Thickness(0, 0, 0, 1);
                            newTableRow.Cells[intColumnCounter].TextAlignment = TextAlignment.Center;
                            if (intColumnCounter == 5)
                            {
                                newTableRow.Cells[intColumnCounter].ColumnSpan = 2;
                            }

                        }
                    }

                    //Set up page and print
                    fdProjectReport.ColumnWidth = pdProjectReport.PrintableAreaWidth;
                    fdProjectReport.PageHeight = pdProjectReport.PrintableAreaHeight;
                    fdProjectReport.PageWidth = pdProjectReport.PrintableAreaWidth;
                    pdProjectReport.PrintDocument(((IDocumentPaginatorSource)fdProjectReport).DocumentPaginator, "Audit Vehicle Inspection Report");
                    intCurrentRow = 0;

                }
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Project Shop Analysis // Print Expander " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Project Shop Analysis // Print Expander " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
