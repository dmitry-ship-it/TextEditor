using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TextEditor.Extensions;
using TextEditor.FileHandling;
using TextEditor.Resources;

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IFileHandler _file;
        private readonly SettingsWindow _settingsWindow;
        private readonly GeneralData _generalData;

        public MainWindow(IFileHandler file, SettingsWindow settingsWindow, GeneralData generalData)
        {
            _file = file;
            _settingsWindow = settingsWindow;
            _generalData = generalData;

            SubscribeEvents();
            InitializeComponent();

            LoadSettings();
        }

        private void LoadSettings()
        {
            ChangeFont(_generalData.Font.ToFontFamily());
        }

        private void SubscribeEvents()
        {
            _settingsWindow.OnFontChange += ChangeFont;
            _settingsWindow.OnFontSizeChange += ChangeFontSize;
        }

        private void ChangeFont(FontFamily fontFamily)
        {
            Content.FontFamily = fontFamily;
        }

        private void ChangeFontSize(double fontSize)
        {
            Content.FontSize = fontSize;
        }

        private void CreateNewFile_Click(object sender, RoutedEventArgs e)
        {
            Content.Focus();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                FileName = "Text file",
                DefaultExt = ".txt",
                Filter = "Text document (.txt)|*.txt"
            };

            if (dialog.ShowDialog() is true)
            {
                _file.SetNewFileInstance(dialog.FileName);
                Content.Text = _file.ReadAllText();
            }
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _file.WriteAllText(Content.Text);
        }

        private void KeyBinding_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers is ModifierKeys.Control &&
                ((Content.FontSize >= 12 && e.Delta < 0) || (Content.FontSize <= 100 && e.Delta > 0)))
            {
                Content.FontSize += e.Delta >> 5;
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            _settingsWindow.Show();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            _settingsWindow?.Close();
            Close();
        }
    }
}
