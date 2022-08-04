using System.Text.RegularExpressions;

namespace MTUM_Wasm.Shared.Core.Common.Extension;

public static class StringExtensions
{
    private static readonly Regex MultipleSpaces =
        new Regex(@" {2,}", RegexOptions.Compiled);

    public static string NormalizeWhitespaces(this string input)
    {
        return MultipleSpaces.Replace(input, " ");
    }
}
