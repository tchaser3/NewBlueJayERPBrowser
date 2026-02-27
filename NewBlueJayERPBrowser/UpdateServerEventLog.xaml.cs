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

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for UpdateServerEventLog.xaml
    /// </summary>
    public partial class UpdateServerEventLog : Page
    {
        //setting up the classes
        WPFMessagesClass wpfMessagesClass = new WPFMessagesClass();
        EventLogClass eventLogClass = new EventLogClass();
        EmployeeDateEntryClass employeeDateEntryClass = new EmployeeDateEntryClass();
        SendEmailClass sendEmailClass = new SendEmailClass();

        FindServerEventLogForReportsEntryDataSet findServerEventLogForReportsEntryDataSet = new FindServerEventLogForReportsEntryDataSet();
        FindServerEventLogForReportsSpecificDataSet findServerEventLogForReportsSpecificDataSet = new FindServerEventLogForReportsSpecificDataSet();

        public UpdateServerEventLog()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            int intNumberOfRecords;

            PleaseWait pleaseWait = new PleaseWait();
            pleaseWait.Show();

            try
            {
                findServerEventLogForReportsEntryDataSet = eventLogClass.FindServerEventLogForReportsEntry();
                dgrResults.ItemsSource = findServerEventLogForReportsEntryDataSet.FindServerEventLogForReportsEntry;

                employeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "Update Server Event Log");

                intNumberOfRecords = findServerEventLogForReportsEntryDataSet.FindServerEventLogForReportsEntry.Rows.Count;

                if (intNumberOfRecords > 0)
                {
                    expProcess.IsEnabled = true;
                }
                else
                {
                    expProcess.IsEnabled = false;
                }
            }
            catch (Exception Ex)
            {
                eventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Update Server Event Log // Page Loaded " + Ex.ToString());
                sendEmailClass.SendEventLog("New Blue Jay ERP Browser // Update Server Event Log // Page Loaded " + Ex.ToString());
                wpfMessagesClass.ErrorMessage(Ex.ToString());
            }

            pleaseWait.Close(); 

        }

        private void expProcess_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError = false;
            DateTime datTransactionDate;
            string strLogonName;
            string strItemAccessed;
            string strEventNotes;
            int intRecordCount;

            PleaseWait pleaseWait = new PleaseWait();

            try
            {
                pleaseWait.Show();
                intNumberOfRecords = findServerEventLogForReportsEntryDataSet.FindServerEventLogForReportsEntry.Rows.Count;

                if (intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        datTransactionDate = findServerEventLogForReportsEntryDataSet.FindServerEventLogForReportsEntry[intCounter].TransactionDate;
                        strLogonName = "Just Beginging";
                        strItemAccessed = "Date Goes Here";
                        strEventNotes = findServerEventLogForReportsEntryDataSet.FindServerEventLogForReportsEntry[intCounter].EventNotes;

                        char[] delims = new[] { '\n', '\t', '\r' };
                        string[] strNewItems = strEventNotes.Split(delims, StringSplitOptions.RemoveEmptyEntries);

                        strLogonName = strNewItems[5];

                        if (strNewItems.Length < 16)
                        {
                            strItemAccessed = strNewItems[strNewItems.Length - 1];
                        }
                        else
                        {
                            strItemAccessed = strNewItems[16];
                        }

                        //findServerEventLogForReportsSpecificDataSet = eventLogClass.FindServerEventLogForReportsSpecific(datTransactionDate, strLogonName, strItemAccessed);

                        //intRecordCount = findServerEventLogForReportsSpecificDataSet.FindServerEventLogForReportsSpecific.Rows.Count;

                        //if (intRecordCount == 0)
                        //{
                            blnFatalError = eventLogClass.InsertServerEventLogForReports(datTransactionDate, strLogonName, strItemAccessed);

                            if (blnFatalError == true)
                                throw new Exception();
                        //}
                    }
                }

                pleaseWait.Close();
                
                wpfMessagesClass.InformationMessage("Process Completed");
                ResetControls();
            }
            catch (Exception Ex)
            {
                pleaseWait.Close();
                eventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Update Server Event Log // Page Loaded " + Ex.ToString());
                sendEmailClass.SendEventLog("New Blue Jay ERP Browser // Update Server Event Log // Page Loaded " + Ex.ToString());
                wpfMessagesClass.ErrorMessage(Ex.ToString());
            }

             
        }
    }
}
