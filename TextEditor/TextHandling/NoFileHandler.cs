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

        public async Task<string> ReadAllTextAsync()
        {
            return await Task.FromResult(ReadAllText());
        }

        public void WriteAllText(string text)
        {
            _content = text;
        }

        public async Task WriteAllTextAsync(string text)
        {
            await Task.Run(() => WriteAllText(text));
        }
    }
}
