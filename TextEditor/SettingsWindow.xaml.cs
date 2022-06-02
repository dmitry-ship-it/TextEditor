using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            File.WriteAllText(EditorSettings.FilePath, JsonSerializer.Serialize(_data));
        }

        private void LoadSettings()
        {
            LoadFont();
            LoadFontSize();
        }

        private void LoadFont()
        {
            foreach (var font in Fonts.SystemFontFamilies)
            {
                FontChoose.Items.Add(font.ToFamilyString());
            }

            FontChoose.SelectedIndex = FontChoose.Items.IndexOf(_data.Font);
        }

        private void LoadFontSize()
        {
            FontSizeChoose.Text = _data.FontSize.ToString();
        }

        public event Action<FontFamily>? OnFontChange;
        public event Action<double>? OnFontSizeChange;

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
    }
}
