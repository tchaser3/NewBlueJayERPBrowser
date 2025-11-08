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
using AfterHoursWorkDLL;
using NewEmployeeDLL;
using NewEventLogDLL;
using VehicleMainDLL;
using DataValidationDLL;
using ProjectMatrixDLL;
using DepartmentDLL;
using ProjectsDLL;
using DateSearchDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for CreateAfterHoursReport.xaml
    /// </summary>
    public partial class CreateAfterHoursReport : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        AfterHoursWorkClass TheAfterHoursClass = new AfterHoursWorkClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        ProjectClass TheProjectClass = new ProjectClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        FindSortedDepartmentDataSet TheFindSortedDepartmentDataSet = new FindSortedDepartmentDataSet();
        ComboEmployeeDataSet TheComboEmployeeDataSet = new ComboEmployeeDataSet();
        FindActiveVehicleMainByVehicleNumberDataSet TheFindActiveVehicleMainByVehicleNumberDataSet = new FindActiveVehicleMainByVehicleNumberDataSet();
        FindProjectMatrixByCustomerProjectIDDataSet TheFindProjectMatrixByCustomerProjectIDDataSet = new FindProjectMatrixByCustomerProjectIDDataSet();
        FindProjectMatrixByAssignedProjectIDDataSet TheFindProjectMatrixByAssignedProjectIDDataSet = new FindProjectMatrixByAssignedProjectIDDataSet();
        AfterWorkEmployeesDataSet TheAfterWorkEmployeesDataSet = new AfterWorkEmployeesDataSet();
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindEmployeeOverNightWorkByManagerID2DataSet TheFindEmployeeOverNightWorkByManagerID2DataSet = new FindEmployeeOverNightWorkByManagerID2DataSet();
        FindDepartmentByDepartmentIDDataSet TheFindDepartmentByDepartmentIDDataSet = new FindDepartmentByDepartmentIDDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindVehicleMainByVehicleIDDataSet ThefindVehicleMainByVehicleIDDataSet = new FindVehicleMainByVehicleIDDataSet();
        FindProjectByProjectIDDataSet TheFindProjectByProjectIDDataSet = new FindProjectByProjectIDDataSet();
        SubmitAfterHoursWorkDataSet TheSubmitAfterHoursWorkDataSet = new SubmitAfterHoursWorkDataSet();
        EmployeesAssignedDataSet TheEmployeeAssignedDataSet = new EmployeesAssignedDataSet();

        //setting up global variables
        bool gblnVehicleFound;
        string gstrBuildingAccess;

        public CreateAfterHoursReport()
        {
            InitializeComponent();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            //this method is used to set up the controls
            int intCounter;
            int intNumberOfRecords;

            try
            {
                TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count;
                cboSelectOffice.Items.Clear();
                cboSelectOffice.Items.Add("Select Office");

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectOffice.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectOffice.SelectedIndex = 0;

                TheFindSortedDepartmentDataSet = TheDepartmentClass.FindSortedDepartment();

                intNumberOfRecords = TheFindSortedDepartmentDataSet.FindSortedDepartment.Rows.Count;

                cboSelectDepartment.Items.Clear();
                cboSelectDepartment.Items.Add("Select Department");

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectDepartment.Items.Add(TheFindSortedDepartmentDataSet.FindSortedDepartment[intCounter].Department);
                }

                cboSelectDepartment.SelectedIndex = 0;

                cboBuidlingAccess.Items.Clear();
                cboBuidlingAccess.Items.Add("Select Building Access");
                cboBuidlingAccess.Items.Add("Yes");
                cboBuidlingAccess.Items.Add("No");
                cboBuidlingAccess.SelectedIndex = 0;

                TheAfterWorkEmployeesDataSet.afterhoursemployees.Rows.Clear();

                dgrAssignedEmployees.ItemsSource = TheSubmitAfterHoursWorkDataSet.submitafterhourswork;

                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");
                cboSelectDepartment.SelectedIndex = 0;

                cboSelectEmployee.Items.Clear();
                gstrBuildingAccess = "NO ACCESS";

                TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.gintEmployeeID, "Project Management Sheet // Create After Hours Work");

                txtEnterLastName.Text = "";
                txtEndETA.Text = "";
                txtEnterDate.Text = "";
                txtEnterVehicleNumber.Text = "";
                txtEnterWorkLocation.Text = "";
                txtStartTime.Text = "";
                expSubmitWork.IsEnabled = false;
                cboSelectOffice.Focus();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Project Management Sheet // Create After Hours Report // Reset Controls Method " + Ex.ToString());

                TheSendEmailClass.SendEventLog("Project Management Sheet // Create After Hours Report // Reset Controls Method " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expResetWindow_Expanded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void cboBuidlingAccess_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex = 0;

            intSelectedIndex = cboBuidlingAccess.SelectedIndex;

            if (intSelectedIndex == 1)
                gstrBuildingAccess = "ACCESS IS NEEDED";
            else if (intSelectedIndex == 2)
                gstrBuildingAccess = "NO ACCESS";
            else
                gstrBuildingAccess = "NO ACCESS";
        }

        private void cboSelectOffice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectOffice.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    MainWindow.gintWarehouseID = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;
                }
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("Project Management Sheet // Create After Hours Work // Select Office Combo Box " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Project Management Sheet // Create After Hours Work // Select Office Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectDepartment.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    MainWindow.gintDepartmentID = TheFindSortedDepartmentDataSet.FindSortedDepartment[intSelectedIndex].DepartmentID;
                }
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("Project Management Sheet // Create After Hours Work // Select Department Combo Box " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Project Management Sheet // Create After Hours Work // Select Department Combo Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intLength;
            int intNumberOfRecords;
            int intCounter;

            try
            {
                strLastName = txtEnterLastName.Text;

                intLength = strLastName.Length;

                if (intLength > 2)
                {
                    TheComboEmployeeDataSet = TheEmployeeClass.FillEmployeeComboBox(strLastName);

                    cboSelectEmployee.Items.Clear();
                    cboSelectEmployee.Items.Add("Select Employee");

                    intNumberOfRecords = TheComboEmployeeDataSet.employees.Rows.Count - 1;

                    if (intNumberOfRecords < 0)
                    {
                        TheMessagesClass.ErrorMessage("Employee Was Not Found");
                        return;
                    }

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        cboSelectEmployee.Items.Add(TheComboEmployeeDataSet.employees[intCounter].FullName);
                    }

                    cboSelectEmployee.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("Project Management Sheet // Create After Hours Work // Last Name Text Box Change " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Project Management Sheet // Create After Hours Work // Last Name Text Box Change " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void txtEnterVehicleNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting up the variables
            int intRecordsReturned;
            int intLength;

            try
            {
                gblnVehicleFound = false;

                MainWindow.gstrVehicleNumber = txtEnterVehicleNumber.Text;
                intLength = MainWindow.gstrVehicleNumber.Length;

                if (intLength == 4)
                {
                    TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(MainWindow.gstrVehicleNumber);

                    intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                    if (intRecordsReturned > 0)
                    {
                        MainWindow.gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;

                        TheMessagesClass.InformationMessage("Vehicle was Found");

                        gblnVehicleFound = true;
                    }
                }
                else if (intLength == 6)
                {
                    TheFindActiveVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindActiveVehicleMainByVehicleNumber(MainWindow.gstrVehicleNumber);

                    intRecordsReturned = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber.Rows.Count;

                    if (intRecordsReturned > 0)
                    {
                        MainWindow.gintVehicleID = TheFindActiveVehicleMainByVehicleNumberDataSet.FindActiveVehicleMainByVehicleNumber[0].VehicleID;

                        TheMessagesClass.InformationMessage("Vehicle was Found");

                        gblnVehicleFound = true;
                    }
                    else
                    {
                        TheMessagesClass.ErrorMessage("Vehicle Was Not Found");
                        return;
                    }
                }
                else if (intLength > 6)
                {
                    TheMessagesClass.ErrorMessage("Vehicle Was Not Found, To Many Characters");
                    return;
                }
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("Project Management Sheet // Create After Hours Work // Enter Vehicle Number Text Box Change " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Project Management Sheet // Create After Hours Work // Enter Vehicle Number Text Box Change " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }


        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intEmployeeID;
            string strFirstName;
            string strLastName;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    intEmployeeID = TheComboEmployeeDataSet.employees[intSelectedIndex].EmployeeID;
                    strFirstName = TheComboEmployeeDataSet.employees[intSelectedIndex].FirstName;
                    strLastName = TheComboEmployeeDataSet.employees[intSelectedIndex].LastName;

                    AfterWorkEmployeesDataSet.afterhoursemployeesRow NewEmployeeRow = TheAfterWorkEmployeesDataSet.afterhoursemployees.NewafterhoursemployeesRow();

                    NewEmployeeRow.EmployeeID = intEmployeeID;
                    NewEmployeeRow.FirstName = strFirstName;
                    NewEmployeeRow.LastName = strLastName;

                    TheAfterWorkEmployeesDataSet.afterhoursemployees.Rows.Add(NewEmployeeRow);

                    dgrAssignedEmployees.ItemsSource = TheAfterWorkEmployeesDataSet.afterhoursemployees;

                    txtEnterLastName.Text = "";
                }
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("Project Management Sheet // Create After Hours Report // Select Employee Combo Box " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Project Management // Create After Hours Report // Select Employe Combo Box " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expAddWork_Expanded(object sender, RoutedEventArgs e)
        {
            string strValueForValidation;
            string strErrorMessage = "";
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            DateTime datWorkDate = DateTime.Now;
            string strOutTime;
            string strWorkLocation;
            string strInETA;
            int intCounter;
            int intNumberOfRecords;
            int intEmployeeID;
            string strLastName;
            string strFirstName;

            try
            {
                expAddWork.IsExpanded = false;

                //beginning data validation
                if (cboBuidlingAccess.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Building Access was not Seleted\n";
                }
                if (cboSelectOffice.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Office was not Selected\n";
                }
                if (cboSelectDepartment.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Department was not Selected\n";
                }
                if (TheAfterWorkEmployeesDataSet.afterhoursemployees.Rows.Count < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employees were not Added\n";
                }
                if (gblnVehicleFound == false)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Vehicle was not Added\n";
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
                    datWorkDate = Convert.ToDateTime(strValueForValidation);
                }
                strOutTime = txtStartTime.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyTime(strOutTime);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Out Time is not a Time\n";
                }
                strWorkLocation = txtEnterWorkLocation.Text;
                if (strWorkLocation == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Work Location Was Not Entered\n";
                }
                strInETA = txtEndETA.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyTime(strInETA);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The ETA Time In is not a Time\n";
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                intNumberOfRecords = TheAfterWorkEmployeesDataSet.afterhoursemployees.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intEmployeeID = TheAfterWorkEmployeesDataSet.afterhoursemployees[intCounter].EmployeeID;
                    strFirstName = TheAfterWorkEmployeesDataSet.afterhoursemployees[intCounter].FirstName;
                    strLastName = TheAfterWorkEmployeesDataSet.afterhoursemployees[intCounter].LastName;

                    SubmitAfterHoursWorkDataSet.submitafterhoursworkRow NewWorkRow = TheSubmitAfterHoursWorkDataSet.submitafterhourswork.NewsubmitafterhoursworkRow();

                    NewWorkRow.DataEntryDate = DateTime.Now;
                    NewWorkRow.DepartmentID = MainWindow.gintDepartmentID;
                    NewWorkRow.EmployeeID = intEmployeeID;
                    NewWorkRow.InETA = strInETA;
                    NewWorkRow.ManagerID = MainWindow.gintEmployeeID;
                    NewWorkRow.OutTime = strOutTime;
                    NewWorkRow.ProjectID = MainWindow.gintProjectID;
                    NewWorkRow.VehicleID = MainWindow.gintVehicleID;
                    NewWorkRow.WarehouseID = MainWindow.gintWarehouseID;
                    NewWorkRow.WorkDate = datWorkDate;
                    NewWorkRow.WorkLocation = strWorkLocation;
                    NewWorkRow.BuildingAccess = gstrBuildingAccess;

                    TheSubmitAfterHoursWorkDataSet.submitafterhourswork.Rows.Add(NewWorkRow);

                    EmployeesAssignedDataSet.employeesassignedRow NewEmployeeRow = TheEmployeeAssignedDataSet.employeesassigned.NewemployeesassignedRow();

                    NewEmployeeRow.FirstName = strFirstName;
                    NewEmployeeRow.LastName = strLastName;
                    NewEmployeeRow.ProjectID = MainWindow.gstrCustomerProjectID;
                    NewEmployeeRow.VehicleNumber = MainWindow.gstrVehicleNumber;
                    NewEmployeeRow.WorkDate = datWorkDate;
                    NewEmployeeRow.WorkLocation = strWorkLocation;

                    TheEmployeeAssignedDataSet.employeesassigned.Rows.Add(NewEmployeeRow);

                }

                const string message = "Do You have Another Project to Enter After Hours Work?";
                const string caption = "Another Project";
                MessageBoxResult result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    MainWindow.gstrModule = "ANOTHER PROJECT";
                    EnterProjectID EnterProjectID = new EnterProjectID();
                    EnterProjectID.ShowDialog();
                }
                else
                {
                    const string message2 = "Would You Like to Submit This Report ?";
                    const string caption2 = "Another Project";
                    MessageBoxResult result2 = MessageBox.Show(message2, caption2, MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result2 == MessageBoxResult.Yes)
                    {
                        SubmitReport();
                    }
                }

                ResetControls();
                expSubmitWork.IsEnabled = true;

                dgrAssignedEmployees.ItemsSource = TheEmployeeAssignedDataSet.employeesassigned;
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Create After Hours Report // Add Work Expander " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Create After Hours Report // Add Work Expander " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void SubmitReport()
        {
            int intCounter;
            int intNumberOfRecords;
            int intWarehouseID;
            int intDepartmentID;
            int intEmployeeID;
            int intProjectID;
            int intVehicleID;
            DateTime datWorkDate = DateTime.Now;
            string strOutTime;
            string strWorkLocation;
            string strInETA;
            bool blnFatalError = false;
            DateTime datStartDate = DateTime.Now;
            DateTime datLimitingDate = DateTime.Now;
            DateTime datEndDate;
            string strBuildingAccess;

            try
            {
                expSubmitWork.IsExpanded = false;
                intNumberOfRecords = TheSubmitAfterHoursWorkDataSet.submitafterhourswork.Rows.Count;

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    intWarehouseID = TheSubmitAfterHoursWorkDataSet.submitafterhourswork[intCounter].WarehouseID;
                    intDepartmentID = TheSubmitAfterHoursWorkDataSet.submitafterhourswork[intCounter].DepartmentID;
                    intEmployeeID = TheSubmitAfterHoursWorkDataSet.submitafterhourswork[intCounter].EmployeeID;
                    intProjectID = TheSubmitAfterHoursWorkDataSet.submitafterhourswork[intCounter].ProjectID;
                    intVehicleID = TheSubmitAfterHoursWorkDataSet.submitafterhourswork[intCounter].VehicleID;
                    datWorkDate = TheSubmitAfterHoursWorkDataSet.submitafterhourswork[intCounter].WorkDate;
                    strOutTime = TheSubmitAfterHoursWorkDataSet.submitafterhourswork[intCounter].OutTime;
                    strWorkLocation = TheSubmitAfterHoursWorkDataSet.submitafterhourswork[intCounter].WorkLocation;
                    strInETA = TheSubmitAfterHoursWorkDataSet.submitafterhourswork[intCounter].InETA;
                    strBuildingAccess = TheSubmitAfterHoursWorkDataSet.submitafterhourswork[intCounter].BuildingAccess;

                    if (datWorkDate > datLimitingDate)
                    {
                        datLimitingDate = datWorkDate;
                    }

                    blnFatalError = TheAfterHoursClass.InsertEmployeeOverNightWork2(intWarehouseID, intDepartmentID, intEmployeeID, MainWindow.gintEmployeeID, intVehicleID, intProjectID, datWorkDate, strOutTime, strWorkLocation, strInETA, DateTime.Now, strBuildingAccess);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                datStartDate = TheDateSearchClass.RemoveTime(datStartDate);
                datEndDate = TheDateSearchClass.AddingDays(datLimitingDate, 1);

                TheFindEmployeeOverNightWorkByManagerID2DataSet = TheAfterHoursClass.FindEmployeeOverNightWorkByManagerID2(MainWindow.gintEmployeeID, datStartDate, datEndDate);

                CreateMessage();

                TheMessagesClass.InformationMessage("The Report has been Sent\n");

                this.Close();

                //ResetControls();
                //TheSubmitAfterHoursWorkDataSet.submitafterhourswork.Rows.Clear();
                //TheEmployeeAssignedDataSet.employeesassigned.Rows.Clear();
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Create After Hours Report // Submit Form Expander " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Create After Hours Report // Submit Form Expander " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expSubmitWork_Expanded(object sender, RoutedEventArgs e)
        {
            SubmitReport();
        }
        private void CreateMessage()
        {
            int intCounter;
            int intNumberOfRecords;
            string strMessage;
            string strHeader = "After Hours Work Log";
            string strDepartment;
            string strVehicleNumber;
            string strProjectID;
            int intOfficeID;
            string strOffice;
            string strEmployee;
            string strWorkDate;
            string strOutTime;
            string strWorkLocation;
            string strInETA;
            bool blnFatalError;
            string strEmailAddress;
            string strBuildingAccess;

            try
            {
                intNumberOfRecords = TheFindEmployeeOverNightWorkByManagerID2DataSet.FindEmployeeOverNightWorkByManagerID2.Rows.Count - 1;

                strMessage = "<h1>After Hours Work Log</h1>";
                strMessage += "<table>";
                strMessage += "<tr>";
                strMessage += "<td><b>Office</b></td>";
                strMessage += "<td><b>Department</b></td>";
                strMessage += "<td><b>Employee</b></td>";
                strMessage += "<td><b>Vehicle</b></td>";
                strMessage += "<td><b>Date</b></td>";
                strMessage += "<td><b>Out Time</b></td>";
                strMessage += "<td><b>Project</b></td>";
                strMessage += "<td><b>Work Location</b></td>";
                strMessage += "<td><b>In ETA</b></td>";
                strMessage += "<td><b>BuildingAccess</b></td>";
                strMessage += "</tr>";
                strMessage += "<p>      </p>";
                strMessage += "<p>      </p>";

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intOfficeID = TheFindEmployeeOverNightWorkByManagerID2DataSet.FindEmployeeOverNightWorkByManagerID2[intCounter].OfficeID;

                    TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intOfficeID);

                    strOffice = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName;
                    strDepartment = TheFindEmployeeOverNightWorkByManagerID2DataSet.FindEmployeeOverNightWorkByManagerID2[intCounter].Department;
                    strEmployee = TheFindEmployeeOverNightWorkByManagerID2DataSet.FindEmployeeOverNightWorkByManagerID2[intCounter].FirstName + " ";
                    strEmployee += TheFindEmployeeOverNightWorkByManagerID2DataSet.FindEmployeeOverNightWorkByManagerID2[intCounter].LastName;
                    strVehicleNumber = TheFindEmployeeOverNightWorkByManagerID2DataSet.FindEmployeeOverNightWorkByManagerID2[intCounter].VehicleNumber;
                    strProjectID = TheFindEmployeeOverNightWorkByManagerID2DataSet.FindEmployeeOverNightWorkByManagerID2[intCounter].AssignedProjectID;
                    strWorkDate = Convert.ToString(TheFindEmployeeOverNightWorkByManagerID2DataSet.FindEmployeeOverNightWorkByManagerID2[intCounter].WorkDate);
                    strOutTime = TheFindEmployeeOverNightWorkByManagerID2DataSet.FindEmployeeOverNightWorkByManagerID2[intCounter].OutTime;
                    strWorkLocation = TheFindEmployeeOverNightWorkByManagerID2DataSet.FindEmployeeOverNightWorkByManagerID2[intCounter].WorkLocation;
                    strInETA = TheFindEmployeeOverNightWorkByManagerID2DataSet.FindEmployeeOverNightWorkByManagerID2[intCounter].InETA;
                    strBuildingAccess = TheFindEmployeeOverNightWorkByManagerID2DataSet.FindEmployeeOverNightWorkByManagerID2[intCounter].BuildingAccess;

                    strMessage += "<tr>";
                    strMessage += "<td>" + strOffice + "</td>";
                    strMessage += "<td>" + strDepartment + "</td>";
                    strMessage += "<td>" + strEmployee + "</td>";
                    strMessage += "<td>" + strVehicleNumber + "</td>";
                    strMessage += "<td>" + strWorkDate + "</td>";
                    strMessage += "<td>" + strOutTime + "</td>";
                    strMessage += "<td>" + strProjectID + "</td>";
                    strMessage += "<td>" + strWorkLocation + "</td>";
                    strMessage += "<td>" + strInETA + "</td>";
                    strMessage += "<td>" + strBuildingAccess + "</td>";
                    strMessage += "</tr>";
                }

                strMessage += "</table>";

                blnFatalError = TheSendEmailClass.SendEmail("afterhourswork@bluejaycommunications.com", strHeader, strMessage);

                if (blnFatalError == false)
                    throw new Exception();

                TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(MainWindow.gintEmployeeID);

                strEmailAddress = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmailAddress;

                blnFatalError = TheSendEmailClass.SendEmail(strEmailAddress, strHeader, strMessage);

                if (blnFatalError == false)
                    throw new Exception();
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Create After Hours Report // Create Message " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Create After Hours Report // Create Message " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }
    }
}
