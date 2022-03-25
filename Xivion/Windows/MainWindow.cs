using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Diagnostics;
using Microsoft.Win32;
using WeAreDevs_API;
using System.Collections.Generic;

namespace Xivion
{
    public class MainWindow : Window
    {
        #region Properties

        // Panels
        private readonly Grid _mainGrid;
        private readonly Grid _editorGrid;
        private readonly Grid _buttonsGrid;
        private readonly Grid _scriptListGrid;

        private readonly GridSplitter _splitter;
        private readonly StackPanel _buttonLeftPanel;
        private readonly StackPanel _buttonRightPanel;

        // Controls
        private readonly TextEditor _textEditor;

        private readonly TextBox _searchTextBox;
        private readonly ListBox _scriptList;

        private readonly MenuItem _loadMenu;
        private readonly MenuItem _executeMenu;

        private readonly Button _executeButton;
        private readonly Button _clearButton;
        private readonly Button _openFileButton;
        private readonly Button _saveFileButton;

        private readonly Button _settingsButton;
        private readonly Button _attachButton;

        // Service
        private readonly FileSystemWatcher _scriptWatcher;
        private readonly DispatcherTimer _autoAttachTimer;

        // Properties
        private string ScriptsPath => $"{Environment.CurrentDirectory}\\Scripts";
        private string AutoExecPath => $"{Environment.CurrentDirectory}\\AutoExec";
        private string[] SupportedFileType => new[] { "*.txt", "*.lua", "*.luau" };

        private ListBoxItem SelectedItem => _scriptList.SelectedItem as ListBoxItem;
        private ExploitAPI WRDAPI => new ExploitAPI();
        private List<ListBoxItem> _scriptItemList;

        #endregion

        #region Constructor

        /// <summary>
        /// A constructor for <see cref="MainWindow"/> class
        /// </summary>
        public MainWindow()
        {
            Title = nameof(MainWindow);
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Topmost = Settings.Default.AlwaysOnTop;

            Width = 500;
            Height = 330;

            MinWidth = Width;
            MinHeight = Height;

            _mainGrid = new Grid
            {
                Margin = new Thickness(8)
            };

            _editorGrid = new Grid();

            _buttonsGrid = new Grid()
            {
                Margin = new Thickness(0, 8, 0, 0)
            };

            _scriptListGrid = new Grid()
            {
                Margin = new Thickness(8, 0, 0, 0)
            };

            _splitter = new GridSplitter()
            {
                Margin = new Thickness(0, 0, -8, 0),
                ResizeDirection = GridResizeDirection.Columns,
                Width = 8,
                Background = Brushes.Transparent
            };

            // Panels
            _buttonRightPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            _buttonLeftPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            _textEditor = new TextEditor()
            {
                Options = { AllowScrollBelowDocument = true, EnableHyperlinks = false },
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.LightGray,
                ShowLineNumbers = true,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 16,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Padding = new Thickness(3),
            };

            _loadMenu = new MenuItem()
            { 
                Header = "Load" 
            };

            _executeMenu = new MenuItem()
            { 
                Header = "Execute"
            };

            _searchTextBox = new TextBox()
            {
                Padding = new Thickness(2),
            };

            _scriptList = new ListBox()
            { 
                Margin = new Thickness(0, 8, 0, 0),
                Padding = new Thickness(3)
            };

            _scriptList.ContextMenu = new ContextMenu();
            _scriptList.ContextMenu.Items.Add(_loadMenu);
            _scriptList.ContextMenu.Items.Add(_executeMenu);

            _loadMenu.Click += LoadMenu_Click;
            _executeMenu.Click += ExecuteMenu_Click;
            _searchTextBox.TextChanged += SearchTextBox_TextChanged;

            _executeButton = new Button()
            {
                Content = "Execute",
                Padding = new Thickness(8, 3, 8, 3)
            };

            _clearButton = new Button()
            { 
                Content = "Clear",
                Margin = new Thickness(5, 0, 0, 0),
                Padding = new Thickness(8, 3, 8, 3)
            };

            _openFileButton = new Button()
            {
                Content = "Open",
                Margin = new Thickness(5, 0, 0, 0),
                Padding = new Thickness(8, 3, 8, 3)
            };

            _saveFileButton = new Button()
            {
                Content = "Save",
                Margin = new Thickness(5, 0, 0, 0),
                Padding = new Thickness(8, 3, 8, 3)
            };

            _settingsButton = new Button()
            {
                Content = "Settings",
                Padding = new Thickness(8, 3, 8, 3)
            };

            _attachButton = new Button()
            {
                Content = "Attach",
                Margin = new Thickness(5, 0, 0, 0),
                Padding = new Thickness(8, 3, 8, 3)
            };

            _scriptWatcher = new FileSystemWatcher(ScriptsPath)
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = true
            };

            _autoAttachTimer = new DispatcherTimer()
            { Interval = TimeSpan.FromSeconds(8) };

            int PID = 0;
            _autoAttachTimer.Tick += async (_, __) =>
            {
                if (!Settings.Default.AutoAttach)
                    return;

                Process[] rProcs = Process.GetProcessesByName("RobloxPlayerBeta");
                if (rProcs.Length < 1 || rProcs[0].MainWindowHandle == IntPtr.Zero)
                    return;

                if (rProcs[0].Id == PID || WRDAPI.isAPIAttached())
                    return;

                PID = rProcs[0].Id;
                await Task.Delay(3000);
                WRDAPI.LaunchExploit();
            };
            _autoAttachTimer.Start();

