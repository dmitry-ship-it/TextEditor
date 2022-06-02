using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TextEditor.Resources
{
    [Serializable]
    public sealed class EditorSettings
    {
        public string Font { get; set; } = "Segoe UI";

        public double FontSize { get; set; } = 11;

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
