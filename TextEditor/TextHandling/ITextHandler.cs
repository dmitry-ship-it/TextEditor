using System.IO;

namespace TextEditor.TextHandling
{
    public interface ITextHandler
    {
        public bool IsAvailable { get; }

        public string ReadAllText();

        public void WriteAllText(string text);
    }
}