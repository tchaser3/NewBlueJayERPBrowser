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
using DataValidationDLL;
using EmployeeDateEntryDLL;
using FleetIOVehicleDLL;
using VehicleMainDLL;
using DateSearchDLL;
using Excel = Microsoft.Office.Interop.Excel;
using System.Threading;
using NewEmployeeDLL;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for ImportFleetIOVehicles.xaml
    /// </summary>
    public partial class ImportFleetIOVehicles : Page
    {
        //setting up classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();
        FleetIOVehicleClass TheFleetIOVehicleClass = new FleetIOVehicleClass();
        VehicleMainClass TheVehicleMainClass = new VehicleMainClass();
        SendEmailClass TheSendEmailClass = new SendEmailClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        //setting up data sets
        FleetIOVehicleImportDataSet TheFleetIOVehicleImportDataSet = new FleetIOVehicleImportDataSet();
        FleetIOVehicleImportDataSet TheSecondFleetIOVehicleImportDataSet = new FleetIOVehicleImportDataSet();
        FindVehicleMainByVehicleNumberDataSet TheFindVehicleMainByVehicleNumberDataSet = new FindVehicleMainByVehicleNumberDataSet();
        FindFleetIOVehicleByVehilceNumberDataSet TheFindFleetIOVehicleByVehicleNumberDataSet = new FindFleetIOVehicleByVehilceNumberDataSet();
        FindWarehousesDataSet TheFindWarehousesDataSet = new FindWarehousesDataSet();
        FindVehicleMainToDeactivateDataSet TheFindVehicleMainToDeactivateDataSet = new FindVehicleMainToDeactivateDataSet();

        //setting up Vehicle Boolean
        bool gblnExcelProcessed;
        bool gblnVehicles;
        bool gblnTrailers;

        public ImportFleetIOVehicles()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TheFleetIOVehicleImportDataSet.vehiclesforimport.Clear();

            dgrVehicles.ItemsSource = TheFleetIOVehicleImportDataSet.vehiclesforimport;

            gblnExcelProcessed = false;
            gblnVehicles = false;
            gblnTrailers = false;
            expProcessImport.IsEnabled = false;

            TheFindWarehousesDataSet = TheEmployeeClass.FindWarehouses();

            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP Browser \\ Import Fleet IO Vehicles");
        }

        private void expImportVehicles_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError;

            try
            {
                TheSecondFleetIOVehicleImportDataSet.vehiclesforimport.Rows.Clear();
                expImportOther.IsEnabled = false;
                expImportTrailers.IsEnabled = false;
                expImportVehicles.IsExpanded = false;

                if (gblnExcelProcessed == false)
                {
                    blnFatalError = ImportExcel();
                    if (blnFatalError == true)
                        throw new Exception();
                }

                intNumberOfRecords = TheFleetIOVehicleImportDataSet.vehiclesforimport.Rows.Count;

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    if (TheFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleGroup.ToLower().Contains("vehicle"))
                    {
                        FleetIOVehicleImportDataSet.vehiclesforimportRow NewVehicleRow = TheSecondFleetIOVehicleImportDataSet.vehiclesforimport.NewvehiclesforimportRow();
                        NewVehicleRow.FleetIOVehicleGroup = TheFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleGroup;
                        NewVehicleRow.FleetIOVehicleID = TheFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleID;
                        NewVehicleRow.FleetIOVehicleLicensePlate = TheFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleLicensePlate;
                        NewVehicleRow.FleetIOVehicleMake = TheFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleMake;
                        NewVehicleRow.FleetIOVehicleModel = TheFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleModel;
                        NewVehicleRow.FleetIOVehicleNumber = TheFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleNumber;
                        NewVehicleRow.FleetIOVehicleType = TheFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleType;
                        NewVehicleRow.FleetIOVehicleVIN = TheFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleVIN;
                        NewVehicleRow.FleetIOVehicleYear = TheFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleYear;
                        NewVehicleRow.FleetIOVehicleStatus = TheFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleStatus;

                        TheSecondFleetIOVehicleImportDataSet.vehiclesforimport.Rows.Add(NewVehicleRow);
                    }
                }

                dgrVehicles.ItemsSource = TheSecondFleetIOVehicleImportDataSet.vehiclesforimport;
                gblnTrailers = false;
                gblnVehicles = true;
                expProcessImport.IsEnabled = true;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Import Fleet IO Vehicles // Import Vehicles Expander " + Ex.Message);

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Import Fleet IO Vehicles // Import Vehicles Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }
        private bool ImportExcel()
        {
            bool blnFatalError = false;

            Excel.Application xlDropOrder;
            Excel.Workbook xlDropBook;
            Excel.Worksheet xlDropSheet;
            Excel.Range range;

            int intColumnRange = 0;
            int intCounter;
            int intNumberOfRecords;
            double douDate;
            string strFleetIOID;
            int intFleetIOID;
            string strVehicleNumber;
            int intVehicleYear;
            string strVehicleMake;
            string strVehicleModel;
            string strVIN;
            string strStatus;
            string strType;
            string strGroup;
            string strLicensePlate;
            string strVehicleYear;

            try
            {
                TheFleetIOVehicleImportDataSet.vehiclesforimport.Rows.Clear();
                
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = "Document"; // Default file name
                dlg.DefaultExt = ".xlsx"; // Default file extension
                dlg.Filter = "Excel (.xlsx)|*.xlsx"; // Filter files by extension

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    // Open document
                    string filename = dlg.FileName;
                }

                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();
                
                xlDropOrder = new Excel.Application();
                xlDropBook = xlDropOrder.Workbooks.Open(dlg.FileName, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlDropSheet = (Excel.Worksheet)xlDropOrder.Worksheets.get_Item(1);
                
                range = xlDropSheet.UsedRange;
                intNumberOfRecords = range.Rows.Count;
                intColumnRange = range.Columns.Count;

                for (intCounter = 2; intCounter <= intNumberOfRecords; intCounter++)
                {
                    //strCellNumber = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2).ToUpper();
                    strFleetIOID = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2).ToUpper();
                    intFleetIOID = Convert.ToInt32(strFleetIOID);
                    strVehicleNumber = Convert.ToString((range.Cells[intCounter, 2] as Excel.Range).Value2).ToUpper();
                    try
                    {
                        strVehicleYear = Convert.ToString((range.Cells[intCounter, 5] as Excel.Range).Value2).ToUpper();
                        intVehicleYear = Convert.ToInt32(strVehicleYear);
                    }
                    catch
                    {
                        intVehicleYear = 0;
                    }
                    try
                    {
                        strVehicleMake = Convert.ToString((range.Cells[intCounter, 6] as Excel.Range).Value2).ToUpper();
                    }
                    catch
                    {
                        strVehicleMake = "";
                    }
                    try
                    {
                        strVehicleModel = Convert.ToString((range.Cells[intCounter, 7] as Excel.Range).Value2).ToUpper();
                    }
                    catch
                    {
                        strVehicleModel = "";
                    }
                    try
                    {
                        strVIN = Convert.ToString((range.Cells[intCounter, 8] as Excel.Range).Value2).ToUpper();
                    }
                    catch
                    {
                        strVIN = "";
                    }
                    try
                    {
                        strStatus = Convert.ToString((range.Cells[intCounter, 9] as Excel.Range).Value2).ToUpper();
                    }
                    catch
                    {
                        strStatus = "";
                    }
                    try
                    {
                        strType = Convert.ToString((range.Cells[intCounter, 10] as Excel.Range).Value2).ToUpper();
                    }
                    catch
                    {
                        strType = "";
                    }
                    try
                    {
                        strGroup = Convert.ToString((range.Cells[intCounter, 11] as Excel.Range).Value2).ToUpper();
                    }
                    catch
                    {
                        strGroup = "";
                    }
                    try
                    {
                        strLicensePlate = Convert.ToString((range.Cells[intCounter, 12] as Excel.Range).Value2).ToUpper();
                    }
                    catch
                    {
                        strLicensePlate = "";
                    }

                    FleetIOVehicleImportDataSet.vehiclesforimportRow NewVehicleRow = TheFleetIOVehicleImportDataSet.vehiclesforimport.NewvehiclesforimportRow();
                    NewVehicleRow.FleetIOVehicleGroup = strGroup;
                    NewVehicleRow.FleetIOVehicleID = Convert.ToInt32(strFleetIOID);
                    NewVehicleRow.FleetIOVehicleLicensePlate = strLicensePlate;
                    NewVehicleRow.FleetIOVehicleMake = strVehicleMake;
                    NewVehicleRow.FleetIOVehicleModel = strVehicleModel;
                    NewVehicleRow.FleetIOVehicleNumber = strVehicleNumber;
                    NewVehicleRow.FleetIOVehicleType = strType;
                    NewVehicleRow.FleetIOVehicleStatus = strStatus;
                    NewVehicleRow.FleetIOVehicleVIN = strVIN;
                    NewVehicleRow.FleetIOVehicleYear = intVehicleYear;

                    TheFleetIOVehicleImportDataSet.vehiclesforimport.Rows.Add(NewVehicleRow);
                }
                                
                PleaseWait.Close();
                gblnExcelProcessed = true;
            }
            catch (Exception Ex)
            {
                blnFatalError = true;

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Import Fleet IO Vehicles // Import Excel " + Ex.Message);

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Import Fleet IO Vehicles // Import Excel " + Ex.Message);
                
                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
            return blnFatalError;
            
        }

        private void expImportTrailers_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            bool blnFatalError;

            try
            {
                TheSecondFleetIOVehicleImportDataSet.vehiclesforimport.Rows.Clear();
                expImportOther.IsEnabled = false;
                expImportVehicles.IsEnabled = false;
                expImportTrailers.IsExpanded = false;

                if (gblnExcelProcessed == false)
                {
                    blnFatalError = ImportExcel();
                    if (blnFatalError == true)
                        throw new Exception();
                }

                intNumberOfRecords = TheFleetIOVehicleImportDataSet.vehiclesforimport.Rows.Count;

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    if (TheFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleGroup.ToLower().Contains("trailer"))
                    {
                        FleetIOVehicleImportDataSet.vehiclesforimportRow NewVehicleRow = TheSecondFleetIOVehicleImportDataSet.vehiclesforimport.NewvehiclesforimportRow();
                        NewVehicleRow.FleetIOVehicleGroup = TheFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleGroup;
                        NewVehicleRow.FleetIOVehicleID = TheFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleID;
                        NewVehicleRow.FleetIOVehicleLicensePlate = TheFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleLicensePlate;
                        NewVehicleRow.FleetIOVehicleMake = TheFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleMake;
                        NewVehicleRow.FleetIOVehicleModel = TheFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleModel;
                        NewVehicleRow.FleetIOVehicleNumber = TheFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleNumber;
                        NewVehicleRow.FleetIOVehicleType = TheFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleType;
                        NewVehicleRow.FleetIOVehicleStatus = TheFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleStatus;
                        NewVehicleRow.FleetIOVehicleVIN = TheFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleVIN;
                        NewVehicleRow.FleetIOVehicleYear = TheFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleYear;

                        TheSecondFleetIOVehicleImportDataSet.vehiclesforimport.Rows.Add(NewVehicleRow);
                    }
                }

                dgrVehicles.ItemsSource = TheSecondFleetIOVehicleImportDataSet.vehiclesforimport;
                gblnTrailers = false;
                gblnVehicles = true;
                expProcessImport.IsEnabled = true;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Import Fleet IO Vehicles // Import Trailers Expander " + Ex.Message);

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Import Fleet IO Vehicles // Import Trailers Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void ResetControls()
        {
            TheSecondFleetIOVehicleImportDataSet.vehiclesforimport.Rows.Clear();
            TheFleetIOVehicleImportDataSet.vehiclesforimport.Rows.Clear();
            expImportVehicles.IsEnabled = true;
            expImportTrailers.IsEnabled = true;
            expImportOther.IsEnabled = true;
            expResetWindow.IsExpanded = false;
            gblnExcelProcessed = false;
            gblnTrailers = false;
            gblnVehicles = false;
            expProcessImport.IsEnabled = false;
        }
        private void expResetWindow_Expanded(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }

        private void expProcessImport_Expanded(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;

            expImportVehicles.IsEnabled = true;
            expImportTrailers.IsEnabled = true;
            expImportOther.IsEnabled = true;
            expResetWindow.IsExpanded = false;
            expProcessImport.IsExpanded = false;
            if(gblnVehicles == true)
            {
                blnFatalError = VehicleTableImport();
            }
            if (gblnTrailers == true)
            {
                blnFatalError = TrailerTableImport();
            }
            if(blnFatalError == true)
            {
                TheMessagesClass.ErrorMessage("IMPORT FAILED");
            }
        }
        private bool VehicleTableImport()
        {
            bool blnFatalError = false;
            int intCounter;
            int intNumberOfRecords;
            string strVIN;
            string strVehicleNumber;
            bool blnFleetItemFound;
            bool blnVehicleFound;
            int intRecordsReturned;
            int intFleetIOID;
            int intVehicleYear;
            string strMake;
            string strModel;
            string strLicensePlate;
            string strOffice;
            DateTime datOilChangeDate = DateTime.Now;
            string strVehicleGroup;
            int intEmployeeID;
            int intBlueJayVehicleID;
            string strVehicleType;
            string strVehicleStatus;
            int intVehicleID;
            bool blnVehicleActive;

            try
            {
                intNumberOfRecords = TheSecondFleetIOVehicleImportDataSet.vehiclesforimport.Rows.Count;
                intEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;

                if(intNumberOfRecords > 0 )
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        blnFleetItemFound = false;
                        blnVehicleFound = false;
                        strVIN = TheSecondFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleVIN;
                        strVehicleNumber = TheSecondFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleNumber;
                        intFleetIOID = TheSecondFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleID;
                        intVehicleYear = TheSecondFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleYear;
                        strMake = TheSecondFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleMake;
                        strModel = TheSecondFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleModel;
                        strLicensePlate = TheSecondFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleLicensePlate;
                        strVehicleType = TheSecondFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleType;
                        strVehicleStatus = TheSecondFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleStatus;
                        
                        //this will get the office
                        strVehicleGroup = TheSecondFleetIOVehicleImportDataSet.vehiclesforimport[intCounter].FleetIOVehicleGroup;
                        intBlueJayVehicleID = 0;

                        if (strVehicleGroup.Contains("TOL"))
                            strOffice = "TOLEDO";
                        else if (strVehicleGroup.Contains("YNG"))
                            strOffice = "YOUNGSTOWN";
                        else
                            strOffice = "CLEVELAND";

                        TheFindVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindVehicleMainByVehicleNumber(strVehicleNumber);
                        intRecordsReturned = TheFindVehicleMainByVehicleNumberDataSet.FindVehicleMainByVehicleNumber.Rows.Count;
                        blnVehicleActive = true;

                        if (strVehicleStatus.ToLower().Contains("out of service"))
                        {
                            blnVehicleActive = false;

                            if (intRecordsReturned == 0)
                            {
                                blnFatalError = TheVehicleMainClass.InsertVehicleMain(intBlueJayVehicleID, strVehicleNumber, intVehicleYear, strMake, strModel, strLicensePlate, strVIN, 0, datOilChangeDate, intEmployeeID, strVehicleGroup, strOffice);

                                if (blnFatalError == true)
                                    throw new Exception();

                                TheFindVehicleMainByVehicleNumberDataSet = TheVehicleMainClass.FindVehicleMainByVehicleNumber(strVehicleNumber);

                                intVehicleID = TheFindVehicleMainByVehicleNumberDataSet.FindVehicleMainByVehicleNumber[0].VehicleID;

                                blnFatalError = TheVehicleMainClass.UpdateVehicleMainActive(intVehicleID, false);

                                if (blnFatalError == true)
                                    throw new Exception();

                            }
                            else if (intRecordsReturned == 1)
                            {
                                intVehicleID = TheFindVehicleMainByVehicleNumberDataSet.FindVehicleMainByVehicleNumber[0].VehicleID;

                                blnFatalError = TheVehicleMainClass.UpdateVehicleMainActive(intVehicleID, false);

                                if (blnFatalError == true)
                                    throw new Exception();
                            }                            
                        }
                        else
                        {                          

                            if (intRecordsReturned == 0)
                            {
                                blnFatalError = TheVehicleMainClass.InsertVehicleMain(intBlueJayVehicleID, strVehicleNumber, intVehicleYear, strMake, strModel, strLicensePlate, strVIN, 0, datOilChangeDate, intEmployeeID, strVehicleGroup, strOffice);

                                if (blnFatalError == true)
                                    throw new Exception();
                            }
                        }

                        TheFindFleetIOVehicleByVehicleNumberDataSet = TheFleetIOVehicleClass.FindFleetIOVehicleByVehicleNumber(strVehicleNumber);
                        intRecordsReturned = TheFindFleetIOVehicleByVehicleNumberDataSet.FindFleetIOVehicleByVehicleNumber.Rows.Count;

                        if(intRecordsReturned == 0)
                        {
                            blnFatalError = TheFleetIOVehicleClass.InsertFleetIOVehicle(intFleetIOID, intVehicleYear, strVehicleNumber, strModel, strModel, strVIN, strVehicleType, strVehicleGroup, strLicensePlate, blnVehicleActive, strVehicleStatus);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                }

                TheMessagesClass.InformationMessage("Import has Completed");
                ResetControls();
                
            }
            catch (Exception Ex)
            {
                blnFatalError = true;

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Import Fleet IO Vehicles // Vehicle Table Import " + Ex.Message);

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Import Fleet IO Vehicles // Vehicle Table Import " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            return blnFatalError;
        }
        private int FindWarehouseID(string strWarehouseName)
        {
            int intOfficeID = 0;

            return intOfficeID;
        }
        private bool TrailerTableImport()
        {
            bool blnFatalError = false;


            return blnFatalError;
        }

        private void expDeactivateVehicles_Expanded(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intVehicleID;
            string strFleetIOStatus;
            int intFleetIOID;
            bool blnDeactivateVehicle;
            bool blnFatalError;

            try
            {
                TheFindVehicleMainToDeactivateDataSet = TheVehicleMainClass.FindVehicleMainToDeactivate();

                intNumberOfRecords = TheFindVehicleMainToDeactivateDataSet.FindVehicleMainToDeactivate.Rows.Count;

                if(intNumberOfRecords > 0)
                {
                    for(intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        blnDeactivateVehicle = false;
                        intVehicleID = TheFindVehicleMainToDeactivateDataSet.FindVehicleMainToDeactivate[intCounter].VehicleID;

                        if (TheFindVehicleMainToDeactivateDataSet.FindVehicleMainToDeactivate[intCounter].IsFleetIOVehicleIDNull())
                        {
                            blnDeactivateVehicle = true;
                        }
                        else
                        {

                        }

                        if(blnDeactivateVehicle == true)
                        {
                            blnFatalError = TheVehicleMainClass.UpdateVehicleMainActive(intVehicleID, false);

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                        
                    }

                }

                TheMessagesClass.InformationMessage("The Deactivate Routine Has Run");
                ResetControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP Browser // Import Fleet IO Vehicles // Deactivate Vehicles Expander " + Ex.Message);

                TheSendEmailClass.SendEventLog("New Blue Jay ERP Browser // Import Fleet IO Vehicles // Deactivate Vehicles Expander " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
