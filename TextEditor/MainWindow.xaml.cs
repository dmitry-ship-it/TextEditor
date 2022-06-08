using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TextEditor.Extensions;
using TextEditor.TextHandling;
using TextEditor.Resources;
using TextEditor.Visual;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Text;
using System.Linq;

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
            _editorSettings = editorSettings;
            _settingsWindow = new SettingsWindow(_editorSettings);

            InitializeComponent();

            SubscribeEvents();
            LoadSettings();
            SetNewTextHandler().Wait();
        }

        private async Task SetNewTextHandler(string? filePath = null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                _handler = new NoFileHandler();
            }
            else
            {
                _handler = new FileHandler(filePath);
            }

            EditorContent.Text = await _handler.ReadAllTextAsync();

            if (_handler is FileHandler)
            {
                await StatusBlock.SetColoredTextAsync("File opened", Brushes.BlueViolet, filePath!);
            }
        }

        public async Task ProcessStartupArgs(string[] args)
        {
            if (args is not null && args.Length == 1 && File.Exists(args[0]))
            {
                await SetNewTextHandler(args[0]);
            }
        }

        protected override async void OnClosing(CancelEventArgs e)
        {
            // check for unsaved text
            if ((_handler is NoFileHandler || await _handler.ReadAllTextAsync() != EditorContent.Text)
                && !string.IsNullOrEmpty(EditorContent.Text))
            {
                var msgBoxResult = ModernWpf.MessageBox.Show(
                    messageBoxText: "Save unsaved content?",
                    caption: "Saveing",
                    button: MessageBoxButton.YesNoCancel,
                    image: MessageBoxImage.Warning);

                if (msgBoxResult == MessageBoxResult.Yes)
                {
                    SaveCommand_Executed(this, null!);
                }

                e.Cancel = msgBoxResult is null
                    || msgBoxResult == MessageBoxResult.Cancel
                    || msgBoxResult == MessageBoxResult.None;
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
            ChangeFont(_editorSettings.Font.ToFontFamily());
            ChangeFontSize(_editorSettings.FontSize);
            SwitchTextWrap(_editorSettings.TextWrap);
            SwitchStatusBar(_editorSettings.StatusBar);
            SwitchLineNumbers(_editorSettings.LineNumbers);
        }

        private void SubscribeEvents()
        {
            _settingsWindow.OnFontChange += ChangeFont;
            _settingsWindow.OnFontSizeChange += ChangeFontSize;
            _settingsWindow.OnTextWrapChange += SwitchTextWrap;
            _settingsWindow.OnStatusBarChange += SwitchStatusBar;
            _settingsWindow.OnLineNumbersChange += SwitchLineNumbers;
        }

        private void ChangeFont(FontFamily fontFamily)
        {
            EditorContent.FontFamily = fontFamily;
        }

        private void ChangeFontSize(double fontSize)
        {
            EditorContent.FontSize = fontSize;
            EditorLineNumbers.FontSize = fontSize;
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

        private void SwitchLineNumbers(bool visibilityState)
        {
            if (visibilityState)
            {
                EditorLineNumbers.Visibility = Visibility.Visible;
            }
            else
            {
                EditorLineNumbers.Visibility = Visibility.Collapsed;
            }

            LineNumbersTrigger.IsChecked = visibilityState;
            _editorSettings.LineNumbers = visibilityState;
        }

        private async void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            await SetNewTextHandler();
        }

        private async void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = FileHandler.CreateFileDialog(DialogType.Open);

            if (dialog.ShowDialog() is true)
            {
                await SetNewTextHandler(dialog.FileName);
            }
        }

        private async void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_handler is NoFileHandler)
            {
                var dialog = FileHandler.CreateFileDialog(DialogType.Save);

                if (dialog.ShowDialog() is true)
                {
                    await SetNewTextHandler(dialog.FileName);
                }
            }

            if (_handler.IsAvailable)
            {
                await _handler.WriteAllTextAsync(EditorContent.Text);
            }
        }

        private async void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            await SetNewTextHandler();
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
            _settingsWindow.Close();
            Close();
        }

        private void InsertDateTimeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EditorContent.Text = EditorContent.Text.Insert(EditorContent.CaretIndex, DateTime.Now.ToString());
        }

        private void KeyBinding_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers is ModifierKeys.Control &&
                ((EditorContent.FontSize >= 12 && e.Delta < 0) || (EditorContent.FontSize <= 110 && e.Delta > 0)))
            {
                _editorSettings.FontSize += e.Delta / 30d;
                EditorContent.FontSize = _editorSettings.FontSize;
                EditorLineNumbers.FontSize = _editorSettings.FontSize;

                EditorContent_TextChanged(this, null!);
            }
        }

        private void EditorContent_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            NumberLinesScroll.ScrollToVerticalOffset(e.VerticalOffset);
        }

        private void EditorContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            // TODO: fix line numerating with text wrapping
            var sb = new StringBuilder();

            for (var i = 1; i <= EditorContent.LineCount; i++)
            {
                sb.Append(i);
                sb.Append('\n');
            }

            EditorLineNumbers.Text = sb.ToString();
        }

        private void TextWrapTrigger_Click(object sender, RoutedEventArgs e)
        {
            SwitchTextWrap(TextWrapTrigger.IsChecked);
        }

        private void StatusBarTrigger_Click(object sender, RoutedEventArgs e)
        {
            SwitchStatusBar(StatusBarTrigger.IsChecked);
        }

        private void LineNumbersTrigger_Click(object sender, RoutedEventArgs e)
        {
            SwitchLineNumbers(LineNumbersTrigger.IsChecked);
        }

        private async void EditorContent_Drop(object sender, DragEventArgs e)
        {
            // FIXME: drag and drop not working
#if DEBUG
            MessageBox.Show("Drag and Dropped");
#endif

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (files is not null && files.Length > 0)
                {
                    await SetNewTextHandler(files[0]);
                }
            }
        }

        private void EditorContent_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Move;
            }
        }
    }
}
