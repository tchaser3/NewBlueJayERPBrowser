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
using ProductionProjectUpdatesDLL;
using EmployeeDateEntryDLL;
using DateSearchDLL;
using NewEmployeeDLL;
using WorkOrderDLL;
using ProjectsDLL;
using OutageProjectDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for EditProjectID.xaml
    /// </summary>
    public partial class EditProjectID : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ProductionProjectClass TheProductionProjectClass = new ProductionProjectClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();
        ProductionProjectUpdatesClass TheProductionProjectUpdatesClass = new ProductionProjectUpdatesClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        ProjectClass TheProjectClass = new ProjectClass();
        OutageProjectClass TheOutageProjectClass = new OutageProjectClass();

        //Setting up the data
        FindProjectMatrixByProjectIDDataSet TheFindProjectMatrixByProjectIDDataSet = new FindProjectMatrixByProjectIDDataSet();
        FindProjectByProjectIDDataSet TheFindProjectByProjectIDDataSet = new FindProjectByProjectIDDataSet();
        FindProjectMatrixByCustomerAssignedIDDataSet TheFindProjectMatrixByCustomerAssignedIDDataSet = new FindProjectMatrixByCustomerAssignedIDDataSet();
        FindOutageProjectByProjectIDDataSet TheFindOutageProjectByProjectIDDataSet = new FindOutageProjectByProjectIDDataSet();

        int gintTransactionID;

        public EditProjectID()
        {
            InitializeComponent();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                TheFindProjectMatrixByProjectIDDataSet = TheProjectMatrixClass.FindProjectMatrixByProjectID(MainWindow.gintProjectID);

                gintTransactionID = TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].TransactionID;

                txtOldCustomerID.Text = TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].CustomerAssignedID;
                txtBlueJayProjectID.Text = TheFindProjectMatrixByProjectIDDataSet.FindProjectMatrixByProjectID[0].AssignedProjectID;

                TheFindProjectByProjectIDDataSet = TheProjectClass.FindProjectByProjectID(MainWindow.gintProjectID);
                txtProjectName.Text = TheFindProjectByProjectIDDataSet.FindProjectByProjectID[0].ProjectName;

                System.Threading.Thread.Sleep(1000);
            }
            catch(Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Edit Project ID // Window Loaded " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Edit Project ID // Edit Project ID " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProcess_Expanded(object sender, RoutedEventArgs e)
        {
            string strNewCustomerProjectID;
            string strProjectName;
            int intRecordsReturned;
            bool blnFatalError = false;

            try
            {
                strNewCustomerProjectID = txtNewCustomerID.Text;
                if(strNewCustomerProjectID.Length < 7)
                {
                    TheMessagesClass.ErrorMessage("The Project ID Entered is not Long Enough");
                    return;
                }

                TheFindProjectMatrixByCustomerAssignedIDDataSet = TheProjectMatrixClass.FindProjectMatrixByCustomerAssignedID(strNewCustomerProjectID);

                intRecordsReturned = TheFindProjectMatrixByCustomerAssignedIDDataSet.FindProjectMatrixByCustomerAssignedID.Rows.Count;

                if (intRecordsReturned > 0)
                {
                    TheMessagesClass.ErrorMessage("The Project ID Entered Already Exists");
                    return;
                }

                strProjectName = txtProjectName.Text;

                blnFatalError = TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.gintLoggedInEmployeeID, "New Blue Jay ERP Browser // Editing Project ID From " + txtOldCustomerID + " To " + strNewCustomerProjectID);

                if (blnFatalError == true)
                {
                    throw new Exception();
                }

                blnFatalError = TheProjectClass.UpdateProjectProject(MainWindow.gintProjectID, strNewCustomerProjectID, strProjectName);

                if(blnFatalError == true)
                {
                    throw new Exception();
                }

                blnFatalError = TheProjectMatrixClass.UpdateProjectMatrixCustomerProjectID(gintTransactionID, strNewCustomerProjectID);

                if (blnFatalError == true)
                {
                    throw new Exception();
                }

                TheFindOutageProjectByProjectIDDataSet = TheOutageProjectClass.FindOutageProjectByProjectID(MainWindow.gintProjectID);

                if(TheFindOutageProjectByProjectIDDataSet.FindOutageProjectByProjectID.Rows.Count > 0)
                {
                    blnFatalError = TheOutageProjectClass.DeleteOutageProject(MainWindow.gintProjectID);
                    if (blnFatalError == true)
                    {
                        throw new Exception();
                    }
                }
                MainWindow.gstrCustomerProjectID = strNewCustomerProjectID;
                this.Close();

            }
            catch(Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Edit Project ID // Process Expander " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Edit Project ID // Process Expander " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
