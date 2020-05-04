using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

using KatvaSoft.SimpleAppInsightQuerier.AppInsightClient;


namespace AzureAppInsightsSimpleQuery
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public DateTime? DateFrom { get; set; } = DateTime.Now;

        public String SelectedEventType { get; set; }

        

        public ObservableCollection<string> eventTypeStrs = new ObservableCollection<string>() { "traces", "exceptions" };

        private List<LogRow> LogRows;

        private List<LogRow> FilteredLogRows = null;

        private List<String> LoggerNames = new List<string>();

        private List<String> LoggingLevels = new List<string>();

        private List<String> OperationNames = new List<string>();

        private Boolean CanFilter = false;

        

        public MainWindow()
        {
            InitializeComponent();
            dateSelect.DataContext = this;
            eventTypesSelect.DataContext = this;
            ResultTxtBox.DataContext = this;
            eventTypesSelect.ItemsSource = eventTypeStrs;
            Progress.Visibility = Visibility.Hidden;
            loggerNames_combo.DataContext = this;
            
            loggingLevel_combo.DataContext = this;
            ToggleFilterFields(false);
            
        } 

        private void ToggleFilterFields(bool enabled)
        {
            loggerNames_combo.IsEnabled = enabled;
            loggingLevel_combo.IsEnabled = enabled;
            //filterBtn.IsEnabled = enabled;
            search_TxtBox.IsEnabled = enabled;
        }

        private async void queryBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.SelectedEventType == null)
            {
                MessageBox.Show("Please select event type!", "Simple log querier");
                return;
            }
            var querier = CheckSettings();
            if (querier != null)
            {
                Progress.Visibility = Visibility.Visible;

                var selected = this.SelectedEventType;
                
                var selectedDate = this.DateFrom;
                var today = DateTime.Today;
                try
                {
                    if (selectedDate != null && selectedDate.HasValue)
                    {
                        var result = await querier.QueryAppInsights(selected, today.Subtract(selectedDate.Value), null);
                        this.LogRows = result;

                    }
                    else
                    {
                        var result = await querier.QueryAppInsights(selected, null, null);
                        this.LogRows = result;
                    }
                } catch (Exception exp)
                {
                    MessageBox.Show("Ooops. The query did not succeed.", "Simple log querier");
                }
               

                Progress.Visibility = Visibility.Hidden;
                AddResultToTxtBox(this.LogRows);
                GetFilterValues(this.LogRows);
                ToggleFilterFields(true);
            } else
            {
                MessageBox.Show("You must save Application Insights applicationId and apikey first!", "Simple log querier");
            }
            
        }

        private void AddResultToTxtBox(List<LogRow> logRows)
        {
            string result = logRows.Select(row => row.ToString()).Aggregate("", (acc, x) => acc +"\n" + x);
            ResultTxtBox.Text = result;
            ResultTxtBox.IsReadOnly = true;
        }

        private AppInsightClient CheckSettings()
        {
            if ((AzureAppInsightsSimpleQuery.AppSettings.Default.AppId != null && AzureAppInsightsSimpleQuery.AppSettings.Default.AppId.Length > 0)
             &&
             (AzureAppInsightsSimpleQuery.AppSettings.Default.ApiKey != null && AzureAppInsightsSimpleQuery.AppSettings.Default.ApiKey.Length > 0))
            {
                return new AppInsightClient(AzureAppInsightsSimpleQuery.AppSettings.Default.AppId, AzureAppInsightsSimpleQuery.AppSettings.Default.ApiKey);
            } else
            {
                return null;
            }

        }

        private void GetFilterValues(List<LogRow> logRows)
        {
            try
            {
                this.LoggerNames = logRows.Select(row => row.LoggerName)
                .Distinct().ToList();

                this.loggerNames_combo.ItemsSource = LoggerNames;
                this.loggingLevel_combo.DataContext = this;
                this.LoggingLevels = logRows.Select(row => row.LoggingLevel)
                    .Distinct().ToList();
                this.loggingLevel_combo.ItemsSource = this.LoggingLevels;
                this.loggingLevel_combo.DataContext = this;
                this.OperationNames = logRows.Select(row => row.OperationName)
                    .Distinct().ToList();
                this.CanFilter = true;
            } catch(Exception exp)
            {
                this.CanFilter = false;
            }
            
        }

        private void FilterValues()
        {
            if(this.LogRows != null)
            {
                var rowsToFilter = this.FilteredLogRows != null ? this.FilteredLogRows : this.LogRows;

                if(this.loggerNames_combo.SelectedItem != null)
                {
                    this.FilteredLogRows = rowsToFilter.Where(row => row.LoggerName.Equals(this.loggerNames_combo.SelectedItem.ToString())).ToList();
                }
                if(this.loggingLevel_combo.SelectedItem != null)
                {
                    this.FilteredLogRows = this.FilteredLogRows != null ? this.FilteredLogRows : this.LogRows;
                    this.FilteredLogRows = this.FilteredLogRows.Where(row => row.LoggingLevel.Equals(this.loggingLevel_combo.SelectedItem.ToString())).ToList();
                }
                if (this.FilteredLogRows != null)
                {
                    this.AddResultToTxtBox(this.FilteredLogRows);
                }
                
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new Settings();
            settingsWindow.Show();
        }

  

        private void filterBtn_click(object sender, RoutedEventArgs e)
        {
            FilterValues();
        }

        private void search_TxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchMessage(this.search_TxtBox.Text);
        }

        private void searchMessage(String searchStr)
        {
            if((this.FilteredLogRows != null || this.LogRows != null) && searchStr != null)
            {

                var rowsToSearch = this.FilteredLogRows != null ? this.FilteredLogRows : this.LogRows;

                var searchRows = rowsToSearch.Where(row => row.Message.Contains(searchStr)).ToList();

                

                this.AddResultToTxtBox(searchRows);

            } else
            {
                if(this.FilteredLogRows != null)
                {
                    this.AddResultToTxtBox(this.FilteredLogRows);
                }
                
            }
        }

        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            this.loggerNames_combo.SelectedItem = null;
            this.loggingLevel_combo.SelectedItem = null;
            this.search_TxtBox.Text = null;
            this.FilteredLogRows = null;
            if(this.LogRows != null)
            {
                this.AddResultToTxtBox(this.LogRows);
            }
        }

   
        private void loggingLevel_combo_Selected(object sender, RoutedEventArgs e)
        {
            if (this.LogRows != null && this.loggingLevel_combo.SelectedItem != null)
            {
                var rowsToFilter = this.FilteredLogRows != null ? this.FilteredLogRows : this.LogRows;
                this.FilteredLogRows = rowsToFilter.Where(row => row.LoggingLevel.Equals(this.loggingLevel_combo.SelectedItem.ToString())).ToList();
                this.AddResultToTxtBox(this.FilteredLogRows);
            }
        }

        private void loggerNames_combo_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (this.LogRows != null && this.loggerNames_combo.SelectedItem != null)
            {
                var rowsToFilter = this.FilteredLogRows != null ? this.FilteredLogRows : this.LogRows;
                this.FilteredLogRows = rowsToFilter.Where(row => row.LoggerName.Equals(this.loggerNames_combo.SelectedItem.ToString())).ToList();
                this.AddResultToTxtBox(this.FilteredLogRows);
            }
        }
    }

}
