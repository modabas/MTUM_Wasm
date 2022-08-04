using System;
using System.Collections;
using System.Linq;

namespace MTUM_Wasm.Client.Core.Utility.PasswordGenerator;

internal class PasswordGenerator
{
    public int MinimumLengthPassword { get; private set; }
    public int MaximumLengthPassword { get; private set; }
    public int MinimumLowerCaseChars { get; private set; }
    public int MinimumUpperCaseChars { get; private set; }
    public int MinimumNumericChars { get; private set; }
    public int MinimumSpecialChars { get; private set; }

    public static string AllLowerCaseChars { get; private set; }
    public static string AllUpperCaseChars { get; private set; }
    public static string AllNumericChars { get; private set; }
    public static string AllSpecialChars { get; private set; }
    private readonly string _allAvailableChars;

    private readonly RandomSecureVersion _randomSecure = new RandomSecureVersion();
    private int _minimumNumberOfChars;

    static PasswordGenerator()
    {
        // Define characters that are valid and reject ambiguous characters such as ilo, IO and 1 or 0
        AllLowerCaseChars = GetCharRange('a', 'z', exclusiveChars: "ilo");
        AllUpperCaseChars = GetCharRange('A', 'Z', exclusiveChars: "IO");
        AllNumericChars = GetCharRange('2', '9');
        AllSpecialChars = "(^$*.[]{}()?-\"!@#%&/\\,><':;|_~`+=)";
    }

    public PasswordGenerator(
        int minimumLengthPassword = 8,
        int maximumLengthPassword = 12,
        int minimumLowerCaseChars = 1,
        int minimumUpperCaseChars = 1,
        int minimumNumericChars = 1,
        int minimumSpecialChars = 1)
    {
        if (minimumLengthPassword < 8)
        {
            throw new ArgumentException("The minimumlength is smaller than 15.",
                "minimumLengthPassword");
        }

        if (minimumLengthPassword > maximumLengthPassword)
        {
            throw new ArgumentException("The minimumLength is bigger than the maximum length.",
                "minimumLengthPassword");
        }

        if (minimumLowerCaseChars < 1)
        {
            throw new ArgumentException("The minimumLowerCase is smaller than 2.",
                "minimumLowerCaseChars");
        }

        if (minimumUpperCaseChars < 1)
        {
            throw new ArgumentException("The minimumUpperCase is smaller than 2.",
                "minimumUpperCaseChars");
        }

        if (minimumNumericChars < 1)
        {
            throw new ArgumentException("The minimumNumeric is smaller than 2.",
                "minimumNumericChars");
        }

        if (minimumSpecialChars < 1)
        {
            throw new ArgumentException("The minimumSpecial is smaller than 2.",
                "minimumSpecialChars");
        }

        _minimumNumberOfChars = minimumLowerCaseChars + minimumUpperCaseChars +
                                minimumNumericChars + minimumSpecialChars;

        if (minimumLengthPassword < _minimumNumberOfChars)
        {
            throw new ArgumentException(
                "The minimum length of the password is smaller than the sum " +
                "of the minimum characters of all catagories.",
                "maximumLengthPassword");
        }

        MinimumLengthPassword = minimumLengthPassword;
        MaximumLengthPassword = maximumLengthPassword;

        MinimumLowerCaseChars = minimumLowerCaseChars;
        MinimumUpperCaseChars = minimumUpperCaseChars;
        MinimumNumericChars = minimumNumericChars;
        MinimumSpecialChars = minimumSpecialChars;

        _allAvailableChars =
            OnlyIfOneCharIsRequired(minimumLowerCaseChars, AllLowerCaseChars) +
            OnlyIfOneCharIsRequired(minimumUpperCaseChars, AllUpperCaseChars) +
            OnlyIfOneCharIsRequired(minimumNumericChars, AllNumericChars) +
            OnlyIfOneCharIsRequired(minimumSpecialChars, AllSpecialChars);
    }

    private string OnlyIfOneCharIsRequired(int minimum, string allChars)
    {
        return minimum > 0 || _minimumNumberOfChars == 0 ? allChars : string.Empty;
    }

    public string Generate()
    {
        var lengthOfPassword = _randomSecure.Next(MinimumLengthPassword, MaximumLengthPassword);

        // Get the required number of characters of each catagory and 
        // add random charactes of all catagories
        var minimumChars = GetRandomString(AllLowerCaseChars, MinimumLowerCaseChars) +
                        GetRandomString(AllUpperCaseChars, MinimumUpperCaseChars) +
                        GetRandomString(AllNumericChars, MinimumNumericChars) +
                        GetRandomString(AllSpecialChars, MinimumSpecialChars);
        var rest = GetRandomString(_allAvailableChars, lengthOfPassword - minimumChars.Length);
        var unshuffeledResult = minimumChars + rest;

        // Shuffle the result so the order of the characters are unpredictable
        var result = unshuffeledResult.ShuffleTextSecure();
        return result;
    }

    private string GetRandomString(string possibleChars, int lenght)
    {
        var result = string.Empty;
        for (var position = 0; position < lenght; position++)
        {
            var index = _randomSecure.Next(possibleChars.Length);
            result += possibleChars[index];
        }
        return result;
    }

    private static string GetCharRange(char minimum, char maximum, string exclusiveChars = "")
    {
        var result = string.Empty;
        for (char value = minimum; value <= maximum; value++)
        {
            result += value;
        }
        if (!string.IsNullOrEmpty(exclusiveChars))
        {
            var inclusiveChars = result.Except(exclusiveChars).ToArray();
            result = new string(inclusiveChars);
        }
        return result;
    }
}
