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
using IncentivePayDLL.FindIncentivePayStatusByTransactionStatusDataSetTableAdapters;

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

        int gintTransactionID;

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

        string gstrPunchType;

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
            gintTransactionID = 0;
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

                cboPunchType.Items.Clear();
                cboPunchType.Items.Add("Select Punch Type");
                cboPunchType.Items.Add("IN");
                cboPunchType.Items.Add("OUT");
                cboPunchType.SelectedIndex = 0;

                strFullName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName + " " + TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;
                txtEmployeeName.Text = strFullName;
                txtEmployeeID.Text = Convert.ToString(MainWindow.gintEmployeeID);
                txtPayID.Text = Convert.ToString(TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].PayID);
                txtPayGroup.Text = "HOURLY";
                txtPunchMode.Text = "MANUAL";
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
                        NewEmployeeTime.TransactionComplete = true;

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

        private void expAddPunch_Expanded(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            string strErrorMessage = "";
            DateTime datNewPunchDate = DateTime.Now;
            int intNumberOfRecords;
            int intTransactionID;
            int intCounter;
            bool blnPunchExists;

            expAddPunch.IsExpanded = false;

            try
            {
                if(cboPunchType.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Punch Type Was Not Selected\n";
                }
                strValueForValidation = txtEnterMissedPunch.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Information Entered is not a Date\n";
                }
                else
                {
                    datNewPunchDate = Convert.ToDateTime(strValueForValidation);
                    blnThereIsAProblem = TheDataValidationClass.verifyDateRange(datNewPunchDate, DateTime.Now);
                    if(blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Punch Date is after Today\n";
                    }
                    else
                    {
                        if(datNewPunchDate < EditEmployeePunches.gdatStartDate)
                        {
                            blnFatalError = true;
                            strErrorMessage += "The Punch Date Is Before The Beginning of the Pay Period\n";
                        }
                        else
                        {
                            if(datNewPunchDate > EditEmployeePunches.gdatPayPeriodEndingDate)
                            {
                                blnFatalError = true;
                                strErrorMessage += "The Punche Date Is After The PayPeriod\n";
                            }
                        }
                    }
                }

                intNumberOfRecords = TheCurrentEmployeePunchesData.currentpunches.Rows.Count;

                if (intNumberOfRecords > 0)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        if (datNewPunchDate == TheCurrentEmployeePunchesData.currentpunches[intCounter].PunchedDateTime)
                        {
                            blnFatalError = true;
                            strErrorMessage += "The Punch Time already exists\n";
                        }
                    }
                }

                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnPunchExists = CheckDate(datNewPunchDate);

                if(blnPunchExists == true)
                {
                    TheMessagesClass.ErrorMessage("Punch Exists Within an Already Existing Range");
                    return;
                }

                intTransactionID = (intNumberOfRecords * -1) - 1;

                //this will add the punch to the punch table
                CurrentEmployeePunchesDataSet.currentpunchesRow NewPunchRow = TheCurrentEmployeePunchesData.currentpunches.NewcurrentpunchesRow();

                NewPunchRow.TransactionID = intTransactionID;
                NewPunchRow.EmployeeID = MainWindow.gintEmployeeID;
                NewPunchRow.PayID = Convert.ToInt32(txtPayID.Text);
                NewPunchRow.ActualDateTime = datNewPunchDate;
                NewPunchRow.PunchedDateTime = datNewPunchDate;
                NewPunchRow.PayGroup = txtPayGroup.Text;
                NewPunchRow.PunchMode = txtPunchMode.Text;
                NewPunchRow.PunchType = gstrPunchType;
                NewPunchRow.PunchSource = txtPunchSource.Text;
                NewPunchRow.PunchUser = txtPunchUser.Text;
                NewPunchRow.PunchIPAddress = txtPunchIPAddress.Text;

                TheCurrentEmployeePunchesData.currentpunches.Rows.Add(NewPunchRow);

                txtEnterMissedPunch.Text = "";
                cboPunchType.SelectedIndex = 0;
                
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Add Employee Punch // Add Punch Expander " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Add Employee Punch // Add Punch Expander " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboPunchType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cboPunchType.SelectedIndex == 1)
            {
                gstrPunchType = "IN";
            }
            else if(cboPunchType.SelectedIndex == 2)
            {
                gstrPunchType = "OUT";
            }
        }
        private bool CheckDate(DateTime datPunchedDate)
        {
            bool blnPunchExists = false;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                intNumberOfRecords = TheCurrentEmployeeTimeDataSet.currentemployeetime.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        if(datPunchedDate >= TheCurrentEmployeeTimeDataSet.currentemployeetime[intCounter].StartDate)
                        {
                            if(datPunchedDate <= TheCurrentEmployeeTimeDataSet.currentemployeetime[intCounter].EndDate)
                            {
                                blnPunchExists = true;
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                blnPunchExists = true;

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Add Employee Punch // Add Punch Expander " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Add Employee Punch // Add Punch Expander " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
            return blnPunchExists;
        }
        private void expValidatePunch_Expanded(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intPunchCounter;
            int intPunchNumberOfRecords;
            bool blnPunchFound;
            bool blnHoursFound;
            int intHoursCounter;
            int intHoursNumberOfRecords;
            DateTime datPunchedDate = DateTime.Now;
            string strPunchType = "";
            DateTime datEndDate = DateTime.Now;
            TimeSpan tspTotalHours;
            decimal decTotalHours;
            decimal decMinutes;
            decimal decSeconds;
            decimal decHours;
            int intTransactionID;
            
            try
            {
                expValidatePunch.IsExpanded = false;
                TheCurrentEmployeeTimeDataSet.currentemployeetime.Rows.Clear();
                intPunchNumberOfRecords = TheCurrentEmployeePunchesData.currentpunches.Rows.Count;

                if(intPunchNumberOfRecords > 0)
                {
                    for (intPunchCounter = 0; intPunchCounter < intPunchNumberOfRecords; intPunchCounter++)
                    {
                        blnPunchFound = false;
                        blnHoursFound = false;
                        gintTransactionID -= 1;

                        intHoursNumberOfRecords = TheCurrentEmployeeTimeDataSet.currentemployeetime.Rows.Count;
                        datPunchedDate = TheCurrentEmployeePunchesData.currentpunches[intPunchCounter].PunchedDateTime;
                        strPunchType = TheCurrentEmployeePunchesData.currentpunches[intPunchCounter].PunchType; 

                        for (intHoursCounter = 0; intHoursCounter < intHoursNumberOfRecords; intHoursCounter++)
                        {
                            if (TheCurrentEmployeeTimeDataSet.currentemployeetime[intHoursCounter].TransactionComplete == false)
                            {
                                if (TheCurrentEmployeeTimeDataSet.currentemployeetime[intHoursCounter].EndDate.Date == datEndDate.Date)
                                {
                                    blnPunchFound = true;
                                    TheCurrentEmployeeTimeDataSet.currentemployeetime[intHoursCounter].EndDate = datPunchedDate;
                                    tspTotalHours = datPunchedDate - TheCurrentEmployeeTimeDataSet.currentemployeetime[intHoursCounter].StartDate;
                                    decMinutes = (tspTotalHours.Minutes) / 60;
                                    decSeconds = (tspTotalHours.Seconds) / 129;
                                    decHours = tspTotalHours.Hours;
                                    //strTotalHours = Convert.ToString(tspTotalHours.Hours) + "." + Convert.ToString(tspTotalHours.Minutes);
                                    decTotalHours = decHours + decMinutes + decSeconds;
                                    TheCurrentEmployeeTimeDataSet.currentemployeetime[intHoursCounter].DailyHours = decTotalHours;
                                    TheCurrentEmployeeTimeDataSet.currentemployeetime[intHoursCounter].TransactionComplete = true;
                                }
                            }
                            
                        }

                        blnHoursFound = false;

                        if ((strPunchType == "IN") && (blnPunchFound == false))
                        {
                            if (TheCurrentEmployeePunchesData.currentpunches[intPunchCounter].TransactionID > 0)
                            {
                                blnHoursFound = CheckDate(datPunchedDate);
                            }

                            if(blnHoursFound == false)
                            {
                                CurrentEmployeeTimeDataSet.currentemployeetimeRow NewTimeEntry = TheCurrentEmployeeTimeDataSet.currentemployeetime.NewcurrentemployeetimeRow();

                                NewTimeEntry.DailyHours = 0;
                                NewTimeEntry.EmployeeID = MainWindow.gintEmployeeID;

                                if (strPunchType == "IN")
                                {
                                    NewTimeEntry.StartDate = datPunchedDate;
                                    NewTimeEntry.EndDate = datEndDate;
                                }

                                NewTimeEntry.FirstName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;
                                NewTimeEntry.LastName = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;
                                NewTimeEntry.TransactionID = gintTransactionID;
                                NewTimeEntry.TransactionComplete = false;

                                TheCurrentEmployeeTimeDataSet.currentemployeetime.Rows.Add(NewTimeEntry);
                            }
                        }
                    }
                }

                intHoursNumberOfRecords = TheCurrentEmployeeTimeDataSet.currentemployeetime.Rows.Count;
                blnHoursFound = true;

                for(intHoursCounter = 0; intHoursCounter < intHoursNumberOfRecords; intHoursCounter++)
                {
                    if (TheCurrentEmployeeTimeDataSet.currentemployeetime[intHoursCounter].TransactionComplete == false)
                    {
                        intTransactionID = TheCurrentEmployeeTimeDataSet.currentemployeetime[intHoursCounter].TransactionID;

                        TheCurrentEmployeeTimeDataSet.currentemployeetime[intHoursCounter].EndDate = TheCurrentEmployeeTimeDataSet.currentemployeetime[intHoursCounter].StartDate;

                        blnHoursFound = false;
                    }
                }

                if(blnHoursFound == false) 
                {
                    TheMessagesClass.ErrorMessage("There Are Uncompleted Transactions In The Table");
                    return;
                }
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Add Employee Punch // Validate Punch Expander " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Add Employee Punch // Validate Punch Expander " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProcess_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError = false;
            int intEmployeeID;
            int intPayID;
            DateTime datActualDateTime;
            string strPayGroup;
            string strPunchMode;
            string strPunchType;
            string strPunchSource;
            string strPunchUser;
            string strPunchIPAddress;
            DateTime datPunchTimeUpdated;
            DateTime datPunchDateTime;
            DateTime datStartDate;
            DateTime datEndDate;
            decimal decTotalHours;

            try
            {
                expProcess.IsExpanded = false;
                intNumberOfRecords = TheCurrentEmployeePunchesData.currentpunches.Rows.Count;                

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        if (TheCurrentEmployeePunchesData.currentpunches[intCounter].TransactionID < 0)
                        {
                            intEmployeeID = TheCurrentEmployeePunchesData.currentpunches[intCounter].EmployeeID;
                            intPayID = TheCurrentEmployeePunchesData.currentpunches[intCounter].PayID;
                            datActualDateTime = TheCurrentEmployeePunchesData.currentpunches[intCounter].ActualDateTime;
                            strPayGroup = txtPayGroup.Text;
                            strPunchIPAddress = txtPunchIPAddress.Text;
                            strPunchMode = txtPunchMode.Text;
                            strPunchSource = txtPunchSource.Text;
                            strPunchType = TheCurrentEmployeePunchesData.currentpunches[intCounter].PunchType;
                            strPunchUser = txtPunchUser.Text;
                            datPunchTimeUpdated = DateTime.Now;
                            datPunchDateTime = TheCurrentEmployeePunchesData.currentpunches[intCounter].PunchedDateTime;

                            blnFatalError = TheEmployeePunchedHoursClass.InsertAholaClockPunches(intEmployeeID, intPayID, datActualDateTime, datPunchDateTime, DateTime.Now, strPayGroup, strPunchMode, strPunchType, strPunchSource, strPunchUser, strPunchIPAddress, datPunchTimeUpdated);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                }

                intNumberOfRecords = TheCurrentEmployeeTimeDataSet.currentemployeetime.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        if (TheCurrentEmployeeTimeDataSet.currentemployeetime[intCounter].TransactionID < 0)
                        {
                            intEmployeeID = TheCurrentEmployeeTimeDataSet.currentemployeetime[intCounter].EmployeeID;
                            datStartDate = TheCurrentEmployeeTimeDataSet.currentemployeetime[intCounter].StartDate;
                            datEndDate = TheCurrentEmployeeTimeDataSet.currentemployeetime[intCounter].EndDate;
                            decTotalHours = TheCurrentEmployeeTimeDataSet.currentemployeetime[intCounter].DailyHours;

                            blnFatalError = TheEmployeePunchedHoursClass.InsertIntoAholaEmployeePunch(intEmployeeID, datStartDate, datEndDate, decTotalHours);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                        
                    }
                }

                TheMessagesClass.InformationMessage("The Missing Punches Have Been Entered");

                this.Close();

            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Add Employee Punch // Process Expander " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Add Employee Punch // Process Expander " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
