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
using HelpDeskDLL;
using NewEventLogDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for ViewHelpDeskAttachments.xaml
    /// </summary>
    public partial class ViewHelpDeskAttachments : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        HelpDeskClass TheHelpDeskClass = new HelpDeskClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        //setting up the data
        FindHelpDeskTicketDocumentationByTicketIDDataSet TheFindHelpDeskTicketDocumentationByTicketIDDataSet = new FindHelpDeskTicketDocumentationByTicketIDDataSet();

        public ViewHelpDeskAttachments()
        {
            InitializeComponent();
        }
        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void dgrDocuments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid;
            DataGridRow selectedRow;
            DataGridCell DocumentPath;
            string strDocumentPath;

            try
            {
                if (dgrDocuments.SelectedIndex > -1)
                {
                    //setting local variable
                    dataGrid = dgrDocuments;
                    selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    DocumentPath = (DataGridCell)dataGrid.Columns[3].GetCellContent(selectedRow).Parent;
                    strDocumentPath = ((TextBlock)DocumentPath.Content).Text;

                    System.Diagnostics.Process.Start(strDocumentPath);
                }
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // View Help Desk Documents // Documentation Grid View Selection " + Ex.ToString());
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // View Help Desk Documents // Documentation Grid View Selection " + Ex.ToString());
                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TheFindHelpDeskTicketDocumentationByTicketIDDataSet = TheHelpDeskClass.FindHelpDeskTicketDocumentationByTicketID(MainWindow.gintHelpDeskTicketID);

            dgrDocuments.ItemsSource = TheFindHelpDeskTicketDocumentationByTicketIDDataSet.FindHelpDeskTicketDocumentationByTicketID;
        }
    }
}
