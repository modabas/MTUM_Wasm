using MTUM_Wasm.Shared.Core.Common.Utility;
using System.Collections.Generic;

namespace MTUM_Wasm.Server.Core.Database.Entity;

internal class AuditLogEntry
{
    private Dictionary<string, object?> _propertyBag = new();

    public string ToJsonString()
    {
        return JsonHelper.SerializeJson(_propertyBag);
    }

    public AuditLogEntry()
    {

    }

    public AuditLogEntry(params AuditLogItem[] auditLogItems)
    {
        foreach (var auditLogItem in auditLogItems)
        {
            _propertyBag.Add(auditLogItem.Name, auditLogItem.Value);
        }
    }
}

internal class AuditLogItem
{
    public string Name { get; set; } = string.Empty;
    public object? Value { get; set; }

    public AuditLogItem()
    {

    }

    public AuditLogItem(string name, object? value)
    {
        Name = name;
        Value = value;
    }
}


