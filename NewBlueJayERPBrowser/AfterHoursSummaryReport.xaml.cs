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
using AfterHoursWorkDLL;
using EmployeeDateEntryDLL;
using NewEmployeeDLL;
using IncentivePayDLL;
using Microsoft.Win32;
using System.Security.Cryptography;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for AfterHoursSummaryReport.xaml
    /// </summary>
    public partial class AfterHoursSummaryReport : Page
    {
        EventLogClass TheEventLogClass = new EventLogClass();
        AfterHoursWorkClass TheAfterHoursWorkClass = new AfterHoursWorkClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();

        FindEmployeeAfterHoursWorkThiryDayReportDataSet TheFindEmployeeAfterHoursWorkThiryDayReportDataSet = new FindEmployeeAfterHoursWorkThiryDayReportDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        EmployeeAfterHourWorkReportDataSet TheEmployeeAfterHourWorkReportDataSet = new EmployeeAfterHourWorkReportDataSet();

        public AfterHoursSummaryReport()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAfterHoursDataSet();

            dgrResults.ItemsSource = TheEmployeeAfterHourWorkReportDataSet.employeeafterhourwork;
        }

        private void expEmailReport_Expanded(object sender, RoutedEventArgs e)
        {
            expEmailReport.IsExpanded = false;

            EmailOverNightReport();
        }
        public void EmailOverNightReport()
        {
            int intCounter;
            int intNumberOfRecords;
            string strWorkDate;
            string strEmployee;
            string strManager;
            string strVehicleNumber;
            string strCustomerProjectID;
            string strAssignedProjectID;
            string strProjectName;
            string strOuttime;
            string strInETA;
            string strAssignedOffice;
            string strBuildingAccess;
            string strEmailAddress = "ERPProjectReports@bluejaycommunications.com";
            string strHeader;
            string strBody;

            try
            {
                LoadAfterHoursDataSet();

                strHeader = "After Hours Summary Report For The Last 30 Days";
                strBody = "<h1>" + strHeader + "</h1>";

                strBody += "<table>";
                strBody += "<tr>";
                strBody += "<td><b>Work Date</b></td>";
                strBody += "<td><b>Employee</b></td>";
                strBody += "<td><b>Manager</b></td>";
                strBody += "<td><b>Vehicle Number</b></td>";
                strBody += "<td><b>Customer Project ID</b></td>";
                strBody += "<td><b>Assigned Project ID</b></td>";
                strBody += "<td><b>Project Name</b></td>";
                strBody += "<td><b>Out Time</b></td>";
                strBody += "<td><b>In ETA</b></td>";
                strBody += "<td><b>Assigned Office</b></td>";
                strBody += "<td><b>Building Access</b></td>";
                strBody += "</tr>";

                intNumberOfRecords = TheEmployeeAfterHourWorkReportDataSet.employeeafterhourwork.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        strWorkDate = Convert.ToString(TheEmployeeAfterHourWorkReportDataSet.employeeafterhourwork[intCounter].WorkDate);
                        strEmployee = TheEmployeeAfterHourWorkReportDataSet.employeeafterhourwork[intCounter].Employee;
                        strManager = TheEmployeeAfterHourWorkReportDataSet.employeeafterhourwork[intCounter].Manager;
                        strVehicleNumber = TheEmployeeAfterHourWorkReportDataSet.employeeafterhourwork[intCounter].VehicleNumber;
                        strCustomerProjectID = TheEmployeeAfterHourWorkReportDataSet.employeeafterhourwork[intCounter].CustomerProjectID;
                        strAssignedProjectID = TheEmployeeAfterHourWorkReportDataSet.employeeafterhourwork[intCounter].AssignedProjectID;
                        strProjectName = TheEmployeeAfterHourWorkReportDataSet.employeeafterhourwork[intCounter].ProjectName;
                        strOuttime = TheEmployeeAfterHourWorkReportDataSet.employeeafterhourwork[intCounter].OutTime;
                        strInETA = TheEmployeeAfterHourWorkReportDataSet.employeeafterhourwork[intCounter].InETA;
                        strAssignedOffice = TheEmployeeAfterHourWorkReportDataSet.employeeafterhourwork[intCounter].AssignedOffice;
                        strBuildingAccess = TheEmployeeAfterHourWorkReportDataSet.employeeafterhourwork[intCounter].BuildingAccess;

                        strBody += "<tr>";
                        strBody += "<td>" + strWorkDate + "</td>";
                        strBody += "<td>" + strEmployee + "</td>";
                        strBody += "<td>" + strManager + "</td>";
                        strBody += "<td>" + strVehicleNumber + "</td>";
                        strBody += "<td>" + strCustomerProjectID + "</td>";
                        strBody += "<td>" + strAssignedProjectID + "</td>";
                        strBody += "<td>" + strProjectName + "</td>";
                        strBody += "<td>" + strOuttime + "</td>";
                        strBody += "<td>" + strInETA + "</td>";
                        strBody += "<td>" + strAssignedOffice + "</td>";
                        strBody += "<td>" + strBuildingAccess + "</td>";
                        strBody += "</tr>";
                    }
                }

                strBody += "</table>";

                TheSendEmailClass.SendEmail(strEmailAddress, strHeader, strBody);

            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // After Hours Summary Report // Email Overnight Report Expander " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser //  After Hours Summary Report // Email Overnight Report Expander " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }
        private void LoadAfterHoursDataSet()
        {
            int intCounter;
            int intNumberOfRecords;
            int intEmployeeID;
            string strManagerName = "";
            string strOfficeName = "";

            try
            {
                TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.gintLoggedInEmployeeID, "New Blue Jay ERP Browser // After Hours Summary Report");

                TheEmployeeAfterHourWorkReportDataSet.employeeafterhourwork.Rows.Clear();
                TheFindEmployeeAfterHoursWorkThiryDayReportDataSet = TheAfterHoursWorkClass.FindEmployeeOverNightWorkThirtyDayReport();

                intNumberOfRecords = TheFindEmployeeAfterHoursWorkThiryDayReportDataSet.FindEmployeeAfterHoursWorkThirtyDayReport.Rows.Count;

                if (intNumberOfRecords > 0)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        intEmployeeID = TheFindEmployeeAfterHoursWorkThiryDayReportDataSet.FindEmployeeAfterHoursWorkThirtyDayReport[intCounter].ManagerID;

                        TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intEmployeeID);

                        if (TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID.Rows.Count > 0)
                        {
                            strManagerName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName + " ";
                            strManagerName += TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;
                        }

                        intEmployeeID = TheFindEmployeeAfterHoursWorkThiryDayReportDataSet.FindEmployeeAfterHoursWorkThirtyDayReport[intCounter].OfficeID;

                        TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intEmployeeID);

                        strOfficeName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;

                        EmployeeAfterHourWorkReportDataSet.employeeafterhourworkRow NewAfterWork = TheEmployeeAfterHourWorkReportDataSet.employeeafterhourwork.NewemployeeafterhourworkRow();

                        NewAfterWork.AssignedOffice = strOfficeName;
                        NewAfterWork.BuildingAccess = TheFindEmployeeAfterHoursWorkThiryDayReportDataSet.FindEmployeeAfterHoursWorkThirtyDayReport[intCounter].BuildingAccess;
                        NewAfterWork.CustomerProjectID = TheFindEmployeeAfterHoursWorkThiryDayReportDataSet.FindEmployeeAfterHoursWorkThirtyDayReport[intCounter].CustomerAssignedID;
                        NewAfterWork.AssignedProjectID = TheFindEmployeeAfterHoursWorkThiryDayReportDataSet.FindEmployeeAfterHoursWorkThirtyDayReport[intCounter].AssignedProjectID;
                        NewAfterWork.Employee = TheFindEmployeeAfterHoursWorkThiryDayReportDataSet.FindEmployeeAfterHoursWorkThirtyDayReport[intCounter].Manager;
                        NewAfterWork.Manager = strManagerName;
                        NewAfterWork.OutTime = TheFindEmployeeAfterHoursWorkThiryDayReportDataSet.FindEmployeeAfterHoursWorkThirtyDayReport[intCounter].OutTime;
                        NewAfterWork.InETA = TheFindEmployeeAfterHoursWorkThiryDayReportDataSet.FindEmployeeAfterHoursWorkThirtyDayReport[intCounter].InETA;
                        NewAfterWork.ProjectName = TheFindEmployeeAfterHoursWorkThiryDayReportDataSet.FindEmployeeAfterHoursWorkThirtyDayReport[intCounter].ProjectName;
                        NewAfterWork.VehicleNumber = TheFindEmployeeAfterHoursWorkThiryDayReportDataSet.FindEmployeeAfterHoursWorkThirtyDayReport[intCounter].VehicleNumber;
                        NewAfterWork.WorkDate = TheFindEmployeeAfterHoursWorkThiryDayReportDataSet.FindEmployeeAfterHoursWorkThirtyDayReport[intCounter].WorkDate;

                        TheEmployeeAfterHourWorkReportDataSet.employeeafterhourwork.Rows.Add(NewAfterWork);
                    }
                }

               
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // After Hours Summary Report // Load Afterhours Dataset " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser //  After Hours Summary Report // Load Afterhours Dataset " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
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
                intRowNumberOfRecords = TheEmployeeAfterHourWorkReportDataSet.employeeafterhourwork.Rows.Count;
                intColumnNumberOfRecords = TheEmployeeAfterHourWorkReportDataSet.employeeafterhourwork.Columns.Count;

                for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                {
                    worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeeAfterHourWorkReportDataSet.employeeafterhourwork.Columns[intColumnCounter].ColumnName;

                    cellColumnIndex++;
                }

                cellRowIndex++;
                cellColumnIndex = 1;

                //Loop through each row and read value from each column. 
                for (intRowCounter = 0; intRowCounter < intRowNumberOfRecords; intRowCounter++)
                {
                    for (intColumnCounter = 0; intColumnCounter < intColumnNumberOfRecords; intColumnCounter++)
                    {
                        worksheet.Cells[cellRowIndex, cellColumnIndex] = TheEmployeeAfterHourWorkReportDataSet.employeeafterhourwork.Rows[intRowCounter][intColumnCounter].ToString();

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
            catch (System.Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // After Hours Summary Report // Page Load " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // After Hours Summary Report // Export to Excel " + Ex.Message);

                MessageBox.Show(Ex.ToString());
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
