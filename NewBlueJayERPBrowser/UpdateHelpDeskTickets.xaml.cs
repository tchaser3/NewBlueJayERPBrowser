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
using HelpDeskDLL;
using PhonesDLL;
using DataValidationDLL;
using EmployeeDateEntryDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for UpdateHelpDeskTickets.xaml
    /// </summary>
    public partial class UpdateHelpDeskTickets : Page
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        HelpDeskClass TheHelpDeskClass = new HelpDeskClass();
        PhonesClass ThePhoneClass = new PhonesClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the data
        FindOpenHelpDeskTicketsDataSet TheFindOpenHelpDeskTicketsDataSet = new FindOpenHelpDeskTicketsDataSet();
        //FindHelpDeskTicketsByTicketIDDataSet TheFindHelpDeskTicketsByTicketIDDataSet = new FindHelpDeskTicketsByTicketIDDataSet();
        //FindHelpDeskTicketUpdatesByTicketIDDataSet TheFindHelpDeskTicketUpdatesByTicketIDDataSet = new FindHelpDeskTicketUpdatesByTicketIDDataSet();
        FindSortedHelpDeskProblemTypeDataSet TheFindSortedHelpDeskProblemTypeDataSet = new FindSortedHelpDeskProblemTypeDataSet();
        //FindPhoneExtensionByEmployeeIDDataSet TheFindPhoneExtensionByEmployeeIDDataSet = new FindPhoneExtensionByEmployeeIDDataSet();
        //FindEmployeeByDepartmentDataSet TheFindEmployeeByDepartmentDataSet = new FindEmployeeByDepartmentDataSet();
        //FindEmployeeByEmployeeIDDataSet TheFindEmployeeByEmployeeIDDataSet = new FindEmployeeByEmployeeIDDataSet();
        FindOpenHelpDeskTicketsAssignedDataSet TheFindOpenHelpDeskTicketsAssignedDataSet = new FindOpenHelpDeskTicketsAssignedDataSet();
        OpenHelpDeskTicketsDataSet TheOpenHelpDeskTicketsDataSet = new OpenHelpDeskTicketsDataSet();
        FindHelpDeskTicketCurrentAssignmentDataSet TheFindHelpDeskCurrentAssignmentDataSet = new FindHelpDeskTicketCurrentAssignmentDataSet();

        string gstrTicketStatus;
        string gstrUserEmailAddress;
        bool gblnEmailAddress;

        public UpdateHelpDeskTickets()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            //setting variable
            int intCounter;
            int intNumberOfRecords;
            int intHelpDeskTicketID;
            bool blnFatalError = false;

            try
            {
                TheFindOpenHelpDeskTicketsDataSet.FindOpenHelpDeskTickets.Rows.Clear();
                dgrOpenTickets.ItemsSource = TheFindOpenHelpDeskTicketsDataSet.FindOpenHelpDeskTickets;

                TheFindOpenHelpDeskTicketsDataSet = TheHelpDeskClass.FindOpenHelpDeskTickets();

                TheOpenHelpDeskTicketsDataSet.openhelpdesktickets.Rows.Clear();

                intNumberOfRecords = TheFindOpenHelpDeskTicketsDataSet.FindOpenHelpDeskTickets.Rows.Count;

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    OpenHelpDeskTicketsDataSet.openhelpdeskticketsRow NewTicketRow = TheOpenHelpDeskTicketsDataSet.openhelpdesktickets.NewopenhelpdeskticketsRow();

                    NewTicketRow.FirstName = TheFindOpenHelpDeskTicketsDataSet.FindOpenHelpDeskTickets[intCounter].FirstName;
                    NewTicketRow.LastName = TheFindOpenHelpDeskTicketsDataSet.FindOpenHelpDeskTickets[intCounter].LastName;
                    NewTicketRow.ReportedProblem = TheFindOpenHelpDeskTicketsDataSet.FindOpenHelpDeskTickets[intCounter].ReportedProblem;
                    NewTicketRow.TicketDate = TheFindOpenHelpDeskTicketsDataSet.FindOpenHelpDeskTickets[intCounter].TicketDate;
                    NewTicketRow.TicketID = TheFindOpenHelpDeskTicketsDataSet.FindOpenHelpDeskTickets[intCounter].TicketID;

                    TheOpenHelpDeskTicketsDataSet.openhelpdesktickets.Rows.Add(NewTicketRow);

                    intHelpDeskTicketID = TheFindOpenHelpDeskTicketsDataSet.FindOpenHelpDeskTickets[intCounter].TicketID;
                    
                    TheFindHelpDeskCurrentAssignmentDataSet = TheHelpDeskClass.FindHelpDeskTicketCurrentAssignment(intHelpDeskTicketID);

                    if (TheFindHelpDeskCurrentAssignmentDataSet.FindHelpDeskTicketCurrentAssignment.Rows.Count == 0)
                    {
                        blnFatalError = TheHelpDeskClass.InsertHelpDeskTicketAssignment(intHelpDeskTicketID, 1000);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }

                dgrOpenTickets.ItemsSource = TheFindOpenHelpDeskTicketsDataSet.FindOpenHelpDeskTickets;

                TheFindSortedHelpDeskProblemTypeDataSet = TheHelpDeskClass.FindSortedHelpDeskProblemType();
                

                TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP Browser // Update Help Desk Tickets");
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Update Help Desk Tickets // Reset Controls " + Ex.ToString());        

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Update Help Desk Tickets // Reset Controls " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }

        private void dgrOpenTickets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid dataGrid;
                DataGridRow selectedRow;
                DataGridCell TicketID;
                string strTicketID;
                int intSelectedIndex;

                try
                {
                    intSelectedIndex = dgrOpenTickets.SelectedIndex;
                    if(intSelectedIndex > -1)
                    {
                        dataGrid = dgrOpenTickets;
                        selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                        TicketID = (DataGridCell)dataGrid.Columns[0].GetCellContent(selectedRow).Parent;
                        strTicketID = ((TextBlock)TicketID.Content).Text;

                        //find the record
                        MainWindow.gintHelpDeskTicketID = Convert.ToInt32(strTicketID);

                        EditHelpDeskTicket editHelpDeskTicket = new EditHelpDeskTicket();
                        editHelpDeskTicket.ShowDialog();
                        //dgrOpenTickets.Items.Clear();
                        ResetControls();
                    }
                   
                }
                catch (Exception Ex)
                {
                    TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Update Help Desk Tickets Page // Data Grid Selection " + Ex.ToString());

                    TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Update Help Desk Tickets Page // Data Grid Selection " + Ex.ToString());

                    TheMessagesClass.ErrorMessage(Ex.ToString());
                }
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Update Help Desk Tickets // Open Tickets Selection Changed " + Ex.ToString());
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Update Help Desk Tickets // Open Tickets Selection Changed " + Ex.ToString());
                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }

        private void expResetWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expResetWindow.IsExpanded = false;
            ResetControls();
        }
    }
}
