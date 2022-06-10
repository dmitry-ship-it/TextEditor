using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace TextEditor.Visual
{
    public static class TextBlockExtensions
    {
        private const int DelayValue = 5000;

        public static async Task SetColoredTextAsync(this TextBlock textBlock, string text, Brush brush, string? filePath = null)
        {
            textBlock.Text = text;
            textBlock.Foreground = brush;

            await textBlock.ResetAsync(filePath);
        }

        private static async Task ResetAsync(this TextBlock textBlock, string? filePath)
        {
            // change text only after delay
            await Task.Delay(DelayValue);

            if (string.IsNullOrEmpty(filePath))
            {
                textBlock.Text = string.Empty;
            }
            else
            {
                var lastDirSeparator = filePath.LastIndexOf(Path.DirectorySeparatorChar) + 1;
                textBlock.Text = filePath[lastDirSeparator..];
            }

            textBlock.ClearValue(TextBlock.ForegroundProperty);
        }
    }
}
