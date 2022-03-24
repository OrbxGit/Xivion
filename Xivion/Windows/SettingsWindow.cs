using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Xivion
{
    public class SettingsWindow : Window
    {
        // Panels
        private readonly Grid _mainGrid;
        private readonly StackPanel _checkPanel;

        // Controls
        private readonly CheckBox _alwaysOnTopBox;
        private readonly CheckBox _autoAttachBox;
        private readonly CheckBox _autoExecuteBox;

        private readonly Button _killRobloxButton;

        public SettingsWindow()
        {
            Title = nameof(SettingsWindow);
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.CanMinimize;
            Topmost = Settings.Default.AlwaysOnTop;

            Width = 300;
            Height = 330;

            _mainGrid = new Grid()
            { 
                Margin = new Thickness(5)
            };
            _checkPanel = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Orientation = Orientation.Vertical,
            };
            
            _alwaysOnTopBox = new CheckBox()
            { 
                Content = "Top Most"
            };
            _autoAttachBox = new CheckBox()
            { 
                Content = "Auto Attach"
            };
            _autoExecuteBox = new CheckBox()
            { 
                Content = "Auto Execute"
            };
            _killRobloxButton = new Button()
            {
                Content = "Kill Roblox",
                Margin = new Thickness(0, 20, 0, 0)
            };
            _killRobloxButton.Click += _killRobloxButton_Click;

            _mainGrid.Children.Add(_checkPanel);

            _checkPanel.Children.Add(_alwaysOnTopBox);
            _checkPanel.Children.Add(_autoAttachBox);
            _checkPanel.Children.Add(_autoExecuteBox);
            _checkPanel.Children.Add(_killRobloxButton);

            AddChild(_mainGrid);

            Loaded += SettingsWindow_Loaded;
            Closing += SettingsWindow_Closing;
        }

        private void _killRobloxButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Process rProc in Process.GetProcessesByName("RobloxPlayerBeta"))
                rProc.Kill();
        }

        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _alwaysOnTopBox.IsChecked = Settings.Default.AlwaysOnTop;
            _autoAttachBox.IsChecked = Settings.Default.AutoAttach;
            _autoExecuteBox.IsChecked = Settings.Default.AutoExecute;
        }
        private void SettingsWindow_Closing(object sender, CancelEventArgs e)
        {
            if (e.Cancel)
                return;

            Settings.Default.AlwaysOnTop = _alwaysOnTopBox.IsChecked.GetValueOrDefault();
            Settings.Default.AutoAttach = _autoAttachBox.IsChecked.GetValueOrDefault();
            Settings.Default.AutoExecute = _autoExecuteBox.IsChecked.GetValueOrDefault();

            foreach (Window window in Application.Current.Windows)
                window.Topmost = Settings.Default.AlwaysOnTop || _alwaysOnTopBox.IsChecked.GetValueOrDefault();
        }
    }
}
