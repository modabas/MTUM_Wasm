﻿using MTUM_Wasm.Shared.Core.Common.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTUM_Wasm.Server.Core.Common.Extension;

internal static class HttpContextAccessorExtensions
{
    public static IServiceResult<string> GetRequestIP(this IHttpContextAccessor _httpContextAccessor, bool tryUseXForwardHeader = true)
    {
        string? ip = null;

        // todo support new "Forwarded" header (2014) https://en.wikipedia.org/wiki/X-Forwarded-For

        // X-Forwarded-For (csv list):  Using the First entry in the list seems to work
        // for 99% of cases however it has been suggested that a better (although tedious)
        // approach might be to read each IP from right to left and use the first public IP.
        // http://stackoverflow.com/a/43554000/538763
        //
        if (tryUseXForwardHeader)
            ip = _httpContextAccessor.GetHeaderValueAs<string>("X-Forwarded-For")?.SplitCsv().FirstOrDefault();

        // RemoteIpAddress is always null in DNX RC1 Update1 (bug).
        if (string.IsNullOrWhiteSpace(ip) && _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress != null)
            ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

        if (string.IsNullOrWhiteSpace(ip))
            ip = _httpContextAccessor.GetHeaderValueAs<string>("REMOTE_ADDR");

        if (string.IsNullOrWhiteSpace(ip))
            return Result<string>.Fail("Unable to determine caller's IP.");

        return Result<string>.Success(ip, string.Empty);
    }

    public static T? GetHeaderValueAs<T>(this IHttpContextAccessor _httpContextAccessor, string headerName)
    {
        StringValues values;

        if (_httpContextAccessor.HttpContext?.Request.Headers.TryGetValue(headerName, out values) == true)
        {
            string rawValues = values.ToString();   // writes out as Csv when there are multiple.

            if (!string.IsNullOrWhiteSpace(rawValues))
                return (T)Convert.ChangeType(values.ToString(), typeof(T));
        }
        return default;
    }

    private static IEnumerable<string> SplitCsv(this string csvList)
    {
        if (string.IsNullOrWhiteSpace(csvList))
            return Array.Empty<string>();

        return csvList
            .TrimEnd(',')
            .Split(',')
            .Select(s => s.Trim())
            .ToList().AsReadOnly();
    }
}
