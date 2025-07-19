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
using ProjectsDLL;
using ProductionProjectDLL;
using NewEventLogDLL;
using DataValidationDLL;
using NewEmployeeDLL;
using DepartmentDLL;
using EmployeeDateEntryDLL;
using ProjectMatrixDLL;
using System.Runtime.Serialization;
using JobTypeDLL;
using OutageProjectDLL;
using DesignProjectsDLL;
using WorkOrderDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for AddOutageProject.xaml
    /// </summary>
    public partial class AddOutageProject : Page
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ProjectClass TheProjectClass = new ProjectClass();
        ProductionProjectClass TheProductionProjectClass = new ProductionProjectClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();
        EmployeeDateEntryClass TheEmployeeDataEntryClass = new EmployeeDateEntryClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        JobTypeClass TheJobTypeClass = new JobTypeClass();
        OutageProjectClass TheOutageProjectClass = new OutageProjectClass();
        WorkOrderClass TheWorkOrderClass = new WorkOrderClass();

        //setting up the data
        FindProductionProjectInfoDataSet TheFindProductionProjectInfoDataSet = new FindProductionProjectInfoDataSet();
        FindSortedJobTypeDataSet TheFindSortedJobTypeDataSet = new FindSortedJobTypeDataSet();
        FindDesignProjectsByAssignedProjectIDDataSet TheFindDesignProjectsbyAssignedProjectIDDataSet = new FindDesignProjectsByAssignedProjectIDDataSet();
        FindProductionManagersDataSet TheFindProductionManagersDataSet = new FindProductionManagersDataSet();
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindSortedCustomerLinesDataSet TheFindSortedCustomerLinesDataSet = new FindSortedCustomerLinesDataSet();
        FindProjectMatrixByCustomerProjectIDDataSet TheFindProjectMatrixByCustomerProjectIDDataSet = new FindProjectMatrixByCustomerProjectIDDataSet();
        FindProjectMatrixByAssignedProjectIDDataSet TheFindProjectMatrxiByAssignedProjectIDDataSet = new FindProjectMatrixByAssignedProjectIDDataSet();
        FindProjectByProjectIDDataSet TheFindProjectByProjectIDDataSet = new FindProjectByProjectIDDataSet();
        FindProjectByAssignedProjectIDDataSet TheFindProjectByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();
        FindProjectMatrixByProjectIDDataSet TheFindProjectMatrixByProjectIDDataSet = new FindProjectMatrixByProjectIDDataSet();
        FindProductionProjectByProjectIDDataSet TheFindProductionProjectByProjectIDDataSet = new FindProductionProjectByProjectIDDataSet();
        FindProdutionProjectsByAssignedProjectIDDataSet TheFindProductionProjectByAssignedProjectIDDataSet = new FindProdutionProjectsByAssignedProjectIDDataSet();
        FindSortedOutageProjectWorkStatusDataSet TheFindSortedOutageProjectWorkStatusDataSet = new FindSortedOutageProjectWorkStatusDataSet();
        FindWorkOrderStatusByStatusDataSet TheFindWorkOrderStatusByStatusDataSet = new FindWorkOrderStatusByStatusDataSet();

        int gintDepartmentID;
        int gintManagerID;
        int gintOfficeID;
        int gintStatusID;
        //int gintProjectID;
        bool gblnProjectExists;
        bool gblnProjectMatrixExists;
        bool gblnOver2500;
        int gintJobTypeID;
        string gstrOutageStatus;
        bool gblnNeedsUnderground;

        public AddOutageProject()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            //setting up the variabvles
            int intCounter;
            int intNumberOfRecords;
            int intSelectedIndex = 0;

            txtAssignedProjectID.Text = "";
            txtCustomerProjectID.Text = "";
            txtProjectAddress.Text = "";
            txtProjectCity.Text = "";
            txtProjectState.Text = "";
            txtDateReceived.Text = "";
            txtECDDate.Text = "";
            txtEnterProjectNotes.Text = "";
            txtProjectName.Text = "";
            cboSelectDepartment.SelectedIndex = 0;
            cboSelectManager.SelectedIndex = 0;
            cboSelectOffice.SelectedIndex = 0;
            cboSelectStatus.SelectedIndex = 0;
            gblnProjectExists = false;
            gblnProjectMatrixExists = false;
            txtPointOfContact.Text = "";
            txtPONumber.Text = "";
            txtPOAmount.Text = "";
            DateTime datProjectID = DateTime.Now;
            string strCustomerProjectID;

            try
            {
                //loading up the combo boxes
                cboSelectManager.Items.Clear();

                TheFindProductionManagersDataSet = TheEmployeeClass.FindProductionManagers();
                cboSelectManager.Items.Add("Select Manager");
                intNumberOfRecords = TheFindProductionManagersDataSet.FindProductionManagers.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectManager.Items.Add(TheFindProductionManagersDataSet.FindProductionManagers[intCounter].FullName);
                }

                cboSelectManager.SelectedIndex = 0;

                cboSelectDepartment.Items.Clear();
                cboSelectDepartment.Items.Add("Select Department");

                TheFindSortedCustomerLinesDataSet = TheDepartmentClass.FindSortedCustomerLines();

                intNumberOfRecords = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectDepartment.Items.Add(TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intCounter].Department);
                }

                cboSelectDepartment.SelectedIndex = 0;

                cboSelectOffice.Items.Clear();
                cboSelectOffice.Items.Add("Select Office");

                TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();
                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectOffice.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectOffice.SelectedIndex = 0;

                cboSelectStatus.Items.Clear();
                cboSelectStatus.Items.Add("Select Status");

                TheFindSortedOutageProjectWorkStatusDataSet = TheOutageProjectClass.FindSortedOutageProjectWorkStatus();

                cboSelectStatus.IsEnabled = true;

                intNumberOfRecords = TheFindSortedOutageProjectWorkStatusDataSet.FindSortedOutageProjectWorkStatus.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectStatus.Items.Add(TheFindSortedOutageProjectWorkStatusDataSet.FindSortedOutageProjectWorkStatus[intCounter].OutageWorkStatus);

                    if (TheFindSortedOutageProjectWorkStatusDataSet.FindSortedOutageProjectWorkStatus[intCounter].OutageWorkStatus == "OPEN")
                    {
                        intSelectedIndex = intCounter + 1;
                    }
                }

                cboSelectStatus.SelectedIndex = intSelectedIndex;

                cboSelectStatus.IsEnabled = false;

                cboSelectJobtype.Items.Add("Select Job Type");

                TheFindSortedJobTypeDataSet = TheJobTypeClass.FindSortedJobType();

                intNumberOfRecords = TheFindSortedJobTypeDataSet.FindSortedJobType.Rows.Count;

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectJobtype.Items.Add(TheFindSortedJobTypeDataSet.FindSortedJobType[intCounter].JobType);
                }

                cboSelectJobtype.SelectedIndex = 0;

                cboNeedsUnderground.Items.Clear();
                cboNeedsUnderground.Items.Add("Select Underground Needs");
                cboNeedsUnderground.Items.Add("Yes");
                cboNeedsUnderground.Items.Add("No");
                cboNeedsUnderground.SelectedIndex = 0;

                strCustomerProjectID = Convert.ToString(datProjectID.Year);
                strCustomerProjectID += Convert.ToString(datProjectID.Month);
                strCustomerProjectID += Convert.ToString(datProjectID.Day);
                strCustomerProjectID += Convert.ToString(datProjectID.Hour);
                strCustomerProjectID += Convert.ToString(datProjectID.Minute);
                txtCustomerProjectID.Text = strCustomerProjectID;

                txtAssignedProjectID.Text = Convert.ToString(datProjectID.Year) + "-003";

                txtDateReceived.Text = Convert.ToString(DateTime.Now);
                txtECDDate.Text = Convert.ToString(DateTime.Now.AddDays(180));
                txtPOAmount.Text = "0.00";
                txtPONumber.Text = "UNKNOWN";
                txtProjectName.Text = "OUTAGE FOR ";
                txtEnterProjectNotes.Text = "OUTAGE PROJECT FOR " + Convert.ToString(DateTime.Now);
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Add Outage Project // Reset Controls " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Add Outage Project // Reset Controls " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }           

        }

        private void expResetWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expResetWindow.IsExpanded = false;
            ResetControls();
        }

        private void cboSelectDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            string strAssignedProjectID;

            intSelectedIndex = cboSelectDepartment.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                gintDepartmentID = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intSelectedIndex].DepartmentID;
            }
        }

        private void cboSelectManager_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectManager.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                gintManagerID = TheFindProductionManagersDataSet.FindProductionManagers[intSelectedIndex].EmployeeID;
            }
        }

        private void cboSelectOffice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectOffice.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                gintOfficeID = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;
            }
        }

        private void cboSelectJobtype_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectJobtype.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                gintJobTypeID = TheFindSortedJobTypeDataSet.FindSortedJobType[intSelectedIndex].JobTypeID;
            }
        }

        private void cboNeedsUnderground_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           if (cboNeedsUnderground.SelectedIndex == 1)
            {
                gblnNeedsUnderground = true;
            }
            else if (cboNeedsUnderground.SelectedIndex == 2)
            {
                gblnNeedsUnderground = false;
            }
        }

        private void cboSelectStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intCounter;

            intSelectedIndex = cboSelectStatus.SelectedIndex - 1;            

            if (intSelectedIndex > -1)
            {
                gstrOutageStatus = TheFindSortedOutageProjectWorkStatusDataSet.FindSortedOutageProjectWorkStatus[intSelectedIndex].OutageWorkStatus;

                TheFindWorkOrderStatusByStatusDataSet = TheWorkOrderClass.FindWorkOrderStatusByStatus(gstrOutageStatus);

                gintStatusID = TheFindWorkOrderStatusByStatusDataSet.FindWorkOrderStatusByStatus[0].StatusID;
            }
                
        }

        private void expProcess_Expanded(object sender, RoutedEventArgs e)
        {
            string strCustomerProjectID;
            string strAssignedProjectID;
            string strProjectName;
            string strProjectAddress;
            string strProjectCity;
            string strProjectState;
            DateTime datDateReceived;
            DateTime datECDDate;
            string strPointOfContact;
            string strPONumber;
            decimal decPOAmount;
            string strProjectNotes;
            bool blnFatalError = false;
            string strErrorMessage = "";
            int intEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;

            try
            {
                
                if(cboSelectDepartment.SelectedIndex < 1)
                {
                    blnFatalError= true;
                    strErrorMessage += "You Must Select a Department\n";
                }
                strCustomerProjectID = txtCustomerProjectID.Text;
                if (strCustomerProjectID.Length < 7)
                {
                    blnFatalError = true;
                    strErrorMessage += "You Must enter Customer Project ID\n";
                }
                strAssignedProjectID = txtAssignedProjectID.Text;
                if (strAssignedProjectID.Length < 7)
                {
                    blnFatalError = true;
                    strErrorMessage += "You Must enter Assigned Project ID\n";
                }
                strProjectName = txtProjectName.Text;
                if (strProjectName.Length < 8)
                {
                    blnFatalError = true;
                    strErrorMessage += "You Must enter Project Name\n";
                }
                strProjectAddress = txtProjectAddress.Text;
                if (strProjectAddress.Length < 5)
                {
                    blnFatalError = true;
                    strErrorMessage += "You Must enter Project Address\n";
                }
                strProjectCity = txtProjectCity.Text;
                if (strProjectCity.Length < 3)
                {
                    blnFatalError = true;
                    strErrorMessage += "You Must enter Project City\n";
                }
                strProjectState = txtProjectState.Text;
                if (strProjectState.Length < 2)
                {
                    blnFatalError = true;
                    strErrorMessage += "You Must enter Project State\n";
                }
                if(cboSelectManager.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "You Must Select a Manager\n";
                }
                if (cboSelectOffice.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "You Must Select an Office\n";
                }
                if(cboSelectJobtype.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "You Must Select a Job Type\n";
                }
                datDateReceived = Convert.ToDateTime(txtDateReceived.Text);
                datECDDate = Convert.ToDateTime(txtECDDate.Text);
                if(cboNeedsUnderground.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "You Must Select Underground Needs\n";
                }
                strPointOfContact = txtPointOfContact.Text;
                if (strPointOfContact.Length < 7)
                {
                    blnFatalError = true;
                    strErrorMessage += "You Must enter Point Of Contact\n";
                }
                strPONumber = txtPONumber.Text;
                decPOAmount = Convert.ToDecimal(txtPOAmount.Text);
                strProjectNotes = txtEnterProjectNotes.Text;
                if (strProjectNotes.Length < 10)
                {
                    blnFatalError = true;
                    strErrorMessage += "You Must enter Project Notes\n";
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheProjectClass.InsertProject(strCustomerProjectID, strProjectName);

                if (blnFatalError == true)
                    throw new Exception();

                TheFindProjectByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID(strCustomerProjectID);

                MainWindow.gintProjectID = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectID;

                blnFatalError = TheProductionProjectClass.InsertProdutionProject(MainWindow.gintProjectID, gintDepartmentID, strProjectAddress, strProjectCity, strProjectState, gintManagerID, gintOfficeID, datDateReceived, datECDDate, gintStatusID, strProjectNotes);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheOutageProjectClass.InsertOutageProject(MainWindow.gstrUserName, DateTime.Now, strCustomerProjectID, MainWindow.gintProjectID, strProjectAddress, "", strProjectCity, strProjectState, "", "OPEN");

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheProductionProjectClass.InsertProductionProjectUpdate(MainWindow.gintProjectID, intEmployeeID, DateTime.Now, strProjectNotes);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheEmployeeDataEntryClass.InsertIntoEmployeeDateEntry(intEmployeeID, "New Blue Jay ERP // Add Project Number " + strAssignedProjectID + " Has Been Added");

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheProjectMatrixClass.InsertProjectMatrix(MainWindow.gintProjectID, strAssignedProjectID, strCustomerProjectID, DateTime.Now, intEmployeeID, gintOfficeID, gintDepartmentID);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheProductionProjectClass.InsertProductionProjectInfo(MainWindow.gintProjectID, gintJobTypeID, strPointOfContact, strPONumber, decPOAmount);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheProductionProjectClass.InsertProductionProjectNTP(MainWindow.gstrUserName, strPONumber, decPOAmount, MainWindow.gintProjectID);

                if (blnFatalError == true)
                    throw new Exception();

                if (gblnNeedsUnderground == true)
                {
                    AddProjectUnderground AddProjectUnderground = new AddProjectUnderground();
                    AddProjectUnderground.ShowDialog();
                }

                AddDocuments(intEmployeeID);
                expAddDocuments.IsEnabled = true;

                TheMessagesClass.InformationMessage("Project Has Been Entered");

                TheFindProductionProjectByProjectIDDataSet = TheProductionProjectClass.FindProductionProjectByProjectID(MainWindow.gintProjectID);

                MainWindow.gintTransactionID = TheFindProductionProjectByProjectIDDataSet.FindProductionProjectByProjectID[0].TransactionID;

                blnFatalError = TheProductionProjectClass.UpdateProductionProjectStatusDate(MainWindow.gintTransactionID, DateTime.Now);

                ResetControls();

            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Add Outage Project // expProcess_Expanded " + Ex.ToString());
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Add Outage Project // expProcess_Expanded " + Ex.ToString());
                TheMessagesClass.ErrorMessage(Ex.ToString());
            }            
        }
        private void AddDocuments(int intEmployeeID)
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

                            blnFatalError = TheProductionProjectClass.InsertProductionProjectDocumentation(MainWindow.gintProjectID, intEmployeeID, DateTime.Now, strDocumentPath);

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Add Project // Add Documents Method " + Ex.Message);

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Add Project // Add Documents Method " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
