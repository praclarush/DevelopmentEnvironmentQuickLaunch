using IWshRuntimeLibrary;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using File = System.IO.File;

namespace DevelopmentEnvironmentQuicklaunch
{
    /// <summary>
    /// Main window for the Development Environment Quicklaunch application.
    /// Provides UI and logic for managing and launching Visual Studio solutions,
    /// SQL Server Management Studio, a text editor, and additional applications.
    /// Also handles user settings persistence and application startup registration.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Path to the user settings JSON file in AppData.
        /// </summary>
        private static readonly string SettingsPath =
            Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
                         "DevelopmentEnvironmentQuicklaunch", "usersettings.json");

        /// <summary>
        /// Collection of solution file paths displayed in the UI.
        /// </summary>
        public ObservableCollection<string> SolutionFiles { get; set; } = new();

        /// <summary>
        /// Holds the current user settings.
        /// </summary>
        private UserSettings _settings = new();

        /// <summary>
        /// Initializes the main window, loads settings, and updates startup shortcut.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            SolutionFilesListBox.ItemsSource = SolutionFiles;
            LoadSettings();
            UpdateStartupShortcut(_settings.LaunchOnStartup);
        }

        /// <summary>
        /// Handles the Add Solution button click event.
        /// Opens a file dialog to add one or more .sln files to the solution list.
        /// </summary>
        private void AddSolutionButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Visual Studio Solution (*.sln)|*.sln",
                Multiselect = true
            };
            if (dlg.ShowDialog() == true)
            {
                foreach (var file in dlg.FileNames)
                {
                    if (!SolutionFiles.Contains(file))
                        SolutionFiles.Add(file);
                }
                SaveSettings();
            }
        }

        /// <summary>
        /// Handles the Remove Selected button click event.
        /// Removes selected solution files from the list.
        /// </summary>
        private void RemoveSolutionButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = SolutionFilesListBox.SelectedItems;
            for (int i = selected.Count - 1; i >= 0; i--)
            {
                SolutionFiles.Remove((string)selected[i]);
            }
            SaveSettings();
        }

        /// <summary>
        /// Handles the Start button click event.
        /// Launches all configured applications and selected solution files.
        /// </summary>
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            bool runAsAdmin = _settings.LaunchAsAdministrator;

            // Launch Visual Studio with each solution
            if (!string.IsNullOrWhiteSpace(_settings.VisualStudioPath) && File.Exists(_settings.VisualStudioPath))
            {
                foreach (var sln in SolutionFiles)
                {
                    TryLaunchProcess(_settings.VisualStudioPath, $"\"{sln}\"", runAsAdmin, "Error Launching Solution");
                }
            }

            // Launch SSMS if enabled and not already open
            if (!string.IsNullOrWhiteSpace(_settings.SqlServerManagementStudioPath) &&
                File.Exists(_settings.SqlServerManagementStudioPath))
            {
                bool ssmsOpen = Process.GetProcessesByName(
                    Path.GetFileNameWithoutExtension(_settings.SqlServerManagementStudioPath)).Any();

                if (!_settings.DisableSSMSIfOpen || !ssmsOpen)
                {
                    TryLaunchProcess(_settings.SqlServerManagementStudioPath, null, runAsAdmin, "Error Launching SSMS");
                }
            }

            // Launch text editor if enabled
            if (_settings.LaunchTextEditor &&
                !string.IsNullOrWhiteSpace(_settings.TextEditorPath) &&
                File.Exists(_settings.TextEditorPath))
            {
                TryLaunchProcess(_settings.TextEditorPath, null, runAsAdmin, "Error Launching Text Editor");
            }

            // Launch additional applications
            if (_settings.AdditionalApplications != null)
            {
                foreach (var app in _settings.AdditionalApplications)
                {
                    if (File.Exists(app))
                    {
                        TryLaunchProcess(app, null, runAsAdmin, "Error Launching Additional Application");
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Settings button click event.
        /// Opens the settings window and saves settings if changed.
        /// Also updates the startup shortcut if the launch on startup setting changes.
        /// </summary>
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsCopy = JsonSerializer.Deserialize<UserSettings>(
                JsonSerializer.Serialize(_settings)); // Deep copy
            var dlg = new SettingsWindow(settingsCopy);
            if (dlg.ShowDialog() == true)
            {
                _settings = dlg.Settings;
                SaveSettings();
                UpdateStartupShortcut(_settings.LaunchOnStartup);
            }
        }

        /// <summary>
        /// Loads user settings from disk and populates the solution list.
        /// </summary>
        private void LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    var json = File.ReadAllText(SettingsPath);
                    _settings = JsonSerializer.Deserialize<UserSettings>(json) ?? new UserSettings();
                    SolutionFiles.Clear();
                    foreach (var file in _settings.SolutionFiles)
                        SolutionFiles.Add(file);
                }
            }
            catch (Exception ex)
            {
                _settings = new UserSettings();
            }
        }

        /// <summary>
        /// Saves user settings to disk.
        /// </summary>
        private void SaveSettings()
        {
            try
            {
                var dir = Path.GetDirectoryName(SettingsPath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                _settings.SolutionFiles = new System.Collections.Generic.List<string>(SolutionFiles);
                var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsPath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Saving Settings", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Adds or removes a shortcut to the application in the Windows Startup folder
        /// based on the launch on startup setting.
        /// </summary>
        /// <param name="enable">If true, adds the shortcut; otherwise, removes it.</param>
        private void UpdateStartupShortcut(bool enable)
        {
            try
            {
                string startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                string shortcutPath = Path.Combine(startupFolder, "DevelopmentEnvironmentQuicklaunch.lnk");
                string exePath = Assembly.GetExecutingAssembly().Location;

                if (enable)
                {
                    // Create shortcut
                    var shell = new WshShell();
                    var shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                    shortcut.TargetPath = exePath;
                    shortcut.WorkingDirectory = Path.GetDirectoryName(exePath);
                    shortcut.Save();
                }
                else
                {
                    // Remove shortcut if it exists
                    if (File.Exists(shortcutPath))
                        File.Delete(shortcutPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Startup Shortcut Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles the context menu "Launch" action for an individual solution file.
        /// Launches the selected solution file in Visual Studio.
        /// </summary>
        private void LaunchSolutionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (SolutionFilesListBox.SelectedItem is string slnFile &&
                !string.IsNullOrWhiteSpace(_settings.VisualStudioPath) &&
                File.Exists(_settings.VisualStudioPath) &&
                File.Exists(slnFile))
            {
                bool runAsAdmin = _settings.LaunchAsAdministrator;
                TryLaunchProcess(_settings.VisualStudioPath, $"\"{slnFile}\"", runAsAdmin, "Error Launching Solution");
            }
        }

        /// <summary>
        /// Helper to launch a process with error handling and optional arguments/admin.
        /// </summary>
        /// <param name="exePath">Path to the executable.</param>
        /// <param name="arguments">Arguments to pass to the process.</param>
        /// <param name="runAsAdmin">Whether to launch as administrator.</param>
        /// <param name="errorTitle">Title for the error dialog if launch fails.</param>
        private void TryLaunchProcess(string exePath, string? arguments, bool runAsAdmin, string errorTitle)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = exePath,
                    UseShellExecute = true,
                    Verb = runAsAdmin ? "runas" : ""
                };
                if (!string.IsNullOrWhiteSpace(arguments))
                    psi.Arguments = arguments;

                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, errorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}