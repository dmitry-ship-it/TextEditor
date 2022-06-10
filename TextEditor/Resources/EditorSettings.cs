using ModernWpf;
using System;
using System.IO;
using System.Text.Json;

namespace TextEditor.Resources
{
    [Serializable]
    public sealed class EditorSettings
    {
        public string Font { get; set; } = "Segoe UI";

        public double FontSize { get; set; } = 14;

        public bool TextWrap { get; set; } = true;

        public bool StatusBar { get; set; } = true;

        public bool LineNumbers { get; set; } = false;

        public ApplicationTheme ApplicationTheme { get; set; } = ThemeManager.Current.ActualApplicationTheme;

        [NonSerialized]
        public const string FilePath = $"{nameof(EditorSettings)}.json";

        internal static void CreateFile()
        {
            if (!File.Exists(FilePath))
            {
                File.WriteAllText(FilePath, JsonSerializer.Serialize(new EditorSettings()));
            }
        }
    }
}
