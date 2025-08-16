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
using DateSearchDLL;
using EmployeeDateEntryDLL;
using DepartmentDLL;
using WorkOrderDLL;
using ProductionProjectDLL;
using ProjectMatrixDLL;
using Microsoft.Win32;
using ProductionProjectUpdatesDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for ProjectManagementPage.xaml
    /// </summary>
    public partial class ProjectManagementPage : Page
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();
        WorkOrderClass TheWorkOrderClass = new WorkOrderClass();
        ProductionProjectClass TheProductionProjectClass = new ProductionProjectClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        ProductionProjectUpdatesClass TheProductionProjectUpdatesClass = new ProductionProjectUpdatesClass();

        //setting up non public datasets
        FindWorkOrderStatusSortedDataSet TheFindWorkOrderStatustSortedDataSet = new FindWorkOrderStatusSortedDataSet();
        FindSortedCustomerLinesDataSet TheFindSortedCustomerLinesDataSet = new FindSortedCustomerLinesDataSet();
        FindProductionProjectsNotUpdatedDataSet TheFindProductionProjectsNotUpdatedDataSet = new FindProductionProjectsNotUpdatedDataSet();
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindEmployeeByLastNameDataSet TheFindEmployeeByLastNameDataSet = new FindEmployeeByLastNameDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindProductionProjectsForDepartmentDataSet TheFindProductionProjectsForDepartmentDataSet = new FindProductionProjectsForDepartmentDataSet();
        FindProductionProjectsByDepartmentStatusDataSet TheFindProductionProjectsByDepartmentStatusDataSet = new FindProductionProjectsByDepartmentStatusDataSet();
        FindProductionProjectsByDepartmentOfficeDataSet TheFindProductionProjectsByDepartmentOfficeDataSet = new FindProductionProjectsByDepartmentOfficeDataSet();
        FindProductionProjectsbyDepartmentStatusOfficeDataSet TheFindProductionProjectsByDepartmentOfficeStatusDataSet = new FindProductionProjectsbyDepartmentStatusOfficeDataSet();
        FindProjectMatrixByProjectIDDataSet TheFindProjectMatrixByProjectIDDataSet = new FindProjectMatrixByProjectIDDataSet();
        ProjectManagementProjectsDataSet TheProjectManagemenProjectsDataSet = new ProjectManagementProjectsDataSet();
        FindProjectMatrixByCustomerAssignedIDDataSet TheFindProjectMatrixByCustomerAssignedIDDataSet = new FindProjectMatrixByCustomerAssignedIDDataSet();

        FindLastProductionProjectUpdateForProjectDataSet TheFindLastProductionProjectUpdateForProjectDataSet = new FindLastProductionProjectUpdateForProjectDataSet();
        FindProductionProjectInfoPOAmountDataSet TheFindProductionProjectInfoPOAmountDataSet = new FindProductionProjectInfoPOAmountDataSet();
        FindOpenProductionProjectUndergroundDataSet TheFindOpenProductionProjectUndergroundDataSet = new FindOpenProductionProjectUndergroundDataSet();

        //setting up public variables
        public static VerifyLogonDataSet TheVerifyLogonDataSet = new VerifyLogonDataSet();
        public ProjectManagementPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //setting variables
            int intCounter;
            int intNumberOfRecords;

            try
            {
                expExportToExcel.IsEnabled = false;

                TheFindSortedCustomerLinesDataSet = TheDepartmentClass.FindSortedCustomerLines();
                TheFindWorkOrderStatustSortedDataSet = TheWorkOrderClass.FindWorkOrderStatusSorted();
                TheProjectManagemenProjectsDataSet.projectmanagementprojects.Rows.Clear();

                intNumberOfRecords = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines.Rows.Count;

                cboSelectDepartment.Items.Clear();
                cboSelectDepartment.Items.Add("Select Department");

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectDepartment.Items.Add(TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intCounter].Department);
                }

                cboSelectDepartment.SelectedIndex = 0;

                intNumberOfRecords = TheFindWorkOrderStatustSortedDataSet.FindWorkOrderStatusSorted.Rows.Count;
                cboSelectStatus.Items.Clear();
                cboSelectStatus.Items.Add("Select Work Order Status");

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectStatus.Items.Add(TheFindWorkOrderStatustSortedDataSet.FindWorkOrderStatusSorted[intCounter].WorkOrderStatus);
                }

                cboSelectStatus.SelectedIndex = 0;

                TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

                intNumberOfRecords = TheFindWarehousesDataSet.FindWarehouses.Rows.Count;
                cboSelectOffice.Items.Clear();
                cboSelectOffice.Items.Add("Select Office");

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboSelectOffice.Items.Add(TheFindWarehousesDataSet.FindWarehouses[intCounter].FirstName);
                }

                cboSelectOffice.SelectedIndex = 0;

                TheFindProductionProjectsNotUpdatedDataSet = TheProductionProjectClass.FindProductionProjectsNotUpdated();

                dgrResults.ItemsSource = TheFindProductionProjectsNotUpdatedDataSet.FindProductionProjectsNotUpdated;

            }
            catch (Exception Ex)
            {

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Project Management Page // Window Loaded " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());

            }
        }
        private void LoadDepartmentOnlyProjects()
        {
            int intSelectedIndex;
            int intCounter;
            int intNumberOfRecords;
            int intProjectID;
            string strProjectUpdate;
            decimal decPOAmount;

            try
            {

                intSelectedIndex = cboSelectDepartment.SelectedIndex - 1;
                cboSelectOffice.SelectedIndex = 0;
                cboSelectStatus.SelectedIndex = 0;
                TheProjectManagemenProjectsDataSet.projectmanagementprojects.Rows.Clear();
                //txtEnterCustomerID.Text = "";

                if (intSelectedIndex > -1)
                {
                    expExportToExcel.IsEnabled = true;
                    MainWindow.gintDepartmentID = TheFindSortedCustomerLinesDataSet.FindSortedCustomerLines[intSelectedIndex].DepartmentID;

                    if (MainWindow.gintDepartmentID == 1002)
                    {
                        TheFindOpenProductionProjectUndergroundDataSet = TheProductionProjectClass.FindOpenProductionProjectUnderground();

                        intNumberOfRecords = TheFindOpenProductionProjectUndergroundDataSet.FindOpenProductionProjectUnderground.Rows.Count;

                        if (intNumberOfRecords > 1)
                        {
                            for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                            {
                                ProjectManagementProjectsDataSet.projectmanagementprojectsRow NewProjectRow = TheProjectManagemenProjectsDataSet.projectmanagementprojects.NewprojectmanagementprojectsRow();

                                strProjectUpdate = TheFindOpenProductionProjectUndergroundDataSet.FindOpenProductionProjectUnderground[intCounter].ProjectNotes;

                                intProjectID = TheFindOpenProductionProjectUndergroundDataSet.FindOpenProductionProjectUnderground[intCounter].ProjectID;

                                TheFindLastProductionProjectUpdateForProjectDataSet = TheProductionProjectUpdatesClass.FindLastProductionProjectUpdateForProject(intProjectID);

                                if (TheFindLastProductionProjectUpdateForProjectDataSet.FindLastProductionProjectUpdateForProject.Rows.Count > 0)
                                {
                                    strProjectUpdate = TheFindLastProductionProjectUpdateForProjectDataSet.FindLastProductionProjectUpdateForProject[0].ProjectUpdate;
                                }

                                NewProjectRow.AssignedOffice = TheFindOpenProductionProjectUndergroundDataSet.FindOpenProductionProjectUnderground[intCounter].AssignedOffice;
                                NewProjectRow.AssignedProjectID = TheFindOpenProductionProjectUndergroundDataSet.FindOpenProductionProjectUnderground[intCounter].AssignedProjectID;
                                NewProjectRow.BusinessAddress = TheFindOpenProductionProjectUndergroundDataSet.FindOpenProductionProjectUnderground[intCounter].BusinessAddress;
                                NewProjectRow.CustomerAssignedID = TheFindOpenProductionProjectUndergroundDataSet.FindOpenProductionProjectUnderground[intCounter].CustomerAssignedID;
                                NewProjectRow.DateReceived = TheFindOpenProductionProjectUndergroundDataSet.FindOpenProductionProjectUnderground[intCounter].DateReceived;
                                NewProjectRow.ECDDate = TheFindOpenProductionProjectUndergroundDataSet.FindOpenProductionProjectUnderground[intCounter].ECDDate;
                                NewProjectRow.ProjectID = intProjectID;
                                NewProjectRow.ProjectName = TheFindOpenProductionProjectUndergroundDataSet.FindOpenProductionProjectUnderground[intCounter].ProjectName;
                                NewProjectRow.ProjectNotes = strProjectUpdate;
                                NewProjectRow.WorkOrderStatus = TheFindOpenProductionProjectUndergroundDataSet.FindOpenProductionProjectUnderground[intCounter].WorkOrderStatus;
                                NewProjectRow.POAmount = TheFindOpenProductionProjectUndergroundDataSet.FindOpenProductionProjectUnderground[intCounter].POAmount;

                                TheProjectManagemenProjectsDataSet.projectmanagementprojects.Rows.Add(NewProjectRow);
                            }
                        }
                    }
                    else
                    {
                        TheFindProductionProjectsForDepartmentDataSet = TheProductionProjectClass.FindProductionProjectsForDepartment(MainWindow.gintDepartmentID);

                        intNumberOfRecords = TheFindProductionProjectsForDepartmentDataSet.FindProductionProjectsForDepartment.Rows.Count;

                        if (intNumberOfRecords > 0)
                        {


                            for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                            {
                                intProjectID = TheFindProductionProjectsForDepartmentDataSet.FindProductionProjectsForDepartment[intCounter].ProjectID;
                                TheFindLastProductionProjectUpdateForProjectDataSet = TheProductionProjectUpdatesClass.FindLastProductionProjectUpdateForProject(intProjectID);

                                if (TheFindLastProductionProjectUpdateForProjectDataSet.FindLastProductionProjectUpdateForProject.Rows.Count > 0)
                                {
                                    strProjectUpdate = TheFindLastProductionProjectUpdateForProjectDataSet.FindLastProductionProjectUpdateForProject[0].ProjectUpdate;
                                }
                                else
                                {
                                    strProjectUpdate = "NO NOTES FOUND";
                                }

                                TheFindProductionProjectInfoPOAmountDataSet = TheProductionProjectClass.FindProductionProjectInfoPOAmount(intProjectID);

                                if (TheFindProductionProjectInfoPOAmountDataSet.FindProductionProjectInfoPOAmount.Rows.Count > 0)
                                {
                                    decPOAmount = TheFindProductionProjectInfoPOAmountDataSet.FindProductionProjectInfoPOAmount[0].POAmount;
                                }
                                else
                                {
                                    decPOAmount = 0;
                                }

                                ProjectManagementProjectsDataSet.projectmanagementprojectsRow NewProjectRow = TheProjectManagemenProjectsDataSet.projectmanagementprojects.NewprojectmanagementprojectsRow();

                                NewProjectRow.AssignedOffice = TheFindProductionProjectsForDepartmentDataSet.FindProductionProjectsForDepartment[intCounter].AssignedOffice;
                                NewProjectRow.AssignedProjectID = TheFindProductionProjectsForDepartmentDataSet.FindProductionProjectsForDepartment[intCounter].AssignedProjectID;
                                NewProjectRow.BusinessAddress = TheFindProductionProjectsForDepartmentDataSet.FindProductionProjectsForDepartment[intCounter].BusinessAddress;
                                NewProjectRow.CustomerAssignedID = TheFindProductionProjectsForDepartmentDataSet.FindProductionProjectsForDepartment[intCounter].CustomerAssignedID;
                                NewProjectRow.DateReceived = TheFindProductionProjectsForDepartmentDataSet.FindProductionProjectsForDepartment[intCounter].DateReceived;
                                NewProjectRow.ECDDate = TheFindProductionProjectsForDepartmentDataSet.FindProductionProjectsForDepartment[intCounter].ECDDate;
                                NewProjectRow.ProjectID = TheFindProductionProjectsForDepartmentDataSet.FindProductionProjectsForDepartment[intCounter].ProjectID;
                                NewProjectRow.ProjectName = TheFindProductionProjectsForDepartmentDataSet.FindProductionProjectsForDepartment[intCounter].ProjectName;
                                NewProjectRow.ProjectNotes = strProjectUpdate;
                                NewProjectRow.WorkOrderStatus = TheFindProductionProjectsForDepartmentDataSet.FindProductionProjectsForDepartment[intCounter].WorkOrderStatus;
                                NewProjectRow.POAmount = decPOAmount;

                                TheProjectManagemenProjectsDataSet.projectmanagementprojects.Rows.Add(NewProjectRow);
                            }
                        }
                    }



                    dgrResults.ItemsSource = TheProjectManagemenProjectsDataSet.projectmanagementprojects;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Project Management Page // Load Department Only Projects " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Project Management Page // Load Department Only Projects " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadDepartmentOnlyProjects();
        }
        private void LoadDepartmentStatusProjects()
        {
            int intSelectedIndex;
            int intCounter;
            int intNumberOfRecords;
            int intProjectID;
            string strProjectUpdate;
            decimal decPOAmount;


            try
            {
                //txtEnterCustomerID.Text = "";
                TheProjectManagemenProjectsDataSet.projectmanagementprojects.Rows.Clear();

                cboSelectOffice.SelectedIndex = 0;

                if ((cboSelectDepartment.SelectedIndex < 1) && (cboSelectStatus.SelectedIndex > 1))
                {
                    TheMessagesClass.ErrorMessage("The Department Was Not Selected");

                    return;
                }

                intSelectedIndex = cboSelectStatus.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    MainWindow.gintStatusID = TheFindWorkOrderStatustSortedDataSet.FindWorkOrderStatusSorted[intSelectedIndex].StatusID;

                    TheFindProductionProjectsByDepartmentStatusDataSet = TheProductionProjectClass.FindProductionProjectsByDepartmentStatus(MainWindow.gintDepartmentID, MainWindow.gintStatusID);

                    intNumberOfRecords = TheFindProductionProjectsByDepartmentStatusDataSet.FindProductionProjectsByDepartmentStatus.Rows.Count;

                    if (intNumberOfRecords > 0)
                    {
                        for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            intProjectID = TheFindProductionProjectsByDepartmentStatusDataSet.FindProductionProjectsByDepartmentStatus[intCounter].ProjectID;

                            TheFindLastProductionProjectUpdateForProjectDataSet = TheProductionProjectUpdatesClass.FindLastProductionProjectUpdateForProject(intProjectID);

                            if (TheFindLastProductionProjectUpdateForProjectDataSet.FindLastProductionProjectUpdateForProject.Rows.Count > 0)
                            {
                                strProjectUpdate = TheFindLastProductionProjectUpdateForProjectDataSet.FindLastProductionProjectUpdateForProject[0].ProjectUpdate;
                            }
                            else
                            {
                                strProjectUpdate = "NO NOTES FOUND";
                            }

                            TheFindProductionProjectInfoPOAmountDataSet = TheProductionProjectClass.FindProductionProjectInfoPOAmount(intProjectID);

                            if (TheFindProductionProjectInfoPOAmountDataSet.FindProductionProjectInfoPOAmount.Rows.Count > 0)
                            {
                                decPOAmount = TheFindProductionProjectInfoPOAmountDataSet.FindProductionProjectInfoPOAmount[0].POAmount;
                            }
                            else
                            {
                                decPOAmount = 0;
                            }

                            ProjectManagementProjectsDataSet.projectmanagementprojectsRow NewProjectRow = TheProjectManagemenProjectsDataSet.projectmanagementprojects.NewprojectmanagementprojectsRow();

                            NewProjectRow.AssignedOffice = TheFindProductionProjectsByDepartmentStatusDataSet.FindProductionProjectsByDepartmentStatus[intCounter].AssignedOffice;
                            NewProjectRow.AssignedProjectID = TheFindProductionProjectsByDepartmentStatusDataSet.FindProductionProjectsByDepartmentStatus[intCounter].AssignedProjectID;
                            NewProjectRow.BusinessAddress = TheFindProductionProjectsByDepartmentStatusDataSet.FindProductionProjectsByDepartmentStatus[intCounter].BusinessAddress;
                            NewProjectRow.CustomerAssignedID = TheFindProductionProjectsByDepartmentStatusDataSet.FindProductionProjectsByDepartmentStatus[intCounter].CustomerAssignedID;
                            NewProjectRow.DateReceived = TheFindProductionProjectsByDepartmentStatusDataSet.FindProductionProjectsByDepartmentStatus[intCounter].DateReceived;
                            NewProjectRow.ECDDate = TheFindProductionProjectsByDepartmentStatusDataSet.FindProductionProjectsByDepartmentStatus[intCounter].ECDDate;
                            NewProjectRow.ProjectID = TheFindProductionProjectsByDepartmentStatusDataSet.FindProductionProjectsByDepartmentStatus[intCounter].ProjectID;
                            NewProjectRow.ProjectName = TheFindProductionProjectsByDepartmentStatusDataSet.FindProductionProjectsByDepartmentStatus[intCounter].ProjectName;
                            NewProjectRow.ProjectNotes = strProjectUpdate;
                            NewProjectRow.WorkOrderStatus = TheFindProductionProjectsByDepartmentStatusDataSet.FindProductionProjectsByDepartmentStatus[intCounter].WorkOrderStatus;
                            NewProjectRow.POAmount = decPOAmount;

                            TheProjectManagemenProjectsDataSet.projectmanagementprojects.Rows.Add(NewProjectRow);
                        }
                    }

                    dgrResults.ItemsSource = TheProjectManagemenProjectsDataSet.projectmanagementprojects;

                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Project Management Page // Load Department Status Projects " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Project Management Page // Load Department Status Projects " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void LoadDepartmentOfficeStatusProjects()
        {
            int intSelectedIndex;
            int intCounter;
            int intNumberOfRecords;
            int intProjectID;
            string strProjectUpdate;
            decimal decPOAmount;

            try
            {
                //txtEnterCustomerID.Text = "";
                TheProjectManagemenProjectsDataSet.projectmanagementprojects.Rows.Clear();

                if ((cboSelectDepartment.SelectedIndex < 1) && (cboSelectOffice.SelectedIndex > 1))
                {
                    TheMessagesClass.ErrorMessage("The Department Was Not Selected");

                    return;
                }

                intSelectedIndex = cboSelectOffice.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    MainWindow.gintOfficeID = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;

                    TheFindProductionProjectsByDepartmentOfficeStatusDataSet = TheProductionProjectClass.FindProductionProjectsByDepartmentStatusOffice(MainWindow.gintDepartmentID, MainWindow.gintStatusID, MainWindow.gintOfficeID);

                    intNumberOfRecords = TheFindProductionProjectsByDepartmentOfficeStatusDataSet.FindProductionProjectsByDepartmentStatusOffice.Rows.Count;

                    if (intNumberOfRecords > 0)
                    {
                        for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            intProjectID = TheFindProductionProjectsByDepartmentOfficeStatusDataSet.FindProductionProjectsByDepartmentStatusOffice[intCounter].ProjectID;

                            TheFindLastProductionProjectUpdateForProjectDataSet = TheProductionProjectUpdatesClass.FindLastProductionProjectUpdateForProject(intProjectID);

                            if (TheFindLastProductionProjectUpdateForProjectDataSet.FindLastProductionProjectUpdateForProject.Rows.Count > 0)
                            {
                                strProjectUpdate = TheFindLastProductionProjectUpdateForProjectDataSet.FindLastProductionProjectUpdateForProject[0].ProjectUpdate;
                            }
                            else
                            {
                                strProjectUpdate = "NO NOTES FOUND";
                            }

                            TheFindProductionProjectInfoPOAmountDataSet = TheProductionProjectClass.FindProductionProjectInfoPOAmount(intProjectID);

                            if (TheFindProductionProjectInfoPOAmountDataSet.FindProductionProjectInfoPOAmount.Rows.Count > 0)
                            {
                                decPOAmount = TheFindProductionProjectInfoPOAmountDataSet.FindProductionProjectInfoPOAmount[0].POAmount;
                            }
                            else
                            {
                                decPOAmount = 0;
                            }

                            ProjectManagementProjectsDataSet.projectmanagementprojectsRow NewProjectRow = TheProjectManagemenProjectsDataSet.projectmanagementprojects.NewprojectmanagementprojectsRow();

                            NewProjectRow.AssignedOffice = TheFindProductionProjectsByDepartmentOfficeStatusDataSet.FindProductionProjectsByDepartmentStatusOffice[intCounter].AssignedOffice;
                            NewProjectRow.AssignedProjectID = TheFindProductionProjectsByDepartmentOfficeStatusDataSet.FindProductionProjectsByDepartmentStatusOffice[intCounter].AssignedProjectID;
                            NewProjectRow.BusinessAddress = TheFindProductionProjectsByDepartmentOfficeStatusDataSet.FindProductionProjectsByDepartmentStatusOffice[intCounter].BusinessAddress;
                            NewProjectRow.CustomerAssignedID = TheFindProductionProjectsByDepartmentOfficeStatusDataSet.FindProductionProjectsByDepartmentStatusOffice[intCounter].CustomerAssignedID;
                            NewProjectRow.DateReceived = TheFindProductionProjectsByDepartmentOfficeStatusDataSet.FindProductionProjectsByDepartmentStatusOffice[intCounter].DateReceived;
                            NewProjectRow.ECDDate = TheFindProductionProjectsByDepartmentOfficeStatusDataSet.FindProductionProjectsByDepartmentStatusOffice[intCounter].ECDDate;
                            NewProjectRow.ProjectID = TheFindProductionProjectsByDepartmentOfficeStatusDataSet.FindProductionProjectsByDepartmentStatusOffice[intCounter].ProjectID;
                            NewProjectRow.ProjectName = TheFindProductionProjectsByDepartmentOfficeStatusDataSet.FindProductionProjectsByDepartmentStatusOffice[intCounter].ProjectName;
                            NewProjectRow.ProjectNotes = strProjectUpdate;
                            NewProjectRow.POAmount = decPOAmount;
                            NewProjectRow.WorkOrderStatus = TheFindProductionProjectsByDepartmentOfficeStatusDataSet.FindProductionProjectsByDepartmentStatusOffice[intCounter].WorkOrderStatus;

                            TheProjectManagemenProjectsDataSet.projectmanagementprojects.Rows.Add(NewProjectRow);
                        }
                    }

                    dgrResults.ItemsSource = TheProjectManagemenProjectsDataSet.projectmanagementprojects;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Project Management Page // Load Department Office Status Projects " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Project Management Page // Load Department Office Status Projects " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void LoadDepartmentOfficeProjects()
        {
            int intSelectedIndex;
            int intCounter;
            int intNumberOfRecords;
            int intProjectID;
            string strProjectUpdate;
            decimal decPOAmount;

            try
            {
                //txtEnterCustomerID.Text = "";
                TheProjectManagemenProjectsDataSet.projectmanagementprojects.Rows.Clear();

                if ((cboSelectDepartment.SelectedIndex < 1) && (cboSelectOffice.SelectedIndex > 1))
                {
                    TheMessagesClass.ErrorMessage("The Department Was Not Selected");

                    return;
                }

                intSelectedIndex = cboSelectOffice.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    MainWindow.gintOfficeID = TheFindWarehousesDataSet.FindWarehouses[intSelectedIndex].EmployeeID;

                    TheFindProductionProjectsByDepartmentOfficeDataSet = TheProductionProjectClass.FindProductionProjectsByDepartmentOffice(MainWindow.gintDepartmentID, MainWindow.gintOfficeID);

                    intNumberOfRecords = TheFindProductionProjectsByDepartmentOfficeDataSet.FindProductionProjectsByDepartmentOffice.Rows.Count;

                    if (intNumberOfRecords > 0)
                    {
                        for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            intProjectID = TheFindProductionProjectsByDepartmentOfficeDataSet.FindProductionProjectsByDepartmentOffice[intCounter].ProjectID;

                            TheFindLastProductionProjectUpdateForProjectDataSet = TheProductionProjectUpdatesClass.FindLastProductionProjectUpdateForProject(intProjectID);

                            if (TheFindLastProductionProjectUpdateForProjectDataSet.FindLastProductionProjectUpdateForProject.Rows.Count > 0)
                            {
                                strProjectUpdate = TheFindLastProductionProjectUpdateForProjectDataSet.FindLastProductionProjectUpdateForProject[0].ProjectUpdate;
                            }
                            else
                            {
                                strProjectUpdate = "NO NOTES FOUND";
                            }

                            TheFindProductionProjectInfoPOAmountDataSet = TheProductionProjectClass.FindProductionProjectInfoPOAmount(intProjectID);

                            if (TheFindProductionProjectInfoPOAmountDataSet.FindProductionProjectInfoPOAmount.Rows.Count > 0)
                            {
                                decPOAmount = TheFindProductionProjectInfoPOAmountDataSet.FindProductionProjectInfoPOAmount[0].POAmount;
                            }
                            else
                            {
                                decPOAmount = 0;
                            }

                            ProjectManagementProjectsDataSet.projectmanagementprojectsRow NewProjectRow = TheProjectManagemenProjectsDataSet.projectmanagementprojects.NewprojectmanagementprojectsRow();

                            NewProjectRow.AssignedOffice = TheFindProductionProjectsByDepartmentOfficeDataSet.FindProductionProjectsByDepartmentOffice[intCounter].AssignedOffice;
                            NewProjectRow.AssignedProjectID = TheFindProductionProjectsByDepartmentOfficeDataSet.FindProductionProjectsByDepartmentOffice[intCounter].AssignedProjectID;
                            NewProjectRow.BusinessAddress = TheFindProductionProjectsByDepartmentOfficeDataSet.FindProductionProjectsByDepartmentOffice[intCounter].BusinessAddress;
                            NewProjectRow.CustomerAssignedID = TheFindProductionProjectsByDepartmentOfficeDataSet.FindProductionProjectsByDepartmentOffice[intCounter].CustomerAssignedID;
                            NewProjectRow.DateReceived = TheFindProductionProjectsByDepartmentOfficeDataSet.FindProductionProjectsByDepartmentOffice[intCounter].DateReceived;
                            NewProjectRow.ECDDate = TheFindProductionProjectsByDepartmentOfficeDataSet.FindProductionProjectsByDepartmentOffice[intCounter].ECDDate;
                            NewProjectRow.ProjectID = TheFindProductionProjectsByDepartmentOfficeDataSet.FindProductionProjectsByDepartmentOffice[intCounter].ProjectID;
                            NewProjectRow.ProjectName = TheFindProductionProjectsByDepartmentOfficeDataSet.FindProductionProjectsByDepartmentOffice[intCounter].ProjectName;
                            NewProjectRow.ProjectNotes = strProjectUpdate;
                            NewProjectRow.POAmount = decPOAmount;
                            NewProjectRow.WorkOrderStatus = TheFindProductionProjectsByDepartmentOfficeDataSet.FindProductionProjectsByDepartmentOffice[intCounter].WorkOrderStatus;

                            TheProjectManagemenProjectsDataSet.projectmanagementprojects.Rows.Add(NewProjectRow);
                        }
                    }

                    dgrResults.ItemsSource = TheProjectManagemenProjectsDataSet.projectmanagementprojects;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Project Management Page // Load Department Office Projects " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Project Management Page // Load Department Office Projects " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSelectStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadDepartmentStatusProjects();
        }

        private void cboSelectOffice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboSelectStatus.SelectedIndex < 1)
            {
                LoadDepartmentOfficeProjects();
            }
            else if (cboSelectStatus.SelectedIndex > 0)
            {
                LoadDepartmentOfficeStatusProjects();
            }
        }

        private void dgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell ProjectID;
            string strProjectID;

            try
            {
                if ((cboSelectDepartment.SelectedIndex > 0) && (dgrResults.SelectedIndex > -1))
                {
                    //setting local variable
                    dataGrid = dgrResults;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    ProjectID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                    strProjectID = ((TextBlock)ProjectID.Content).Text;

                    //find the record
                    MainWindow.gintProjectID = Convert.ToInt32(strProjectID);

                    TheFindProjectMatrixByProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByProjectID(MainWindow.gintProjectID);

                    MainWindow.gstrCustomerProjectID = TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].CustomerAssignedID;

                    EditOutageProject editOutageProject = new EditOutageProject();
                    editOutageProject.ShowDialog();


                    if ((cboSelectDepartment.SelectedIndex > 0) && (cboSelectOffice.SelectedIndex < 1) && (cboSelectStatus.SelectedIndex < 1))
                    {
                        LoadDepartmentOnlyProjects();
                    }
                    else if ((cboSelectDepartment.SelectedIndex > 0) && (cboSelectOffice.SelectedIndex < 1) && (cboSelectStatus.SelectedIndex > 0))
                    {
                        LoadDepartmentStatusProjects();
                    }
                    else if ((cboSelectDepartment.SelectedIndex > 0) && (cboSelectOffice.SelectedIndex > 0) && (cboSelectStatus.SelectedIndex < 1))
                    {
                        LoadDepartmentOfficeProjects();
                    }
                    else if ((cboSelectDepartment.SelectedIndex > 0) && (cboSelectOffice.SelectedIndex > 0) && (cboSelectStatus.SelectedIndex > 0))
                    {
                        LoadDepartmentOfficeStatusProjects();
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Project Management Page // Data Grid Selection " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Project Management Page // Data Grid Selection " + Ex.ToString());

                //TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void dgrResults_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DateTime datECDDate = DateTime.Now;
            DateTime datCurrentDate = DateTime.Now;
            DateTime datWarningDate = DateTime.Now;
            string strProjectStatus;
            int intReminder;
            int intIndex;
            int intColumn;
            string strECDDate;


            try
            {
                if (cboSelectDepartment.SelectedIndex > 0)
                {
                    intIndex = e.Row.GetIndex();

                    datECDDate = TheProjectManagemenProjectsDataSet.projectmanagementprojects[intIndex].ECDDate;
                    strProjectStatus = TheProjectManagemenProjectsDataSet.projectmanagementprojects[intIndex].WorkOrderStatus;

                    datWarningDate = TheDateSearchClass.RemoveTime(datWarningDate);
                    datWarningDate = TheDateSearchClass.AddingDays(datWarningDate, 4);
                    datCurrentDate = TheDateSearchClass.RemoveTime(datCurrentDate);

                    //intReminder = intIndex % 2;

                    if ((datECDDate < datCurrentDate) && ((strProjectStatus != "CLOSED") && (strProjectStatus != "CANCEL")))
                    {

                        e.Row.Background = Brushes.Red;
                        e.Row.Foreground = Brushes.White;
                    }
                    else if ((datECDDate < datWarningDate) && (datECDDate > datCurrentDate) && ((strProjectStatus != "CLOSED") && (strProjectStatus != "CANCEL")))
                    {
                        e.Row.Background = Brushes.Yellow;
                        e.Row.Foreground = Brushes.Black;
                    }
                    else if ((datECDDate > datCurrentDate) && ((strProjectStatus != "CLOSED") && (strProjectStatus != "CANCEL")))
                    {
                        e.Row.Background = Brushes.Green;
                        e.Row.Foreground = Brushes.White;
                    }
                }
            }
            catch (Exception Ex)
            {
                if (Ex.Message.Contains("There is no row at position"))
                {

                }
                else
                {
                    TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Project Management Page // Data Grid Loading Row " + Ex.ToString());

                    TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Project Management Page // Data Grid Loading Row " + Ex.ToString());

                    //TheMessagesClass.ErrorMessage(Ex.ToString());
                }
            }

        }
    }
    
}
