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
using OutageProjectDLL;
using NewEventLogDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for AddOutageProjectStatus.xaml
    /// </summary>
    public partial class AddOutageProjectStatus : Page
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        OutageProjectClass TheOutageProjectClass = new OutageProjectClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        //setting up the data
        FindOutageStatusByKeywordDataSet TheFindOutageStatusByKeywordDataSet = new FindOutageStatusByKeywordDataSet();
        FindSortedOutageProjectWorkStatusDataSet TheFindSortedOutageProjectWorkStatusDataSet = new FindSortedOutageProjectWorkStatusDataSet();
        
        public AddOutageProjectStatus()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            int intCounter;
            int intNumberOfRecords;

            txtEnterNewStatus.Text = "";
            cboExistingStatus.Items.Clear();

            cboExistingStatus.Items.Add("Existing Statuses");

            TheFindSortedOutageProjectWorkStatusDataSet = TheOutageProjectClass.FindSortedOutageProjectWorkStatus();

            intNumberOfRecords = TheFindSortedOutageProjectWorkStatusDataSet.FindSortedOutageProjectWorkStatus.Rows.Count;

            if(intNumberOfRecords > 0 )
            {
                for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboExistingStatus.Items.Add(TheFindSortedOutageProjectWorkStatusDataSet.FindSortedOutageProjectWorkStatus[intCounter].OutageWorkStatus);
                }
            }

            cboExistingStatus.SelectedIndex = 0;

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP Browser // Add Outage Project Status");

        }

        private void expResetWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expResetWindow.IsExpanded = false;
            ResetControls();
        }

        private void txtEnterNewStatus_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strNewStatus;
            int intCounter;
            int intNumberOfRecords;

            try
            {
                cboExistingStatus.Items.Clear();
                cboExistingStatus.Items.Add("Existing Statuses");
                strNewStatus = txtEnterNewStatus.Text;

                if(strNewStatus.Length > 3)
                {
                    TheFindOutageStatusByKeywordDataSet = TheOutageProjectClass.FindOutageStatusByKeyword(strNewStatus);

                    intNumberOfRecords = TheFindOutageStatusByKeywordDataSet.FindOutageWorkStatusByKeyword.Rows.Count;

                    if(intNumberOfRecords > 0)
                    {
                        for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                        {
                            cboExistingStatus.Items.Add(TheFindOutageStatusByKeywordDataSet.FindOutageWorkStatusByKeyword[intCounter].OutageWorkStatus);
                        }
                    }
                }

                cboExistingStatus.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Add Outage Project Status // Enter New Status Textbox " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Add Outage Project Status // Enter New Status Textbox " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expProcess_Expanded(object sender, RoutedEventArgs e)
        {
            int intNumberOfRecords;
            string strNewStatus;
            bool blnFatalError = false;

            try
            {
                expProcess.IsExpanded = false;
                strNewStatus = txtEnterNewStatus.Text;

                if(strNewStatus.Length < 4)
                {
                    TheMessagesClass.ErrorMessage("The Outage Project Status Length is to Short");

                    return;
                }

                TheFindOutageStatusByKeywordDataSet = TheOutageProjectClass.FindOutageStatusByKeyword(strNewStatus);

                intNumberOfRecords = TheFindOutageStatusByKeywordDataSet.FindOutageWorkStatusByKeyword.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    TheMessagesClass.ErrorMessage("The Outage Project Status Already Exists");

                    return;
                }

                blnFatalError = TheOutageProjectClass.InsertOutageProjectStatus(MainWindow.gstrUserName, strNewStatus);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Outage Project Status Has Been Entered");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Add Outage Project Status // Process Expander " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Add Outage Project Status // Process Expander " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
