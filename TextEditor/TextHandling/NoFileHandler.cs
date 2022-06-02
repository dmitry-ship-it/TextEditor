using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor.TextHandling
{
    public sealed class NoFileHandler : ITextHandler
    {
        private string _content;

        public NoFileHandler()
        {
            _content = string.Empty;
        }

        public bool IsAvailable => true;

        public string ReadAllText()
        {
            return (string)_content.Clone();
        }

        public void WriteAllText(string text)
        {
            _content = (string)text.Clone();
        }
    }
}