            _scriptWatcher.Changed += OnScript_Changed;
            _scriptWatcher.Created += OnScript_Changed;
            _scriptWatcher.Deleted += OnScript_Changed;
            _scriptWatcher.Renamed += OnScript_Changed;

            OnScript_Changed(null, null);
            void OnScript_Changed(object _, FileSystemEventArgs __)
                => Dispatcher.Invoke(RelocateScripts);

            Button[] utilityButton = {
                _executeButton, _clearButton,
                _openFileButton, _saveFileButton,
                _settingsButton, _attachButton 
            };

            for (int i = 0; i < utilityButton.Length; i++)
                utilityButton[i].Click += UtilityButton_Click;

            _mainGrid.RowDefinitions.Add(new RowDefinition());
            _mainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            _editorGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridHelper.GetGridLengthFromString("2*") });
            _editorGrid.ColumnDefinitions.Add(new ColumnDefinition());

            _mainGrid.Children.Add(_editorGrid);
            _mainGrid.Children.Add(_buttonsGrid);

            _editorGrid.Children.Add(_textEditor);
            _editorGrid.Children.Add(_scriptListGrid);
            _editorGrid.Children.Add(_splitter);

            _scriptListGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(26) });
            _scriptListGrid.RowDefinitions.Add(new RowDefinition());

            _scriptListGrid.Children.Add(_searchTextBox);
            _scriptListGrid.Children.Add(_scriptList);

            _searchTextBox.SetRow(0);
            _scriptList.SetRow(1);

            _buttonsGrid.Children.Add(_buttonRightPanel);
            _buttonsGrid.Children.Add(_buttonLeftPanel);

            _buttonRightPanel.Children.Add(_settingsButton);
            _buttonRightPanel.Children.Add(_attachButton);

            _buttonLeftPanel.Children.Add(_executeButton);
            _buttonLeftPanel.Children.Add(_clearButton);
            _buttonLeftPanel.Children.Add(_openFileButton);
            _buttonLeftPanel.Children.Add(_saveFileButton);

            _editorGrid.SetRow(0);
            _buttonsGrid.SetRow(1);

            _textEditor.SetColumn(0);
            _scriptListGrid.SetColumn(1);

            AddChild(_mainGrid);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(_searchTextBox.Text) ||
                string.IsNullOrWhiteSpace(_searchTextBox.Text))
            {
                RelocateScripts();
                return;
            }

            _scriptList.Items.Clear();

            foreach (ListBoxItem script in _scriptItemList)
            {
                if (script.Content.ToString().ToLower().Contains(_searchTextBox.Text.ToLower()))
                    _scriptList.Items.Add(script);
            }
        }

        #endregion

        #region Helpers

        private void RelocateScripts()
        {
            _scriptList.Items.Clear();
            _scriptItemList = new List<ListBoxItem>();

            var dirInfo = new DirectoryInfo(ScriptsPath);

            foreach (var fType in SupportedFileType)
            {
                foreach (var script in dirInfo.GetFiles(fType, SearchOption.AllDirectories))
                {
                    ListBoxItem scriptItem = new ListBoxItem()
                    {
                        Content = script.Name,
                        Tag = script.FullName // path
                    };

                    _scriptList.Items.Add(scriptItem);
                }
            }

            foreach (ListBoxItem item in _scriptList.Items)
                _scriptItemList.Add(item);
        }

        #endregion

        #region Events

        private void LoadMenu_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null)
                return;

            _textEditor.Load(SelectedItem.Tag.ToString());
        }

        private void ExecuteMenu_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null)
                return;

            WRDAPI.SendLuaScript(File.ReadAllText(SelectedItem.Tag.ToString()));
        }

        private void UtilityButton_Click(object sender, RoutedEventArgs e)
        {
            int BUTTON_IDENTIFIER = sender.GetHashCode();

            if (BUTTON_IDENTIFIER == _executeButton.GetHashCode())
                WRDAPI.SendLuaScript(_textEditor.Text);
            else if (BUTTON_IDENTIFIER == _clearButton.GetHashCode())
                _textEditor.Clear();
            else if (BUTTON_IDENTIFIER == _openFileButton.GetHashCode())
            {
                var OpenDialog = new OpenFileDialog()
                {
                    Title = "Open File",
                    Filter = "Txt files (*.txt)|*.txt|Lua files (*.lua)|*.lua"
                };

                if (OpenDialog.ShowDialog().GetValueOrDefault())
                    _textEditor.Load(OpenDialog.FileName);
            }
            else if (BUTTON_IDENTIFIER == _saveFileButton.GetHashCode())
            {
                var SaveDialog = new SaveFileDialog()
                {
                    Title = "Save File",
                    Filter = "Txt files (*.txt)|*.txt|Lua files (*.lua)|*.lua"
                };

                if (SaveDialog.ShowDialog().GetValueOrDefault())
                    _textEditor.Save(SaveDialog.FileName);
            }
            else if (BUTTON_IDENTIFIER == _settingsButton.GetHashCode())
                new SettingsWindow() { Owner = this }.ShowDialog();
            else if (BUTTON_IDENTIFIER == _attachButton.GetHashCode())
            {
                WRDAPI.LaunchExploit();
                
                if (!Settings.Default.AutoExecute)
                    return;

                Task.Factory.StartNew(() =>
                {
                    while (!WRDAPI.isAPIAttached()) { }

                    foreach (var fType in SupportedFileType)
                        foreach (var file in Directory.GetFiles(AutoExecPath, fType, SearchOption.AllDirectories))
                            WRDAPI.SendLuaScript(File.ReadAllText(file));
                });
            }
        }

        #endregion
    }
}