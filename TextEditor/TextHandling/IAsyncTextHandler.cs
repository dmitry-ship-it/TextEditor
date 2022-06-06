using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor.TextHandling
{
    public interface IAsyncTextHandler
    {
        public Task<string> ReadAllTextAsync();

        public Task WriteAllTextAsync(string text);
    }
}
