using System;
using System.Runtime.Serialization;

namespace MTUM_Wasm.Server.Core.Identity.Utility;

[Serializable]
internal class UserNotAuthenticated : Exception
{
    public UserNotAuthenticated(string message) : base(message) { }
    public UserNotAuthenticated(string message, Exception innerException) : base(message, innerException) { }
    protected UserNotAuthenticated(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
