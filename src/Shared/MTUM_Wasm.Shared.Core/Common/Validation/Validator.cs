using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTUM_Wasm.Shared.Core.Common.Validation
{
    public class Validator
    {
        public class Expression
        {
            public const string OnlyWords = @"^[\p{L}][\p{L} ]*$";
            public const string EmptyOrOnlyWords = @"^$|^[\p{L}][\p{L} ]*$";
            public const string Password = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[\^$*.\[\]{}\(\)?\-“!@#%&/,><\':;|_~`])\S{4,99}$";
            public const string IPv4Address = @"^(?:(?:25[0-5]|2[0-4]\d|1?\d\d?)\.){3}(?:25[0-5]|2[0-4]\d|1?\d\d?)$";
            public const string IPv6Address = @"^(?i)(?:[\da-f]{0,4}:){1,7}(?:(?<ipv4>(?:(?:25[0-5]|2[0-4]\d|1?\d\d?)\.){3}(?:25[0-5]|2[0-4]\d|1?\d\d?))|[\da-f]{0,4})$";
            public const string IPAddress = $"{IPv4Address}|{IPv6Address}";
        }

        public class Message
        {
            public const string OnlyWords = "Must consist of letters.";
            public const string EmptyOrOnlyWords = "Must be empty or consist of letters.";
            public const string Password = "Must contain at least 1 number, 1 lowercase, 1 uppercase letter, 1 special character (^$*.[]{}()?-\"!@#%&/\\,><':;|_~`+=)";
            public const string IPAddress = "Must be valid IPv4 or IPv6 address.";
        }

    }
}
