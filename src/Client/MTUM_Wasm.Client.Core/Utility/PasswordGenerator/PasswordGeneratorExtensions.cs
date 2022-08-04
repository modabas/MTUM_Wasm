using System;
using System.Collections.Generic;
using System.Linq;

namespace MTUM_Wasm.Client.Core.Utility.PasswordGenerator;

internal static class PasswordGeneratorExtensions
{
    private static readonly Lazy<RandomSecureVersion> RandomSecure =
        new Lazy<RandomSecureVersion>(() => new RandomSecureVersion());
    public static IEnumerable<T> ShuffleSecure<T>(this IEnumerable<T> source)
    {
        var sourceArray = source.ToArray();
        for (int counter = 0; counter < sourceArray.Length; counter++)
        {
            int randomIndex = RandomSecure.Value.Next(counter, sourceArray.Length);
            yield return sourceArray[randomIndex];

            sourceArray[randomIndex] = sourceArray[counter];
        }
    }

    public static string ShuffleTextSecure(this string source)
    {
        var shuffeldChars = source.ShuffleSecure().ToArray();
        return new string(shuffeldChars);
    }
}
