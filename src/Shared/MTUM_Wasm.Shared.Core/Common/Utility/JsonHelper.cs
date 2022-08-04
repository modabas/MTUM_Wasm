using MTUM_Wasm.Shared.Core.Common.Result;
using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace MTUM_Wasm.Shared.Core.Common.Utility;

public class JsonHelper
{
    public static IServiceResult<T> TryDeserializeJson<T>(string jsonString)
    {
        try
        {
            var result = JsonSerializer.Deserialize<T>(jsonString);
            return Result<T>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<T>.Fail(ex.ToString());
        }
    }

    public static string SerializeJson<T>(T? input)
    {
        if (input is null)
            return string.Empty;
        return JsonSerializer.Serialize(input);
    }

    public static T? DeserializeJson<T>(string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
            return default;
        return JsonSerializer.Deserialize<T>(jsonString);
    }

    public static string PrettifyJsonString(string jsonString)
    {
        try
        {
            using (var jDoc = JsonDocument.Parse(jsonString))
            {
                var options = new JsonSerializerOptions(JsonSerializerDefaults.General) 
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = true 
                };
                var ret = JsonSerializer.Serialize(jDoc, options);
                return ret;
            }
        }
        catch
        {
            return jsonString;
        }
    }
}
