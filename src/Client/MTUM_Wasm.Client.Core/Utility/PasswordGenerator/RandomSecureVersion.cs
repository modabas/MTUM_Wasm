using System.Security.Cryptography;
using System;

namespace MTUM_Wasm.Client.Core.Utility.PasswordGenerator;

internal class RandomSecureVersion
{
    //Never ever ever never use Random() in the generation of anything that requires true security/randomness
    //and high entropy or I will hunt you down with a pitchfork!! Only RNGCryptoServiceProvider() is safe.
    private readonly RandomNumberGenerator _rngProvider = RandomNumberGenerator.Create();

    public int Next()
    {
        var randomBuffer = new byte[4];
        _rngProvider.GetBytes(randomBuffer);
        var result = BitConverter.ToInt32(randomBuffer, 0);
        return result;
    }

    public int Next(int maximumValue)
    {
        // Do not use Next() % maximumValue because the distribution is not OK
        return Next(0, maximumValue);
    }

    public int Next(int minimumValue, int maximumValue)
    {
        var seed = Next();

        //  Generate uniformly distributed random integers within a given range.
        return new Random(seed).Next(minimumValue, maximumValue);
    }
}