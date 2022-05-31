namespace TextEditor.FileHandling
{
    public interface ITextStreamHandler
    {
        public string ReadAllText();

        public void WriteAllText(string text);

        public void AppendString(string text);
    }
}