using ModernWpf;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TextEditor.Extensions;
using TextEditor.Resources;

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly EditorSettings _data;

        public SettingsWindow(EditorSettings data)
        {
            InitializeComponent();

            _data = data;
            LoadSettings();
        }

        /// <summary>
        /// Save settings on window close.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            File.WriteAllText(EditorSettings.FilePath, JsonSerializer.Serialize(_data));
        }

        private void LoadSettings()
        {
            // load font family
            foreach (var font in Fonts.SystemFontFamilies)
            {
                FontChoose.Items.Add(font.ToFamilyString());
            }

            FontChoose.SelectedIndex = FontChoose.Items.IndexOf(_data.Font);

            // load font size
            FontSizeChoose.Text = _data.FontSize.ToString();

            // load text wrap
            EditorWordWrap.IsChecked = _data.TextWrap;

            // load status bar visibility
            EditorStatusBar.IsChecked = _data.StatusBar;

            // load line numbers visibility
            EditorLineNumbers.IsChecked = _data.LineNumbers;

            // load application theme
            ThemeManager.Current.ApplicationTheme = _data.ApplicationTheme;
        }

        // events
        public event Action<FontFamily>? OnFontChange;
        public event Action<double>? OnFontSizeChange;
        public event Action<bool>? OnTextWrapChange;
        public event Action<bool>? OnStatusBarChange;
        public event Action<bool>? OnLineNumbersChange;

        // Applies the selected in this windows settings.
        #region Event handlers

        private void FontChoose_DropDownClosed(object sender, EventArgs e)
        {
            var fonts = Fonts.SystemFontFamilies;
            var fontFamily = fonts.Single(ff => ff.ToFamilyString() == FontChoose.Text);

            _data.Font = fontFamily.ToFamilyString();

            OnFontChange?.Invoke(fontFamily);
        }

        private void FontSizeChoose_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FontSizeChoose.Text))
            {
                return;
            }

            FontSizeChoose.Text = FontSizeChoose.Text.Where(c => char.IsDigit(c)).ToNormalString();
            _data.FontSize = double.Parse(FontSizeChoose.Text);

            if (_data.FontSize >= 2)
            {
                OnFontSizeChange?.Invoke(_data.FontSize);
            }
        }

        private void EditorWordWrap_Changed(object sender, RoutedEventArgs e)
        {
            _data.TextWrap = EditorWordWrap.IsChecked!.Value;
            OnTextWrapChange?.Invoke(_data.TextWrap);
        }

        private void EditorStatusBar_Changed(object sender, RoutedEventArgs e)
        {
            _data.StatusBar = EditorStatusBar.IsChecked!.Value;
            OnStatusBarChange?.Invoke(_data.StatusBar);
        }

        private void EditorLineNumbers_Changed(object sender, RoutedEventArgs e)
        {
            _data.LineNumbers = EditorLineNumbers.IsChecked!.Value;
            OnLineNumbersChange?.Invoke(_data.LineNumbers);
        }

        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            if (ThemeManager.Current.ApplicationTheme == ApplicationTheme.Light)
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
            }
            else
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
            }

            _data.ApplicationTheme = ThemeManager.Current.ApplicationTheme!.Value;
        }

        #endregion
    }
}
