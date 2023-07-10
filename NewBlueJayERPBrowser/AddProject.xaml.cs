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
using DesignProjectsDLL;
using NewEmployeeDLL;
using DepartmentDLL;
using WorkOrderDLL;
using EmployeeDateEntryDLL;
using ProjectMatrixDLL;
using System.Runtime.Serialization;
using ProjectNumberAssignmentDLL;
using ATTProjectNumberAssignmentDLL;
using JobTypeDLL;
using OutageProjectDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for AddProject.xaml
    /// </summary>
    public partial class AddProject : Page
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ProjectClass TheProjectClass = new ProjectClass();
        ProductionProjectClass TheProductionProjectClass = new ProductionProjectClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DesignProjectsClass TheDesignProjectsClass = new DesignProjectsClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DepartmentClass TheDepartmentClass = new DepartmentClass();
        WorkOrderClass TheWorkOrderClass = new WorkOrderClass();
        EmployeeDateEntryClass TheEmployeeDataEntryClass = new EmployeeDateEntryClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();
        ProjectNumberAssignment TheProjectNumberAssignmentClass = new ProjectNumberAssignment();
        ATTProjectNumberAssignmentClass TheATTProjectNumberAssignmentClass = new ATTProjectNumberAssignmentClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        JobTypeClass TheJobTypeClass = new JobTypeClass();
        OutageProjectClass TheOutageProjectClass = new OutageProjectClass();

        FindProductionProjectInfoDataSet TheFindProductionProjectInfoDataSet = new FindProductionProjectInfoDataSet();
        FindSortedJobTypeDataSet TheFindSortedJobTypeDataSet = new FindSortedJobTypeDataSet();
        FindDesignProjectsByAssignedProjectIDDataSet TheFindDesignProjectsbyAssignedProjectIDDataSet = new FindDesignProjectsByAssignedProjectIDDataSet();
        FindProductionManagersDataSet TheFindProductionManagersDataSet = new FindProductionManagersDataSet();
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindSortedCustomerLinesDataSet TheFindSortedCustomerLinesDataSet = new FindSortedCustomerLinesDataSet();
        FindWorkOrderStatusSortedDataSet TheFindWorkOrderStatusSortedDataSet = new FindWorkOrderStatusSortedDataSet();
        FindProjectMatrixByCustomerProjectIDDataSet TheFindProjectMatrixByCustomerProjectIDDataSet = new FindProjectMatrixByCustomerProjectIDDataSet();
        FindProjectMatrixByAssignedProjectIDDataSet TheFindProjectMatrxiByAssignedProjectIDDataSet = new FindProjectMatrixByAssignedProjectIDDataSet();
        FindProjectByProjectIDDataSet TheFindProjectByProjectIDDataSet = new FindProjectByProjectIDDataSet();
        FindProjectByAssignedProjectIDDataSet TheFindProjectByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();
        FindProjectMatrixByProjectIDDataSet TheFindProjectMatrixByProjectIDDataSet = new FindProjectMatrixByProjectIDDataSet();
        FindProductionProjectByProjectIDDataSet TheFindProductionProjectByProjectIDDataSet = new FindProductionProjectByProjectIDDataSet();
        FindProdutionProjectsByAssignedProjectIDDataSet TheFindProductionProjectByAssignedProjectIDDataSet = new FindProdutionProjectsByAssignedProjectIDDataSet();

        public AddProject()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void ResetControls()
        {
            try
            {

            }
            catch (Exception Ex)
            {

            }
        }
    }
}
