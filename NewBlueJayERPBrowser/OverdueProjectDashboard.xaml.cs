using DateSearchDLL;
using EmployeeDateEntryDLL;
using NewEventLogDLL;
using ProductionProjectDLL;
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
using System.Windows.Threading;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for OverdueProjectDashboard.xaml
    /// </summary>
    public partial class OverdueProjectDashboard : Page
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        ProductionProjectClass TheProductionProjectClass = new ProductionProjectClass();
        EmployeeDateEntryClass TheEmployeeDateEntryClass = new EmployeeDateEntryClass();

        //setting up the timer
        DispatcherTimer MyTimer = new DispatcherTimer();

        //setting up the data
        FindTotalOverdueProductionProjectsDataSet TheFindTotalOverdueProductionProjectsDataSet = new FindTotalOverdueProductionProjectsDataSet();
        FindTotalOverdueProjectStatusDataSet TheFindTotalOverdueProjectStatusDataSet = new FindTotalOverdueProjectStatusDataSet();
        FindOverdueProjectsByOfficeBusinessDataSet TheFindOverdueProjectsByOfficeBusinessDataSet = new FindOverdueProjectsByOfficeBusinessDataSet();
        FindOverdueProjectsByDateRangeDataSet TheFindOverdueProjectsByDateRangeDataSet = new FindOverdueProjectsByDateRangeDataSet();
        AgedOverdueProjectsDataSet TheAgedOverdueProjectsDataSet = new AgedOverdueProjectsDataSet();
        FindOverdueOpenProductionProjectsDataSet TheFindOverdueProductionProjectsDataSet = new FindOverdueOpenProductionProjectsDataSet();
        TopAgedOverdueProjectsDataSet TheTopAgedOverdueProjectsDataSet = new TopAgedOverdueProjectsDataSet();

        //setting global variables
        int gintCounter;
        int gintNumberOfRecords;

        public OverdueProjectDashboard()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadControls();
            ResetControls();
        }
        private void ResetControls()
        {
            TheEmployeeDateEntryClass.InsertIntoEmployeeDateEntry(MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "New Blue Jay ERP // Overdue Project Dashboard ");

            MyTimer.Tick += new EventHandler(BeginTheProcess);
            MyTimer.Interval = new TimeSpan(0, 0, 20);
            MyTimer.Start();
        }
        private void LoadControls()
        {
            //setting local variables
            DateTime datTransactionDate = DateTime.Now;
            int intRecordsReturned;

            datTransactionDate = TheDateSearchClass.AddingDays(datTransactionDate, 3);

            TheFindTotalOverdueProductionProjectsDataSet = TheProductionProjectClass.FindTotalOverdueProductionProjects(datTransactionDate);

            intRecordsReturned = TheFindTotalOverdueProductionProjectsDataSet.FindTotalOverdueProductionProjects.Rows.Count;

            if (intRecordsReturned < 1)
            {
                txtTotalOverdueProjects.Text = "0";
            }
            else if (intRecordsReturned > 0)
            {
                txtTotalOverdueProjects.Text = Convert.ToString(TheFindTotalOverdueProductionProjectsDataSet.FindTotalOverdueProductionProjects[0].TotalOpen);
            }

            TheFindTotalOverdueProjectStatusDataSet = TheProductionProjectClass.FindTotalOverdueProjectStatus(datTransactionDate);

            dgrTotalProjects.ItemsSource = TheFindTotalOverdueProjectStatusDataSet.FindTotalOverdueProjectsStatus;

            TheFindOverdueProjectsByOfficeBusinessDataSet = TheProductionProjectClass.FindOverdueProjectsByOfficeBusiness(datTransactionDate);

            dgrOfficeOverdue.ItemsSource = TheFindOverdueProjectsByOfficeBusinessDataSet.FindOverdueProjectsByOfficeBusiness;

            ComputeTotalDays();

            FindTopOverdueProjects();
        }
        private void FindTopOverdueProjects()
        {
            int intCounter;
            DateTime datTransactionDate = DateTime.Now;
            int intNumberOfRecords;

            try
            {
                datTransactionDate = TheDateSearchClass.AddingDays(datTransactionDate, 3);

                TheTopAgedOverdueProjectsDataSet.topagedoverdueprojects.Rows.Clear();

                TheFindOverdueProductionProjectsDataSet = TheProductionProjectClass.FindOverdueProductionProjects(datTransactionDate);

                intNumberOfRecords = TheFindOverdueProductionProjectsDataSet.FindOverdueOpenProductionProjects.Rows.Count;

                if (intNumberOfRecords > 10)
                {
                    intNumberOfRecords = 10;
                }

                if (intNumberOfRecords > 0)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        TopAgedOverdueProjectsDataSet.topagedoverdueprojectsRow NewProjectReport = TheTopAgedOverdueProjectsDataSet.topagedoverdueprojects.NewtopagedoverdueprojectsRow();

                        NewProjectReport.AssignedProjectID = TheFindOverdueProductionProjectsDataSet.FindOverdueOpenProductionProjects[intCounter].AssignedProjectID;
                        NewProjectReport.CustomerAssignedID = TheFindOverdueProductionProjectsDataSet.FindOverdueOpenProductionProjects[intCounter].CustomerAssignedID;
                        NewProjectReport.ECDDate = TheFindOverdueProductionProjectsDataSet.FindOverdueOpenProductionProjects[intCounter].ECDDate;
                        NewProjectReport.ProjectName = TheFindOverdueProductionProjectsDataSet.FindOverdueOpenProductionProjects[intCounter].ProjectName;

                        TheTopAgedOverdueProjectsDataSet.topagedoverdueprojects.Rows.Add(NewProjectReport);
                    }
                }

                dgrLongestAgedProjects.ItemsSource = TheTopAgedOverdueProjectsDataSet.topagedoverdueprojects;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Overdue Project Dashboard // Find Top Overdue Projects " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void BeginTheProcess(object sender, EventArgs e)
        {
            LoadControls();
        }
        private void ComputeTotalDays()
        {
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate = DateTime.Now;
            int intCounter;
            int intNumberOfRecords;
            int intSecondCounter;
            string strOffice;
            string strDepartment;
            int intTotalCount;
            bool blnItemFound;

            try
            {
                TheAgedOverdueProjectsDataSet.agedoverdueprojects.Rows.Clear();
                gintCounter = 0;
                gintNumberOfRecords = 0;

                datStartDate = TheDateSearchClass.SubtractingDays(datStartDate, 30);
                datEndDate = TheDateSearchClass.AddingDays(datEndDate, 3);

                TheFindOverdueProjectsByDateRangeDataSet = TheProductionProjectClass.FindOverdueProjectsByDateRange(datStartDate, datEndDate);

                intNumberOfRecords = TheFindOverdueProjectsByDateRangeDataSet.FindOverdueProjectsByDateRange.Rows.Count;

                if (intNumberOfRecords > 0)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        strOffice = TheFindOverdueProjectsByDateRangeDataSet.FindOverdueProjectsByDateRange[intCounter].Office;
                        strDepartment = TheFindOverdueProjectsByDateRangeDataSet.FindOverdueProjectsByDateRange[intCounter].Department;
                        intTotalCount = TheFindOverdueProjectsByDateRangeDataSet.FindOverdueProjectsByDateRange[intCounter].TotalProjects;
                        blnItemFound = false;

                        if (gintCounter > 0)
                        {
                            for (intSecondCounter = 0; intSecondCounter < gintNumberOfRecords; intSecondCounter++)
                            {
                                if (TheAgedOverdueProjectsDataSet.agedoverdueprojects[intSecondCounter].Office == strOffice)
                                {
                                    if (TheAgedOverdueProjectsDataSet.agedoverdueprojects[intSecondCounter].BusinessLine == strDepartment)
                                    {
                                        blnItemFound = true;
                                        TheAgedOverdueProjectsDataSet.agedoverdueprojects[intSecondCounter].LessThan30 += intTotalCount;
                                    }
                                }
                            }
                        }

                        if (blnItemFound == false)
                        {
                            AgedOverdueProjectsDataSet.agedoverdueprojectsRow NewProjectRow = TheAgedOverdueProjectsDataSet.agedoverdueprojects.NewagedoverdueprojectsRow();

                            NewProjectRow.Between30and60 = 0;
                            NewProjectRow.BusinessLine = strDepartment;
                            NewProjectRow.LessThan30 = intTotalCount;
                            NewProjectRow.Office = strOffice;
                            NewProjectRow.Over60 = 0;
                            TheAgedOverdueProjectsDataSet.agedoverdueprojects.Rows.Add(NewProjectRow);
                            gintNumberOfRecords = gintCounter;
                            gintCounter++;
                        }
                    }
                }

                datEndDate = datStartDate;
                datStartDate = TheDateSearchClass.SubtractingDays(datStartDate, 30);

                TheFindOverdueProjectsByDateRangeDataSet = TheProductionProjectClass.FindOverdueProjectsByDateRange(datStartDate, datEndDate);

                intNumberOfRecords = TheFindOverdueProjectsByDateRangeDataSet.FindOverdueProjectsByDateRange.Rows.Count;

                if (intNumberOfRecords > 0)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        strOffice = TheFindOverdueProjectsByDateRangeDataSet.FindOverdueProjectsByDateRange[intCounter].Office;
                        strDepartment = TheFindOverdueProjectsByDateRangeDataSet.FindOverdueProjectsByDateRange[intCounter].Department;
                        intTotalCount = TheFindOverdueProjectsByDateRangeDataSet.FindOverdueProjectsByDateRange[intCounter].TotalProjects;
                        blnItemFound = false;

                        if (gintCounter > 0)
                        {
                            for (intSecondCounter = 0; intSecondCounter < gintNumberOfRecords; intSecondCounter++)
                            {
                                if (TheAgedOverdueProjectsDataSet.agedoverdueprojects[intSecondCounter].Office == strOffice)
                                {
                                    if (TheAgedOverdueProjectsDataSet.agedoverdueprojects[intSecondCounter].BusinessLine == strDepartment)
                                    {
                                        blnItemFound = true;
                                        TheAgedOverdueProjectsDataSet.agedoverdueprojects[intSecondCounter].Between30and60 += intTotalCount;
                                    }
                                }
                            }
                        }

                        if (blnItemFound == false)
                        {
                            AgedOverdueProjectsDataSet.agedoverdueprojectsRow NewProjectRow = TheAgedOverdueProjectsDataSet.agedoverdueprojects.NewagedoverdueprojectsRow();

                            NewProjectRow.Between30and60 = intTotalCount;
                            NewProjectRow.BusinessLine = strDepartment;
                            NewProjectRow.LessThan30 = 0;
                            NewProjectRow.Office = strOffice;
                            NewProjectRow.Over60 = 0;
                            TheAgedOverdueProjectsDataSet.agedoverdueprojects.Rows.Add(NewProjectRow);
                            gintNumberOfRecords = gintCounter;
                            gintCounter++;
                        }
                    }
                }

                TheFindOverdueProjectsByOfficeBusinessDataSet = TheProductionProjectClass.FindOverdueProjectsByOfficeBusiness(datStartDate);

                intNumberOfRecords = TheFindOverdueProjectsByOfficeBusinessDataSet.FindOverdueProjectsByOfficeBusiness.Rows.Count;

                if (intNumberOfRecords > 0)
                {
                    for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                    {
                        strOffice = TheFindOverdueProjectsByOfficeBusinessDataSet.FindOverdueProjectsByOfficeBusiness[intCounter].Office;
                        strDepartment = TheFindOverdueProjectsByOfficeBusinessDataSet.FindOverdueProjectsByOfficeBusiness[intCounter].Department;
                        intTotalCount = TheFindOverdueProjectsByOfficeBusinessDataSet.FindOverdueProjectsByOfficeBusiness[intCounter].TotalProjects;
                        blnItemFound = false;

                        if (gintCounter > 0)
                        {
                            for (intSecondCounter = 0; intSecondCounter < gintNumberOfRecords; intSecondCounter++)
                            {
                                if (TheAgedOverdueProjectsDataSet.agedoverdueprojects[intSecondCounter].Office == strOffice)
                                {
                                    if (TheAgedOverdueProjectsDataSet.agedoverdueprojects[intSecondCounter].BusinessLine == strDepartment)
                                    {
                                        blnItemFound = true;
                                        TheAgedOverdueProjectsDataSet.agedoverdueprojects[intSecondCounter].Over60 += intTotalCount;
                                    }
                                }
                            }
                        }

                        if (blnItemFound == false)
                        {
                            AgedOverdueProjectsDataSet.agedoverdueprojectsRow NewProjectRow = TheAgedOverdueProjectsDataSet.agedoverdueprojects.NewagedoverdueprojectsRow();

                            NewProjectRow.Between30and60 = 0;
                            NewProjectRow.BusinessLine = strDepartment;
                            NewProjectRow.LessThan30 = 0;
                            NewProjectRow.Office = strOffice;
                            NewProjectRow.Over60 = intTotalCount;
                            TheAgedOverdueProjectsDataSet.agedoverdueprojects.Rows.Add(NewProjectRow);
                            gintNumberOfRecords = gintCounter;
                            gintCounter++;
                        }
                    }
                }

                dgrAgedOverdue.ItemsSource = TheAgedOverdueProjectsDataSet.agedoverdueprojects;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Overdue Projects Dashboard " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
