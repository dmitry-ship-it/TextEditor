using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor.FileHandling
{
    public interface IFileHandler : ITextStreamHandler
    {
        public FileInfo? FileInstance { get; }

        public void SetNewFileInstance(string path);
    }
}
