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
using NewEventLogDLL;
using ProductionProjectDLL;
using DataValidationDLL;
using EmployeeDateEntryDLL;
using JobTypeDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for EditProjectInfo.xaml
    /// </summary>
    public partial class EditProjectInfo : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ProductionProjectClass TheProductionProjectClass = new ProductionProjectClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        JobTypeClass TheJobTypeClass = new JobTypeClass();

        FindSortedJobTypeDataSet TheFindSortedJobTypeDataSet = new FindSortedJobTypeDataSet();
        FindProductionProjectInfoDataSet TheFindProductionProjectInfoDataSet = new FindProductionProjectInfoDataSet();
        FindProductionProjectNTPForProjectIDDataSet TheFindProductionProjectNTPForProjectIDDataSet = new FindProductionProjectNTPForProjectIDDataSet();

        string gstrJobType;
        int gintJobTypeID;
        bool gblnSplicingComplete;
        bool gblnRestorationComplete;
        bool gblnQCRestored;
        int gintTransactionID;

        public EditProjectInfo()
        {
            InitializeComponent();
        }
        private void expCloseWindow_Expanded(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        private void ResetControls()
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            string strJobType;
            int intSelectedIndex = 0;
            bool blnFatalError = false;
            string strPONumber;
            decimal decPOAmount;

            try
            {
                TheFindSortedJobTypeDataSet = TheJobTypeClass.FindSortedJobType();

                intNumberOfRecords = TheFindSortedJobTypeDataSet.FindSortedJobType.Rows.Count;
                cboJobType.Items.Clear();
                cboJobType.Items.Add("Select Job Type");

                if (intNumberOfRecords > 0)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        cboJobType.Items.Add(TheFindSortedJobTypeDataSet.FindSortedJobType[intCounter].JobType);
                    }
                }

                cboJobType.SelectedIndex = 0;

                cboQCPerformed.Items.Clear();
                cboQCPerformed.Items.Add("Select QC Performed");
                cboQCPerformed.Items.Add("Yes");
                cboQCPerformed.Items.Add("No");
                cboQCPerformed.SelectedIndex = 0;

                cboSplicingComplete.Items.Clear();
                cboSplicingComplete.Items.Add("Select Splicing Complete");
                cboSplicingComplete.Items.Add("Yes");
                cboSplicingComplete.Items.Add("No");
                cboSplicingComplete.SelectedIndex = 0;

                TheFindProductionProjectInfoDataSet = TheProductionProjectClass.FindProductionProjectInfo(MainWindow.gintProjectID);

                intNumberOfRecords = TheFindProductionProjectInfoDataSet.FindProductionProjectInfo.Rows.Count;

                if (intNumberOfRecords < 1)
                {
                    TheMessagesClass.InformationMessage("Information For This Project Was Not Found, Would You Like To Add IT");
                }
                else if (intNumberOfRecords > 1)
                {
                    TheMessagesClass.ErrorMessage("The Are Multiple Entries for Project Info For This Project.  Please Create a Help Desk Ticket For IT");
                    TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Edit Project Info // There Are Muiltiple Entries in the Production Project Info Table Fort This Project " + MainWindow.gstrAssignedProjectID);
                    this.Close();

                }

                strPONumber = TheFindProductionProjectInfoDataSet.FindProductionProjectInfo[0].PONumber;
                decPOAmount = TheFindProductionProjectInfoDataSet.FindProductionProjectInfo[0].POAmount;

                txtPointOfContact.Text = TheFindProductionProjectInfoDataSet.FindProductionProjectInfo[0].PointOfContact;
                txtPOTotal.Text = Convert.ToString(decPOAmount); ;
                txtPONumber.Text = strPONumber;

                if (TheFindProductionProjectInfoDataSet.FindProductionProjectInfo[0].QCPerformed == true)
                    cboQCPerformed.SelectedIndex = 1;
                else
                    cboQCPerformed.SelectedIndex = 2;

                if (TheFindProductionProjectInfoDataSet.FindProductionProjectInfo[0].SplicingComplete == true)
                    cboSplicingComplete.SelectedIndex = 1;
                else
                    cboSplicingComplete.SelectedIndex = 2;

                strJobType = TheFindProductionProjectInfoDataSet.FindProductionProjectInfo[0].JobType;
                intNumberOfRecords = TheFindSortedJobTypeDataSet.FindSortedJobType.Rows.Count;

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    if (strJobType == TheFindSortedJobTypeDataSet.FindSortedJobType[intCounter].JobType)
                    {
                        intSelectedIndex = intCounter + 1;
                    }
                }

                cboJobType.SelectedIndex = intSelectedIndex;

                TheFindProductionProjectNTPForProjectIDDataSet = TheProductionProjectClass.FindProductionProjectNTPForProjectID(MainWindow.gstrCustomerProjectID);

                if (TheFindProductionProjectNTPForProjectIDDataSet.FindProductionProjectNTPForProjectID.Rows.Count < 1)
                {
                    blnFatalError = TheProductionProjectClass.InsertProductionProjectNTP(MainWindow.gstrUserName, strPONumber, decPOAmount, MainWindow.gintProjectID);

                    if (blnFatalError == true)
                        throw new Exception();
                }
                else if (TheFindProductionProjectNTPForProjectIDDataSet.FindProductionProjectNTPForProjectID.Rows.Count == 1)
                {
                    if (TheFindProductionProjectNTPForProjectIDDataSet.FindProductionProjectNTPForProjectID[0].IsCreateDateTimeNull() == true)
                    {
                        blnFatalError = TheProductionProjectClass.InsertProductionProjectNTP(MainWindow.gstrUserName, strPONumber, decPOAmount, MainWindow.gintProjectID);

                        if (blnFatalError == true)
                            throw new Exception();
                    }
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Edit Project Info // Reset Controls " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Edit Project Info // Reset Controls " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void expAddEditPO_Expanded(object sender, RoutedEventArgs e)
        {
            expAddEditPO.IsExpanded = false;
            AddEditPO AddEditPO = new AddEditPO();
            AddEditPO.ShowDialog();

            ResetControls();
        }

        private void expSave_Expanded(object sender, RoutedEventArgs e)
        {
            try
            {
                bool blnFatalError = false;

                expSave.IsExpanded = false;


                blnFatalError = TheProductionProjectClass.UpdateProductionProjectInfoJobType(gintTransactionID, gintJobTypeID);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheProductionProjectClass.UpdateProductionProjectInfoSplicingComplete(gintTransactionID, gblnSplicingComplete);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheProductionProjectClass.UpdateProductionProjectInfoQCPerformed(gintTransactionID, gblnQCRestored);

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Production Project Info has been Saved");

                this.Close();
            }
            catch (Exception Ex)
            {

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Edit Project Info // Send Expander " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Edit Project Info // Send Expander " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboJobType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboJobType.SelectedIndex;

                if (intSelectedIndex > 0)
                {
                    gstrJobType = TheFindSortedJobTypeDataSet.FindSortedJobType[intSelectedIndex - 1].JobType;
                    gintJobTypeID = TheFindSortedJobTypeDataSet.FindSortedJobType[intSelectedIndex - 1].JobTypeID;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Edit Project Info // Job Type Combo Box " + Ex.ToString());

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Edit Project Info // Job Type Combo Box " + Ex.ToString());

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboSplicingComplete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSplicingComplete.SelectedIndex;

            if (intSelectedIndex == 1)
                gblnSplicingComplete = true;
            else if (intSelectedIndex == 2)
                gblnSplicingComplete = false;
        }

        private void cboQCPerformed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex = cboQCPerformed.SelectedIndex;

            if (intSelectedIndex == 1)
                gblnQCRestored = true;
            else if (intSelectedIndex == 2)
                gblnQCRestored = false;
        }
    }
}
