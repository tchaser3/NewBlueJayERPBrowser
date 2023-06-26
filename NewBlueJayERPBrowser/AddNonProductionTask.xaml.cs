/* Title:       Adding Non-Productivity Task
 * Date:        6-19-2023
 * Author:      Terry Holmes */

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
using EmployeeDateEntryDLL;
using NonProductionProductivityDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for AddNonProductionTask.xaml
    /// </summary>
    public partial class AddNonProductionTask : Page
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        NonProductionProductivityClass TheNonProductionProductivityClass = new NonProductionProductivityClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        //setting up the data
        FindSortedNonProductionTaskDataSet TheFindSortedNonProductionTaskDataSet = new FindSortedNonProductionTaskDataSet();
        FindNonProductionTaskByWorkTaskDataSet TheFindNonProductionTaskByWorkTaskDataSet = new FindNonProductionTaskByWorkTaskDataSet();       

        //setting global variables
        bool gblnAddRecord;

        public AddNonProductionTask()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "//New Blue Jay ERP Browser / Add Non-Production Productivity Task ");

            ResetControls();
        }
        private void ResetControls()
        {
            txtEnterProductivityTask.Text = "";
            txtEnterProductivityTask.Focus();

            TheFindSortedNonProductionTaskDataSet = TheNonProductionProductivityClass.FindSortedNonProductionTask();

            dgrNonProductivityTasks.ItemsSource = TheFindSortedNonProductionTaskDataSet.FindSortedNonProductionTask;


        }

        private void expResetWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expResetWindow.IsExpanded = false;
            ResetControls();
        }

        private void expProcess_Expanded(object sender, RoutedEventArgs e)
        {
            string strWorkTask;
            int intRecordsReturned;
            bool blnFatalError = false;

            expProcess.IsExpanded = false;

            try
            {
                strWorkTask = txtEnterProductivityTask.Text;

                if(strWorkTask.Length < 5)
                {
                    TheMessagesClass.ErrorMessage("The Task Description Is Not Long Enough");
                    return;
                }

                TheFindNonProductionTaskByWorkTaskDataSet = TheNonProductionProductivityClass.FindNOnProductionTaskByWorkTask(strWorkTask);

                intRecordsReturned = TheFindNonProductionTaskByWorkTaskDataSet.FindNonProductionTaskByWorkTask.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    TheMessagesClass.ErrorMessage("The Work Task Has Already Been Added");
                    return;
                }

                blnFatalError = TheNonProductionProductivityClass.InsertNonProductionTask(strWorkTask);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Task Has Been Entered");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Add Non-Production Productivity Task // Process Expander " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Brownser // Add Non-Production Productivity Task // Process Expander " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
