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
        int gintProjectID;
        bool gblnProjectExists;
        bool gblnProjectMatrixExists;
        bool gblnOver2500;
        int gintJobTypeID;
        string gstrOutageStatus;

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

                strCustomerProjectID = Convert.ToString(datProjectID.Year);
                strCustomerProjectID += Convert.ToString(datProjectID.Month);
                strCustomerProjectID += Convert.ToString(datProjectID.Day);
                strCustomerProjectID += Convert.ToString(datProjectID.Hour);
                strCustomerProjectID += Convert.ToString(datProjectID.Minute);
                txtCustomerProjectID.Text = strCustomerProjectID;

                txtAssignedProjectID.Text = Convert.ToString(datProjectID.Year) + "-003";

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
    }
}
