﻿using System;
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
using NewEmployeeDLL;
using NewEventLogDLL;
using RentalTrackingDLL;
using ProjectMatrixDLL;
using DateSearchDLL;
using DataValidationDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the public classes
        public static VerifyLogonDataSet TheVerifyLogonDataSet = new VerifyLogonDataSet();
        public static FindRentalTrackingTransactionsByPONumberDataSet TheFindRentalTrackingTransactionsByPONumberDataSet = new FindRentalTrackingTransactionsByPONumberDataSet();
        public static FindRentalTransactionByProjectIDDataSet TheFindRentalTransactionByProjectIDDataSet = new FindRentalTransactionByProjectIDDataSet();
        ProjectLastDateDataSet TheProjectLastDateDataSet = new ProjectLastDateDataSet();
        FindProjectMatrixByGreaterDateDataSet TheFindProjectMatrixByGreaterDateDataSet = new FindProjectMatrixByGreaterDateDataSet();
        public static VerifyEmployeeDataSet TheVerifyEmployeeDataSet = new VerifyEmployeeDataSet();

        //Setting up global variables
        public static bool gblnLoggedIn;
        public static int gintNoOfMisses;
        public static string gstrEmployeeGroup;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void expClose_Expanded(object sender, RoutedEventArgs e)
        {
            expClose.IsExpanded = false;
            TheMessagesClass.CloseTheProgram();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetSecurity();
            EnableExpanders(false);
            SendNewProjectReport();
        }
        private void EnableExpanders(bool blnExpanderStatus)
        {
            expEmployees.IsEnabled = blnExpanderStatus;
            expProjects.IsEnabled = blnExpanderStatus;
            expAssets.IsEnabled = blnExpanderStatus;
            expClose.IsEnabled = blnExpanderStatus;
            expHelp.IsEnabled = blnExpanderStatus;
            expInventory.IsEnabled = blnExpanderStatus;
            expITInfo.IsEnabled = blnExpanderStatus;
            expRentals.IsEnabled = blnExpanderStatus;
            expTasks.IsEnabled = blnExpanderStatus;
            expTrailers.IsEnabled = blnExpanderStatus;
            expVehicles.IsEnabled = blnExpanderStatus;
        }
        private void LoginComplete()
        {
            EnableExpanders(true);
            SetEmployeeSecurity();
            fraMainWindow.Content = new HomePage();
        }
        private void ResetSecurity()
        {
            expAssetReports.IsEnabled = true;
            expAssets.IsEnabled = true;
            expEmployeeAdministration.IsEnabled = true;
            expEmployeeDataEntry.IsEnabled = true;
            expEmployeeReports.IsEnabled = true;
            expEmployees.IsEnabled = true;
            expProjects.IsEnabled = true;
            expProjectAdministration.IsEnabled = true;
            expProjectDataEntry.IsEnabled = true;
            expProjectReports.IsEnabled = true;
            expITInfo.IsEnabled = true;
            expITDateEntry.IsEnabled = true;
            expITReports.IsEnabled = true;
            expPhoneDataEntry.IsEnabled = true;
            expPhoneReports.IsEnabled = true;         
            expRentals.IsEnabled = true;
            expHelpDeskTicketReport.IsEnabled = true;
            expServerAuditLogReport.IsExpanded = true;
            expEmployeeReports.IsEnabled = true;
            expEditProject.IsEnabled = true;
            expAddIncentivePayTitle.IsEnabled = true;
            expVerifyIncentivePay.IsEnabled = true;
            expEmployeeProjectLaborReport.IsExpanded = true;
            expVehicles.IsEnabled = true;

        }
        private void SetEmployeeSecurity()
        {
            try
            {
                if (gstrEmployeeGroup == "USERS")
                {
                    expITInfo.IsEnabled = false;
                    expAssets.IsEnabled = false;
                    expTrailerAdministration.IsEnabled = false;
                    expTrailerDataEntry.IsEnabled = false;
                    expEmployees.IsEnabled = false;
                    expProjects.IsEnabled = false;
                    expAssets.IsEnabled = false;
                    expRentals.IsEnabled = false;
                    expHelpDeskTicketReport.IsEnabled = false;
                    expServerAuditLogReport.IsEnabled = false;
                    expProjectReports.IsEnabled = false;
                    expEmployeeReports.IsEnabled = false;
                    expEmployeeProjectLaborReport.IsEnabled = false;
                    expVehicles.IsEnabled = false;
                }
                else if (gstrEmployeeGroup == "MANAGERS")
                {
                    expAssets.IsEnabled = false;
                    expTrailerAdministration.IsEnabled = false;
                    expITDateEntry.IsEnabled = false;
                    expITReports.IsEnabled = false;
                    expAssets.IsEnabled = false;
                    expPhoneDataEntry.IsEnabled = false;
                    expProjectAdministration.IsEnabled = false;
                    expEmployeeAdministration.IsEnabled = false;
                    expServerAuditLogReport.IsEnabled = false;
                }
                else if (gstrEmployeeGroup == "OFFICE")
                {
                    expAssets.IsEnabled = false;
                    expTrailerAdministration.IsEnabled = false;
                    expITDateEntry.IsEnabled = false;
                    expAssets.IsEnabled = false;
                    expPhoneDataEntry.IsEnabled = false;
                    expProjectAdministration.IsEnabled = false;
                    expEmployeeAdministration.IsEnabled = false;
                    expHelpDeskTicketReport.IsEnabled = false;
                    expServerAuditLogReport.IsEnabled = false;
                    expEmployeeProjectLaborReport.IsExpanded = false;
                }
                else if (gstrEmployeeGroup == "WAREHOUSE")
                {
                    expEmployees.IsEnabled = false;
                    expITInfo.IsEnabled = false;
                    expProjects.IsEnabled = false;
                    expTrailerAdministration.IsEnabled = false;
                    expProjectAdministration.IsEnabled = false;
                    expEmployeeAdministration.IsEnabled = false;
                    expServerAuditLogReport.IsEnabled = false;
                    expProjectReports.IsEnabled = false;
                    expEmployeeReports.IsEnabled = false;
                    expAddIncentivePayTitle.IsEnabled = false;
                    expVerifyIncentivePay.IsEnabled = false;
                }
                else if (gstrEmployeeGroup == "SUPER USER")
                {
                    expEmployeeAdministration.IsEnabled = false;
                    expProjectAdministration.IsEnabled = false;
                    expTrailerAdministration.IsEnabled = false;
                    expPhoneDataEntry.IsEnabled = false;
                    expITDateEntry.IsEnabled = false;
                    expServerAuditLogReport.IsEnabled = false;
                }
                else if ((gstrEmployeeGroup == "ADMIN") || (gstrEmployeeGroup == "IT"))
                {
                    TheMessagesClass.InformationMessage("Your are an Administrator of the Program");
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Main Window // Set Employee Security " + Ex.Message);

                TheSendEmailClass.SendEventLog("New Blue Jay ERP // Main Window // Set Employee Security " + Ex.ToString());
            }

        }
        private void SendNewProjectReport()
        {
            DateTime datTodaysDate = DateTime.Now;
            DateTime datLastDate;
            int intCounter;
            int intNumberOfRecords;
            string strEmailAddress = "newprojectnotificationdll@bluejaycommunications.com";
            string strHeader = "New Projects Created";
            string strMessage;
            string strManager;
            DateTime datTransactionDate;
            string strCustomerProjectID;
            string strAssignedProjectID;
            string strProjectName;
            string strDepartment;
            bool blnFatalError = false;
            int intTransactionID;

            try
            {
                TheProjectLastDateDataSet = TheProjectMatrixClass.GetProjectLastDateInfo();

                datLastDate = TheProjectLastDateDataSet.projectlastdate[0].LastDate;
                intTransactionID = TheProjectLastDateDataSet.projectlastdate[0].TransactionID;

                //expSubmitAfterHoursWork.IsEnabled = false;

                datLastDate = TheDateSearchClass.RemoveTime(datLastDate);

                datTodaysDate = TheDateSearchClass.RemoveTime(datTodaysDate);

                if ((datTodaysDate.DayOfWeek != DayOfWeek.Saturday) || (datTodaysDate.DayOfWeek != DayOfWeek.Sunday))
                {
                    if (datLastDate < datTodaysDate)
                    {
                        strMessage = "<h1>New Projects Created</h1>";
                        strMessage += "<table>";
                        strMessage += "<tr>";
                        strMessage += "<td><b>Date</b></td>";
                        strMessage += "<td><b>Customer Project ID</b></td>";
                        strMessage += "<td><b>Assigned Project ID</b></td>";
                        strMessage += "<td><b>Project Name</b></td>";
                        strMessage += "<td><b>Department</b></td>";
                        strMessage += "<td><b>Assigned Manager</b></td>";
                        strMessage += "</tr>";
                        strMessage += "<p>               </p>";

                        TheFindProjectMatrixByGreaterDateDataSet = TheProjectMatrixClass.FindProjectMatrixByGreaterDate(datLastDate);

                        intNumberOfRecords = TheFindProjectMatrixByGreaterDateDataSet.FindProjectMatrixByGreaterDate.Rows.Count;

                        if (intNumberOfRecords > 0)
                        {
                            for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                            {
                                datTransactionDate = TheFindProjectMatrixByGreaterDateDataSet.FindProjectMatrixByGreaterDate[intCounter].TransactionDate;
                                strCustomerProjectID = TheFindProjectMatrixByGreaterDateDataSet.FindProjectMatrixByGreaterDate[intCounter].CustomerAssignedID;
                                strAssignedProjectID = TheFindProjectMatrixByGreaterDateDataSet.FindProjectMatrixByGreaterDate[intCounter].AssignedProjectID;
                                strProjectName = TheFindProjectMatrixByGreaterDateDataSet.FindProjectMatrixByGreaterDate[intCounter].ProjectName;
                                strDepartment = TheFindProjectMatrixByGreaterDateDataSet.FindProjectMatrixByGreaterDate[intCounter].Department;
                                strManager = TheFindProjectMatrixByGreaterDateDataSet.FindProjectMatrixByGreaterDate[intCounter].FirstName + " ";
                                strManager += TheFindProjectMatrixByGreaterDateDataSet.FindProjectMatrixByGreaterDate[intCounter].LastName;

                                strMessage += "<tr>";
                                strMessage += "<td>" + Convert.ToString(datTransactionDate) + "</td>";
                                strMessage += "<td>" + strCustomerProjectID + "</td>";
                                strMessage += "<td>" + strAssignedProjectID + "</td>";
                                strMessage += "<td>" + strProjectName + "</td>";
                                strMessage += "<td>" + strDepartment + "</td>";
                                strMessage += "<td>" + strManager + "</td>";
                                strMessage += "</tr>";
                            }
                        }

                        strMessage += "</table>";

                        blnFatalError = !(TheSendEmailClass.SendEmail(strEmailAddress, strHeader, strMessage));

                        if (blnFatalError == true)
                            throw new Exception();

                        blnFatalError = TheProjectMatrixClass.UpdateProjectLastDate(intTransactionID, datTodaysDate);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Main Window // Send New Project Report " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expEmployees_Expanded(object sender, RoutedEventArgs e)
        {
            ResetAssetExpanders();
            ResetHelpExpanders();
            ResetInventoryExpanders();
            ResetITExpanders();
            ResetProjectExpanders();
            ResetRentalExpanders();
            ResetTaskExpanders();
            ResetTrailerExpanders();
            ResetVehicleExpanders();
          
        }

        private void expProjects_Expanded(object sender, RoutedEventArgs e)
        {
            ResetAssetExpanders();
            ResetEmployeeExpanders();
            ResetHelpExpanders();
            ResetInventoryExpanders();
            ResetITExpanders();
            ResetRentalExpanders();
            ResetTaskExpanders();
            ResetTrailerExpanders();
            ResetVehicleExpanders();
        }

        private void expRentals_Expanded(object sender, RoutedEventArgs e)
        {
            ResetAssetExpanders();
            ResetEmployeeExpanders();
            ResetHelpExpanders();
            ResetInventoryExpanders();
            ResetITExpanders();
            ResetProjectExpanders();
            ResetTaskExpanders();
            ResetTrailerExpanders();
            ResetVehicleExpanders();
        }

        private void expVehicles_Expanded(object sender, RoutedEventArgs e)
        {
            ResetAssetExpanders();
            ResetEmployeeExpanders();
            ResetHelpExpanders();
            ResetInventoryExpanders();
            ResetITExpanders();
            ResetProjectExpanders();
            ResetRentalExpanders();
            ResetTaskExpanders();
            ResetTrailerExpanders();
        }

        private void expTrailers_Expanded(object sender, RoutedEventArgs e)
        {
            ResetAssetExpanders();
            ResetEmployeeExpanders();
            ResetHelpExpanders();
            ResetInventoryExpanders();
            ResetITExpanders();
            ResetProjectExpanders();
            ResetRentalExpanders();
            ResetTaskExpanders();
            ResetVehicleExpanders();
        }

        private void expInventory_Expanded(object sender, RoutedEventArgs e)
        {
            ResetAssetExpanders();
            ResetEmployeeExpanders();
            ResetHelpExpanders();
            ResetITExpanders();
            ResetProjectExpanders();
            ResetRentalExpanders();
            ResetTaskExpanders();
            ResetTrailerExpanders();
            ResetVehicleExpanders();
        }

        private void expAssets_Expanded(object sender, RoutedEventArgs e)
        {
            ResetEmployeeExpanders();
            ResetHelpExpanders();
            ResetInventoryExpanders();
            ResetITExpanders();
            ResetProjectExpanders();
            ResetRentalExpanders();
            ResetTaskExpanders();
            ResetTrailerExpanders();
            ResetVehicleExpanders();
        }

        private void expITInfo_Expanded(object sender, RoutedEventArgs e)
        {
            ResetAssetExpanders();
            ResetEmployeeExpanders();
            ResetHelpExpanders();
            ResetInventoryExpanders();
            ResetProjectExpanders();
            ResetRentalExpanders();
            ResetTaskExpanders();
            ResetTrailerExpanders();
            ResetVehicleExpanders();
        }

        private void expTasks_Expanded(object sender, RoutedEventArgs e)
        {
            ResetAssetExpanders();
            ResetEmployeeExpanders();
            ResetHelpExpanders();
            ResetInventoryExpanders();
            ResetITExpanders();
            ResetProjectExpanders();
            ResetRentalExpanders();
            ResetTrailerExpanders();
            ResetVehicleExpanders();
        }

        private void expHelp_Expanded(object sender, RoutedEventArgs e)
        {
            ResetAssetExpanders();
            ResetEmployeeExpanders();
            ResetInventoryExpanders();
            ResetITExpanders();
            ResetProjectExpanders();
            ResetRentalExpanders();
            ResetTaskExpanders();
            ResetTrailerExpanders();
            ResetVehicleExpanders();
        }
        private void ResetEmployeeExpanders()
        {   expEmployees.IsExpanded = false;
            expEmployeeDataEntry.IsExpanded = false;
            expEmployeeReports.IsExpanded = false;
            expEmployeeAdministration.IsExpanded = false;
        }
        private void ResetProjectExpanders()
        {
            expProjects.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false; ;
            expProjectReports.IsExpanded = false ;
            expProjectAdministration.IsExpanded = false ;
        }
        private void ResetRentalExpanders()
        {
            expRentals.IsExpanded = false;
            expRentalDataEntry.IsExpanded = false;
            expRentalReports.IsExpanded = false;
            expRentalAdministration.IsExpanded= false;
        }
        private void ResetVehicleExpanders()
        {
            expVehicles.IsExpanded = false;            
        }
        private void ResetTrailerExpanders()
        {
            expTrailers.IsExpanded = false;
        }
        private void ResetInventoryExpanders()
        {
            expInventory.IsExpanded = false;
            expInventoryReports.IsExpanded= false;
        }
        private void ResetAssetExpanders()
        {
            expAssets.IsExpanded = false;
            expAssetReports.IsExpanded = false;
        }
        private void ResetITExpanders()
        {
            expITInfo.IsExpanded = false;
            expITDateEntry.IsExpanded = false;
            expITReports.IsExpanded = false;
            expPhoneDataEntry.IsExpanded = false;
            expPhoneReports.IsExpanded = false;
        }
        private void ResetTaskExpanders()
        {
            expTasks.IsExpanded = false;            
        }
        private void ResetHelpExpanders()
        {
            expHelp.IsExpanded = false;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strValueForValidation;
            int intEmployeeID = 0;
            string strLastName;
            bool blnFatalError = false;
            int intRecordsReturned;
            string strErrorMessage = "";

            //beginning data validation
            strValueForValidation = pbxEmployeeID.Password;
            strLastName = txtLastName.Text;
            blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
            if (blnFatalError == true)
            {
                strErrorMessage = "The Employee ID is not an Integer\n";
            }
            else
            {
                intEmployeeID = Convert.ToInt32(strValueForValidation);
            }
            if (strLastName == "")
            {
                blnFatalError = true;
                strErrorMessage += "The Last Name Was Not Entered\n";
            }
            if (blnFatalError == true)
            {
                TheMessagesClass.ErrorMessage(strErrorMessage);
                return;
            }

            //filling the data set
            MainWindow.TheVerifyLogonDataSet = TheEmployeeClass.VerifyLogon(intEmployeeID, strLastName);

            intRecordsReturned = MainWindow.TheVerifyLogonDataSet.VerifyLogon.Rows.Count;

            if (intRecordsReturned == 0)
            {
                LogonFailed();
            }
            else
            {
                blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(intEmployeeID, "NEW BLUE JAY ERP BROWSER // USER LOGIN");

                gblnLoggedIn = true;
                gstrEmployeeGroup = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup;
                LoginComplete();
            }
        }
        private void LogonFailed()
        {
            string strLogEntry;


            gintNoOfMisses++;

            if (gintNoOfMisses == 3)
            {
                strLogEntry = "There Have Been Three Attemps to Sign Into New Blue Jay ERP - Browser System";

                //TheSendEmailClass.SendEventLog(strLogEntry);

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, strLogEntry);

                TheSendEmailClass.SendEventLog(strLogEntry);

                TheMessagesClass.ErrorMessage("You Have Tried To Sign In Three Times\nThe Program Will Now Close");

                Application.Current.Shutdown();
            }
            else
            {
                TheMessagesClass.InformationMessage("You Have Failed The Sign In Process");
                return;
            }
        }
    }
}
