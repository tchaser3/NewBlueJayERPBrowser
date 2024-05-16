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
    /// Interaction logic for EditProject.xaml
    /// </summary>
    public partial class EditProject : Page
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
        string gstrAssignedOffice;
        int gintTransactionID;
        bool gblnEditUnderground;
        bool gblnECDDateChanged;
        string gstrCustomerProjectID;
        int gintProjectID;

        public EditProject()
        {
            InitializeComponent();
        }
         private void ResetControls()
        {
            int intCounter;
            int intNumberOfRecords;

            txtBlueJayID.Text = "";
            txtBusinessAddress.Text = "";
            txtCurrentNotes.Text = "";
            txtCustomerProjectID.Text = "";
            txtDateReceived.Text = "";
            txtDepartment.Text = "";
            txtECDDate.Text = "";
            txtEnterNewNotes.Text = "";
            txtEnterProjectID.Text = "";
            txtProjectID.Text = "";
            txtProjectName.Text = "";

            TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

            intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count;

            cboAssignedOffice.Items.Clear();
            cboAssignedOffice.Items.Add("Select Office");
            cboProjectStatus.Items.Clear();
            cboProjectStatus.Items.Add("Select Status");

            for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
            {
                cboAssignedOffice.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
            }

            cboAssignedOffice.SelectedIndex = 0;

            TheFindWorkOrderStatusSortedDataSet = TheWorkOrderClass.FindWorkOrderStatusSorted();

            intNumberOfRecords = TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted.Rows.Count;

            for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
            {
                cboProjectStatus.Items.Add(TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted[intCounter].WorkOrderStatus);
            }

            cboProjectStatus.SelectedIndex = 0;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void expResetPage_Expanded(object sender, RoutedEventArgs e)
        {
            expResetPage.IsExpanded = false;
            ResetControls();
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            string strProjectNotes = "";

            try
            {
                gstrCustomerProjectID = txtEnterProjectID.Text;
                if(gstrCustomerProjectID.Length < 2)
                {
                    TheMessagesClass.ErrorMessage("The Project ID is not Long Enough");
                    return;
                }

                TheFindProjectMatrixByCustomerAssignedIDDataSet = TheProjectMatrixClass.FindProjectMatrixByCustomerAssignedID(gstrCustomerProjectID);

                intNumberOfRecords = TheFindProjectMatrixByCustomerAssignedIDDataSet.FindProjectMatrixByCustomerAssignedID.Rows.Count;

                if(intNumberOfRecords < 0)
                {
                    TheMessagesClass.ErrorMessage("Project Not Found");
                    return;
                }
                else
                {
                    gintProjectID = TheFindProjectMatrixByCustomerAssignedIDDataSet.FindProjectMatrixByCustomerAssignedID[0].ProjectID;
                }

                TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.gintEmployeeID, "New Blue Jay ERP Browser // Edit Project for Project " + gstrCustomerProjectID);

                TheFindProductionProjectByProjectIDDataSet = TheProductionProjectClass.FindProductionProjectByProjectID(gintProjectID);
                TheFindProductionProjectUndergroundByProjectID = TheProductionProjectClass.FindProductionProjectUndergroundByProjectID(gintProjectID);

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

                txtProjectID.Text = Convert.ToString(TheFindProjectMatrixByCustomerAssignedIDDataSet.FindProjectMatrixByCustomerAssignedID[0].ProjectID);
                txtBlueJayID.Text = TheFindProjectMatrixByCustomerAssignedIDDataSet.FindProjectMatrixByCustomerAssignedID[0].AssignedProjectID;
                txtCustomerProjectID.Text = TheFindProjectMatrixByCustomerAssignedIDDataSet.FindProjectMatrixByCustomerAssignedID[0].CustomerAssignedID;
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

                TheFindProductionProjectUpdateByProjectIDDataSet = TheProductionProjectUpdatesClass.FindProductionProjectUpdateByProjectID(gintProjectID);

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
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Edit Project // Find Button " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Edit Project // Find Button " + Ex.ToString());

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
                    gstrAssignedOffice = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].FirstName;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Edit Project // Select Office Combo Box " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Enter Project // Select Office Combo Box " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboProjectStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //this will set the warehouse id
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
                        TheFindProductionProjectUndergroundByProjectID = TheProductionProjectClass.FindProductionProjectUndergroundByProjectID(gintProjectID);

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Edit Project // Select Status Combo Box " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Enter Project // Select Status Combo Box " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
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

                            blnFatalError = TheProductionProjectClass.InsertProductionProjectDocumentation(gintProjectID, MainWindow.gintEmployeeID, DateTime.Now, strDocumentPath);

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Edit Project // Add Documents Method " + Ex.Message);

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Edit Project // Add Documents Method " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

    }
}
