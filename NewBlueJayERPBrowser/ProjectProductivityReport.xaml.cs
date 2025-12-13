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
using DataValidationDLL;
using DepartmentDLL;
using DesignProjectsDLL;
using EmployeeDateEntryDLL;
using NewEmployeeDLL;
using NewEventLogDLL;
using ProductionProjectDLL;
using ProductionProjectUpdatesDLL;
using ProjectMatrixDLL;
using ProjectsDLL;
using EmployeeProjectAssignmentDLL;
using ProjectTaskDLL;
using System.Printing;

using WorkOrderDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for ProjectProductivityReport.xaml
    /// </summary>
    public partial class ProjectProductivityReport : Page
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ProjectClass TheProjectClass = new ProjectClass();
        ProductionProjectClass TheProductionProjectClass = new ProductionProjectClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        WorkOrderClass TheWorkOrderClass = new WorkOrderClass();
        EmployeeDateEntryClass TheEmployeeDataEntryClass = new EmployeeDateEntryClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();
        ProductionProjectUpdatesClass TheProductionProjectsUpdatesClass = new ProductionProjectUpdatesClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();
        ProjectTaskClass TheProjectTaskClass = new ProjectTaskClass();

        //setting up the data
        FindProjectMatrixByCustomerProjectIDDataSet TheFindProjectMatrixByCustomerProjectIDDataSet = new FindProjectMatrixByCustomerProjectIDDataSet();
        FindProjectByProjectIDDataSet TheFindProjectByProjectIDDataSet = new FindProjectByProjectIDDataSet();
        FindProjectByAssignedProjectIDDataSet TheFindProjectByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();
        FindProjectMatrixByProjectIDDataSet TheFindProjectMatrixByProjectIDDataSet = new FindProjectMatrixByProjectIDDataSet();
        FindProductionProjectByProjectIDDataSet TheFindProductionProjectByProjectIDDataSet = new FindProductionProjectByProjectIDDataSet();
        FindProdutionProjectsByAssignedProjectIDDataSet TheFindProductionProjectByAssignedProjectIDDataSet = new FindProdutionProjectsByAssignedProjectIDDataSet();
        FindProductionProjectInfoDataSet TheFindProductionProjectInfoDataSet = new FindProductionProjectInfoDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindWorkOrderStatusByStatusIDDataSet TheFindWorkOrderStatusByStatusIDDataSet = new FindWorkOrderStatusByStatusIDDataSet();
        FindProjectProductionByAssignedProjectIDDataSet TheFindProjectProductionByAssignedProjectIDDataSet = new FindProjectProductionByAssignedProjectIDDataSet();
        FindProjectTaskHoursByAssignedProjectIDDataSet TheFindProjectTaskHoursByAssignedProjectIDDataSet = new FindProjectTaskHoursByAssignedProjectIDDataSet();
        FindProductionProjectUpdateByProjectIDDataSet TheFindProductionProjectUpdateByProjectIDDataSet = new FindProductionProjectUpdateByProjectIDDataSet();

        //setting up variables
        int gintDepartmentID;
        int gintManagerID;
        int gintOfficeID;
        int gintStatusID;
        int gintProjectID;
        bool gblnProjectExists;
        bool gblnDoNotRun;
        int gintTransactionID;
        string gstrAssignedProjectID;

        public ProjectProductivityReport()
        {
            InitializeComponent();
        }
        private void ResetControls()
        {
            txtBluejayProjectID.Text = "";
            txtCurrentStatus.Text = "";
            txtEnterProjectID.Text = "";
            txtProjectID.Text = "";
            txtProjectManager.Text = "";
            txtProjectName.Text = "";
            txtTotalCost.Text = "";
            txtTotalHours.Text = "";
            dgProjectProductivityReport.ItemsSource = null;
        }

        private void expResetWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expResetWindow.IsExpanded = false;
            ResetControls();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void btnFindProject_Click(object sender, RoutedEventArgs e)
        {
            string strCustomerProjectID;
            int intLength;
            int intRecordsReturned;
            int intBusinessLineID;

            try
            {
                strCustomerProjectID = txtEnterProjectID.Text;
                gstrAssignedProjectID = strCustomerProjectID;
                intLength = strCustomerProjectID.Length;

                if ((intLength > 5) && (intLength < 12))
                {
                    TheFindProjectMatrixByCustomerProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByCustomerProjectID(strCustomerProjectID);

                    intRecordsReturned = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID.Rows.Count;

                    if (intRecordsReturned > 0)
                    {
                        gintProjectID = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID[0].ProjectID;
                        intBusinessLineID = TheFindProjectMatrixByCustomerProjectIDDataSet.FindProjectMatrixByCustomerProjectID[0].BusinessLineID;

                        TheFindProductionProjectInfoDataSet = TheProductionProjectClass.FindProductionProjectInfo(gintProjectID);

                        intRecordsReturned = TheFindProductionProjectInfoDataSet.FindProductionProjectInfo.Rows.Count;

                        if ((intRecordsReturned < 1) && (intBusinessLineID == 1009))
                        {
                            TheMessagesClass.InformationMessage("There is no Production Project Info Entered, The Window will now Open");

                            MainWindow.gintProjectID = gintProjectID;
                        }

                        gblnProjectExists = true;

                        FillControls();
                    }
                    else
                    {
                        TheMessagesClass.ErrorMessage("The Project Was Not Found");
                        return;
                    }
                }
                else if ((intLength >= 12) || (intLength <= 5))
                {
                    TheMessagesClass.ErrorMessage("The Project is not the Correct Format");
                    return;
                }
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Production Project Productivity Report // Find Project Button " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Production Project Productivity Report // Find Project Button  " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void FillControls()
        {
            //setting up local variables
            int intCounter;
            int intNumberOfRecords;
            int intDepartmentID;
            int intManagerID;
            int intOfficeID;
            int intStatusID;
            int intSelectedIndex = 0;
            int intRecordsReturned;
            decimal decTotalHours = 0;

            try
            {
                TheFindProjectMatrixByProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByProjectID(gintProjectID);
                TheFindProjectByProjectIDDataSet = TheProjectClass.FindProjectByProjectID(gintProjectID);
                TheFindProductionProjectByProjectIDDataSet = TheProductionProjectClass.FindProductionProjectByProjectID(gintProjectID);

                intRecordsReturned = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID.Rows.Count;

                if (intRecordsReturned < 1)
                {
                    TheMessagesClass.ErrorMessage("The Project Is Not Completely Entered, Please Go To Add Project");
                    return;
                }

                gintTransactionID = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].TransactionID;
                gblnDoNotRun = true;

                if (gblnProjectExists == true)
                {
                    txtBluejayProjectID.Text = TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].AssignedProjectID;
                }

                txtProjectID.Text = Convert.ToString(gintProjectID);
                txtProjectName.Text = TheFindProjectByProjectIDDataSet.FindProjectByProjectID[0].ProjectName;
                
                intManagerID = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].ProjectManagerID;
                intStatusID = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].CurrentStatusID;
                intDepartmentID = TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].DepartmentID;
                intOfficeID = TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].WarehouseID;

                TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intManagerID);
                txtProjectManager.Text = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName + " " + TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;

                TheFindWorkOrderStatusByStatusIDDataSet = TheWorkOrderClass.FindWorkOrderStatusByStatusID(intStatusID);
                txtCurrentStatus.Text = TheFindWorkOrderStatusByStatusIDDataSet.FindWorkOrderStatusByStatusID[0].WorkOrderStatus;

                TheFindProjectProductionByAssignedProjectIDDataSet = TheEmployeeProjectAssignmentClass.FindProjectProductionByAssignedProjectID(gstrAssignedProjectID);

                TheFindProjectTaskHoursByAssignedProjectIDDataSet = TheProjectTaskClass.FindProjectTaskHoursByAssignedProjectID(gstrAssignedProjectID);

                intNumberOfRecords = TheFindProjectTaskHoursByAssignedProjectIDDataSet.FindProjectTaskHoursByAssignedProjectID.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        decTotalHours += TheFindProjectTaskHoursByAssignedProjectIDDataSet.FindProjectTaskHoursByAssignedProjectID[intCounter].EmployeeTotalHours;
                    }
                }

                txtTotalHours.Text = Convert.ToString(decTotalHours);
                txtTotalCost.Text = (decTotalHours * 35).ToString("C");

                TheFindProductionProjectUpdateByProjectIDDataSet = TheProductionProjectsUpdatesClass.FindProductionProjectUpdateByProjectID(gintProjectID);

                dgProjectProductivityReport.ItemsSource = TheFindProductionProjectUpdateByProjectIDDataSet.FindProductionProjectUpdatesByProjectID;

            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Project Productivity Report // Fill Controls " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Project Productivity Report // Fill Controls " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }


        }

        private void expPrintReport_Expanded(object sender, RoutedEventArgs e)
        {
            int intCurrentRow = 0;
            int intCounter;
            int intColumns;
            int intNumberOfRecords;
            string strBlueJayProjectID;
            string strCustomerProjectID;
            string strProjectName;
            string strProjectStatus;
            string strManagerName;
            string strTotalHours;
            string strTotalCost;


            try
            {
                strBlueJayProjectID = txtBluejayProjectID.Text;
                strCustomerProjectID = gstrAssignedProjectID;
                strProjectName = txtProjectName.Text;
                strProjectStatus = txtCurrentStatus.Text;
                strManagerName = txtProjectManager.Text;
                strTotalHours = txtTotalHours.Text;
                strTotalCost = txtTotalCost.Text;

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
                    intColumns = TheFindProductionProjectUpdateByProjectIDDataSet.FindProductionProjectUpdatesByProjectID.Columns.Count;
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Project Productivity Costs for Project " + strCustomerProjectID))));
                    newTableRow.Cells[0].FontSize = 25;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);

                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Blue Jay Project ID: " + strBlueJayProjectID))));
                    newTableRow.Cells[0].FontSize = 16;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);

                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Project Name: " + strProjectName))));
                    newTableRow.Cells[0].FontSize = 16;
                    newTableRow.Cells[0].FontFamily = new FontFamily("Times New Roman");
                    newTableRow.Cells[0].ColumnSpan = intColumns;
                    newTableRow.Cells[0].TextAlignment = TextAlignment.Center;
                    newTableRow.Cells[0].Padding = new Thickness(0, 0, 0, 10);

                    ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                    intCurrentRow++;
                    newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Current Status: " + strProjectStatus + "\t\t" + "Labor Hours: " + strTotalHours + "\t\t" + "Labor Costs: " + strTotalCost))));
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Project Update"))));
                    
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

                    intNumberOfRecords = TheFindProductionProjectUpdateByProjectIDDataSet.FindProductionProjectUpdatesByProjectID.Rows.Count;

                    //Data, Format Data

                    for (int intReportRowCounter = 0; intReportRowCounter < intNumberOfRecords; intReportRowCounter++)
                    {
                        ProjectReportTable.RowGroups[0].Rows.Add(new TableRow());
                        intCurrentRow++;
                        newTableRow = ProjectReportTable.RowGroups[0].Rows[intCurrentRow];
                        for (int intColumnCounter = 0; intColumnCounter < intColumns; intColumnCounter++)
                        {
                            newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheFindProductionProjectUpdateByProjectIDDataSet.FindProductionProjectUpdatesByProjectID[intReportRowCounter][intColumnCounter].ToString()))));

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
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Project Productivity Report // Print Expander " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Product Productivity Report // Print Expander " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
