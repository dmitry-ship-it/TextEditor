using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor.FileHandling
{
    public sealed class FileHandler : IFileHandler
    {
        // TODO: split to 2 abstract classes. First handles file, second memory? stream.
        private FileStream _stream;

        public FileInfo? FileInstance { get; private set; }

        public void AppendString(string text)
        {
            using var stream = FileInstance.AppendText();
            stream.Write(text);
        }

        public string ReadAllText()
        {
            using var stream = FileInstance.OpenRead();
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public void SetNewFileInstance(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new ArgumentException($"File '{path}' not exists.", nameof(path));
            }

            FileInstance = new FileInfo(path);
            _stream = FileInstance.OpenWrite();
        }

        public void WriteAllText(string text)
        {
            using var stream = FileInstance.OpenWrite();
            using var writer = new StreamWriter(stream);
            writer.Write(text);
        }
    }
}
