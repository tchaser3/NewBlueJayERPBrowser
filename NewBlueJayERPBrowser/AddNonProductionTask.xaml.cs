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
            ResetControls();
        }
        private void ResetControls()
        {
            txtEnterProductivityTask.Text = "";

            cboSelectProductivityTask.Items.Clear();
            cboSelectProductivityTask.Items.Add("Select Work Task");

            txtEnterProductivityTask.IsEnabled = false;
            cboSelectProductivityTask.IsEnabled = false;
            expProcess.IsEnabled = false;

            TheFindSortedNonProductionTaskDataSet = TheNonProductionProductivityClass.FindSortedNonProductionTask();

            dgrNonProductivityTasks.ItemsSource = TheFindSortedNonProductionTaskDataSet.FindSortedNonProductionTask;

        }

        private void expResetWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expResetWindow.IsExpanded = false;
            ResetControls();
        }

        private void expAddProductivity_Expanded(object sender, RoutedEventArgs e)
        {
            expAddProductivity.IsExpanded = false;
            cboSelectProductivityTask.IsEnabled = false;
            txtEnterProductivityTask.IsEnabled = true;
            gblnAddRecord = true;
        }
    }
}
