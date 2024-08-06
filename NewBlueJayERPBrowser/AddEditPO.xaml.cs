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
using DataValidationDLL;
using EmployeeDateEntryDLL;
using NewEventLogDLL;
using ProductionProjectDLL;
using ProjectsDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for AddEditPO.xaml
    /// </summary>
    public partial class AddEditPO : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ProductionProjectClass TheProductionProjectClass = new ProductionProjectClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();

        FindProductionProjectInfoDataSet TheFindProductionProjectInfoDataSet = new FindProductionProjectInfoDataSet();
        FindProductionProjectNTPForProjectIDDataSet TheFindProductionProjectNTPForProjectIDDataSet = new FindProductionProjectNTPForProjectIDDataSet();
        ProjectPOSDataSet TheProjectPOSDataSet = new ProjectPOSDataSet();

        //setting global variables
        bool gblnIsAdjustment;
        bool gblnPositiveAdjustment;
        public AddEditPO()
        {
            InitializeComponent();
        }
        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;

            cboIsAdjustment.Items.Clear();
            cboIsAdjustment.Items.Add("Please Select");
            cboIsAdjustment.Items.Add("Adjustment to PO");
            cboIsAdjustment.Items.Add("New PO");
            cboIsAdjustment.SelectedIndex = 0;

            cboTypeOfAdjustment.Items.Clear();
            cboTypeOfAdjustment.Items.Add("Please Select");
            cboTypeOfAdjustment.Items.Add("Positive");
            cboTypeOfAdjustment.Items.Add("Negative");
            cboTypeOfAdjustment.SelectedIndex = 0;
            cboTypeOfAdjustment.IsEnabled = false;

            gblnIsAdjustment = false;
            gblnPositiveAdjustment = false;

            try
            {
                TheProjectPOSDataSet.projectpos.Rows.Clear();

                TheFindProductionProjectNTPForProjectIDDataSet = TheProductionProjectClass.FindProductionProjectNTPForProjectID(MainWindow.gstrCustomerProjectID);

                intNumberOfRecords = TheFindProductionProjectNTPForProjectIDDataSet.FindProductionProjectNTPForProjectID.Rows.Count;

                if (intNumberOfRecords > 0)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        ProjectPOSDataSet.projectposRow NewPORow = TheProjectPOSDataSet.projectpos.NewprojectposRow();

                        NewPORow.POAmount = TheFindProductionProjectNTPForProjectIDDataSet.FindProductionProjectNTPForProjectID[intCounter].POAmount;
                        NewPORow.PONumber = TheFindProductionProjectNTPForProjectIDDataSet.FindProductionProjectNTPForProjectID[intCounter].PONumber;
                        NewPORow.TransactionID = TheFindProductionProjectNTPForProjectIDDataSet.FindProductionProjectNTPForProjectID[intCounter].TransactionID;

                        TheProjectPOSDataSet.projectpos.Rows.Add(NewPORow);
                    }
                }


                dgrPurchaseOrders.ItemsSource = TheProjectPOSDataSet.projectpos;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Add Edit PO // Window Loaded " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Add Edit PO // Window Loaded " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void cboIsAdjustment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboIsAdjustment.SelectedIndex;

                if (intSelectedIndex == 1)
                {
                    gblnIsAdjustment = true;
                    cboTypeOfAdjustment.IsEnabled = true;
                }
                else if (intSelectedIndex == 2)
                {
                    gblnIsAdjustment = false;
                    cboTypeOfAdjustment.IsEnabled = false;
                    gblnPositiveAdjustment = true;
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Add Edit PO // Is Adjustment Combobox " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Add Edit PO // Is Adjustment Combobox " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboTypeOfAdjustment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboTypeOfAdjustment.SelectedIndex == 1)
            {
                gblnPositiveAdjustment = true;
            }
            else if (cboTypeOfAdjustment.SelectedIndex == 2)
            {
                gblnPositiveAdjustment = false;
            }
        }

        private void expSave_Expanded(object sender, RoutedEventArgs e)
        {
            string strPONumber;
            string strValueForValidation;
            decimal decPOAmount = 0;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            int intCounter;
            int intNumberOfRecords;

            try
            {
                strPONumber = txtPONumber.Text;
                if (strPONumber.Length < 4)
                {
                    blnFatalError = true;
                    strErrorMessage += "PO Number is not Long Enough or Not Present\n";
                }
                strValueForValidation = txtPOAmount.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDoubleData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The PO Amount is not Numeric\n";
                }
                else
                {
                    decPOAmount = Convert.ToDecimal(strValueForValidation);
                }
                if (cboIsAdjustment.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Adjustment Combo Box was not Selected\n";
                }
                if (cboTypeOfAdjustment.SelectedIndex < 1)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Type Of Adjustment Combo Box was not Selected\n";
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                TheFindProductionProjectNTPForProjectIDDataSet = TheProductionProjectClass.FindProductionProjectNTPForProjectID(MainWindow.gstrCustomerProjectID);

                intNumberOfRecords = TheFindProductionProjectNTPForProjectIDDataSet.FindProductionProjectNTPForProjectID.Rows.Count;

                if (intNumberOfRecords > 0)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        if (strPONumber == TheFindProductionProjectNTPForProjectIDDataSet.FindProductionProjectNTPForProjectID[intCounter].PONumber)
                        {
                            if (gblnIsAdjustment == false)
                            {
                                TheMessagesClass.ErrorMessage("This PO Number has been Used Before With This Project, Please Change the Adjustment");
                                return;
                            }
                        }
                    }
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Add Edit PO // Save Expander " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Add Edit PO // Save Expander " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
