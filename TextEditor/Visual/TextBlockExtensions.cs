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
        public static void SetColoredText(this TextBlock textBlock, string text, Brush brush)
        {
            textBlock.Text = text;
            textBlock.Foreground = brush;
        }
    }
}
