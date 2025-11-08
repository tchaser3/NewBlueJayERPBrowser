using NewEventLogDLL;
using ProjectMatrixDLL;
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

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for EnterProjectID.xaml
    /// </summary>
    public partial class EnterProjectID : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ProjectMatrixClass TheProjectMatrixClass = new ProjectMatrixClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        //setting up data
        FindProjectMatrixByCustomerAssignedIDDataSet TheFindProjectMatrixByCustomerAssignedIDDataSet = new FindProjectMatrixByCustomerAssignedIDDataSet();

        public EnterProjectID()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtEnterProjectID.Focus();
        }
        private void ReturnToMainWindow()
        {
            const string message = "Do You Want to Return To The Main Window?";
            const string caption = "Form Closing";
            MessageBoxResult result = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            string strCustomerAssignedID;

            try
            {
                strCustomerAssignedID = txtEnterProjectID.Text;

                if (strCustomerAssignedID.Length < 3)
                {
                    TheMessagesClass.ErrorMessage("The Customer Project ID is not Long Enough");
                    ReturnToMainWindow();
                    return;
                }

                TheFindProjectMatrixByCustomerAssignedIDDataSet = TheProjectMatrixClass.FindProjectMatrixByCustomerAssignedID(strCustomerAssignedID);

                if (TheFindProjectMatrixByCustomerAssignedIDDataSet.FindProjectMatrixByCustomerAssignedID.Rows.Count < 1)
                {
                    TheMessagesClass.ErrorMessage("The Project Is Not Found");
                    ReturnToMainWindow();
                    return;
                }
                else if (TheFindProjectMatrixByCustomerAssignedIDDataSet.FindProjectMatrixByCustomerAssignedID.Rows.Count > 1)
                {
                    TheMessagesClass.ErrorMessage("Multiple Projects Have Been Found, Please Create a Help Desk Ticket for IT");
                    ReturnToMainWindow();
                    return;
                }
                else
                {
                    MainWindow.gstrCustomerProjectID = strCustomerAssignedID;
                    MainWindow.gintProjectID = TheFindProjectMatrixByCustomerAssignedIDDataSet.FindProjectMatrixByCustomerAssignedID[0].ProjectID;

                    if (MainWindow.gstrModule == "ENTER PRODUCTIVITY")
                    {
                        EnterProductivity EnterProductivity = new EnterProductivity();
                        EnterProductivity.Show();
                    }
                    else if (MainWindow.gstrModule == "AFTERHOURS")
                    {
                        CreateAfterHoursReport CreateAfterHoursReport = new CreateAfterHoursReport();
                        CreateAfterHoursReport.Show();
                    }

                    this.Close();
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Enter Project ID // Find Button " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Enter Project ID // Find Button " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
