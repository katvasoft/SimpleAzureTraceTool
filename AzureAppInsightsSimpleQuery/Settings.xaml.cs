using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AzureAppInsightsSimpleQuery
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            CheckAppSettings();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CheckAppSettings()
        {
            if ((AzureAppInsightsSimpleQuery.AppSettings.Default.AppId != null && AzureAppInsightsSimpleQuery.AppSettings.Default.AppId.Length > 0)
                &&
                (AzureAppInsightsSimpleQuery.AppSettings.Default.ApiKey != null && AzureAppInsightsSimpleQuery.AppSettings.Default.ApiKey.Length > 0))
            {
                aiAppId_txt.Text = AzureAppInsightsSimpleQuery.AppSettings.Default.AppId;
                aiApiKey_txt.Text = AzureAppInsightsSimpleQuery.AppSettings.Default.ApiKey;
                try {
                    int maxResults = AzureAppInsightsSimpleQuery.AppSettings.Default.MaxResults;
                    aiMaxResults_txt.Text = maxResults.ToString();
                    
                } catch (Exception e)
                {

                }
  
            }

        }

        private void Save_btn_Click(object sender, RoutedEventArgs e)
        {
            var appId = aiAppId_txt.Text;
            var apiKey = aiApiKey_txt.Text;
            

            AzureAppInsightsSimpleQuery.AppSettings.Default.ApiKey = apiKey;
            AzureAppInsightsSimpleQuery.AppSettings.Default.AppId = appId;

            try
            {
                int maxResults = Int32.Parse(aiMaxResults_txt.Text);
                AzureAppInsightsSimpleQuery.AppSettings.Default.MaxResults = maxResults;
            } catch(Exception ee)
            {

            }

            AzureAppInsightsSimpleQuery.AppSettings.Default.Save();

            MessageBox.Show("Settings saved", "Simple log querier");
            this.Close();
        }
    }
}
