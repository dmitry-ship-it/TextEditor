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
using System.Diagnostics;

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

            _handler = GetNewTextHandler().Result;
        }

        /// <summary>
        /// Creates new text handler. If <c>filePath</c> is not null or empty,
        /// then method checks if file exists. If file exists then 
        /// <c>FileHandler</c> well be created. In other cases <c>NoFileHandler</c>
        /// will be created.
        /// </summary>
        /// <param name="filePath">Full or realtive path to target file.</param>
        /// <returns><c>ITextHandler</c> which can be <c>NoFileHandler</c> or <c>FileHandler</c>.</returns>
        private async Task<ITextHandler> GetNewTextHandler(string? filePath = null)
        {
            ITextHandler textHandler = string.IsNullOrEmpty(filePath)
                ? new NoFileHandler()
                : new FileHandler(filePath);

            EditorContent.Text = await textHandler.ReadAllTextAsync();

            if (textHandler is FileHandler)
            {
                await StatusBlock.SetColoredTextAsync("File opened", Brushes.BlueViolet, filePath);
            }

            return textHandler;
        }

        /// <summary>
        /// This method is overridden to process command line args.
        /// </summary>
        protected override async void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            var args = Environment.GetCommandLineArgs();
            var file = args?.Length > 1 && File.Exists(args[1])
                ? args[1]
                : null;

            _handler = await GetNewTextHandler(file);
        }

        /// <summary>
        /// Method is overridden to check if text in editor is saved.
        /// If not, <c>ModernWpf.MessageBox</c> will be shown with a save request.
        /// </summary>
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

        /// <summary>
        /// In some cases application is not closing properly,
        /// so it becomes necessary to explicitly call <c>Shutdown</c> on app close.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Sets editor related settings.
        /// </summary>
        private void LoadSettings()
        {
            ChangeFont(_editorSettings.Font.ToFontFamily());
            ChangeFontSize(_editorSettings.FontSize);
            SwitchTextWrap(_editorSettings.TextWrap);
            SwitchStatusBar(_editorSettings.StatusBar);
            SwitchLineNumbers(_editorSettings.LineNumbers);
        }

        /// <summary>
        /// Sets event handlers for changes in the settings window.
        /// </summary>
        private void SubscribeEvents()
        {
            _settingsWindow.OnFontChange += ChangeFont;
            _settingsWindow.OnFontSizeChange += ChangeFontSize;
            _settingsWindow.OnTextWrapChange += SwitchTextWrap;
            _settingsWindow.OnStatusBarChange += SwitchStatusBar;
            _settingsWindow.OnLineNumbersChange += SwitchLineNumbers;
        }

        /// <summary>
        /// One of the event handlers. Sets <c>FontFamily</c> of text editor.
        /// </summary>
        /// <param name="fontFamily">Font family from <c>System.Windows.Media</c> namespace.</param>
        private void ChangeFont(FontFamily fontFamily)
        {
            EditorContent.FontFamily = fontFamily;
        }

        /// <summary>
        /// One of the event handlers. Sets <c>FontSize</c> of text editor and line numbers.
        /// </summary>
        private void ChangeFontSize(double fontSize)
        {
            EditorContent.FontSize = fontSize;
            EditorLineNumbers.FontSize = fontSize;
        }

        /// <summary>
        /// One of the event handlers. Changes text wrapping parameter.
        /// </summary>
        /// <param name="isChecked">
        /// True to set <c>TextWrappping</c> to <c>Wrap</c>.
        /// False to set <c>TextWrapping</c> to <c>NoWrap</c>.
        /// </param>
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

        /// <summary>
        /// One of the event handlers. Changes status bar visibility state.
        /// </summary>
        /// <param name="visibilityState">
        /// True to set <c>Visibility</c> to <c>Visible</c>.
        /// False to set <c>Visibility</c> to <c>Collapsed</c>.
        /// </param>
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

        /// <summary>
        /// One of the event handlers. Changes line numbers textbox visibility state.
        /// </summary>
        /// <param name="visibilityState">
        /// True to set <c>Visibility</c> to <c>Visible</c>.
        /// False to set <c>Visibility</c> to <c>Collapsed</c>.
        /// </param>
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

        /// <summary>
        /// Handler for standard command <b>New</b>.
        /// </summary>
        private async void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _handler = await GetNewTextHandler();
        }

        /// <summary>
        /// Handler for standard command <b>Open</b>.
        /// </summary>
        private async void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = FileHandler.CreateFileDialog(FileDialogType.Open);

            if (dialog.ShowDialog() is true)
            {
                _handler = await GetNewTextHandler(dialog.FileName);
            }
        }

        /// <summary>
        /// Handler for standard command <b>Save</b>.
        /// </summary>
        private async void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_handler is NoFileHandler)
            {
                var dialog = FileHandler.CreateFileDialog(FileDialogType.Save);

                if (dialog.ShowDialog() is true)
                {
                    _handler = await GetNewTextHandler(dialog.FileName);
                }
            }

            if (_handler.IsAvailable)
            {
                await _handler.WriteAllTextAsync(EditorContent.Text);
            }
        }

        /// <summary>
        /// Handler for standard command <b>SaveAs</b>.
        /// </summary>
        private async void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _handler = await GetNewTextHandler();
            SaveCommand_Executed(sender, e);
        }

        /// <summary>
        /// Handler for custom command <b>GotoSettings</b>.
        /// Command is defined in <c>ApplicationCommandsExtension</c> class.
        /// </summary>
        private void GotoSettingsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _settingsWindow = new SettingsWindow(_editorSettings);
            SubscribeEvents();
            _settingsWindow.Show();
        }

        /// <summary>
        /// Handler for custom command <b>ExitApplication</b>.
        /// Command is defined in <c>ApplicationCommandsExtension</c> class.
        /// </summary>
        private void ExitApplicationCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _settingsWindow.Close();
            Close();
        }

        /// <summary>
        /// Handler for custom command <b>InsertDateTime</b>.
        /// Command is defined in <c>ApplicationCommandsExtension</c> class.
        /// </summary>
        private void InsertDateTimeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            EditorContent.Text = EditorContent.Text.Insert(EditorContent.CaretIndex, DateTime.Now.ToString());
        }

        /// <summary>
        /// Event handler. Changes font size of editor and line numbers.
        /// </summary>
        private void KeyBinding_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers is ModifierKeys.Control &&
                ((EditorContent.FontSize >= 12 && e.Delta < 0) || (EditorContent.FontSize <= 110 && e.Delta > 0)))
            {
                _editorSettings.FontSize += e.Delta / 30d;
                ChangeFontSize(_editorSettings.FontSize);
                EditorContent_TextChanged(this, null!);
            }
        }

        /// <summary>
        /// Event handler. Changes vertical scroll offset of line numbers textbox.
        /// </summary>
        private void EditorContent_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            EditorLineNumbers.ScrollToVerticalOffset(e.VerticalOffset);
        }

        /// <summary>
        /// Event handler. Sets new value to line numbers textbox.
        /// </summary>
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

        /// <summary>
        /// Event handler. Changes text wrapping by pressing the corresponding button in the menu.
        /// </summary>
        private void TextWrapTrigger_Click(object sender, RoutedEventArgs e)
        {
            SwitchTextWrap(TextWrapTrigger.IsChecked);
        }

        /// <summary>
        /// Event handler. Changes status bar visibility by pressing the corresponding button in the menu.
        /// </summary>
        private void StatusBarTrigger_Click(object sender, RoutedEventArgs e)
        {
            SwitchStatusBar(StatusBarTrigger.IsChecked);
        }

        /// <summary>
        /// Event handler. Changes line numbers visibility by pressing the corresponding button in the menu.
        /// </summary>
        private void LineNumbersTrigger_Click(object sender, RoutedEventArgs e)
        {
            SwitchLineNumbers(LineNumbersTrigger.IsChecked);
        }

        /// <summary>
        /// Event handler for drag and dropping. Opens dropped file.
        /// </summary>
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
                    _handler = await GetNewTextHandler(files[0]);
                }
            }
        }
    }
}
