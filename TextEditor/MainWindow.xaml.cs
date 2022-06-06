using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TextEditor.Extensions;
using TextEditor.TextHandling;
using TextEditor.Resources;
using TextEditor.Visual;
using System;
using System.IO;
using System.ComponentModel;

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

        protected override async void OnClosing(CancelEventArgs e)
        {
            if ((_handler is NoFileHandler || await _handler.ReadAllTextAsync() != EditorContent.Text)
                && !string.IsNullOrEmpty(EditorContent.Text))
            {
                var msgBoxResult = MessageBox.Show(
                    messageBoxText: "Save unsaved content?",
                    caption: "Saveing",
                    button: MessageBoxButton.YesNoCancel,
                    icon: MessageBoxImage.Warning);

                if (msgBoxResult == MessageBoxResult.Yes)
                {
                    SaveCommand_Executed(this, null!);
                }

                e.Cancel = msgBoxResult == MessageBoxResult.Cancel;
            }

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void LoadSettings()
        {
            // file settings
            ChangeFont(_editorSettings.Font.ToFontFamily());
            ChangeFontSize(_editorSettings.FontSize);
            SwitchTextWrap(_editorSettings.TextWrap);
            SwitchStatusBar(_editorSettings.StatusBar);
        }

        private void SubscribeEvents()
        {
            _settingsWindow.OnFontChange += ChangeFont;
            _settingsWindow.OnFontSizeChange += ChangeFontSize;
            _settingsWindow.OnTextWrapChange += SwitchTextWrap;
            _settingsWindow.OnStatusBarChange += SwitchStatusBar;
        }

        private void ChangeFont(FontFamily fontFamily)
        {
            EditorContent.FontFamily = fontFamily;
        }

        private void ChangeFontSize(double fontSize)
        {
            EditorContent.FontSize = fontSize;
        }

        private void SwitchTextWrap(bool isChecked)
        {
            if (isChecked)
            {
                EditorContent.TextWrapping = TextWrapping.Wrap;
            }
            else
            {
                EditorContent.TextWrapping = TextWrapping.NoWrap;
            }

            TextWrapTrigger.IsChecked = isChecked;
            _editorSettings.TextWrap = isChecked;
        }

        private void SwitchStatusBar(bool visibilityState)
        {
            if (visibilityState)
            {
                DockPanel.Visibility = Visibility.Visible;
            }
            else
            {
                DockPanel.Visibility = Visibility.Collapsed;
            }

            StatusBarTrigger.IsChecked = visibilityState;
            _editorSettings.StatusBar = visibilityState;
        }

        private async void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _handler = new NoFileHandler();
            EditorContent.Text = await _handler.ReadAllTextAsync();
            EditorContent.Focus();
        }

        private async void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = FileHandler.CreateFileDialog(DialogType.Open);

            if (dialog.ShowDialog() is true)
            {
                _handler = new FileHandler(dialog.FileName);
                EditorContent.Text = await _handler.ReadAllTextAsync();
                await StatusBlock.SetColoredText("File opened.", Brushes.Green);
            }
        }

        private async void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_handler is NoFileHandler)
            {
                var dialog = FileHandler.CreateFileDialog(DialogType.Save);

                if (dialog.ShowDialog() is true)
                {
                    _handler = new FileHandler(dialog.FileName);
                    await StatusBlock.SetColoredText("File opened.", Brushes.Green);
                }
            }

            if (_handler.IsAvailable)
            {
                await _handler.WriteAllTextAsync(EditorContent.Text);
            }

            if (_handler is FileHandler)
            {
                await StatusBlock.SetColoredText("File saved.", Brushes.Green);
            }
        }

        private async void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _handler = new NoFileHandler();
            await _handler.WriteAllTextAsync(EditorContent.Text);
            SaveCommand_Executed(sender, e);
        }

        private void GotoSettingsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _settingsWindow = new SettingsWindow(_editorSettings);
            SubscribeEvents();
            _settingsWindow.Show();
        }

        private void ExitApplicationCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _settingsWindow?.Close();
            Close();
        }

        private void InsertDateTimeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EditorContent.Text = EditorContent.Text.Insert(EditorContent.CaretIndex, DateTime.Now.ToString());
        }

        private void KeyBinding_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers is ModifierKeys.Control &&
                ((EditorContent.FontSize >= 12 && e.Delta < 0) || (EditorContent.FontSize <= 100 && e.Delta > 0)))
            {
                EditorContent.FontSize += e.Delta / 30d;
                _editorSettings.FontSize = EditorContent.FontSize;
            }
        }

        private void TextWrapTrigger_Click(object sender, RoutedEventArgs e)
        {
            SwitchTextWrap(!TextWrapTrigger.IsChecked);
        }

        private void StatusBarTrigger_Click(object sender, RoutedEventArgs e)
        {
            SwitchStatusBar(!StatusBarTrigger.IsChecked);
        }
    }
}
