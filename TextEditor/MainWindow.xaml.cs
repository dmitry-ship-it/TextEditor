using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TextEditor.Extensions;
using TextEditor.TextHandling;
using TextEditor.Resources;
using TextEditor.Visual;
using System;

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ITextHandler _handler;
        private readonly EditorSettings _editorSettings;
        private SettingsWindow _settingsWindow;

        public MainWindow(EditorSettings editorSettings)
        {
            _handler = new NoFileHandler();
            _editorSettings = editorSettings;
            _settingsWindow = new SettingsWindow(_editorSettings);

            SubscribeEvents();
            InitializeComponent();

            LoadSettings();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void LoadSettings()
        {
            ChangeFont(_editorSettings.Font.ToFontFamily());
            ChangeFontSize(_editorSettings.FontSize);
        }

        private void SubscribeEvents()
        {
            _settingsWindow.OnFontChange += ChangeFont;
            _settingsWindow.OnFontSizeChange += ChangeFontSize;
        }

        private void ChangeFont(FontFamily fontFamily)
        {
            EditorContent.FontFamily = fontFamily;
        }

        private void ChangeFontSize(double fontSize)
        {
            EditorContent.FontSize = fontSize;
            FontSizeBlock.Text = fontSize.ToString();
        }

        private void CreateNewFile_Click(object sender, RoutedEventArgs e)
        {
            _handler = new NoFileHandler();
            EditorContent.Text = _handler.ReadAllText();
            EditorContent.Focus();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = FileHandler.CreateFileDialog(DialogType.Open);

            if (dialog.ShowDialog() is true)
            {
                _handler = new FileHandler(dialog.FileName);
                EditorContent.Text = _handler.ReadAllText();
                StatusBlock.SetColoredText("File opened.", Brushes.Green);
            }
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_handler is NoFileHandler)
            {
                var dialog = FileHandler.CreateFileDialog(DialogType.Save);

                if (dialog.ShowDialog() is true)
                {
                    _handler = new FileHandler(dialog.FileName);
                }
            }

            if (_handler.IsAvailable)
            {
                _handler.WriteAllText(EditorContent.Text);
                StatusBlock.SetColoredText("File saved.", Brushes.Green);
            }
        }

        private void KeyBinding_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers is ModifierKeys.Control &&
                ((EditorContent.FontSize >= 12 && e.Delta < 0) || (EditorContent.FontSize <= 100 && e.Delta > 0)))
            {
                EditorContent.FontSize += e.Delta / 30d;
                FontSizeBlock.Text = EditorContent.FontSize.ToString();
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            _settingsWindow = new SettingsWindow(_editorSettings);
            SubscribeEvents();
            _settingsWindow.Show();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            _settingsWindow?.Close();
            Close();
        }
    }
}
