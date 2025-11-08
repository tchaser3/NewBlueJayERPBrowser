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
using ProductionProjectDLL;
using DataValidationDLL;
using ProjectMatrixDLL;
using NewEmployeeDLL;
using WorkOrderDLL;
using ProductionProjectUpdatesDLL;
using DateSearchDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for EditOutageProject.xaml
    /// </summary>
    public partial class EditOutageProject : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ProductionProjectClass TheProductionProjectClass = new ProductionProjectClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        WorkOrderClass TheWorkOrderClass = new WorkOrderClass();
        ProductionProjectUpdatesClass TheProductionProjectUpdatesClass = new ProductionProjectUpdatesClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data        
        FindProjectMatrixByProjectIDDataSet TheFindProjectMatrixByProjectIDDataSet = new FindProjectMatrixByProjectIDDataSet();
        FindProjectMatrixByCustomerAssignedIDDataSet TheFindProjectMatrixByCustomerAssignedIDDataSet = new FindProjectMatrixByCustomerAssignedIDDataSet();
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindWorkOrderStatusSortedDataSet TheFindWorkOrderStatusSortedDataSet = new FindWorkOrderStatusSortedDataSet();
        FindProductionProjectUpdateByProjectIDDataSet TheFindProductionProjectUpdateByProjectIDDataSet = new FindProductionProjectUpdateByProjectIDDataSet();
        FindProductionProjectByProjectIDDataSet TheFindProductionProjectByProjectIDDataSet = new FindProductionProjectByProjectIDDataSet();
        FindProductionProjectUndergroundByProjectIDDataSet TheFindProductionProjectUndergroundByProjectID = new FindProductionProjectUndergroundByProjectIDDataSet();

        //global variables
        int gintWarehouseID;
        string gstrWorkOrderStatus;
        int gintStatusID;
        int gintTransactionID;
        bool gblnEditUnderground;
        bool gblnECDDateChanged;
        
        public EditOutageProject()
        {
            InitializeComponent();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseWindow.IsExpanded = false;
            this.Close();
        }
        private void ResetControls()
        {
            int intCounter;
            int intNumberOfRecords;
            expEditProjectID.IsEnabled = false;
            expSave.IsEnabled = false;

            txtBlueJayID.Text = "";
            txtBusinessAddress.Text = "";
            txtCurrentNotes.Text = "";
            txtCustomerProjectID.Text = "";
            txtDateReceived.Text = "";
            txtDepartment.Text = "";
            txtECDDate.Text = "";
            txtEnterNewNotes.Text = "";
            txtProjectID.Text = "";
            txtProjectName.Text = "";
            MainWindow.gblnOutageProject = true;

            TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

            intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count;

            cboAssignedOffice.Items.Clear();
            cboAssignedOffice.Items.Add("Select Office");
            cboProjectStatus.Items.Clear();
            cboProjectStatus.Items.Add("Select Status");

            for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
            {
                cboAssignedOffice.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
            }

            cboAssignedOffice.SelectedIndex = 0;

            TheFindWorkOrderStatusSortedDataSet = TheWorkOrderClass.FindWorkOrderStatusSorted();

            intNumberOfRecords = TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted.Rows.Count;

            for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
            {
                cboProjectStatus.Items.Add(TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted[intCounter].WorkOrderStatus);
            }

            cboProjectStatus.SelectedIndex = 0;
            txtCurrentNotes.Background = Brushes.White;
            txtCurrentNotes.Foreground = Brushes.Black;

            FindProject();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        private void FindProject()
        {
            int intCounter;
            int intNumberOfRecords;
            string strProjectNotes = "";
            string strCustomerProjectID;

            try
            {
                TheFindProjectMatrixByProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByProjectID(MainWindow.gintProjectID);
                strCustomerProjectID = TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].CustomerAssignedID;
                TheFindProjectMatrixByCustomerAssignedIDDataSet = TheProjectMatrixClass.FindProjectMatrixByCustomerAssignedID(strCustomerProjectID);

                TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.gintLoggedInEmployeeID, "New Blue Jay ERP Browser // Edit Project for Project " + TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].CustomerAssignedID);

                TheFindProductionProjectByProjectIDDataSet = TheProductionProjectClass.FindProductionProjectByProjectID(MainWindow.gintProjectID);
                TheFindProductionProjectUndergroundByProjectID = TheProductionProjectClass.FindProductionProjectUndergroundByProjectID(MainWindow.gintProjectID);

                intNumberOfRecords = TheFindProductionProjectUndergroundByProjectID.FindProductionProjectUndergroundByProjectID.Rows.Count;

                if (intNumberOfRecords > 0)
                {
                    expAddUnderground.Header = "Edit Underground";
                    gblnEditUnderground = true;
                }
                else if (intNumberOfRecords < 1)
                {
                    gblnEditUnderground = false;
                    expAddUnderground.Header = "Add Underground";
                }

                gintTransactionID = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].TransactionID;

                txtProjectID.Text = Convert.ToString(TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].ProjectID);
                txtBlueJayID.Text = TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].AssignedProjectID;
                txtCustomerProjectID.Text = TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].CustomerAssignedID;
                txtProjectName.Text = TheFindProjectMatrixByCustomerAssignedIDDataSet.FindProjectMatrixByCustomerAssignedID[0].ProjectName;

                gintWarehouseID = TheFindProjectMatrixByCustomerAssignedIDDataSet.FindProjectMatrixByCustomerAssignedID[0].AssignedOfficeID;

                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count;

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    if (TheFindWarehousesDataSet.FindWarehouses[intCounter].EmployeeID == gintWarehouseID)
                    {
                        cboAssignedOffice.SelectedIndex = intCounter + 1;
                        break;
                    }
                }

                txtDepartment.Text = TheFindProjectMatrixByCustomerAssignedIDDataSet.FindProjectMatrixByCustomerAssignedID[0].Department;
                txtBusinessAddress.Text = TheFindProjectMatrixByCustomerAssignedIDDataSet.FindProjectMatrixByCustomerAssignedID[0].BusinessAddress;

                TheFindWorkOrderStatusSortedDataSet = TheWorkOrderClass.FindWorkOrderStatusSorted();

                intNumberOfRecords = TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted.Rows.Count;

                gstrWorkOrderStatus = TheFindProjectMatrixByCustomerAssignedIDDataSet.FindProjectMatrixByCustomerAssignedID[0].WorkOrderStatus;

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    if (gstrWorkOrderStatus == TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted[intCounter].WorkOrderStatus)
                    {
                        cboProjectStatus.SelectedIndex = intCounter + 1;
                        break;
                    }
                }

                txtDateReceived.Text = Convert.ToString(TheFindProjectMatrixByCustomerAssignedIDDataSet.FindProjectMatrixByCustomerAssignedID[0].DateReceived);
                txtECDDate.Text = Convert.ToString(TheFindProjectMatrixByCustomerAssignedIDDataSet.FindProjectMatrixByCustomerAssignedID[0].ECDDate);

                if (TheFindProjectMatrixByCustomerAssignedIDDataSet.FindProjectMatrixByCustomerAssignedID[0].ECDDate < DateTime.Now)
                {
                    if (TheFindProjectMatrixByCustomerAssignedIDDataSet.FindProjectMatrixByCustomerAssignedID[0].WorkOrderStatus != "CLOSED")
                    {
                        if (TheFindProjectMatrixByCustomerAssignedIDDataSet.FindProjectMatrixByCustomerAssignedID[0].WorkOrderStatus != "CANCEL")
                        {
                            if (TheFindProjectMatrixByCustomerAssignedIDDataSet.FindProjectMatrixByCustomerAssignedID[0].WorkOrderStatus != "SUBMITTED")
                            {
                                txtCurrentNotes.Background = Brushes.Red;
                                txtCurrentNotes.Foreground = Brushes.White;
                            }
                        }
                    }
                }
                else if (TheFindProjectMatrixByCustomerAssignedIDDataSet.FindProjectMatrixByCustomerAssignedID[0].ECDDate < TheDateSearchClass.AddingDays(DateTime.Now, 3))
                {
                    txtCurrentNotes.Background = Brushes.Yellow;
                    txtCurrentNotes.Foreground = Brushes.Black;
                }
                else if (TheFindProjectMatrixByCustomerAssignedIDDataSet.FindProjectMatrixByCustomerAssignedID[0].WorkOrderStatus != "IN PROCESS")
                {
                    txtCurrentNotes.Background = Brushes.Green;
                    txtCurrentNotes.Foreground = Brushes.White;
                }

                TheFindProductionProjectUpdateByProjectIDDataSet = TheProductionProjectUpdatesClass.FindProductionProjectUpdateByProjectID(MainWindow.gintProjectID);

                intNumberOfRecords = TheFindProductionProjectUpdateByProjectIDDataSet.FindProductionProjectUpdatesByProjectID.Rows.Count;

                if (intNumberOfRecords > 0)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        strProjectNotes += Convert.ToString(TheFindProductionProjectUpdateByProjectIDDataSet.FindProductionProjectUpdatesByProjectID[intCounter].TransactionDate);
                        strProjectNotes += "\t";
                        strProjectNotes += TheFindProductionProjectUpdateByProjectIDDataSet.FindProductionProjectUpdatesByProjectID[intCounter].FirstName + " ";
                        strProjectNotes += TheFindProductionProjectUpdateByProjectIDDataSet.FindProductionProjectUpdatesByProjectID[intCounter].LastName + "\t";
                        strProjectNotes += TheFindProductionProjectUpdateByProjectIDDataSet.FindProductionProjectUpdatesByProjectID[intCounter].ProjectUpdate + "\n\n";
                    }
                }

                txtCurrentNotes.Text = strProjectNotes;

                if (MainWindow.gstrEmployeeGroup == "ADMIN")
                {
                    expEditECDDate.IsEnabled = true;
                }
                else
                {
                    expEditECDDate.IsEnabled = false;
                    txtECDDate.IsReadOnly = true;
                }

                gblnECDDateChanged = false;
                expEditProjectID.IsEnabled = true;
                expSave.IsEnabled = true;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Edit Outage Project // Find Project " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Edit Outate Project // Find Project " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expEditProjectID_Expanded(object sender, RoutedEventArgs e)
        {
            expEditProjectID.IsExpanded = false;
            MainWindow.gintProjectID = Convert.ToInt32(txtProjectID.Text);
            EditProjectID EditProjectID = new EditProjectID();
            EditProjectID.ShowDialog();
            txtCustomerProjectID.Text = MainWindow.gstrCustomerProjectID;
        }

        private void expAddDocuments_Expanded(object sender, RoutedEventArgs e)
        {
            expAddDocuments.IsExpanded = false;
            AddDocuments();
        }
        private void AddDocuments()
        {
            //setting local variables
            string strDocumentPath;
            bool blnFatalError = false;
            DateTime datTransactionDate = DateTime.Now;
            int intCounter;
            int intNumberOfRecords;

            try
            {

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Multiselect = true;
                dlg.FileName = "Document"; // Default file name

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    intNumberOfRecords = dlg.FileNames.Length - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            strDocumentPath = dlg.FileNames[intCounter].ToUpper();

                            blnFatalError = TheProductionProjectClass.InsertProductionProjectDocumentation(MainWindow.gintProjectID, MainWindow.gintLoggedInEmployeeID, DateTime.Now, strDocumentPath);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                }
                else
                {
                    return;
                }

                TheMessagesClass.InformationMessage("The Documents have been Added");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Edit Outage Project // Add Documents Method " + Ex.Message);

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Edit Outage Project // Add Documents Method " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expAddUnderground_Expanded(object sender, RoutedEventArgs e)
        {
            expAddUnderground.IsExpanded = false;

            if (gblnEditUnderground == false)
            {
                AddProjectUnderground AddProjectUnderground = new AddProjectUnderground();
                AddProjectUnderground.ShowDialog();
            }
            else if (gblnEditUnderground == true)
            {
                EditProjectUnderground editProjectUnderground = new EditProjectUnderground();
                editProjectUnderground.ShowDialog();
            }
        }

        private void expViewDocuments_Expanded(object sender, RoutedEventArgs e)
        {
            expViewDocuments.IsExpanded = false;
            ViewProjectDocumentation ViewProjectDocumentation = new ViewProjectDocumentation();
            ViewProjectDocumentation.ShowDialog();
        }

        private void expEditECDDate_Expanded(object sender, RoutedEventArgs e)
        {
            txtECDDate.IsReadOnly = false;
            txtECDDate.Background = Brushes.White;
            gblnECDDateChanged = true;
            expEditECDDate.IsExpanded = false;
        }

        private void expEditProjectInfo_Expanded(object sender, RoutedEventArgs e)
        {
            expEditProjectInfo.IsExpanded = false;
            EditProjectInfo EditProjectInfo = new EditProjectInfo();
            EditProjectInfo.ShowDialog();
        }

        private void expSave_Expanded(object sender, RoutedEventArgs e)
        {
            //this will save the information for the project
            //setting local variables
            bool blnFatalError = false;
            string strProjectNotes;
            string strValueForValidation;
            DateTime datECDDate = DateTime.Now;

            try
            {
                expSave.IsExpanded = false;
                strProjectNotes = txtEnterNewNotes.Text;

                if (strProjectNotes.Length < 10)
                {
                    TheMessagesClass.ErrorMessage("The Notes Entered is not Long Enough");

                    return;
                }

                if ((MainWindow.gstrEmployeeGroup == "ADMIN") && (gblnECDDateChanged == true))
                {
                    strValueForValidation = txtECDDate.Text;

                    blnFatalError = TheDataValidationClass.VerifyDateData(strValueForValidation);

                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("The ECD Date is not a Date");
                        return;

                    }
                    else
                    {
                        datECDDate = Convert.ToDateTime(strValueForValidation);
                    }
                }

                if ((cboProjectStatus.SelectedIndex > 0) && (cboAssignedOffice.SelectedIndex > 0))
                {
                    blnFatalError = TheProductionProjectClass.UpdateProductionProjectStatus(gintTransactionID, gintStatusID);

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheProductionProjectClass.UpdateProductionProjectAssignedOffice(gintTransactionID, gintWarehouseID);

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheProductionProjectClass.UpdateProductionProjectStatusDate(gintTransactionID, DateTime.Now);

                    if (blnFatalError == true)
                        throw new Exception();

                    blnFatalError = TheProductionProjectUpdatesClass.InsertProductionProjectUpdate(MainWindow.gintProjectID, MainWindow.gintLoggedInEmployeeID, strProjectNotes);

                    if (blnFatalError == true)
                        throw new Exception();

                    if ((MainWindow.gstrEmployeeGroup == "ADMIN") && (gblnECDDateChanged == true))
                    {
                        blnFatalError = TheProductionProjectClass.UpdateProductionProjectECDDate(gintTransactionID, datECDDate);

                        if (blnFatalError == true)
                            throw new Exception();

                        TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.gintLoggedInEmployeeID, "Employee is Updating the ECD Date for Project " + txtCustomerProjectID.Text);

                        TheSendEmailClass.SendEventLog("Employee is Editing the ECD Date For Project " + txtCustomerProjectID.Text);

                        TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Edit Project // Save Expander // Employee Is Editing the ECD Date For Project " + txtCustomerProjectID.Text);

                    }

                    txtECDDate.IsReadOnly = true;
                    txtECDDate.Background = Brushes.LightGray;
                    txtEnterNewNotes.Text = "";

                    TheMessagesClass.InformationMessage("The Project Has Been Updated");

                    this.Close();

                }
                else
                {
                    TheMessagesClass.ErrorMessage("Both the Office and Status Must Be Selected");
                    return;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Edit Outage Project // Save Expander " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Enter Outage Project // Save Expander " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboProjectStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboProjectStatus.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    gstrWorkOrderStatus = TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted[intSelectedIndex].WorkOrderStatus;
                    gintStatusID = TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted[intSelectedIndex].StatusID;

                    if ((gintStatusID == 1002) || (gintStatusID == 1007))
                    {
                        TheFindProductionProjectUndergroundByProjectID = TheProductionProjectClass.FindProductionProjectUndergroundByProjectID(MainWindow.gintProjectID);

                        if (TheFindProductionProjectUndergroundByProjectID.FindProductionProjectUndergroundByProjectID.Rows.Count > 0)
                        {
                            if (TheFindProductionProjectUndergroundByProjectID.FindProductionProjectUndergroundByProjectID[0].IsDateCompletedNull() == true)
                            {
                                TheMessagesClass.ErrorMessage("Underground for this Project is not Complete");
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Edit Outage Project // Select Status Combo Box " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Enter Outage Project // Select Status Combo Box " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboAssignedOffice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //this will set the warehouse id
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboAssignedOffice.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    gintWarehouseID = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;
                    MainWindow.gstrAssignedProjectID = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].FirstName;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Edit Outage Project // Select Office Combo Box " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Edit Outage Project // Select Office Combo Box " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProductivity_Expanded(object sender, RoutedEventArgs e)
        {
            expProductivity.IsExpanded = false;
            EnterProductivity EnterProductivity = new EnterProductivity();
            EnterProductivity.ShowDialog();
        }

        private void expCreateAfterHours_Expanded(object sender, RoutedEventArgs e)
        {
            expCreateAfterHours.IsExpanded = false;
            CreateAfterHoursReport CreateAfterHours = new CreateAfterHoursReport();
            CreateAfterHours.ShowDialog();
        }
    }
}
