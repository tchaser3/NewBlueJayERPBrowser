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
using Microsoft.Win32;
using System.Windows.Forms.DataVisualization.Charting;
using NewEventLogDLL;
using ProductionProjectDLL;
using DateSearchDLL;
using System.Windows.Threading;

namespace NewBlueJayERPBrowser
{
    /// <summary>
    /// Interaction logic for OpenProjectDashboard.xaml
    /// </summary>
    public partial class OpenProjectDashboard : Page
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ProductionProjectClass TheProductionProjectClass = new ProductionProjectClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();

        //setting up the data
        FindTotalOpenProductionProjectsDataSet TheFindTotalProductionProjectsDataSet = new FindTotalOpenProductionProjectsDataSet();
        FindTotalOpenStatusProjectsDataSet TheFindOpenStatusProjectsDataSet = new FindTotalOpenStatusProjectsDataSet();
        FindTotalOverdueProductionProjectsDataSet TheFindTotalOverdueProductionProjectsDataSet = new FindTotalOverdueProductionProjectsDataSet();
        FindTotalOverdueProjectStatusDataSet TheFindTotalOverdueProjectStatusDataSet = new FindTotalOverdueProjectStatusDataSet();

        DispatcherTimer MyTimer = new DispatcherTimer();

        public OpenProjectDashboard()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadControls();
            ResetControls();
        }
        private void BeginTheProcess(object sender, EventArgs e)
        {
            LoadControls();
        }
        private void LoadControls()
        {
            DateTime datTransactionDate = DateTime.Now;

            datTransactionDate = TheDateSearchClass.AddingDays(datTransactionDate, 3);

            TheFindTotalProductionProjectsDataSet = TheProductionProjectClass.FindTotalOpenProductionProjects();

            txtTotalOpenProjects.Text = Convert.ToString(TheFindTotalProductionProjectsDataSet.FindTotalOpenProductionProjects[0].TotalOpen);

            TheFindOpenStatusProjectsDataSet = TheProductionProjectClass.FindTotalOpenStatusProjects();

            dgrProjectStatus.ItemsSource = TheFindOpenStatusProjectsDataSet.FindTotalOpenStatusProjects;

            TheFindTotalOverdueProductionProjectsDataSet = TheProductionProjectClass.FindTotalOverdueProductionProjects(datTransactionDate);

            txtTotalOverdueProjects.Text = Convert.ToString(TheFindTotalOverdueProductionProjectsDataSet.FindTotalOverdueProductionProjects[0].TotalOpen);

            TheFindTotalOverdueProjectStatusDataSet = TheProductionProjectClass.FindTotalOverdueProjectStatus(datTransactionDate);

            dgrOrderdueProjects.ItemsSource = TheFindTotalOverdueProjectStatusDataSet.FindTotalOverdueProjectsStatus;

            CurrentOpenProjectsGraph();

            OverdueProjectsGraph();
        }
        private void ResetControls()
        {
            MyTimer.Tick += new EventHandler(BeginTheProcess);
            MyTimer.Interval = new TimeSpan(0, 0, 20);
            MyTimer.Start();

        }
        private void OverdueProjectsGraph()
        {
            int intCounter;
            int intNumberOfRecords;

            try
            {
                Chart chart = this.FindName("gphOverdueStatus") as Chart;
                Series Projects = chart.Series["OverdueProjects"];

                Projects.Points.Clear();
                chart.ChartAreas[0].AxisX.CustomLabels.Clear();
                chart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
                chart.ChartAreas[0].AxisX.Interval = 15;

                intNumberOfRecords = TheFindTotalOverdueProjectStatusDataSet.FindTotalOverdueProjectsStatus.Rows.Count;

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    Projects.Points.Add(TheFindTotalOverdueProjectStatusDataSet.FindTotalOverdueProjectsStatus[intCounter].TotalOpen);

                    chart.ChartAreas[0].AxisX.CustomLabels.Add(intCounter + .5, intCounter + 1.5, TheFindTotalOverdueProjectStatusDataSet.FindTotalOverdueProjectsStatus[intCounter].WorkOrderStatus, 1, LabelMarkStyle.None);
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Open Project Dashboard // Overdue Projects Graph" + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void CurrentOpenProjectsGraph()
        {
            int intCounter;
            int intNumberOfRecords;

            try
            {
                Chart chart = this.FindName("gphProjectStatus") as Chart;
                Series Projects = chart.Series["Projects"];

                Projects.Points.Clear();
                chart.ChartAreas[0].AxisX.CustomLabels.Clear();
                chart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
                chart.ChartAreas[0].AxisX.Interval = 15;

                intNumberOfRecords = TheFindOpenStatusProjectsDataSet.FindTotalOpenStatusProjects.Rows.Count;

                for (intCounter = 0; intCounter < intNumberOfRecords; intCounter++)
                {
                    Projects.Points.Add(TheFindOpenStatusProjectsDataSet.FindTotalOpenStatusProjects[intCounter].TotalOpen);

                    chart.ChartAreas[0].AxisX.CustomLabels.Add(intCounter + .5, intCounter + 1.5, TheFindOpenStatusProjectsDataSet.FindTotalOpenStatusProjects[intCounter].WorkOrderStatus, 1, LabelMarkStyle.None);
                }


            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "New Blue Jay ERP // Open Project Dashboard // Current Open Projects Graph" + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

        }

        private void expOfficeInfo_Expanded(object sender, RoutedEventArgs e)
        {
            expOfficeInfo.IsExpanded = false;
            OfficeInfoDashboard OfficeInfoDashboard = new OfficeInfoDashboard();
            OfficeInfoDashboard.ShowDialog();
        }
    }
}
