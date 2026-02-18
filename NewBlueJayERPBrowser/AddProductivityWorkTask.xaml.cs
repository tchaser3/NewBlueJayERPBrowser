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
using WorkTaskDLL;
using NewEventLogDLL;
using DepartmentDLL;
using EmployeeDateEntryDLL;
using DataValidationDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for AddProductivityWorkTask.xaml
    /// </summary>
    public partial class AddProductivityWorkTask : Page
    {
        WorkTaskClass TheWorkTaskClass = new WorkTaskClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up the data
        FindProductivityWorkTaskByWorkTaskIDDataSet TheFindProductivityWorkTaskByWorkTaskIDDataSet = new FindProductivityWorkTaskByWorkTaskIDDataSet();
        FindSortedCustomerLinesDataSet TheFindSortedCustomerLinesDataSet = new FindSortedCustomerLinesDataSet();
        FindSortedProductionTypesDataSet TheFindSortedProductionTypesDataSet = new FindSortedProductionTypesDataSet();
        FindWorkTaskByTaskKeywordDataSet TheFindWorkTaskByTaskKeyWordDataSet = new FindWorkTaskByTaskKeywordDataSet();
        FindWorkTaskDepartmentForBusinessLineDataSet TheFindWorkTaskDepartmentForBusinessLineDataSet = new FindWorkTaskDepartmentForBusinessLineDataSet();  
        FindWorkTaskDepartmentByWorkTaskDataSet TheFindWorkTaskDepartmentByWorkTaskDataSet = new FindWorkTaskDepartmentByWorkTaskDataSet();
        FindWorkTaskDepartmentWorkTaskMatchDataSet TheFindWorkTaskDepartmentWorkTaskMatchDataSet = new FindWorkTaskDepartmentWorkTaskMatchDataSet();

        //setting up global variables
        int gintBusinessLineID;
        int gintDepartmentID;
        int gintWorkTaskID;
        bool gblnAllBusinessLines;
        bool gblnAllDepartments;
        bool gblnWorkTaskFound;

        public AddProductivityWorkTask()
        {
            InitializeComponent();
        }

        private void ResetControls()
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError = false;

            try
            {
                txtEnterTask.Text = "";
                cboSelectTask.Items.Clear();
                cboSelectTask.Items.Add("Select Task");
                cboSelectTask.SelectedIndex = 0;
                gblnWorkTaskFound = true;

                TheFindSortedCustomerLinesDataSet = TheDepartmentClass.FindSortedCustomerLines();

                intNumberOfRecords = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines.Rows.Count;
                cboSelectBusinessLine.Items.Clear();
                cboSelectBusinessLine.Items.Add("Select Business Line");

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectBusinessLine.Items.Add(TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intCounter].Department);
                }

                cboSelectBusinessLine.Items.Add("All");
                cboSelectBusinessLine.SelectedIndex = 0;

                cboSelectDepartment.Items.Clear();
                cboSelectDepartment.Items.Add("Select Department");

                TheFindSortedProductionTypesDataSet = TheDepartmentClass.FindSortedProductionTypes();

                intNumberOfRecords = TheFindSortedProductionTypesDataSet.FindSortedProductionTypes.Rows.Count;

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectDepartment.Items.Add(TheFindSortedProductionTypesDataSet.FindSortedProductionTypes[intCounter].Department);
                }

                cboSelectDepartment.Items.Add("All");
                cboSelectDepartment.SelectedIndex = 0;

                blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP Browser // Add Productivity Work Task ");

                cboSelectActive.Items.Clear();
                cboSelectActive.Items.Add("Select Active");
                cboSelectActive.Items.Add("Yes");
                cboSelectActive.Items.Add("No");
                cboSelectActive.SelectedIndex = 0;
                cboSelectActive.IsEnabled = false;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Add Productivity Work Task // Reset Controls " + Ex.Message);

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Add Productivity Work Task // Reset Controls " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void txtEnterTask_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strWorkTask;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                cboSelectActive.IsEnabled = false;
                strWorkTask = txtEnterTask.Text;
                if (strWorkTask.Length > 2)
                {
                    TheFindWorkTaskByTaskKeyWordDataSet = TheWorkTaskClass.FindWorkTaskByTaskKeyword(strWorkTask);
                    cboSelectTask.Items.Clear();
                    cboSelectTask.Items.Add("Select Work Task");                   

                    intNumberOfRecords = TheFindWorkTaskByTaskKeyWordDataSet.FindWorkTaskByTaskKeyword.Rows.Count;

                    if(gblnWorkTaskFound==true)
                    {
                        if (intNumberOfRecords < 1)
                        {
                            dgrExistingWorkTasks.ItemsSource = null;
                            TheMessagesClass.ErrorMessage("The Work Task was not Found");
                            cboSelectTask.IsEnabled = false;
                            gblnWorkTaskFound = false;
                            return;
                        }
                    }                
                    
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        gblnWorkTaskFound = true;
                        cboSelectTask.Items.Add(TheFindWorkTaskByTaskKeyWordDataSet.FindWorkTaskByTaskKeyword[intCounter].WorkTask);
                        cboSelectTask.IsEnabled = true;
                    }
                    dgrExistingWorkTasks.ItemsSource = TheFindWorkTaskByTaskKeyWordDataSet.FindWorkTaskByTaskKeyword;
                    cboSelectTask.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Add Productivity Work Task // Enter Task Text Box " + Ex.Message);

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Add Productivity Work Task // Enter Task Text Box " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intNumberOfRecords;
            int intCounter;

            intSelectedIndex = cboSelectTask.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {                       
                gintWorkTaskID = TheFindWorkTaskByTaskKeyWordDataSet.FindWorkTaskByTaskKeyword[intSelectedIndex].WorkTaskID;
                txtTaskCost.Text = Convert.ToString(TheFindWorkTaskByTaskKeyWordDataSet.FindWorkTaskByTaskKeyword[intSelectedIndex].TaskCost);

                TheFindWorkTaskDepartmentByWorkTaskDataSet = TheWorkTaskClass.FindWorkTaskDepartmentByWorkTask(gintWorkTaskID);

                intNumberOfRecords = TheFindWorkTaskDepartmentByWorkTaskDataSet.FindWorkTaskDepartmentByWorkTask.Rows.Count;

                if (intNumberOfRecords > 0)
                {
                    TheMessagesClass.InformationMessage("The Work Task has been Associated with Multiple Departments and Business Lines. Please Select Carefully");
                }

                cboSelectActive.IsEnabled = true;
            }
        }

        private void cboSelectBusinessLine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            string strBusinessLine;

            intSelectedIndex = cboSelectBusinessLine.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                strBusinessLine = cboSelectBusinessLine.SelectedItem.ToString();

                if (strBusinessLine == "All")
                {
                    gblnAllBusinessLines = true;
                }
                else
                {
                    gintBusinessLineID = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intSelectedIndex].DepartmentID;

                    gblnAllBusinessLines = false;
                }
            }
        }

        private void cboSelectDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            string strDepartment;

            intSelectedIndex = cboSelectDepartment.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                strDepartment = cboSelectDepartment.SelectedItem.ToString();

                if (strDepartment == "All")
                {
                    gblnAllDepartments = true;
                }
                else
                {
                    gintDepartmentID = TheFindSortedProductionTypesDataSet.FindSortedProductionTypes[intSelectedIndex].DepartmentID;

                    gblnAllDepartments = false;
                }
            }
        }
        private void AddNewTask()
        {
            string strErrorMessage = "";
            bool blnFatalError = false;
            int intDepartmentCounter;
            int intDepartmentNumberOfRecords;
            int intBOLCounter;
            int intBOLNumberOfRecords;
            int intDepartmentID;
            int intEmployeeID;
            string strWorkTask;
            string strValueForValidation;
            decimal decTaskCost = 0;
            decimal decProductionValue = 0;

            try
            {
                intEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;
                strWorkTask = txtEnterTask.Text;

                if(strWorkTask.Length < 4)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Work Task Length is not Long Enough\n";
                }
                                
                if (cboSelectBusinessLine.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Business Line Was Not Selected\n";
                }
                if (cboSelectDepartment.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Department Was Not Selected\n";
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }
                strValueForValidation = txtTaskCost.Text;
                blnFatalError = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if (blnFatalError == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Task Cost is not Numeric\n";
                }
                else
                {
                    decProductionValue = Convert.ToDecimal(strValueForValidation);                    
                }

                blnFatalError = TheWorkTaskClass.InsertWorkTask(strWorkTask, decTaskCost, decProductionValue);

                if (blnFatalError == true)
                    throw new Exception();

                TheFindWorkTaskByTaskKeyWordDataSet = TheWorkTaskClass.FindWorkTaskByTaskKeyword(strWorkTask);
                gintWorkTaskID = TheFindWorkTaskByTaskKeyWordDataSet.FindWorkTaskByTaskKeyword[0].WorkTaskID;

                intDepartmentNumberOfRecords = TheFindSortedProductionTypesDataSet.FindSortedProductionTypes.Rows.Count;
                intBOLNumberOfRecords = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines.Rows.Count;

                if ((gblnAllDepartments == false) && (gblnAllBusinessLines == false))
                {
                    blnFatalError = TheWorkTaskClass.InsertWorkTaskDepartment(gintWorkTaskID, gintBusinessLineID, gintDepartmentID, intEmployeeID, DateTime.Now);

                    if (blnFatalError == true)
                        throw new Exception();
                }
                else if ((gblnAllDepartments == false) && (gblnAllBusinessLines == true))
                {
                    for (intBOLCounter = 0; intBOLCounter < intBOLNumberOfRecords; intBOLCounter++)
                    {
                        gintBusinessLineID = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intBOLCounter].DepartmentID;

                        blnFatalError = TheWorkTaskClass.InsertWorkTaskDepartment(gintWorkTaskID, gintBusinessLineID, gintDepartmentID, intEmployeeID, DateTime.Now);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }
                else if ((gblnAllDepartments == true) && (gblnAllBusinessLines == false))
                {
                    for (intDepartmentCounter = 0; intDepartmentCounter < intDepartmentNumberOfRecords; intDepartmentCounter++)
                    {
                        intDepartmentID = TheFindSortedProductionTypesDataSet.FindSortedProductionTypes[intDepartmentCounter].DepartmentID;

                        blnFatalError = TheWorkTaskClass.InsertWorkTaskDepartment(gintWorkTaskID, gintBusinessLineID, gintDepartmentID, intEmployeeID, DateTime.Now);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }
                else if ((gblnAllDepartments == true) && (gblnAllBusinessLines == true))
                {
                    for (intBOLCounter = 0; intBOLCounter < intBOLNumberOfRecords; intBOLCounter++)
                    {
                        gintBusinessLineID = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intBOLCounter].DepartmentID;

                        for (intDepartmentCounter = 0; intDepartmentCounter < intDepartmentNumberOfRecords; intDepartmentCounter++)
                        {
                            intDepartmentID = TheFindSortedProductionTypesDataSet.FindSortedProductionTypes[intDepartmentCounter].DepartmentID;

                            blnFatalError = TheWorkTaskClass.InsertWorkTaskDepartment(gintWorkTaskID, gintBusinessLineID, gintDepartmentID, intEmployeeID, DateTime.Now);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                }

                TheMessagesClass.InformationMessage("The Information has been Inserted");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Add Productivity Work Task // Add New Task " + Ex.Message);

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Add Productivity Work Task // Add New Task " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProcess_Expanded(object sender, RoutedEventArgs e)
        {
            expProcess.IsExpanded = false;
            if (gblnWorkTaskFound==false)
            {
                AddNewTask();
            }
            else if(gblnWorkTaskFound==true)
            {
                EditExistingTask();
            }
        }
        private void EditExistingTask()
        {
            string strErrorMessage = "";
            bool blnFatalError = false;
            string strWorkTask;
            string strActive = "";
            string strValueForValidation;
            decimal decTotalCost = 0;
            bool blnThereIsAProblem;
            int intNumberOfRecords;
            int intCounter;
            int intSecondCounter;
            int intSecondNumberOfRecords;

            try
            {
                if (cboSelectTask.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Task Was Not Selected\n";
                }
                strWorkTask = txtEnterTask.Text;
                if (strWorkTask.Length < 5)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Work Task is not Long Enough\n";
                }
                strValueForValidation = txtTaskCost.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Total Costs is not Numeric\n";
                }
                else
                {
                    decTotalCost = Convert.ToDecimal(strValueForValidation);
                }
                if (cboSelectActive.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Active has not Been Selected\n";
                }
                else
                {
                    strActive = cboSelectActive.SelectedItem.ToString().ToUpper();
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                if(gblnAllBusinessLines==true)
                {
                    intNumberOfRecords = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines.Rows.Count;
                    intSecondNumberOfRecords = TheFindSortedProductionTypesDataSet.FindSortedProductionTypes.Rows.Count;

                    if (intNumberOfRecords > 0)
                    {
                        for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            gintBusinessLineID = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intCounter].DepartmentID;

                            if(gblnAllDepartments==true)
                            {
                                for (intSecondCounter = 0; intSecondCounter < intSecondNumberOfRecords; intSecondNumberOfRecords++)
                                {
                                    gintDepartmentID = TheFindSortedProductionTypesDataSet.FindSortedProductionTypes[intSecondCounter].DepartmentID;

                                    TheFindWorkTaskDepartmentWorkTaskMatchDataSet = TheWorkTaskClass.FindWorkTaskDepartmentWorkTaskMatch(gintWorkTaskID, gintBusinessLineID, gintDepartmentID);

                                    if (TheFindWorkTaskDepartmentWorkTaskMatchDataSet.FindWorkTaskDepartmentWorkTaskMatch.Rows.Count < 1)
                                    {
                                        blnFatalError = TheWorkTaskClass.InsertWorkTaskDepartment(gintWorkTaskID, gintBusinessLineID, gintDepartmentID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, DateTime.Now);
                                        if (blnFatalError == true)
                                            throw new Exception();
                                    }

                                }
                            }
                            else if(gblnAllDepartments==false)
                            {
                                TheFindWorkTaskDepartmentWorkTaskMatchDataSet = TheWorkTaskClass.FindWorkTaskDepartmentWorkTaskMatch(gintWorkTaskID, gintBusinessLineID, gintDepartmentID);

                                if (TheFindWorkTaskDepartmentWorkTaskMatchDataSet.FindWorkTaskDepartmentWorkTaskMatch.Rows.Count < 1)
                                {
                                    blnFatalError = TheWorkTaskClass.InsertWorkTaskDepartment(gintWorkTaskID, gintBusinessLineID, gintDepartmentID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, DateTime.Now);
                                    if (blnFatalError == true)
                                        throw new Exception();
                                }
                            }
                                                                                  
                        }
                    }
                }
                else if(gblnAllBusinessLines==false)
                {
                    TheFindWorkTaskDepartmentWorkTaskMatchDataSet = TheWorkTaskClass.FindWorkTaskDepartmentWorkTaskMatch(gintWorkTaskID, gintBusinessLineID, gintDepartmentID);

                    if (TheFindWorkTaskDepartmentWorkTaskMatchDataSet.FindWorkTaskDepartmentWorkTaskMatch.Rows.Count < 1)
                    {
                        blnFatalError = TheWorkTaskClass.InsertWorkTaskDepartment(gintWorkTaskID, gintBusinessLineID, gintDepartmentID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, DateTime.Now);
                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }  

                TheMessagesClass.InformationMessage("The Work Task Has Been Updated");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Add Productivity Work Task // Edit Existing Task " + Ex.ToString());
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Add Producitivty Work Task // Edit Existing Task " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

    }
}
