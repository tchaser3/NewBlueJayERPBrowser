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
using DataValidationDLL;
using System.Data.Odbc;
using DateSearchDLL;

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
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();

        FindEmployeeAfterHoursWorkThiryDayReportDataSet TheFindEmployeeAfterHoursWorkThiryDayReportDataSet = new FindEmployeeAfterHoursWorkThiryDayReportDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        EmployeeAfterHourWorkReportDataSet TheEmployeeAfterHourWorkReportDataSet = new EmployeeAfterHourWorkReportDataSet();
        FindEmployeeAfterHoursReportNotEnteredDataSet TheFindEmployeesAfterHoursNotEnteredDataSet = new FindEmployeeAfterHoursReportNotEnteredDataSet();
        NoRecordDataSet TheNoRecordDataSet = new NoRecordDataSet();
        bool gblnNoRecord;

        int gintNumberOfRecords;
        DateTime gdatPayPeriod;

        public AfterHoursSummaryReport()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAfterHoursDataSet();

            gblnNoRecord = false;

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

                if(gblnNoRecord == true)
                {
                    strHeader = "After Hours Punches Not Reported For Pay Period " + Convert.ToString(gdatPayPeriod);
                    strBody = "<h1>" + strHeader + "</h1>";

                    strBody += "<table>";
                    strBody += "<tr>";
                    strBody += "<td><b>Employee Name</b></td>";
                    strBody += "<td><b>Manager Name</b></td>";
                    strBody += "<td><b>Location</b></td>";
                    strBody += "<td><b>Punch Date</b></td>";
                    strBody += "</tr>";

                    intNumberOfRecords = TheNoRecordDataSet.norecord.Rows.Count;

                    if (intNumberOfRecords > 0)
                    {
                        for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            strWorkDate = Convert.ToString(TheNoRecordDataSet.norecord[intCounter].PunchDate);
                            strEmployee = TheNoRecordDataSet.norecord[intCounter].EmployeeName;
                            strManager = TheNoRecordDataSet.norecord[intCounter].ManagerName;
                            strAssignedOffice = TheNoRecordDataSet.norecord[intCounter].Location;

                            strBody += "<tr>";
                            strBody += "<td>" + strEmployee + "</td>";
                            strBody += "<td>" + strManager + "</td>";
                            strBody += "<td>" + strAssignedOffice + "</td>";
                            strBody += "<td>" + strWorkDate + "</td>";
                            strBody += "</tr>";
                        }
                    }

                    TheSendEmailClass.SendEmail(strEmailAddress, strHeader, strBody);
                }
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // After Hours Summary Report // Email Overnight Report Expander " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser //  After Hours Summary Report // Email Overnight Report Expander " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }
        private void SendNoRecordReport()
        {
            TheMessagesClass.ErrorMessage("Fuck You");
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
                if(gblnNoRecord == false)
                {
                    TheNoRecordDataSet.norecord.Rows.Clear();
                }               

                dgrNoReport.ItemsSource = TheNoRecordDataSet.norecord;                

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

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //setting up the variables
            string strValueForValidation;
            bool blnFatalError = false;
            int intCounter;
            int intNumberOfRecords;
            int intManagerID;
            string strManagerName;
            int intEmployeeID;
            string strOffice;
            DateTime datPunchDate = DateTime.Now;
            DateTime datSecondPunchDate = DateTime.Now;
            string strManagerExplaination = "";
            string strEmployeeName;
            int intTransactionID;
            bool blnItemFound;
            int intSecondCounter;

            try
            {
                TheNoRecordDataSet.norecord.Rows.Clear();
                gintNumberOfRecords = 0;

                strValueForValidation = txtEnterPayPeriod.Text;
                blnFatalError = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The Date Entered is not a Date");
                    return;
                }

                gdatPayPeriod = Convert.ToDateTime(strValueForValidation);

                blnFatalError = TheDataValidationClass.verifyDateRange(gdatPayPeriod, DateTime.Now);

                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The Date Entered is in the Future");
                    return;
                }

                if(gdatPayPeriod.Date.DayOfWeek != DayOfWeek.Sunday)
                {
                    TheMessagesClass.ErrorMessage("The Pay Period is not a Sunday");
                    return;
                }

                TheFindEmployeesAfterHoursNotEnteredDataSet = TheAfterHoursWorkClass.FindEmployeeAfterHoursReportNotEntered(gdatPayPeriod);

                intNumberOfRecords = TheFindEmployeesAfterHoursNotEnteredDataSet.FindEmployeeAfterHoursReportNotEntered.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        intEmployeeID = TheFindEmployeesAfterHoursNotEnteredDataSet.FindEmployeeAfterHoursReportNotEntered[intCounter].EmployeeID;
                        intManagerID = TheFindEmployeesAfterHoursNotEnteredDataSet.FindEmployeeAfterHoursReportNotEntered[intCounter].ManagerID;
                        strEmployeeName = TheFindEmployeesAfterHoursNotEnteredDataSet.FindEmployeeAfterHoursReportNotEntered[intCounter].EmployeeName;
                        datPunchDate = TheFindEmployeesAfterHoursNotEnteredDataSet.FindEmployeeAfterHoursReportNotEntered[intCounter].PunchDate;
                        intTransactionID = TheFindEmployeesAfterHoursNotEnteredDataSet.FindEmployeeAfterHoursReportNotEntered[intCounter].TransactionID;

                        datPunchDate = TheDateSearchClass.RemoveTime(datPunchDate);

                        if(intEmployeeID != 20007)
                        {
                            if (TheFindEmployeesAfterHoursNotEnteredDataSet.FindEmployeeAfterHoursReportNotEntered[intCounter].IsManagerExplainationNull() == true)
                            {
                                strManagerExplaination = "";
                            }
                            else
                            {
                                strManagerExplaination = TheFindEmployeesAfterHoursNotEnteredDataSet.FindEmployeeAfterHoursReportNotEntered[intCounter].ManagerExplaination;
                            }

                            TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intEmployeeID);

                            strOffice = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].HomeOffice;

                            TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intManagerID);
                            strManagerName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName + " ";
                            strManagerName += TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;
                            blnItemFound = false;

                            if (gintNumberOfRecords > 0)
                            {
                                for (intSecondCounter = 0; intSecondCounter < gintNumberOfRecords; intSecondCounter++)
                                {
                                    datSecondPunchDate = TheNoRecordDataSet.norecord[intSecondCounter].PunchDate;
                                    datSecondPunchDate = TheDateSearchClass.RemoveTime(datSecondPunchDate);

                                    if (datPunchDate == datSecondPunchDate) 
                                    {
                                        if (strEmployeeName == TheNoRecordDataSet.norecord[intSecondCounter].EmployeeName)
                                        {
                                            blnItemFound = true;
                                        }
                                    }
                                }
                            }

                            if (blnItemFound == false)
                            {
                                NoRecordDataSet.norecordRow NewEmployeeRow = TheNoRecordDataSet.norecord.NewnorecordRow();

                                NewEmployeeRow.EmployeeName = strEmployeeName;
                                NewEmployeeRow.Location = strOffice;
                                NewEmployeeRow.ManagerExplanation = strManagerExplaination;
                                NewEmployeeRow.ManagerName = strManagerName;
                                NewEmployeeRow.PunchDate = datPunchDate;
                                NewEmployeeRow.TransactionID = intTransactionID;

                                TheNoRecordDataSet.norecord.Rows.Add(NewEmployeeRow);
                                gintNumberOfRecords++;
                            }
                        }                     

                    }
                }

                dgrNoReport.ItemsSource = TheNoRecordDataSet.norecord;
                
                if(TheNoRecordDataSet.norecord.Rows.Count > 0)
                {
                    gblnNoRecord = true;
                }
                else
                {
                    gblnNoRecord = false;
                }
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // After Hours Summary Report // Search Button " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // After Hours Summary Report // Search Button " + Ex.Message);

                MessageBox.Show(Ex.ToString());
            }

        }

        private void dgrNoReport_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = dgrNoReport.SelectedIndex;

                MainWindow.gintTransactionID = TheNoRecordDataSet.norecord[intSelectedIndex].TransactionID;

                EditAfterHoursPunch editAfterHoursPunch = new EditAfterHoursPunch();
                editAfterHoursPunch.ShowDialog();
            }
            catch(Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // After Hours Summary Report // No Records Grid Selection " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // After Hours Summary Report // No Records Grid Selection " + Ex.Message);

                MessageBox.Show(Ex.ToString());
            }
        }
    }
}
