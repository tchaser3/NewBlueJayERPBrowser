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
using DataValidationDLL;
using DepartmentDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for AddEmployee.xaml
    /// </summary>
    public partial class AddEmployee : Page
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        //setting up the data
        FindSortedDepartmentDataSet TheFindSortedDepartmentDataSet = new FindSortedDepartmentDataSet();
        FindEmployeeByPayIDDataSet TheFindEmployeeByPayIDDataSet = new FindEmployeeByPayIDDataSet();
        FindSortedEmployeeGroupDataSet TheFindSortedEmployeeGroupDataSet = new FindSortedEmployeeGroupDataSet();
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindSortedEmployeeManagersDataSet TheFindSortedEmployeeManagersDataSet = new FindSortedEmployeeManagersDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();

        //setting variables
        string gstrSalaryType;
        string gstrDepartment;
        int gintManagerID;
        string gstrEmployeeType;
        string gstrEmployeeGroup;

        public AddEmployee()
        {
            InitializeComponent();
        }
        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ResetControls();
        }

        private void expResetWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expResetWindow.IsExpanded = false;
            ResetControls();
        }
        private void ResetControls()
        {
            //setting local variables;
            int intNumberOfRecords;
            int intCounter;


            txtEmployeeID.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtPhoneNumber.Text = "";
            txtEmail.Text = "";
            txtPayID.Text = "";
            EnableControls(true);

            TheFindSortedEmployeeGroupDataSet = TheEmployeeClass.FindSortedEmpoyeeGroup();

            intNumberOfRecords = TheFindSortedEmployeeGroupDataSet.FindSortedEmployeeGroup.Rows.Count - 1;

            cboSelectGroup.Items.Clear();
            cboSelectGroup.Items.Add("Select Group");

            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectGroup.Items.Add(TheFindSortedEmployeeGroupDataSet.FindSortedEmployeeGroup[intCounter].GroupName);
            }

            cboHomeOffice.Items.Clear();
            cboHomeOffice.Items.Add("Select Office");

            TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

            intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboHomeOffice.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
            }

            cboDepartment.Items.Clear();
            cboDepartment.Items.Add("Select Department");

            TheFindSortedDepartmentDataSet = TheDepartmentClass.FindSortedDepartment();

            intNumberOfRecords = TheFindSortedDepartmentDataSet.FindSortedDepartment.Rows.Count - 1;

            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboDepartment.Items.Add(TheFindSortedDepartmentDataSet.FindSortedDepartment[intCounter].Department);
            }

            cboManager.Items.Clear();
            cboManager.Items.Add("Select Manager");

            TheFindSortedEmployeeManagersDataSet = TheEmployeeClass.FindSortedEmployeeManagers();

            intNumberOfRecords = TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers.Rows.Count - 1;

            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboManager.Items.Add(TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers[intCounter].FullName);
            }

            cboSelectType.Items.Clear();
            cboSelectType.Items.Add("Select Type");
            cboSelectType.Items.Add("EMPLOYEE");
            cboSelectType.Items.Add("CONTRACTOR");

            cboSalaryType.Items.Clear();
            cboSalaryType.Items.Add("Select Salary Type");
            cboSalaryType.Items.Add("EXEMPT");
            cboSalaryType.Items.Add("NON-EXEMPT");

            cboSelectGroup.SelectedIndex = 0;
            cboHomeOffice.SelectedIndex = 0;
            cboSelectType.SelectedIndex = 0;
            cboSalaryType.SelectedIndex = 0;
            cboDepartment.SelectedIndex = 0;
            cboManager.SelectedIndex = 0;
            EnableControls(false);
        }
        private void EnableControls(bool blnValueBoolean)
        {
            txtEmail.IsEnabled = blnValueBoolean;
            txtEmployeeID.IsEnabled = blnValueBoolean;
            txtFirstName.IsEnabled = blnValueBoolean;
            txtLastName.IsEnabled = blnValueBoolean;
            txtPayID.IsEnabled = blnValueBoolean;
            txtPhoneNumber.IsEnabled = blnValueBoolean;
            cboDepartment.IsEnabled = blnValueBoolean;
            cboHomeOffice.IsEnabled = blnValueBoolean;
            cboManager.IsEnabled = blnValueBoolean;
            cboSalaryType.IsEnabled = blnValueBoolean;
            cboSelectGroup.IsEnabled = blnValueBoolean;
            cboSelectType.IsEnabled = blnValueBoolean;
            btnProcess.IsEnabled = blnValueBoolean;
        }

        private void cboManager_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboManager.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                gintManagerID = TheFindSortedEmployeeManagersDataSet.FindSortedEmployeeManagers[intSelectedIndex].employeeID;
            }
        }

        private void cboDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboDepartment.SelectedIndex > 0)
            {
                gstrDepartment = cboDepartment.SelectedItem.ToString();
            }
        }

        private void cboSalaryType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSalaryType.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                gstrSalaryType = cboSalaryType.SelectedItem.ToString();
            }
        }

        private void cboSelectType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectType.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                gstrEmployeeType = cboSelectType.SelectedItem.ToString();
            }
        }

        private void cboHomeOffice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboHomeOffice.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                MainWindow.gstrWarehouseName = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].FirstName;
            }
        }

        private void cboSelectGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectGroup.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                gstrEmployeeGroup = cboSelectGroup.SelectedItem.ToString();
            }
        }

        private void expCreateNewEmployee_Expanded(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intRecordsReturned;
            bool blnEmployeeExists = true;

            while (blnEmployeeExists == true)
            {
                MainWindow.gintEmployeeID = TheEmployeeClass.CreateEmployeeID();

                TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(MainWindow.gintEmployeeID);

                intRecordsReturned = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID.Rows.Count;

                if (intRecordsReturned > 0)
                {
                    blnEmployeeExists = true;
                }
                else if (intRecordsReturned < 1)
                {
                    blnEmployeeExists = false;
                }
            }

            EnableControls(true);

            txtEmployeeID.Text = Convert.ToString(MainWindow.gintEmployeeID);
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting up local variables
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strPhoneNumber;
            string strEmail;
            int intPayID = 0;
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate;
            string strEndDate = "12/31/2999";
            int intRecordsReturned;
            string strEmployeeInformation;

            try
            {
                datEndDate = Convert.ToDateTime(strEndDate);
                strValueForValidation = txtEmployeeID.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee ID Was Not Generated\n";
                }
                else
                {
                    MainWindow.gintEmployeeID = Convert.ToInt32(strValueForValidation);
                }
                MainWindow.gstrFirstName = txtFirstName.Text;
                if (MainWindow.gstrFirstName.Length < 3)
                {
                    blnFatalError = true;
                    strErrorMessage += "The First Name is not Long Enough\n";
                }
                MainWindow.gstrLastName = txtLastName.Text;
                if (MainWindow.gstrLastName.Length < 3)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Last Name is not Long Enough\n";
                }
                strPhoneNumber = txtPhoneNumber.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyPhoneNumberFormat(strPhoneNumber);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Phone Number is not the Correct Format\n";
                }
                strEmail = txtEmail.Text;
                if (strEmail.Length > 0)
                {
                    blnThereIsAProblem = TheDataValidationClass.VerifyEmailAddress(strEmail);
                    if (blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Email Address is not the Correct Format\n";
                    }
                }
                strValueForValidation = txtPayID.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Pay ID is not an Integer\n";
                }
                else
                {
                    intPayID = Convert.ToInt32(strValueForValidation);

                    if (intPayID > 0)
                    {
                        TheFindEmployeeByPayIDDataSet = TheEmployeeClass.FindEmployeeByPayID(intPayID);

                        intRecordsReturned = TheFindEmployeeByPayIDDataSet.FindEmployeeByPayID.Rows.Count;

                        if (intRecordsReturned > 0)
                        {
                            blnFatalError = true;
                            strErrorMessage += "The Pay ID is Already Assigned to an Employee\n";
                        }
                    }
                }
                if (cboSelectGroup.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Group was not Selected\n";
                }
                if (cboHomeOffice.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Home Office was not Selected\n";
                }
                if (cboSelectType.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Type was not Selected\n";
                }
                if (cboSalaryType.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Salary Type was not Selected\n";
                }
                if (cboDepartment.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Department was not Selected\n";
                }
                if (cboManager.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Manager was not Selected\n";
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                MainWindow.gblnKeepNewEmployee = true;

                MainWindow.TheVerifyEmployeeDataSet = TheEmployeeClass.VerifyEmployee(MainWindow.gstrFirstName, MainWindow.gstrLastName);

                intRecordsReturned = MainWindow.TheVerifyEmployeeDataSet.VerifyEmployee.Rows.Count;

                if (intRecordsReturned > 0)
                {
                    ExistingEmployees ExistingEmployees = new ExistingEmployees();
                    ExistingEmployees.ShowDialog();
                }

                if (MainWindow.gblnKeepNewEmployee == false)
                {
                    TheMessagesClass.InformationMessage("Employee Currently In ERP, Please Edit Employee");
                    return;
                }

                blnFatalError =

                blnFatalError = TheEmployeeClass.InsertEmployee(MainWindow.gintEmployeeID, MainWindow.gstrFirstName, MainWindow.gstrLastName, strPhoneNumber, true, gstrEmployeeGroup, MainWindow.gstrWarehouseName, gstrEmployeeType, strEmail, gstrSalaryType, gstrDepartment, gintManagerID, intPayID);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheEmployeeClass.UpdateEmployeeStartDate(MainWindow.gintEmployeeID, datStartDate);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheEmployeeClass.UpdateEmployeeEndDate(MainWindow.gintEmployeeID, datEndDate);

                if (blnFatalError == true)
                    throw new Exception();

                strEmployeeInformation = MainWindow.gstrFirstName + " ";
                strEmployeeInformation += MainWindow.gstrLastName;

                blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Created Employee " + strEmployeeInformation);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("Employee Has Been Added");

                ResetControls();

                EnableControls(false);

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Add Employee // Process Button " + Ex.Message);

                TheSendEmailClass.SendEventLog(Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
    }
}
