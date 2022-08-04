using System;
using System.Runtime.Serialization;

namespace MTUM_Wasm.Server.Core.Common.Utility;

[Serializable]
internal class ThrottlingException : Exception
{
    public ThrottlingException(string message) : base(message) { }
    public ThrottlingException(string message, Exception innerException) : base(message, innerException) { }
    protected ThrottlingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
