using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace TextEditor.Extensions
{
    public static class StringExtensions
    {
        public static string ToFamilyString(this FontFamily fontFamily)
        {
            return string.Join(", ", fontFamily.FamilyNames.Values);
        }

        public static FontFamily ToFontFamily(this string s)
        {
            return new FontFamily(s);
        }

        public static string ToNormalString(this IEnumerable<char> enumerable)
        {
            return new string(enumerable.ToArray());
        }
    }
}
