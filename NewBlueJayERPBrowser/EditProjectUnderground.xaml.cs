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
    /// Interaction logic for EditProjectUnderground.xaml
    /// </summary>
    public partial class EditProjectUnderground : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        ProductionProjectClass TheProductionProjectClass = new ProductionProjectClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //seting up data
        FindProductionProjectUndergroundByProjectIDDataSet TheFindProductionProjectUndergroundByProjectIDDataSet = new FindProductionProjectUndergroundByProjectIDDataSet();

        //setting global variables
        bool gblnRestorationComplete = false;
        bool gblnUnderGroundComplete = false;

        public EditProjectUnderground()
        {
            InitializeComponent();
        }
        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            expCloseWindow.IsExpanded = false;
            this.Close();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void ResetControls()
        {

            try
            {
                txtDateComplete.Text = "";
                txtEnterFootage.Text = "";
                txtRestorationDateComplete.Text = "";

                cboRestorationComplete.Items.Clear();
                cboRestorationComplete.Items.Add("Select Restoration");
                cboRestorationComplete.Items.Add("Yes");
                cboRestorationComplete.Items.Add("No");
                cboRestorationComplete.SelectedIndex = 0;

                cboUndergroundComplete.Items.Clear();
                cboUndergroundComplete.Items.Add("Select Status");
                cboUndergroundComplete.Items.Add("Yes");
                cboUndergroundComplete.Items.Add("No");
                cboUndergroundComplete.SelectedIndex = 0;

                TheFindProductionProjectUndergroundByProjectIDDataSet = TheProductionProjectClass.FindProductionProjectUndergroundByProjectID(MainWindow.gintProjectID);

                txtEnterFootage.Text = Convert.ToString(TheFindProductionProjectUndergroundByProjectIDDataSet.FindProductionProjectUndergroundByProjectID[0].TotalFootage);

                if (TheFindProductionProjectUndergroundByProjectIDDataSet.FindProductionProjectUndergroundByProjectID[0].IsDateCompletedNull() == false)
                {
                    txtDateComplete.Text = Convert.ToString(TheFindProductionProjectUndergroundByProjectIDDataSet.FindProductionProjectUndergroundByProjectID[0].DateCompleted);
                    cboUndergroundComplete.SelectedIndex = 1;
                }
                else
                {
                    cboUndergroundComplete.SelectedIndex = 2;
                }
                if (TheFindProductionProjectUndergroundByProjectIDDataSet.FindProductionProjectUndergroundByProjectID[0].IsRestorationCompleteNull() == false)
                {
                    if (TheFindProductionProjectUndergroundByProjectIDDataSet.FindProductionProjectUndergroundByProjectID[0].RestorationComplete == true)
                    {
                        cboRestorationComplete.SelectedIndex = 1;
                    }
                    else
                    {
                        cboRestorationComplete.SelectedIndex = 2;
                    }
                }
                else
                {
                    cboRestorationComplete.SelectedIndex = 2;
                }
                if (TheFindProductionProjectUndergroundByProjectIDDataSet.FindProductionProjectUndergroundByProjectID[0].IsRestorationCompleteDateNull() == false)
                {
                    txtRestorationDateComplete.Text = Convert.ToString(TheFindProductionProjectUndergroundByProjectIDDataSet.FindProductionProjectUndergroundByProjectID[0].RestorationCompleteDate);
                }

            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Edit Project Underground // Reset Controls " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Edit Project Underground // Reset Controls " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }

        private void cboRestorationComplete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboRestorationComplete.SelectedIndex == 1)
                gblnRestorationComplete = true;
            else
                gblnRestorationComplete = false;
        }
        private void cboUndergroundComplete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboUndergroundComplete.SelectedIndex == 1)
                gblnUnderGroundComplete = true;
            else
                gblnUnderGroundComplete = false;
        }
        private void expSave_Expanded(object sender, RoutedEventArgs e)
        {
            //setting up data validation
            string strValueForValidation;
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            DateTime datDateComplete = DateTime.Now;
            DateTime datRestorationDateComplete = DateTime.Now;
            int intFootage = 0;
            string strErrorMessage = "";

            expSave.IsExpanded = false;

            try
            {
                //validating controls
                strValueForValidation = txtEnterFootage.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Footage is not an Integer\n";
                }
                else
                {
                    intFootage = Convert.ToInt32(strValueForValidation);
                }
                if (cboRestorationComplete.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Restoration Complete was not Selected\n";
                }
                else
                {
                    if (cboRestorationComplete.SelectedIndex == 1)
                    {
                        strValueForValidation = txtRestorationDateComplete.Text;
                        blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);

                        if (blnThereIsAProblem == true)
                        {
                            blnFatalError = true;
                            strErrorMessage += "The Restoration Complete Date is not a Date\n";
                        }
                        else
                        {
                            datRestorationDateComplete = Convert.ToDateTime(strValueForValidation);

                            blnThereIsAProblem = TheDataValidationClass.verifyDateRange(datRestorationDateComplete, DateTime.Now);

                            if (blnThereIsAProblem == true)
                            {
                                blnFatalError = true;
                                strErrorMessage += "The Restoration Date is after Today\n";
                            }
                        }
                    }
                }
                if (cboUndergroundComplete.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "Underground Complete was not Selected\n";
                }
                else
                {
                    if (cboUndergroundComplete.SelectedIndex == 1)
                    {
                        if (cboRestorationComplete.SelectedIndex != 1)
                        {
                            blnFatalError = true;
                            strErrorMessage += "Restation Complete is not Complete\n";
                        }

                        strValueForValidation = txtDateComplete.Text;
                        blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);

                        if (blnThereIsAProblem == true)
                        {
                            blnFatalError = true;
                            strErrorMessage += "The Date Completed is not a Date\n";
                        }
                        else
                        {
                            datDateComplete = Convert.ToDateTime(strValueForValidation);

                            blnThereIsAProblem = TheDataValidationClass.verifyDateRange(datDateComplete, DateTime.Now);

                            if (blnThereIsAProblem == true)
                            {
                                blnFatalError = true;
                                strErrorMessage += "The Date Complete is after Today\n";
                            }
                        }
                    }
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                blnFatalError = TheProductionProjectClass.UpdateProductionProjectUndergroundFootage(MainWindow.gintProjectID, MainWindow.gstrUserName, intFootage);

                if (blnFatalError == true)
                    throw new Exception();

                if (gblnRestorationComplete == true)
                {
                    blnFatalError = TheProductionProjectClass.UpdateProductionProjectUndergroundRestorationCompleted(MainWindow.gintProjectID, MainWindow.gstrUserName, datRestorationDateComplete);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                if (gblnUnderGroundComplete == true)
                {
                    blnFatalError = TheProductionProjectClass.UpdateProductionProjectUndergroundDateCompleted(MainWindow.gintProjectID, MainWindow.gstrUserName, datDateComplete);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                TheMessagesClass.InformationMessage("The Project Underground has been Updated");

                this.Close();
            }
            catch (Exception Ex)
            {
                TheSendEmailClass.SendEventLog("Project Management Sheet // Edit Project Underground // Save Expander " + Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Project Management Sheet // Edit Project Underground // Save Expander " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
