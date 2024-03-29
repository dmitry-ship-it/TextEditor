﻿using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace TextEditor.TextHandling
{
    public sealed class FileHandler : ITextHandler
    {
        private string _path;

        public bool IsAvailable => File.Exists(_path);

        public FileHandler(string path)
        {
            _path = path;

            if (!IsAvailable)
            {
                // create file by writing empty string
                WriteAllText(string.Empty);
            }
        }

        public void SetNewFileInstance()
        {
            var dialog = CreateFileDialog(FileDialogType.Save);

            if (dialog.ShowDialog() is true)
            {
                _path = dialog.FileName;
            }
        }

        public string ReadAllText()
        {
            return File.ReadAllText(_path);
        }

        public async Task<string> ReadAllTextAsync()
        {
            return await File.ReadAllTextAsync(_path);
        }

        public void WriteAllText(string text)
        {
            File.WriteAllText(_path, text);
        }

        public async Task WriteAllTextAsync(string text)
        {
            await File.WriteAllTextAsync(_path, text);
        }

        public static FileDialog CreateFileDialog(FileDialogType dialogType)
        {
            FileDialog dialog = dialogType switch
            {
                FileDialogType.Save => new SaveFileDialog(),
                FileDialogType.Open => new OpenFileDialog(),
                _ => throw new ArgumentException("Unknown dialog type.", nameof(dialogType))
            };

            dialog.FileName = "Text file";
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Text document (*.txt)|*.txt|All files (*.*)|*.*";

            return dialog;
        }
    }
}
