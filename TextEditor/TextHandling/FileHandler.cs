﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor.TextHandling
{
    public sealed class FileHandler : ITextHandler
    {
        private string _path;

        public bool IsAvailable => File.Exists(_path);

        public FileHandler(string path)
        {
            _path = path;
        }

        public void SetNewFileInstance()
        {
            var dialog = new SaveFileDialog
            {
                FileName = "Text file",
                DefaultExt = ".txt",
                Filter = "Text document (.txt)|*.txt"
            };

            if (dialog.ShowDialog() is true)
            {
                _path = dialog.FileName;
            }
        }

        public string ReadAllText()
        {
            return File.ReadAllText(_path);
        }

        public void WriteAllText(string text)
        {
            File.WriteAllText(_path, text);
        }

        public static FileDialog CreateFileDialog(DialogType dialogType)
        {
            FileDialog dialog = dialogType switch
            {
                DialogType.Save => new SaveFileDialog(),
                DialogType.Open => new OpenFileDialog(),
                _ => throw new ArgumentException("Unknown dialog type.", nameof(dialogType))
            };

            dialog.FileName = "Text file";
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Text document (.txt)|*.txt";

            return dialog;
        }
    }
}