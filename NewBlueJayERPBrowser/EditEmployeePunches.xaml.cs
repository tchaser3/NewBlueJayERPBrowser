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
using DataValidationDLL;
using DateSearchDLL;
using Excel = Microsoft.Office.Interop.Excel;
using NewEventLogDLL;
using NewEmployeeDLL;
using EmployeeDateEntryDLL;
using EmployeePunchedHoursDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for EditEmployeePunches.xaml
    /// </summary>
    public partial class EditEmployeePunches : Page
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        EmployeePunchedHoursClass TheEmployeePunchedHoursClass = new EmployeePunchedHoursClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        //setting up the data
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindAholaClockPUnchesForEmployeeDataSet TheFindAholaClockPunchesForEmployeeDataSet = new FindAholaClockPUnchesForEmployeeDataSet();
        FindAlohaEmployeeHoursOverDateRangeDataSet TheFindAlohaEmployeeHoursOverDateRangeDataSet = new FindAlohaEmployeeHoursOverDateRangeDataSet();

        //setting up global variables
        public static int gintEmployeeID;
        public static DateTime gdatPayPeriodEndingDate;
        public static DateTime gdatStartDate;
        public static DateTime gdatEndingDate;

        public EditEmployeePunches()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            int intCounter;
            int intNumberOfRecords;

            try
            {
                expAddPunch.IsEnabled = false;
                expPunchReport.IsEnabled = false;

                TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.gintLoggedInEmployeeID, "NEW BLUE JAY ERP BROWSER // EDIT EMPLOYEE PUNCHES");

                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");
                cboSelectEmployee.SelectedIndex = 0;
                txtEndPayDate.Text = "";
                txtEnterLastName.Text = "";

                TheFindAholaClockPunchesForEmployeeDataSet = TheEmployeePunchedHoursClass.FindAholaClockPunchesForEmployee(0, DateTime.Now, DateTime.Now);

                dgrEmployeePunches.ItemsSource = TheFindAholaClockPunchesForEmployeeDataSet.FindAholaClockPunchesForEmployee;
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Edit Employee Punches // Page Loaded " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Edit Employee Punches // Page Loaded " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intNumberOfRecords;
            int intCounter;
            string strFullName = "";

            try
            {
                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");
                strLastName = txtEnterLastName.Text;
                if(strLastName.Length > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count;

                    if(intNumberOfRecords < 1)
                    {
                        TheMessagesClass.ErrorMessage("Employee Not Found");
                        return;
                    }

                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        strFullName = TheComboEmployeeDataSet.employees[intCounter].FullName;
                        cboSelectEmployee.Items.Add(strFullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Edit Employee Punches // Enter Last Name Text Box " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Edit Employee Punches // Enter Last Name Text Box " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

            try
            {
                if (intSelectedIndex > -1)
                {
                    gintEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
                }
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Edit Employee Punches // Select Employee Combobox " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Edit Employee Punches // Select Employee Combobox " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //this will find the employees punches
            string strValueForValidation;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";

            try
            {
                strValueForValidation = txtEndPayDate.Text; ;

                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);

                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Date Entered is not a Date\n";
                }
                else
                {
                    gdatPayPeriodEndingDate = Convert.ToDateTime(strValueForValidation);

                    if(gdatPayPeriodEndingDate.DayOfWeek != DayOfWeek.Sunday)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Date Entered is not a Sunday\n";
                    }
                }

                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                gdatStartDate = TheDateSearchClass.SubtractingDays(gdatPayPeriodEndingDate, 6);
                gdatEndingDate = TheDateSearchClass.AddingDays(gdatPayPeriodEndingDate, 1);

                expAddPunch.IsEnabled = true;
                expPunchReport.IsEnabled = true;

                UpdateGridInfo();
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Edit Employee Punches // Search Button " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Edit Employee Punches // Search Button " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void UpdateGridInfo()
        {
            int intRecordsReturned;

            try
            {
                TheFindAholaClockPunchesForEmployeeDataSet = TheEmployeePunchedHoursClass.FindAholaClockPunchesForEmployee(gintEmployeeID, gdatStartDate, gdatEndingDate);

                dgrEmployeePunches.ItemsSource = TheFindAholaClockPunchesForEmployeeDataSet.FindAholaClockPunchesForEmployee;

                TheFindAlohaEmployeeHoursOverDateRangeDataSet = TheEmployeePunchedHoursClass.FindAlohaEmployeeHoursOverDateRange(gintEmployeeID, gdatStartDate, gdatEndingDate);

                intRecordsReturned = TheFindAlohaEmployeeHoursOverDateRangeDataSet.FindAlohaEmployeeHoursOverDateRange.Rows.Count;

                if (intRecordsReturned == 1)
                {
                    txtEmployeeTotalHours.Text = Convert.ToString(TheFindAlohaEmployeeHoursOverDateRangeDataSet.FindAlohaEmployeeHoursOverDateRange[0].TotalHours);
                }
                else if (intRecordsReturned > 1)
                {
                    txtEmployeeTotalHours.Text = "0";

                    TheMessagesClass.ErrorMessage("More Than One Record Was Returned, Please Create a Help Desk Ticket For IT");
                    return;
                }
                else if (intRecordsReturned == 0)
                {
                    txtEmployeeTotalHours.Text = "0";
                }
                
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Edit Employee Punches // Update Grid Info " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Edit Employee Punches // Update Grid Info " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expResetWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expResetWindow.IsExpanded = false;
            ResetControls();
        }

        private void expAddPunch_Expanded(object sender, RoutedEventArgs e)
        {
            MainWindow.gintEmployeeID = gintEmployeeID;

            expAddPunch.IsExpanded = false;

            AddEmployeePunch AddEmployeePunch = new AddEmployeePunch();
            AddEmployeePunch.ShowDialog();

            UpdateGridInfo();
        }
    }
}
