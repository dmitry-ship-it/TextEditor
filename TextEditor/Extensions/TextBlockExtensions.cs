using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace TextEditor.Visual
{
    public static class TextBlockExtensions
    {
        private const int DelayValue = 5000;

        public static async Task SetColoredText(this TextBlock textBlock, string text, Brush brush)
        {
            textBlock.Text = text;
            textBlock.Foreground = brush;

            await textBlock.Reset();
        }

        private static async Task Reset(this TextBlock textBlock)
        {
            await Task.Delay(DelayValue);

            textBlock.Text = string.Empty;
            textBlock.Foreground = default;
        }
    }
}
