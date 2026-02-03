using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace DevelopmentEnvironmentQuicklaunch
{
    public partial class SettingsWindow : Window
    {
        public UserSettings Settings { get; private set; }
        public ObservableCollection<string> AdditionalApps { get; set; }

        public SettingsWindow(UserSettings settings)
        {
            InitializeComponent();
            Settings = settings;
            AdditionalApps = new ObservableCollection<string>(settings.AdditionalApplications);

            VisualStudioPathBox.Text = settings.VisualStudioPath;
            SSMSPathBox.Text = settings.SqlServerManagementStudioPath;
            TextEditorPathBox.Text = settings.TextEditorPath;
            DisableSSMSIfOpenCheckBox.IsChecked = settings.DisableSSMSIfOpen;
            LaunchTextEditorCheckBox.IsChecked = settings.LaunchTextEditor;
            LaunchOnStartupCheckBox.IsChecked = settings.LaunchOnStartup;
            LaunchAsAdminCheckBox.IsChecked = settings.LaunchAsAdministrator;
            AdditionalAppsListBox.ItemsSource = AdditionalApps;            
        }

        private void BrowseVS_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Executable (*.exe)|*.exe" };
            if (dlg.ShowDialog() == true)
                VisualStudioPathBox.Text = dlg.FileName;
        }

        private void BrowseSSMS_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Executable (*.exe)|*.exe" };
            if (dlg.ShowDialog() == true)
                SSMSPathBox.Text = dlg.FileName;
        }

        private void BrowseTextEditor_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Executable (*.exe)|*.exe" };
            if (dlg.ShowDialog() == true)
                TextEditorPathBox.Text = dlg.FileName;
        }

        private void AddApp_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog { Filter = "Executable (*.exe)|*.exe" };
            if (dlg.ShowDialog() == true && !AdditionalApps.Contains(dlg.FileName))
                AdditionalApps.Add(dlg.FileName);
        }

        private void RemoveApp_Click(object sender, RoutedEventArgs e)
        {
            if (AdditionalAppsListBox.SelectedItem is string app)
                AdditionalApps.Remove(app);
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Settings.VisualStudioPath = VisualStudioPathBox.Text;
            Settings.SqlServerManagementStudioPath = SSMSPathBox.Text;
            Settings.TextEditorPath = TextEditorPathBox.Text;
            Settings.DisableSSMSIfOpen = DisableSSMSIfOpenCheckBox.IsChecked == true;
            Settings.LaunchTextEditor = LaunchTextEditorCheckBox.IsChecked == true;
            Settings.LaunchOnStartup = LaunchOnStartupCheckBox.IsChecked == true;
            Settings.LaunchAsAdministrator = LaunchAsAdminCheckBox.IsChecked == true;
            Settings.AdditionalApplications = new System.Collections.Generic.List<string>(AdditionalApps);            
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}