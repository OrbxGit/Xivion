using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using System.Windows.Controls;
using WeAreDevs_API;
using Microsoft.Win32;

namespace Xivion
{
    public class MainWindow : Window
    {
        private readonly Grid _mainGrid;
        private readonly Grid _editorGrid;
        private readonly Grid _buttonsGrid;
        private readonly GridSplitter _splitter;

        private readonly GridLengthConverter _gridLengthConverter;
        private readonly StackPanel _buttonLeftPanel;
        private readonly StackPanel _buttonRightPanel;

        private readonly TextEditor _textEditor;
        private readonly ListBox _scriptList;

        private readonly ContextMenu _scriptListMenu;
        private readonly MenuItem _loadMenu;
        private readonly MenuItem _executeMenu;

        private readonly Button _executeButton;
        private readonly Button _clearButton;
        private readonly Button _openFileButton;
        private readonly Button _saveFileButton;
        private readonly Button _attachButton;

        private readonly FileSystemWatcher _scriptWatcher;
        
        private string ScriptsPath => $"{Environment.CurrentDirectory}\\Scripts";
        private ListBoxItem SelectedItem => _scriptList.SelectedItem as ListBoxItem;
        private ExploitAPI WRDAPI => new ExploitAPI();

        public MainWindow()
        {
            // Window properties
            Title = "MainWindow";
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Width = 500;
            Height = 350;

            MinWidth = (Width / 2) * 1.5;
            MinHeight = (Height / 2) * 1.5;

            // Instance
            _mainGrid = new Grid
            {
                Margin = new Thickness(5)
            };
            _editorGrid = new Grid();
            _buttonsGrid = new Grid()
            {
                Margin = new Thickness(0, 5, 0, 0)
            };
            _splitter = new GridSplitter()
            {
                Margin = new Thickness(0, 0, -5, 0),
                ResizeDirection = GridResizeDirection.Columns,
                Width = 5,
            };

            _gridLengthConverter = new GridLengthConverter();

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
                VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
                Padding = new Thickness(3),
            };
            _scriptListMenu = new ContextMenu();

            _loadMenu = new MenuItem()
            { Header = "Load" };
            _executeMenu = new MenuItem()
            { Header = "Execute" };

            _scriptList = new ListBox()
            { Margin = new Thickness(5, 0, 0, 0), Padding = new Thickness(3), };

            _scriptList.ContextMenu = _scriptListMenu;

            _scriptListMenu.Items.Add(_loadMenu);
            _scriptListMenu.Items.Add(_executeMenu);

            _loadMenu.Click += LoadMenu_Click;
            _executeMenu.Click += ExecuteMenu_Click;

            // Buttons
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
            _attachButton = new Button()
            {
                Content = "Attach",
                Padding = new Thickness(8, 3, 8, 3)
            };

            _scriptWatcher = new FileSystemWatcher(ScriptsPath)
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = true
            };

            _scriptWatcher.Changed += OnScript_Changed;
            _scriptWatcher.Created += OnScript_Changed;
            _scriptWatcher.Deleted += OnScript_Changed;
            _scriptWatcher.Renamed += OnScript_Changed;

            void OnScript_Changed(object _, FileSystemEventArgs __)
                => Dispatcher.Invoke(RelocateScripts);

            OnScript_Changed(null, null);

            Button[] utilityButton =
                { _executeButton, _clearButton,
                _openFileButton, _saveFileButton, _attachButton };

            for (int i = 0; i < utilityButton.Length; i++)
                utilityButton[i].Click += UtilityButton_Click;

            // Rows & Columns
            _mainGrid.RowDefinitions.Add(new RowDefinition());
            _mainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            _mainGrid.Children.Add(_editorGrid);
            _mainGrid.Children.Add(_buttonsGrid);

            // Set rows
            Grid.SetRow(_editorGrid, 0);
            Grid.SetRow(_buttonsGrid, 1);

            _editorGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = (GridLength)_gridLengthConverter.ConvertFromString("2*") });
            _editorGrid.ColumnDefinitions.Add(new ColumnDefinition());

            // Controls
            _editorGrid.Children.Add(_textEditor);
            _editorGrid.Children.Add(_scriptList);
            _editorGrid.Children.Add(_splitter);

            Grid.SetColumn(_textEditor, 0);
            Grid.SetColumn(_scriptList, 1);

            _buttonsGrid.Children.Add(_buttonRightPanel);
            _buttonsGrid.Children.Add(_buttonLeftPanel);

            _buttonRightPanel.Children.Add(_attachButton);

            _buttonLeftPanel.Children.Add(_executeButton);
            _buttonLeftPanel.Children.Add(_clearButton);
            _buttonLeftPanel.Children.Add(_openFileButton);
            _buttonLeftPanel.Children.Add(_saveFileButton);

            AddChild(_mainGrid);
        }

        private void RelocateScripts()
        {
            _scriptList.Items.Clear();

            var dirInfo = new DirectoryInfo(ScriptsPath);
            string[] fileType = { "*.txt", "*.lua" };

            foreach (var fType in fileType)
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
        }

        private void ExecuteMenu_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null)
                return;

            WRDAPI.SendLuaScript(File.ReadAllText(SelectedItem.Tag.ToString()));
        }
        private void LoadMenu_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null)
                return;

            _textEditor.Load(SelectedItem.Tag.ToString());
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
            else if (BUTTON_IDENTIFIER == _attachButton.GetHashCode())
                WRDAPI.LaunchExploit();
        }
    }
}