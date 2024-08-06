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
using ProductionProjectDLL;
using NewEventLogDLL;
using DataValidationDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for AddProjectUnderground.xaml
    /// </summary>
    public partial class AddProjectUnderground : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        ProductionProjectClass TheProductionProjectClass = new ProductionProjectClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        public AddProjectUnderground()
        {
            InitializeComponent();
        }

        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseWindow.IsExpanded = false;
            this.Close();
        }

        private void expSave_Expanded(object sender, RoutedEventArgs e)
        {
            //this will add the underground to a project
            string strValueForValidation;
            bool blnFatalError = false;
            int intFootage;

            try
            {
                strValueForValidation = txtEnterFootage.Text;

                //validation for integer
                blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);

                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage("The Value Entered is not an Integer");
                    return;
                }

                intFootage = Convert.ToInt32(strValueForValidation);

                blnFatalError = TheProductionProjectClass.InsertProductionProjectUnderground(MainWindow.gintProjectID, MainWindow.gstrUserName, intFootage);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("Underground Has Been Added to the Project");

                this.Close();
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Add Underground // Save Expander " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Add Underground // Save Expander " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
