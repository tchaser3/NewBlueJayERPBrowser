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
using NewEventLogDLL;
using EmployeeCrewAssignmentDLL;
using EmployeeLaborRateDLL;
using EmployeeProjectAssignmentDLL;
using ProjectTaskDLL;
using DataValidationDLL;
using WorkTaskStatsDLL;
using ProductivityDataEntryDLL;
using DateSearchDLL;
using EmployeeDateEntryDLL;
using ProjectMatrixDLL;
using NewEmployeeDLL;
using ProjectsDLL;
using WorkTaskDLL;
using ProductionProjectUpdatesDLL;
using ProductionProjectDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for EnterProductivity.xaml
    /// </summary>
    public partial class EnterProductivity : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeCrewAssignmentClass TheEmployeeCrewAssignmentClass = new EmployeeCrewAssignmentClass();
        EmployeeLaborRateClass TheEmployeeLaborRateClass = new EmployeeLaborRateClass();
        EmployeeProjectAssignmentClass TheEmployeeProjectAssignmentClass = new EmployeeProjectAssignmentClass();
        ProjectTaskClass TheProjectTaskClass = new ProjectTaskClass();
        WorkTaskClass TheWorkTaskClass = new WorkTaskClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        WorkTaskStatsClass TheWorkTaskStatsClass = new WorkTaskStatsClass();
        ProductivityDataEntryClass TheProductivityDataEntryClass = new ProductivityDataEntryClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EmployeeDateEntryClass TheEmployeeDataEntryClass = new EmployeeDateEntryClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();
        ProjectClass TheProjectClass = new ProjectClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        ProductionProjectUpdatesClass TheProductionProjectUpdatesClass = new ProductionProjectUpdatesClass();
        ProductionProjectClass TheProductionProjectClass = new ProductionProjectClass();

        //setting up the data
        FindProjectMatrixByProjectIDDataSet TheFindProjectMatrixByProjectIDDataSet = new FindProjectMatrixByProjectIDDataSet();
        FindWorkTaskByTaskKeywordDataSet TheFindWorkTaskByKeywordDataSet = new FindWorkTaskByTaskKeywordDataSet();
        FindWorkTaskStatsByTaskIDDataSet TheFindWorkTaskStatsByTaskIDDataSet = new FindWorkTaskStatsByTaskIDDataSet();
        FindProductivityDataEntryByDateDataSet TheFindProductivityDataEntryByDateDataSet = new FindProductivityDataEntryByDateDataSet();
        FindEmployeeHoursOverDateRangeDataSet TheFindEmployeeHoursOverADateRangeDataSet = new FindEmployeeHoursOverDateRangeDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindEmployeeByLastNameEndDateDataSet TheFindEmployeeByLastNameEndDateDataSet = new FindEmployeeByLastNameEndDateDataSet();
        FindProjectByProjectIDDataSet TheFindProjectByProjectIDDataSet = new FindProjectByProjectIDDataSet();
        FindProductionProjectByProjectIDDataSet TheFindProductionProjectByProjectIDDataSet = new FindProductionProjectByProjectIDDataSet();

        //setting global variables
        decimal gdecTotalHours;
        bool gblnCrewIDSet;
        bool gblnHoursEntered;
        bool gblnRecordDeleted;
        decimal gdecHours;
        int gintTransactionID;
        int gintDataEntryTransactionID;
        int gintEmployeeCounter;
        int gintTaskCounter;
        string gstrCrewID;
        decimal gdecDriveTime;
        int gintDriveTimeTaskID;
        bool gblnDriveTimeCalculated;
        decimal gdecNonProductiveTime;
        decimal gdecLunchHour;
        decimal gdecTotalProjectHours;
        int gintProductivityEmployeeID;
        string gstrTaskEntered;

        public EnterProductivity()
        {
            InitializeComponent();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            // Close the window when the expander is expanded
            this.Close();
        }
        private void ResetWindow()
        {
            int intCurrentStatus;
            //this will load up the controls
            cboSelectLunchTaken.Items.Clear();
            cboSelectLunchTaken.Items.Add("Select Lunch");
            cboSelectLunchTaken.Items.Add("Yes");
            cboSelectLunchTaken.Items.Add("No");
            cboSelectLunchTaken.SelectedIndex = 0;

            cboSelectEmployee.Items.Clear();
            cboSelectEmployee.Items.Add("Select Employee");
            cboSelectEmployee.SelectedIndex = 0;

            cboSelectTask.Items.Clear();
            cboSelectTask.Items.Add("Select Task");
            cboSelectTask.SelectedIndex = 0;

            MainWindow.TheEmployeeWorkCompleteDataSet.workcompleted.Rows.Clear();
            MainWindow.TheProjectWorkCompletedDataSet.workcompleted.Rows.Clear();

            dgrResults.ItemsSource = MainWindow.TheEmployeeWorkCompleteDataSet.workcompleted;

            TheFindProjectMatrixByProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByProjectID(MainWindow.gintProjectID);

            MainWindow.gstrAssignedProjectID = TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].AssignedProjectID;
            MainWindow.gstrCustomerProjectID = TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].CustomerAssignedID;

            TheFindProductionProjectByProjectIDDataSet = TheProductionProjectClass.FindProductionProjectByProjectID(MainWindow.gintProjectID);

            intCurrentStatus = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].CurrentStatusID;

            if ((intCurrentStatus == 1002) || (intCurrentStatus == 1003) || (intCurrentStatus == 1004) || (intCurrentStatus == 1006) || (intCurrentStatus == 1011) || (intCurrentStatus == 1012) || (intCurrentStatus == 1013))
            {
                TheMessagesClass.ErrorMessage("Project Currently in Either a WAIT, HOLD, CLOSED, or OPEN Status and Needs to be Changed Before Productivity Can Be Added, Please Change the Status Before Continuing");
                Close();
            }

            txtDriveTimeHours.Text = "";
            txtEnterFootage.Text = "";
            txtEnterHours.Text = "";
            txtEnterLastName.Text = "";
            txtEnterTask.Text = "";
            txtNonProdTime.Text = "";
            txtTotalHours.Text = "";
            txtEnterDate.Text = "";
            gstrTaskEntered = "";

            btnAddTask.IsEnabled = false;
            expProcess.IsEnabled = false;
        }

        private void cboSelectLunchTaken_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboSelectLunchTaken.SelectedIndex == 1)
                gdecLunchHour = 1;
            else if (cboSelectLunchTaken.SelectedIndex == 2)
                gdecLunchHour = 0;
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strValueForValidation;
            bool blnFatalError = false;
            string strLastName;
            int intLength;
            int intCounter;
            int intNumberOfRecords;
            DateTime datEndDate;

            try
            {
                strValueForValidation = txtDriveTimeHours.Text;
                blnFatalError = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The Drive Time is not Entered");
                    return;
                }
                else
                {
                    gdecDriveTime = Convert.ToDecimal(strValueForValidation);

                    if (gdecDriveTime == 0)
                    {
                        TheMessagesClass.ErrorMessage("The Drive Time Cannot be 0");
                        return;
                    }
                    else if (gdecDriveTime > 16)
                    {
                        TheMessagesClass.ErrorMessage("Drive Time Cannot Be Greater Than 16");
                        return;
                    }

                    TheFindWorkTaskByKeywordDataSet = TheWorkTaskClass.FindWorkTaskByTaskKeyword("DRIVE TIME");

                    gintDriveTimeTaskID = TheFindWorkTaskByKeywordDataSet.FindWorkTaskByTaskKeyword[0].WorkTaskID;
                }
                strValueForValidation = txtNonProdTime.Text;
                blnFatalError = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The Non-Productivity Time is not Numeric\n");
                    return;
                }
                else
                {
                    gdecNonProductiveTime = Convert.ToDecimal(strValueForValidation);
                }


                datEndDate = TheDateSearchClass.SubtractingDays(DateTime.Now, 21);

                strLastName = txtEnterLastName.Text;
                intLength = strLastName.Length;

                if (intLength > 2)
                {
                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    TheFindEmployeeByLastNameEndDateDataSet = TheEmployeeClass.FindEmployeeByLastNameEndDate(strLastName, datEndDate);

                    intNumberOfRecords = TheFindEmployeeByLastNameEndDateDataSet.FindEmployeesByLastNameEndDate.Rows.Count - 1;

                    if (intNumberOfRecords == -1)
                    {
                        TheMessagesClass.InformationMessage("Employee Not Found");
                        return;
                    }
                    else
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            cboSelectEmployee.Items.Add(TheFindEmployeeByLastNameEndDateDataSet.FindEmployeesByLastNameEndDate[intCounter].FirstName + " " + TheFindEmployeeByLastNameEndDateDataSet.FindEmployeesByLastNameEndDate[intCounter].LastName);
                        }
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Enter Productivity // Enter Last Name Text Box " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Enter Productivity // Enter Last Name Text Box " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            bool blnFatalError = false;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;
                btnRemoveTask.IsEnabled = false;
                btnRemoveEmployee.IsEnabled = true;

                if (intSelectedIndex > -1)
                {
                    MainWindow.gstrFirstName = TheFindEmployeeByLastNameEndDateDataSet.FindEmployeesByLastNameEndDate[intSelectedIndex].FirstName;
                    MainWindow.gstrLastName = TheFindEmployeeByLastNameEndDateDataSet.FindEmployeesByLastNameEndDate[intSelectedIndex].LastName;
                    gintProductivityEmployeeID = TheFindEmployeeByLastNameEndDateDataSet.FindEmployeesByLastNameEndDate[intSelectedIndex].EmployeeID;

                    TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(gintProductivityEmployeeID);

                    if (TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmployeeType == "CONTRACTOR")
                    {
                        TheMessagesClass.ErrorMessage("You Have Selected a Contractor, Please Select Again");
                        cboSelectEmployee.SelectedIndex = 0;
                        txtEnterLastName.Focus();
                        return;
                    }

                    if (gblnCrewIDSet == false)
                    {
                        gstrCrewID = MainWindow.gstrLastName;
                        gblnCrewIDSet = true;
                    }

                    blnFatalError = AddEmployees();

                    if (blnFatalError == true)
                        TheMessagesClass.ErrorMessage("Employee Was Not Added");
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Enter Productivity // cboSelectEmployee Event" + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Enter Productivity // cboSelectEmployee Event" + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private bool AddEmployees()
        {
            bool blnFatalError = false;

            try
            {
                blnFatalError = TheDataValidationClass.VerifyDoubleData(txtEnterHours.Text);
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The Hours is not Numeric");
                    return true;
                }

                gdecHours = Convert.ToDecimal(txtEnterHours.Text);

                if (gdecHours > 16)
                {
                    TheMessagesClass.ErrorMessage("Employees Cannot work more than 16 Hours");
                    return true;
                }

                gdecHours = gdecHours - gdecDriveTime - gdecNonProductiveTime - gdecLunchHour;

                gdecTotalProjectHours = gdecHours;

                //adding the record
                ProjectWorkCompletedDataSet.workcompletedRow NewWorkRow = MainWindow.TheEmployeeWorkCompleteDataSet.workcompleted.NewworkcompletedRow();

                NewWorkRow.EmployeeID = gintProductivityEmployeeID;
                NewWorkRow.FirstName = MainWindow.gstrFirstName;
                NewWorkRow.LastName = MainWindow.gstrLastName;
                NewWorkRow.ProjectID = MainWindow.gintProjectID;
                NewWorkRow.AssignedProjectID = MainWindow.gstrCustomerProjectID;
                NewWorkRow.TaskID = 0;
                NewWorkRow.WorkTask = "";
                NewWorkRow.Hours = gdecHours;
                NewWorkRow.FootagePieces = 0;

                MainWindow.TheEmployeeWorkCompleteDataSet.workcompleted.Rows.Add(NewWorkRow);

                dgrResults.ItemsSource = MainWindow.TheEmployeeWorkCompleteDataSet.workcompleted;

                txtEnterLastName.Text = "";
                btnAddTask.IsEnabled = true;
                txtEnterFootage.Text = "0";
                gdecTotalHours += gdecHours;
                txtTotalHours.Text = Convert.ToString(gdecTotalHours);
                gblnHoursEntered = false;
                txtEnterLastName.Focus();
                gintEmployeeCounter++;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Enter Productivity // Add Employee Button " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Enter Productivity // Add Employee Button " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());

                blnFatalError = true;
            }

            return blnFatalError;
        }

        private void btnRemoveEmployee_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;

            //gblnRecordDeleted = true;

            try
            {
                dgrResults.SelectedIndex = -1;

                intNumberOfRecords = MainWindow.TheEmployeeWorkCompleteDataSet.workcompleted.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    gdecHours = MainWindow.TheEmployeeWorkCompleteDataSet.workcompleted[intCounter].Hours;

                    if (MainWindow.TheEmployeeWorkCompleteDataSet.workcompleted[intCounter].TransactionID == gintTransactionID)
                    {
                        MainWindow.TheEmployeeWorkCompleteDataSet.workcompleted[intCounter].Delete();
                        intCounter -= 1;
                        intNumberOfRecords -= 1;
                        gdecTotalHours -= gdecHours;
                        gblnRecordDeleted = false;
                        dgrResults.SelectedIndex = -1;

                    }
                }

                dgrResults.ItemsSource = MainWindow.TheEmployeeWorkCompleteDataSet.workcompleted;
                txtTotalHours.Text = Convert.ToString(gdecTotalHours);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Enter Productivity // Remove Employee Button " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Enter Productivity // Remove Employee Button " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void dgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell TransactionID;
            string strTransactionID;
            int intSelectedIndex;

            try
            {
                intSelectedIndex = dgrResults.SelectedIndex;

                if (intSelectedIndex > -1)
                {
                    if (gblnRecordDeleted == false)
                    {
                        //setting local variable
                        dataGrid = dgrResults;
                        selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                        TransactionID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                        strTransactionID = ((TextBlock)TransactionID.Content).Text;
                        gblnRecordDeleted = true;

                        //find the record
                        gintTransactionID = Convert.ToInt32(strTransactionID);
                    }
                    else
                    {
                        gblnRecordDeleted = false;
                    }
                }


            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Enter Productivity // Grid Selection " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Enter Productivity // Grid Selection " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void txtEnterTask_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strWorkTask;
            int intCounter;
            int intNumberOfRecords;

            strWorkTask = txtEnterTask.Text;
            TheFindWorkTaskByKeywordDataSet = TheWorkTaskClass.FindWorkTaskByTaskKeyword(strWorkTask);
            cboSelectTask.Items.Clear();
            cboSelectTask.Items.Add("Select Task");

            intNumberOfRecords = TheFindWorkTaskByKeywordDataSet.FindWorkTaskByTaskKeyword.Rows.Count - 1;

            if (intNumberOfRecords == -1)
            {
                TheMessagesClass.ErrorMessage("The Task Was Not Found");
                return;
            }

            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectTask.Items.Add(TheFindWorkTaskByKeywordDataSet.FindWorkTaskByTaskKeyword[intCounter].WorkTask);
            }

            cboSelectTask.SelectedIndex = 0;
        }

        private void cboSelectTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectTask.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                MainWindow.gintWorkTaskID = TheFindWorkTaskByKeywordDataSet.FindWorkTaskByTaskKeyword[intSelectedIndex].WorkTaskID;
                MainWindow.gstrWorkTask = TheFindWorkTaskByKeywordDataSet.FindWorkTaskByTaskKeyword[intSelectedIndex].WorkTask;

                if (MainWindow.gintWorkTaskID == 1230)
                {
                    TheMessagesClass.ErrorMessage("BJC1 - NON-PRODUCTIVE TIME Cannot Be Used, The Time is calculated in the Non-Prod Time Box");
                    cboSelectTask.SelectedIndex = 0;
                    return;
                }

            }
        }

        private void btnAddTask_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            string strValueForValidation;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            int intFootagePieces = 0;
            int intRecordsReturned = 0;
            double douProbability;
            double douMean;
            double douHoursEntered;
            bool blnOverHours;

            try
            {
                btnRemoveEmployee.IsEnabled = false;
                btnRemoveTask.IsEnabled = true;
                expProcess.IsEnabled = true;

                intNumberOfRecords = MainWindow.TheEmployeeWorkCompleteDataSet.workcompleted.Rows.Count - 1;

                if (intNumberOfRecords == -1)
                {
                    TheMessagesClass.ErrorMessage("There Are No Employees Assigned to this Project");
                    return;
                }
                if (cboSelectTask.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Work Task was not Selected\n";
                }
                strValueForValidation = txtEnterHours.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    strErrorMessage += "Total Hours is not Numeric\n";
                    blnFatalError = true;
                }
                else
                {
                    gdecHours = gdecTotalProjectHours;
                }
                strValueForValidation = txtEnterFootage.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    strErrorMessage += "The Footage or Pieces is not an Integer\n";
                    blnFatalError = true;
                }
                else
                {
                    intFootagePieces = Convert.ToInt32(strValueForValidation);
                }
                strValueForValidation = txtEnterDate.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Date is not a Date\n";
                }
                else
                {
                    MainWindow.gdatProductionDate = Convert.ToDateTime(strValueForValidation);

                    if (MainWindow.gdatProductionDate > DateTime.Now)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Date is in the Future\n";
                    }
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                if (gblnHoursEntered == true)
                {
                    gdecHours = 0;
                }

                TheFindWorkTaskStatsByTaskIDDataSet = TheWorkTaskStatsClass.FindWorkTaskStatsByTaskID(MainWindow.gintWorkTaskID);

                intRecordsReturned = TheFindWorkTaskStatsByTaskIDDataSet.FindWorkTaskStatsByWorkTaskID.Rows.Count;

                if (intRecordsReturned > 0)
                {
                    douMean = Convert.ToDouble(TheFindWorkTaskStatsByTaskIDDataSet.FindWorkTaskStatsByWorkTaskID[0].TaskMean);
                    douHoursEntered = Convert.ToDouble(gdecHours);

                    douProbability = CalculatePropability(douMean, douHoursEntered);


                    if (douProbability < .0001)
                    {
                        TheMessagesClass.ErrorMessage("The Hours Entered Are Outside Expected Range\nPlease Return Return Sheet To Manager");
                        return;
                    }
                }



                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    blnOverHours = CheckEmployeeTotalHours(MainWindow.TheEmployeeWorkCompleteDataSet.workcompleted[intCounter].EmployeeID, gdecHours);

                    if (blnOverHours == true)
                    {
                        TheMessagesClass.ErrorMessage(MainWindow.TheEmployeeWorkCompleteDataSet.workcompleted[intCounter].FirstName + " " + MainWindow.TheEmployeeWorkCompleteDataSet.workcompleted[intCounter].LastName + " Has Worked Over 16 Hours\nPlease Return Sheet To Manager");

                        return;
                    }

                    ProjectWorkCompletedDataSet.workcompletedRow NewWorkRow = MainWindow.TheProjectWorkCompletedDataSet.workcompleted.NewworkcompletedRow();

                    NewWorkRow.AssignedProjectID = MainWindow.TheEmployeeWorkCompleteDataSet.workcompleted[intCounter].AssignedProjectID;
                    NewWorkRow.EmployeeID = MainWindow.TheEmployeeWorkCompleteDataSet.workcompleted[intCounter].EmployeeID;
                    NewWorkRow.FirstName = MainWindow.TheEmployeeWorkCompleteDataSet.workcompleted[intCounter].FirstName;
                    NewWorkRow.LastName = MainWindow.TheEmployeeWorkCompleteDataSet.workcompleted[intCounter].LastName;
                    NewWorkRow.ProjectID = MainWindow.TheEmployeeWorkCompleteDataSet.workcompleted[intCounter].ProjectID;
                    NewWorkRow.TaskID = MainWindow.gintWorkTaskID;
                    NewWorkRow.WorkTask = MainWindow.gstrWorkTask;
                    NewWorkRow.Hours = gdecHours;
                    NewWorkRow.FootagePieces = intFootagePieces;

                    MainWindow.TheProjectWorkCompletedDataSet.workcompleted.Rows.Add(NewWorkRow);
                    gblnHoursEntered = true;

                    gstrTaskEntered = gstrTaskEntered + MainWindow.gstrWorkTask + "\n";

                    txtEnterFootage.Text = "";
                    txtEnterTask.Text = "";

                    txtEnterTask.Focus();
                }


                gintTaskCounter++;
                dgrResults.ItemsSource = MainWindow.TheProjectWorkCompletedDataSet.workcompleted;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Enter Productivity // Add Task Button " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Enter Productivity // Add Task Button " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private double CalculatePropability(double douMean, double douHoursEntered)
        {
            double douPropability = 0;
            double douFirstCalcuation;
            double douSecondCalculation;
            double douThirdCalculation = 1;
            int intCounter;
            int intFactorial;

            try
            {
                douFirstCalcuation = Math.Pow(2.71836, douMean * -1);
                douSecondCalculation = Math.Pow(douMean, douHoursEntered);
                douThirdCalculation = douHoursEntered;

                intFactorial = Convert.ToInt32(douHoursEntered);


                for (intCounter = 1; intCounter <= intFactorial; intCounter++)
                {
                    douThirdCalculation = douThirdCalculation * intCounter;
                }

                douPropability = (douFirstCalcuation * douSecondCalculation) / douThirdCalculation;
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Enter Productivity // Calculate Propability " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Enter Productivity // Calculate Propability " + Ex.ToString());
            }

            return douPropability;
        }
        private bool CheckEmployeeTotalHours(int intEmployeeID, decimal decHoursEntered)
        {
            bool blnOverHours = false;
            int intCounter;
            int intNumberOfRecords;
            decimal decTotalHours = 0;
            DateTime datEndDate = DateTime.Now;

            try
            {
                MainWindow.gdatProductionDate = Convert.ToDateTime(txtEnterDate.Text);
                MainWindow.gdatProductionDate = TheDateSearchClass.RemoveTime(MainWindow.gdatProductionDate);
                datEndDate = MainWindow.gdatProductionDate;

                TheFindEmployeeHoursOverADateRangeDataSet = TheEmployeeProjectAssignmentClass.FindEmployeeHoursOverDateRange(intEmployeeID, MainWindow.gdatProductionDate, datEndDate);

                decTotalHours = decHoursEntered - gdecLunchHour;

                intNumberOfRecords = TheFindEmployeeHoursOverADateRangeDataSet.FindEmployeeHoursOverDateRange.Rows.Count - 1;

                if (intNumberOfRecords > -1)
                {
                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        decTotalHours += TheFindEmployeeHoursOverADateRangeDataSet.FindEmployeeHoursOverDateRange[intCounter].TotalHours;
                    }
                }

                if (decTotalHours > 16)
                {
                    blnOverHours = true;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Enter Productivity // Check Employee Total Hours " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Enter Productivity // Check Employee Total Hours " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());

                blnOverHours = true;
            }

            return blnOverHours;

        }

        private void btnRemoveTask_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;

            gblnRecordDeleted = true;

            try
            {
                dgrResults.SelectedIndex = -1;

                intNumberOfRecords = MainWindow.TheProjectWorkCompletedDataSet.workcompleted.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    gdecHours = MainWindow.TheProjectWorkCompletedDataSet.workcompleted[intCounter].Hours;

                    if (MainWindow.TheProjectWorkCompletedDataSet.workcompleted[intCounter].TransactionID == gintTransactionID)
                    {
                        MainWindow.TheProjectWorkCompletedDataSet.workcompleted[intCounter].Delete();
                        intCounter -= 1;
                        intNumberOfRecords -= 1;
                        gdecTotalHours -= gdecHours;
                        gblnRecordDeleted = false;
                        dgrResults.SelectedIndex = -1;
                    }
                }

                dgrResults.ItemsSource = MainWindow.TheProjectWorkCompletedDataSet.workcompleted;
                txtTotalHours.Text = Convert.ToString(gdecTotalHours);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Enter Productivity // Menu Item Remove Transaction " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Enter Productivity // Menu Item Remove Transaction " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expResetWindow_Expanded(object sender, RoutedEventArgs e)
        {
            ResetWindow();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetWindow();
        }

        private void expViewProductivity_Expanded(object sender, RoutedEventArgs e)
        {
            expViewProductivity.IsExpanded = false;
            ViewProductivity ViewProductivity = new ViewProductivity();
            ViewProductivity.Show();
        }

        private void expProcess_Expanded(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            DateTime datTodaysDate = DateTime.Now;
            int intEmployeeID;
            int intProjectID;
            int intWorkTaskID;
            decimal decTotalHours;
            int intFootagePieces;
            string strErrorMessage = "";

            try
            {
                expProcess.IsExpanded = false;
                btnRemoveTask.IsEnabled = true;
                btnRemoveEmployee.IsEnabled = false;

                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(txtEnterDate.Text);
                if (blnFatalError == true)
                {
                    strErrorMessage += "The Date is not a Date\n";
                    blnFatalError = true;
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                MainWindow.gdatProductionDate = Convert.ToDateTime(txtEnterDate.Text);

                if (MainWindow.gdatProductionDate > datTodaysDate)
                {
                    TheMessagesClass.ErrorMessage("The Date Entered is in the Future");
                    return;
                }

                intNumberOfRecords = MainWindow.TheEmployeeWorkCompleteDataSet.workcompleted.Rows.Count;

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    intEmployeeID = MainWindow.TheEmployeeWorkCompleteDataSet.workcompleted[intCounter].EmployeeID;

                    blnFatalError = TheEmployeeProjectAssignmentClass.InsertEmployeeProjectAssignment(intEmployeeID, MainWindow.gintProjectID, gintDriveTimeTaskID, MainWindow.gdatProductionDate, gdecDriveTime);

                    if (blnFatalError == true)
                        throw new Exception();

                    if (gdecNonProductiveTime > 0)
                    {
                        blnFatalError = TheEmployeeProjectAssignmentClass.InsertEmployeeProjectAssignment(intEmployeeID, 104330, 1230, MainWindow.gdatProductionDate, gdecNonProductiveTime);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }

                intNumberOfRecords = MainWindow.TheProjectWorkCompletedDataSet.workcompleted.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intEmployeeID = MainWindow.TheProjectWorkCompletedDataSet.workcompleted[intCounter].EmployeeID;
                    intProjectID = MainWindow.TheProjectWorkCompletedDataSet.workcompleted[intCounter].ProjectID;
                    intWorkTaskID = MainWindow.TheProjectWorkCompletedDataSet.workcompleted[intCounter].TaskID;
                    decTotalHours = MainWindow.TheProjectWorkCompletedDataSet.workcompleted[intCounter].Hours;
                    intFootagePieces = MainWindow.TheProjectWorkCompletedDataSet.workcompleted[intCounter].FootagePieces;

                    //first insert
                    blnFatalError = TheEmployeeProjectAssignmentClass.InsertEmployeeProjectAssignment(intEmployeeID, intProjectID, intWorkTaskID, MainWindow.gdatProductionDate, decTotalHours);

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheProjectTaskClass.InsertProjectTask(intProjectID, intEmployeeID, intWorkTaskID, Convert.ToDecimal(intFootagePieces), MainWindow.gdatProductionDate);

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheEmployeeCrewAssignmentClass.InsertEmployeeCrewAssignment(gstrCrewID, intEmployeeID, intProjectID, MainWindow.gdatProductionDate);

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheProductivityDataEntryClass.UpdateProductivityDataEntryHoursTasks(gintDataEntryTransactionID, gintEmployeeCounter, gintTaskCounter);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                blnFatalError = TheEmployeeDataEntryClass.InsertIntoEmployeeDateEntry(MainWindow.gintEmployeeID, "Project Management Sheet // Enter Productivity // Project Labor Has Been Added " + MainWindow.gstrCustomerProjectID);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheProductionProjectUpdatesClass.InsertProductionProjectUpdate(MainWindow.gintProjectID, MainWindow.gintEmployeeID, "The Following Productivity Has Been Entered Into This Project\n " + gstrTaskEntered);

                if (blnFatalError == true)
                    throw new Exception();

                //EnterIncentivePay EnterIncentivePay = new EnterIncentivePay();
                //EnterIncentivePay.ShowDialog();

                ResetWindow();

                TheMessagesClass.InformationMessage("The Project Information Has Been Saved");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Enter Productivity // Process Expander " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Enter Productivity // Process Expander " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
