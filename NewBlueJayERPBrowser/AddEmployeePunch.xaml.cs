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
using NewEmployeeDLL;
using DataValidationDLL;
using DateSearchDLL;
using EmployeeDateEntryDLL;
using EmployeePunchedHoursDLL;
using NewEventLogDLL;
using System.ComponentModel;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for AddEmployeePunch.xaml
    /// </summary>
    public partial class AddEmployeePunch : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        EmployeePunchedHoursClass TheEmployeePunchedHoursClass = new EmployeePunchedHoursClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        //data sets
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindAholaClockPUnchesForEmployeeDataSet TheFindAholaClockPunchesForemployeeDataSet = new FindAholaClockPUnchesForEmployeeDataSet();
        FindAholaEmployeePunchHoursDataSet TheFindAholaEmployeePunchHoursDataSet = new FindAholaEmployeePunchHoursDataSet();
        CurrentEmployeePunchesDataSet TheCurrentEmployeePunchesData = new CurrentEmployeePunchesDataSet();
        CurrentEmployeeTimeDataSet TheCurrentEmployeeTimeDataSet = new CurrentEmployeeTimeDataSet();

        /*
         * Items needed for the punch to be inserted
         * EmployeeID
         * PayID
         * ActualDateTime, PunchDateTime, CreatedDateTime - All are the same
         * PayGroup - Hourly
         * PunchMode - MANUAL
         * PunchType - ADDEDPERHR
         * PunchSource - BLUEJAYERP
         * PunchUser - ERP User
         * PunchIPAddress - IP Address of local computer
         * LastUpdate - this will be the same as the PunchDateTime        * 
         * 
         */

        public AddEmployeePunch()
        {
            InitializeComponent();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseWindow.IsExpanded = false;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            int intNumberOfRecords;
            string strFullName;
            string strUserName;
            string strComputerName;
            string strIPAddress;
            int intCounter;

            try
            {
                strComputerName = System.Environment.MachineName;
                strUserName = System.Environment.UserName;
                strIPAddress = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.GetValue(0).ToString();
                TheCurrentEmployeePunchesData.currentpunches.Rows.Clear();
                TheCurrentEmployeeTimeDataSet.currentemployeetime.Rows.Clear();

                TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(MainWindow.gintEmployeeID);

                intNumberOfRecords = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID.Rows.Count;

                if(intNumberOfRecords < 1)
                {
                    throw new Exception("NO RECORDS FOUND");
                }

                strFullName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName + " " + TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;
                txtEmployeeName.Text = strFullName;
                txtEmployeeID.Text = Convert.ToString(MainWindow.gintEmployeeID);
                txtPayID.Text = Convert.ToString(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].PayID);
                txtPayGroup.Text = "HOURLY";
                txtPunchMode.Text = "MANUAL";
                txtPunchType.Text = "ADDPERHR";
                txtPunchUpdateTime.Text = Convert.ToString(DateTime.Now);
                txtPunchSource.Text = "BLUEJAYERP";
                txtPunchUser.Text = strUserName.ToUpper();
                txtPunchIPAddress.Text = strIPAddress.ToUpper();

                TheFindAholaClockPunchesForemployeeDataSet = TheEmployeePunchedHoursClass.FindAholaClockPunchesForEmployee(MainWindow.gintEmployeeID, EditEmployeePunches.gdatStartDate, EditEmployeePunches.gdatEndingDate);

                intNumberOfRecords = TheFindAholaClockPunchesForemployeeDataSet.FindAholaClockPunchesForEmployee.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        CurrentEmployeePunchesDataSet.currentpunchesRow NewEmployeePunch = TheCurrentEmployeePunchesData.currentpunches.NewcurrentpunchesRow();

                        NewEmployeePunch.ActualDateTime = TheFindAholaClockPunchesForemployeeDataSet.FindAholaClockPunchesForEmployee[intCounter].ActualDateTime;
                        NewEmployeePunch.EmployeeID = MainWindow.gintEmployeeID;
                        NewEmployeePunch.PayGroup = TheFindAholaClockPunchesForemployeeDataSet.FindAholaClockPunchesForEmployee[intCounter].PayGroup;
                        NewEmployeePunch.PayID = TheFindAholaClockPunchesForemployeeDataSet.FindAholaClockPunchesForEmployee[intCounter].PayID;
                        NewEmployeePunch.PunchedDateTime = TheFindAholaClockPunchesForemployeeDataSet.FindAholaClockPunchesForEmployee[intCounter].PunchDateTime;
                        NewEmployeePunch.PunchIPAddress = TheFindAholaClockPunchesForemployeeDataSet.FindAholaClockPunchesForEmployee[intCounter].PunchIPAddress;
                        NewEmployeePunch.PunchMode = TheFindAholaClockPunchesForemployeeDataSet.FindAholaClockPunchesForEmployee[intCounter].PunchMode;
                        NewEmployeePunch.PunchSource = TheFindAholaClockPunchesForemployeeDataSet.FindAholaClockPunchesForEmployee[intCounter].PunchSource;
                        NewEmployeePunch.PunchType = TheFindAholaClockPunchesForemployeeDataSet.FindAholaClockPunchesForEmployee[intCounter].PunchType;
                        NewEmployeePunch.PunchUser = TheFindAholaClockPunchesForemployeeDataSet.FindAholaClockPunchesForEmployee[intCounter].PunchUser;
                        NewEmployeePunch.TransactionID = TheFindAholaClockPunchesForemployeeDataSet.FindAholaClockPunchesForEmployee[intCounter].TransactionID;

                        TheCurrentEmployeePunchesData.currentpunches.Rows.Add(NewEmployeePunch);
                    }                    
                }

                drgPunches.ItemsSource = TheCurrentEmployeePunchesData.currentpunches;

                TheFindAholaEmployeePunchHoursDataSet = TheEmployeePunchedHoursClass.FindAholaEmployeePunchHours(MainWindow.gintEmployeeID, EditEmployeePunches.gdatStartDate, EditEmployeePunches.gdatEndingDate);

                intNumberOfRecords = TheFindAholaEmployeePunchHoursDataSet.FindAholaEmployeePunchHours.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        CurrentEmployeeTimeDataSet.currentemployeetimeRow NewEmployeeTime = TheCurrentEmployeeTimeDataSet.currentemployeetime.NewcurrentemployeetimeRow();

                        NewEmployeeTime.DailyHours = TheFindAholaEmployeePunchHoursDataSet.FindAholaEmployeePunchHours[intCounter].DailyHours;
                        NewEmployeeTime.EmployeeID = MainWindow.gintEmployeeID;
                        NewEmployeeTime.EndDate = TheFindAholaEmployeePunchHoursDataSet.FindAholaEmployeePunchHours[intCounter].EndDate;
                        NewEmployeeTime.FirstName = TheFindAholaEmployeePunchHoursDataSet.FindAholaEmployeePunchHours[intCounter].FirstName;
                        NewEmployeeTime.LastName = TheFindAholaEmployeePunchHoursDataSet.FindAholaEmployeePunchHours[intCounter].LastName;
                        NewEmployeeTime.StartDate = TheFindAholaEmployeePunchHoursDataSet.FindAholaEmployeePunchHours[intCounter].StartDate;
                        NewEmployeeTime.TransactionID = TheFindAholaEmployeePunchHoursDataSet.FindAholaEmployeePunchHours[intCounter].TransactionID;

                        TheCurrentEmployeeTimeDataSet.currentemployeetime.Rows.Add(NewEmployeeTime);
                    }
                }

                drgTime.ItemsSource = TheCurrentEmployeeTimeDataSet.currentemployeetime;
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Add Employee Punch // Reset Controls " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Add Employee Punch // Reset Controls " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void lblPunchUpdatetime_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}
