using System;

namespace MTUM_Wasm.Server.Core.Database.Entity;

internal class AuditLogPrimaryKey
{
    public DateTime CreatedDate { get; set; }
    public long Id { get; set; }
}
