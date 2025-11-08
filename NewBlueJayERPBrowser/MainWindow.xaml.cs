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
using NewEmployeeDLL;
using NewEventLogDLL;
using RentalTrackingDLL;
using ProjectMatrixDLL;
using DateSearchDLL;
using DataValidationDLL;
using EmployeeDateEntryDLL;
using System.Windows.Media.Animation;

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
        public static ProjectWorkCompletedDataSet TheEmployeeWorkCompleteDataSet = new ProjectWorkCompletedDataSet();
        public static ProjectWorkCompletedDataSet TheProjectWorkCompletedDataSet = new ProjectWorkCompletedDataSet();
        FindEmployeeByLastNameDataSet TheFindEmployeeByLastNameDataSet = new FindEmployeeByLastNameDataSet();

        //Setting up global variables
        public static bool gblnLoggedIn;
        public static int gintNoOfMisses;
        public static string gstrEmployeeGroup;
        public static string gstrUserName;
        public static int gintLoggedInEmployeeID;
        public static int gintEmployeeID;
        public static int gintTransactionID;
        public static int gintProjectID;
        public static string gstrCustomerProjectID;
        public static string gstrAssignedProjectID;
        public static bool gblnOutageProject;
        public static int gintDepartmentID;
        public static int gintStatusID;
        public static int gintOfficeID;
        public static string gstrLastName;
        public static string gstrFirstName;
        public static int gintWorkTaskID;
        public static string gstrWorkTask;
        public static DateTime gdatProductionDate;
        public static string gstrWarehouseName;
        public static bool gblnKeepNewEmployee;
        public static int gintWarehouseID;
        public static string gstrModule;
        public static string gstrVehicleNumber;
        public static int gintVehicleID;

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
            //setting variables
            int intCounter;
            int intNumberOfRecords;
            string strComputerName;

            try
            {
                ResetSecurity();
                EnableExpanders(false);
                SendNewProjectReport();
                strComputerName = System.Environment.MachineName;
                gstrUserName = System.Environment.UserName;
;
                CheckEmployee(strComputerName, gstrUserName);
                //LoginComplete();
                
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Blue Jay Project Management Class // Main Window // Window Loaded " + Ex.ToString());

                TheSendEmailClass.SendEventLog("Blue Jay Project Management Class // Main Window // Window Loaded " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
                
            }

            
            
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
            expHome.IsEnabled = blnExpanderStatus;
            expProjectManagement.IsEnabled = blnExpanderStatus;
            expEmployeeProductivity.IsEnabled = blnExpanderStatus;
        }
        private void CheckEmployee(string strComputerName, string strUserName)
        {
            string strFirstName;
            string strLastName;
            int intCounter;
            int intNumberOfRecords;
            string strTempFirstName;

            try
            {

                strLastName = strUserName.Substring(1).ToUpper();
                strFirstName = strUserName.Substring(0, 1).ToUpper();

                TheFindEmployeeByLastNameDataSet = TheEmployeeClass.FindEmployeesByLastNameKeyWord(strLastName);

                intNumberOfRecords = TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName.Rows.Count;

                if (intNumberOfRecords == 1)
                {
                    gintEmployeeID = TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName[0].EmployeeID;
                }
                else if (intNumberOfRecords > 1)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        strTempFirstName = TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName[intCounter].FirstName.Substring(0, 1).ToUpper();

                        if (strTempFirstName == strFirstName)
                        {
                            gintEmployeeID = TheFindEmployeeByLastNameDataSet.FindEmployeeByLastName[intCounter].EmployeeID;
                        }
                    }
                }

                MainWindow.TheVerifyLogonDataSet = TheEmployeeClass.VerifyLogon(gintEmployeeID, strLastName);

                intNumberOfRecords = MainWindow.TheVerifyLogonDataSet.VerifyLogon.Rows.Count;
                if(intNumberOfRecords < 1)
                {
                    TheMessagesClass.ErrorMessage("The Employee ID and Last Name Do Not Match");
                    throw new Exception("The Employee ID and Last Name Do Not Match");
                }
                else
                {
                    gstrEmployeeGroup = TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup;
                }

                    TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(gintEmployeeID, strUserName + " " + strComputerName + " New Bluejay ERP Browser");
                LoginComplete();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Bluejay ERP Browser // Main Window // Check Employee " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Bluejay ERP Browser // Main Window // Check Employee " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
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
            expEmployeeProjectLaborReport.IsEnabled = true;
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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Main Window // Set Employee Security " + Ex.Message);

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Main Window // Set Employee Security " + Ex.ToString());
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

        //private void btnSignIn_Click(object sender, RoutedEventArgs e)
        //{
        //    //setting local variables
        //    string strValueForValidation;
        //    string strLastName;
        //    bool blnFatalError = false;
        //    int intRecordsReturned;
        //    string strErrorMessage = "";

        //    //beginning data validation
        //    strValueForValidation = pbxEmployeeID.Password;
        //    strLastName = txtLastName.Text;
        //    blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
        //    if (blnFatalError == true)
        //    {
        //        strErrorMessage = "The Employee ID is not an Integer\n";
        //    }
        //    else
        //    {
        //        gintLoggedInEmployeeID = Convert.ToInt32(strValueForValidation);
        //    }
        //    if (strLastName == "")
        //    {
        //        blnFatalError = true;
        //        strErrorMessage += "The Last Name Was Not Entered\n";
        //    }
        //    if (blnFatalError == true)
        //    {
        //        TheMessagesClass.ErrorMessage(strErrorMessage);
        //        return;
        //    }

        //    //filling the data set
        //    MainWindow.TheVerifyLogonDataSet = TheEmployeeClass.VerifyLogon(gintLoggedInEmployeeID, strLastName);

        //    intRecordsReturned = MainWindow.TheVerifyLogonDataSet.VerifyLogon.Rows.Count;

        //    if (intRecordsReturned == 0)
        //    {
        //        LogonFailed();
        //    }
        //    else
        //    {
        //        blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(gintLoggedInEmployeeID, "NEW BLUE JAY ERP BROWSER // USER LOGIN");

        //        gblnLoggedIn = true;
        //        gstrEmployeeGroup = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup;
                
        //        LoginComplete();
        //    }
        //}
        //private void LogonFailed()
        //{
        //    string strLogEntry;


        //    gintNoOfMisses++;

        //    if (gintNoOfMisses == 3)
        //    {
        //        strLogEntry = "There Have Been Three Attemps to Sign Into New Blue Jay ERP - Browser System";

        //        //TheSendEmailClass.SendEventLog(strLogEntry);

        //        TheEventLogClass.InsertEventLogEntry(DateTime.Now, strLogEntry);

        //        TheSendEmailClass.SendEventLog(strLogEntry);

        //        TheMessagesClass.ErrorMessage("You Have Tried To Sign In Three Times\nThe Program Will Now Close");

        //        Application.Current.Shutdown();
        //    }
        //    else
        //    {
        //        TheMessagesClass.InformationMessage("You Have Failed The Sign In Process");
        //        return;
        //    }
        //}

        private void expAddNonProductionTask_Expanded(object sender, RoutedEventArgs e)
        {
            AddNonProductionTask AddNonProductionTask = new AddNonProductionTask();
            fraMainWindow.Navigate(AddNonProductionTask);
            expAddNonProductionTask.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjects.IsExpanded = false;
        }

        private void expProjectDashboards_Expanded(object sender, RoutedEventArgs e)
        {
            //expProjectDashboards.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            //expJSIDataEntry.IsExpanded = false;
            //expJSIReports.IsExpanded = false;
        }

        private void expProjectDataEntry_Expanded(object sender, RoutedEventArgs e)
        {
            expProjectDashboards.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            //expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            //expJSIDataEntry.IsExpanded = false;
            //expJSIReports.IsExpanded = false;
        }

        private void expJSIDataEntry_Expanded(object sender, RoutedEventArgs e)
        {
            expProjectDashboards.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            //expJSIDataEntry.IsExpanded = false;
            //expJSIReports.IsExpanded = false;
        }

        private void expJSIReports_Expanded(object sender, RoutedEventArgs e)
        {
            expProjectDashboards.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            //expJSIDataEntry.IsExpanded = false;
            //expJSIReports.IsExpanded = false;
        }

        private void expProjectReports_Expanded(object sender, RoutedEventArgs e)
        {
            expProjectDashboards.IsExpanded = false;
            expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            //expProjectReports.IsExpanded = false;
            //expJSIDataEntry.IsExpanded = false;
            //expJSIReports.IsExpanded = false;
        }

        private void expProjectAdministration_Expanded(object sender, RoutedEventArgs e)
        {
            expProjectDashboards.IsExpanded = false;
            //expProjectAdministration.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjectReports.IsExpanded = false;
            //expJSIDataEntry.IsExpanded = false;
            //expJSIReports.IsExpanded = false;
        }

        private void expAddNewProject_Expanded(object sender, RoutedEventArgs e)
        {
            AddProject AddProject = new AddProject();
            fraMainWindow.Navigate(AddProject);
            expAddNewProject.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjects.IsExpanded = false;
            
        }

        private void expAddOutageProject_Expanded(object sender, RoutedEventArgs e)
        {
            AddOutageProject addOutageProject = new AddOutageProject();
            fraMainWindow.Navigate(addOutageProject);
            expAddOutageProject.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjects.IsExpanded = false;
        }

        private void expAddOutageProjectStatus_Expanded(object sender, RoutedEventArgs e)
        {
            AddOutageProjectStatus addOutageProjectStatus = new AddOutageProjectStatus();
            fraMainWindow.Navigate(addOutageProjectStatus);
            expAddOutageProjectStatus.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjects.IsExpanded = false;
        }

        private void expAddOutageProductivity_Expanded(object sender, RoutedEventArgs e)
        {
            AddOutageProductivity addOutageProductivity = new AddOutageProductivity();
            fraMainWindow.Navigate(addOutageProductivity);
            expAddOutageProductivity.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjects.IsExpanded = false;
        }

        private void expHome_Expanded(object sender, RoutedEventArgs e)
        {
            expHome.IsExpanded = false;
            fraMainWindow.Content = new HomePage();
            ResetAssetExpanders();
            ResetEmployeeExpanders();
            ResetInventoryExpanders();
            ResetITExpanders();
            ResetProjectExpanders();
            ResetRentalExpanders();
            ResetTaskExpanders();
            ResetTrailerExpanders();
            ResetVehicleExpanders();
            ResetHelpExpanders();

        }

        private void expEditEmployeePunches_Expanded(object sender, RoutedEventArgs e)
        {
            EditEmployeePunches editEmployeePunches = new EditEmployeePunches();
            fraMainWindow.Navigate(editEmployeePunches);
            expEditEmployeePunches.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjects.IsExpanded = false;
        }

        private void expAfterHoursSummaryReport_Expanded(object sender, RoutedEventArgs e)
        {
            AfterHoursSummaryReport afterHoursSummaryReport = new AfterHoursSummaryReport();
            fraMainWindow.Navigate(afterHoursSummaryReport);
            expProjectReports.IsExpanded = false;
            expProjects.IsExpanded = false;
            expAfterHoursSummaryReport.IsExpanded = false;
        }

        private void expAddNonProductionProductivity_Expanded(object sender, RoutedEventArgs e)
        {

        }

        private void expCompareEmployeeCrews_Expanded(object sender, RoutedEventArgs e)
        {
            CompareCrews compareCrews = new CompareCrews();
            fraMainWindow.Navigate(compareCrews);
            expEmployeeReports.IsExpanded = false;
            expEmployees.IsExpanded = false;
            expCompareEmployeeCrews.IsExpanded = false;
        }

        private void expEditProject_Expanded(object sender, RoutedEventArgs e)
        {
            EditProject editProject = new EditProject();
            fraMainWindow.Navigate(editProject);
            expProjectDataEntry.IsExpanded = false;
            expProjects.IsExpanded = false;
        }

        private void fraMainWindow_Navigated(object sender, NavigationEventArgs e)
        {

        }

        private void expImportFleetIOVehicles_Expanded(object sender, RoutedEventArgs e)
        {
            ImportFleetIOVehicles importFleetIOVehicles = new ImportFleetIOVehicles();
            fraMainWindow.Navigate(importFleetIOVehicles);
            expVehicleAdministration.IsExpanded = false;
            expVehicles.IsExpanded = false;
        }

        private void expEditOutageProject_Expanded(object sender, RoutedEventArgs e)
        {
            SelectOutageProject selectOutageProject = new SelectOutageProject();
            fraMainWindow.Navigate(selectOutageProject);
            expEditOutageProject.IsExpanded = false;
            expEditProject.IsExpanded = false;
            expProjectDataEntry.IsExpanded = false;
            expProjects.IsExpanded = false;
        }

        private void expProjectManagement_Expanded(object sender, RoutedEventArgs e)
        {
            ProjectManagementPage projectManagementPage = new ProjectManagementPage();
            fraMainWindow.Navigate(projectManagementPage);
            expProjectManagement.IsExpanded = false;
        }

        private void expAddEmployee_Expanded(object sender, RoutedEventArgs e)
        {
            AddEmployee addEmployee = new AddEmployee();
            fraMainWindow.Navigate(addEmployee);
            expAddEmployee.IsExpanded = false;
        }

        private void expOpenProjectsDashboard_Expanded(object sender, RoutedEventArgs e)
        {
            OpenProjectDashboard openProjectDashboard = new OpenProjectDashboard();
            fraMainWindow.Navigate(openProjectDashboard);
            expOpenProjectsDashboard.IsExpanded = false;
        }

        private void expOverDueProjectsDashboard_Expanded(object sender, RoutedEventArgs e)
        {
            OverdueProjectDashboard overdueProjectsDashboard = new OverdueProjectDashboard();
            fraMainWindow.Navigate(overdueProjectsDashboard);
            expOverDueProjectsDashboard.IsExpanded = false;
        }

        private void expCreateProductivitySheets_Expanded(object sender, RoutedEventArgs e)
        {
            PrintProductivitySheets printProductivitySheets = new PrintProductivitySheets();
            fraMainWindow.Navigate(printProductivitySheets);
            expCreateProductivitySheets.IsExpanded = false;
        }

        private void expEmployeeProjectLaborReport_Expanded(object sender, RoutedEventArgs e)
        {
            EmployeeProjectLaborReport employeeProjectLaborReport = new EmployeeProjectLaborReport();
            fraMainWindow.Navigate(employeeProjectLaborReport);
            expEmployeeProjectLaborReport.IsExpanded = false;
        }

        private void expOverdueProjectReport_Expanded(object sender, RoutedEventArgs e)
        {
            OverdueProjectReport overdueProjectReport = new OverdueProjectReport();
            fraMainWindow.Navigate(overdueProjectReport);
            expOverdueProjectReport.IsExpanded = false;
        }

        private void expFootagesReport_Expanded(object sender, RoutedEventArgs e)
        {
            TotalFootages totalFootages = new TotalFootages();
            fraMainWindow.Navigate(totalFootages);
            expFootagesReport.IsExpanded = false;
        }

        private void expInvoiceReport_Expanded(object sender, RoutedEventArgs e)
        {
            ProjectOpenInvoicing projectOpenInvoicing = new ProjectOpenInvoicing();
            fraMainWindow.Navigate(projectOpenInvoicing);
            expInvoiceReport.IsExpanded = false;
        }

        private void expProductivityCostingReport_Expanded(object sender, RoutedEventArgs e)
        {
           
        }

        private void expProductivityReport_Expanded(object sender, RoutedEventArgs e)
        {
            expProductivityReport.IsExpanded = false;
            ProjectProductivityReport projectProductivityReport = new ProjectProductivityReport();
            fraMainWindow.Navigate(projectProductivityReport);
        }
    }
}
