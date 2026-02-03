using System.Collections.Generic;

namespace DevelopmentEnvironmentQuicklaunch
{
    public class UserSettings
    {
        public List<string> SolutionFiles { get; set; } = new();
        public string VisualStudioPath { get; set; }
        public string SqlServerManagementStudioPath { get; set; }
        public string TextEditorPath { get; set; }
        public bool LaunchTextEditor { get; set; }
        public bool DisableSSMSIfOpen { get; set; }
        public bool LaunchOnStartup { get; set; }
        public List<string> AdditionalApplications { get; set; } = new();
        public bool LaunchAsAdministrator { get; set; }
    }
}