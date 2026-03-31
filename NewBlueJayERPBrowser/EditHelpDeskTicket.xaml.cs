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
using NewEmployeeDLL;
using NewEventLogDLL;
using HelpDeskDLL;
using PhonesDLL;
using System.IO;
using DataValidationDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for EditHelpDeskTicket.xaml
    /// </summary>
    public partial class EditHelpDeskTicket : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        HelpDeskClass TheHelpDeskClass = new HelpDeskClass();
        PhonesClass ThePhoneClass = new PhonesClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        FindHelpDeskTicketInformationByTicketIDDataSet TheFindHelpDeskTicketInfomationByTicketIDDataSet = new FindHelpDeskTicketInformationByTicketIDDataSet();
        FindHelpDeskTicketUpdatesByTicketIDDataSet TheFindHelpDeskTicketUpdatesByTicketIDDataSet = new FindHelpDeskTicketUpdatesByTicketIDDataSet();
        FindSortedHelpDeskProblemTypeDataSet TheFindSortedHelpDeskProblemTypeDataSet = new FindSortedHelpDeskProblemTypeDataSet();
        FindEmployeeByDepartmentDataSet TheFindEmployeeByDepartmentDataSet = new FindEmployeeByDepartmentDataSet();
        FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindHelpDeskTicketCurrentAssignmentDataSet TheFindHelpDeskCurrentAssignmentDataSet = new FindHelpDeskTicketCurrentAssignmentDataSet();

        string gstrTicketStatus;
        string gstrUserEmailAddress;
        bool gblnEmailAddress;

        public EditHelpDeskTicket()
        {
            InitializeComponent();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseWindow.IsExpanded = false;
            Close();
        }

        private void expCloseProgram_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseProgram.IsExpanded = false;
            TheMessagesClass.CloseTheProgram();
        }
        private void ResetControls()
        {
            //setting variable
            int intCounter;
            int intNumberOfRecords;
            string strFullName;
            string strProblemType;
            int intEmployeeID;

            int intUpdateCount;
            int intITEmployeeID;

            try
            {
                txtComputerName.Text = "";
                txtCurrentUpdte.Text = "";
                txtExtension.Text = "";
                txtUserName.Text = "";

                TheFindHelpDeskTicketInfomationByTicketIDDataSet = TheHelpDeskClass.FindHelpDeskTicketInformationByTicketID(MainWindow.gintHelpDeskTicketID);
                TheFindHelpDeskTicketUpdatesByTicketIDDataSet = TheHelpDeskClass.FindHelpDeskTicketUpdatesByTicketID(MainWindow.gintHelpDeskTicketID);
                intEmployeeID = TheFindHelpDeskTicketInfomationByTicketIDDataSet.FindHelpDeskTicketInformationByTickeID[0].EmployeeID;
                TheFindEmployeeByEmployeeIDDataSet = TheEmployeeClass.FindEmployeeByEmployeeID(intEmployeeID);
                gstrUserEmailAddress = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].EmailAddress.ToLower();
                txtUserName.Text = TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].FirstName + " " + TheFindEmployeeByEmployeeIDDataSet.FindEmployeeByEmployeeID[0].LastName;
                txtComputerName.Text = TheFindHelpDeskTicketInfomationByTicketIDDataSet.FindHelpDeskTicketInformationByTickeID[0].ComputerName;
                txtReportedProblem.Text = TheFindHelpDeskTicketInfomationByTicketIDDataSet.FindHelpDeskTicketInformationByTickeID[0].ReportedProblem;
                if (!(TheFindHelpDeskTicketInfomationByTicketIDDataSet.FindHelpDeskTicketInformationByTickeID[0].IsExtensionNull()))
                {
                    txtExtension.Text = Convert.ToString(TheFindHelpDeskTicketInfomationByTicketIDDataSet.FindHelpDeskTicketInformationByTickeID[0].Extension);
                }
                else
                {
                    txtExtension.Text = "0";
                }

                


                dgrUpdates.ItemsSource = TheFindHelpDeskTicketUpdatesByTicketIDDataSet.FindHelpDeskTicketUpdatesByTicketID;

                TheFindSortedHelpDeskProblemTypeDataSet = TheHelpDeskClass.FindSortedHelpDeskProblemType();

                intNumberOfRecords = TheFindSortedHelpDeskProblemTypeDataSet.FindSortedHelpDeskProblemType.Rows.Count;
                cboProblemType.Items.Clear();
                cboProblemType.Items.Add("Select Problem Type");

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    cboProblemType.Items.Add(TheFindSortedHelpDeskProblemTypeDataSet.FindSortedHelpDeskProblemType[intCounter].ProblemType);
                }

                cboProblemType.SelectedIndex = 0;

                cboTicketStatus.Items.Clear();
                cboTicketStatus.Items.Add("Select Status");
                cboTicketStatus.Items.Add("OPEN");
                cboTicketStatus.Items.Add("PROGRAM RESEARCH");
                cboTicketStatus.Items.Add("IN PROCESS");
                cboTicketStatus.Items.Add("WAITING ON USER");
                cboTicketStatus.Items.Add("WAITING ON PARTS");
                cboTicketStatus.Items.Add("WAITING ON ERP RELEASE");
                cboTicketStatus.Items.Add("CLOSED");
                cboTicketStatus.SelectedIndex = 0;

                TheFindEmployeeByDepartmentDataSet = TheEmployeeClass.FindEmployeeByDepartment("INFORMATION TECHNOLOGY");

                cboSelectEmployee.Items.Clear();
                cboSelectEmployee.Items.Add("Select Employee");

                intNumberOfRecords = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment.Rows.Count;

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    strFullName = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intCounter].FirstName + " ";
                    strFullName += TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intCounter].LastName;

                    cboSelectEmployee.Items.Add(strFullName);
                }

                cboSelectEmployee.SelectedIndex = 0;

                TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Update Help Desk Tickets");

                intUpdateCount = TheFindHelpDeskTicketUpdatesByTicketIDDataSet.FindHelpDeskTicketUpdatesByTicketID.Rows.Count - 1;

                intNumberOfRecords = TheFindHelpDeskTicketInfomationByTicketIDDataSet.FindHelpDeskTicketInformationByTickeID.Rows.Count - 1;

                intITEmployeeID = TheFindHelpDeskTicketInfomationByTicketIDDataSet.FindHelpDeskTicketInformationByTickeID[intNumberOfRecords].employeeid1;

                intNumberOfRecords = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment.Rows.Count;

                if (intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        if(intITEmployeeID == 1000)
                            cboSelectEmployee.SelectedIndex = 0;
                        else if (TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intCounter].EmployeeID == intITEmployeeID)
                        {
                            cboSelectEmployee.SelectedIndex = intCounter + 1;
                            break;
                        }
                    }
                }

                strProblemType = TheFindHelpDeskTicketInfomationByTicketIDDataSet.FindHelpDeskTicketInformationByTickeID[0].ProblemType;

                intNumberOfRecords = cboProblemType.Items.Count - 1;

                if (intNumberOfRecords > -1)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        if (cboProblemType.Items[intCounter + 1].ToString() == strProblemType)
                        {
                            cboProblemType.SelectedIndex = intCounter + 1;
                            break;
                        }
                    }
                }

                intNumberOfRecords = cboTicketStatus.Items.Count - 1;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        if(cboTicketStatus.Items[intCounter + 1].ToString() == TheFindHelpDeskTicketInfomationByTicketIDDataSet.FindHelpDeskTicketInformationByTickeID[0].TicketStatus)
                        {
                            cboTicketStatus.SelectedIndex = intCounter + 1;
                            
                            break;
                        }
                    }
                }

            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Update Help Desk Tickets // Reset Controls " + Ex.ToString());
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Update Help Desk Tickets // Reset Controls " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        private void SendEmailRegardingCurrentAssignment(int intTicketID, string strFullName)
        {
            string strEmailAddress = "itadmin@bluejaycommunications.com";
            string strHeader;
            string strMessage;
            bool blnFatalError = false;

            try
            {
                strHeader = "Ticket " + Convert.ToString(intTicketID) + " Has Been Assigned To " + strFullName;

                strMessage = "<h1>" + strHeader + "</h1>";

                blnFatalError = !(TheSendEmailClass.SendEmail(strEmailAddress, strHeader, strMessage));

                if (blnFatalError == true)
                    throw new Exception();
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Edit Help Desk Tickets // Send Email Regarding Current Assignment " + Ex.Message);
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Edit Help Desk Tickets // Send Email Regarding Current Assignment " + Ex.Message);
                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboProblemType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboProblemType.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                MainWindow.gintProblemTypeID = TheFindSortedHelpDeskProblemTypeDataSet.FindSortedHelpDeskProblemType[intSelectedIndex].ProblemTypeID;
            }
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intRecordsReturned;
            bool blnFatalError = false;
            int intTransactionID;
            string strFullName;
            string strMessage;
            string strHeader;

            try
            {
                intSelectedIndex = cboSelectEmployee.SelectedIndex - 1;

                if (intSelectedIndex > -1)
                {
                    MainWindow.gintEmployeeID = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intSelectedIndex].EmployeeID;

                    strFullName = TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intSelectedIndex].FirstName + " ";
                    strFullName += TheFindEmployeeByDepartmentDataSet.FindEmployeeByDepartment[intSelectedIndex].LastName;

                    strHeader = "Ticket " + Convert.ToString(MainWindow.gintHelpDeskTicketID) + " Has Been Assigned To " + strFullName;
                    strMessage = "<h1>" + strHeader + "</h1>";

                    TheFindHelpDeskCurrentAssignmentDataSet = TheHelpDeskClass.FindHelpDeskTicketCurrentAssignment(MainWindow.gintHelpDeskTicketID);

                    intRecordsReturned = TheFindHelpDeskCurrentAssignmentDataSet.FindHelpDeskTicketCurrentAssignment.Rows.Count;

                    if (intRecordsReturned > 0)
                    {
                        if (MainWindow.gintEmployeeID != TheFindHelpDeskCurrentAssignmentDataSet.FindHelpDeskTicketCurrentAssignment[0].EmployeeID)
                        {
                            intTransactionID = TheFindHelpDeskCurrentAssignmentDataSet.FindHelpDeskTicketCurrentAssignment[0].TransactionID;

                            blnFatalError = TheHelpDeskClass.UpdateHelpDeskTicketCurrrentAssignment(intTransactionID, false);

                            if (blnFatalError == true)
                                throw new Exception();

                            blnFatalError = TheHelpDeskClass.InsertHelpDeskTicketAssignment(MainWindow.gintHelpDeskTicketID, MainWindow.gintEmployeeID);

                            if (blnFatalError == true)
                                throw new Exception();

                            blnFatalError = !(TheSendEmailClass.SendEmail(gstrUserEmailAddress, strHeader, strMessage));

                            if (blnFatalError == true)
                                throw new Exception();

                            blnFatalError = TheHelpDeskClass.InsertHelpDeskTicketUpdate(MainWindow.gintHelpDeskTicketID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, strHeader);

                            if (blnFatalError == true)
                                throw new Exception();

                            blnFatalError = !(TheSendEmailClass.SendEmail("itadmin@bluejaycommunications.com", strHeader, strMessage));

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                    else if (intRecordsReturned < 1)
                    {
                        blnFatalError = TheHelpDeskClass.InsertHelpDeskTicketAssignment(MainWindow.gintHelpDeskTicketID, MainWindow.gintEmployeeID);

                        if (blnFatalError == true)
                            throw new Exception();

                        blnFatalError = !(TheSendEmailClass.SendEmail(gstrUserEmailAddress, strHeader, strMessage));

                        if (blnFatalError == true)
                            throw new Exception();

                        blnFatalError = TheHelpDeskClass.InsertHelpDeskTicketUpdate(MainWindow.gintHelpDeskTicketID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, strHeader);

                        if (blnFatalError == true)
                            throw new Exception();

                        blnFatalError = !(TheSendEmailClass.SendEmail("itadmin@bluejaycommunications.com", strHeader, strMessage));

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Edit Help Desk Tickets // Select Employee Combo Box " + Ex.Message);
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Update Help Desk Tickets // Select Employee Combo Box " + Ex.Message);
                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboTicketStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboTicketStatus.SelectedIndex;

            if (intSelectedIndex > 0)
            {
                gstrTicketStatus = cboTicketStatus.SelectedItem.ToString();
            }
        }

        private void btnViewDocuments_Click(object sender, RoutedEventArgs e)
        {
            ViewHelpDeskAttachments ViewHelpDeskAttachments = new ViewHelpDeskAttachments();
            ViewHelpDeskAttachments.ShowDialog();

        }

        private void btnAddDocuments_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            DateTime datTransactionDate = DateTime.Now;
            string strDocumentPath = "";
            long intResult;
            string strNewLocation = "";
            string strTransactionName;
            bool blnFatalError;
            string strFileExtension;

            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = "Document"; // Default file name                

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    // Open document
                    strDocumentPath = dlg.FileName.ToUpper();
                }
                else
                {
                    return;
                }

                FileInfo FileName = new FileInfo(strDocumentPath);

                strFileExtension = FileName.Extension;

                datTransactionDate = DateTime.Now;

                intResult = datTransactionDate.Year * 10000000000 + datTransactionDate.Month * 100000000 + datTransactionDate.Day * 1000000 + datTransactionDate.Hour * 10000 + datTransactionDate.Minute * 100 + datTransactionDate.Second;
                strTransactionName = Convert.ToString(intResult);

                strNewLocation = "\\\\bjc\\shares\\Documents\\WAREHOUSE\\WhseTrac\\HelpDeskDocuments\\" + strTransactionName + strFileExtension;

                System.IO.File.Copy(strDocumentPath, strNewLocation);

                blnFatalError = TheHelpDeskClass.InsertHelpDeskTicketDocumentation(MainWindow.gintHelpDeskTicketID, datTransactionDate, strNewLocation);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Document Has Been Saved");
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Edit Help Desk Tickets // Attach Documents " + Ex.ToString());
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Update Help Desk Tickets // Attach Documents " + Ex.ToString());
                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void expUpdateTicket_Expanded(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;
            string strErrorMessage = "";
            string strCurrentUpdate;
            string strHeader;
            string strMessage;

            try
            {
                expUpdateTicket.IsExpanded = false;
                if (cboProblemType.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Problem Type Was Not Selected\n";
                }
                if (cboSelectEmployee.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Employee Was Not Selected\n";
                }
                if (cboTicketStatus.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Ticket Status was not Selected\n";
                }
                strCurrentUpdate = txtCurrentUpdte.Text;
                if (strCurrentUpdate.Length < 15)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Current Update is not Long Enough\n";
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheHelpDeskClass.UpdateHelpDeskTicketStatus(MainWindow.gintHelpDeskTicketID, gstrTicketStatus);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheHelpDeskClass.InsertHelpDeskTicketUpdate(MainWindow.gintHelpDeskTicketID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, strCurrentUpdate);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheHelpDeskClass.UpdateHelpDeskTicketProblemType(MainWindow.gintHelpDeskTicketID, MainWindow.gintProblemTypeID);

                if (blnFatalError == true)
                    throw new Exception();

                strHeader = "Ticket Number " + Convert.ToString(MainWindow.gintHelpDeskTicketID) + "Has Been Updated";

                strMessage = "<h1>" + strHeader + "</h1>";
                strMessage += "<p>" + strCurrentUpdate + "</p>";

                blnFatalError = !(TheSendEmailClass.SendEmail(gstrUserEmailAddress, strHeader, strMessage));

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = !(TheSendEmailClass.SendEmail("itadmin@bluejaycommunications.com", strHeader, strMessage));

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Ticket Has Been Updated");

                ResetControls();
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Edit Help Desk Tickets // Update Ticket Expander " + Ex.ToString());
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Update Help Desk Tickets // Update Ticket Expander " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
