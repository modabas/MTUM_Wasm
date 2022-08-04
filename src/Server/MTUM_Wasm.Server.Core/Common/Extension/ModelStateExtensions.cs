using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;

namespace MTUM_Wasm.Server.Core.Common.Extension;

internal static class ModelStateExtensions
{
    public static IDictionary<string, string[]> CreateErrorDictionary(this ModelStateDictionary modelState)
    {
        if (modelState == null)
        {
            throw new ArgumentNullException(nameof(modelState));
        }

        var errorDictionary = new Dictionary<string, string[]>(StringComparer.Ordinal);

        foreach (var keyModelStatePair in modelState)
        {
            var key = keyModelStatePair.Key;
            var errors = keyModelStatePair.Value.Errors;
            if (errors != null && errors.Count > 0)
            {
                if (errors.Count == 1)
                {
                    var errorMessage = GetErrorMessage(errors[0]);
                    errorDictionary.Add(key, new[] { errorMessage });
                }
                else
                {
                    var errorMessages = new string[errors.Count];
                    for (var i = 0; i < errors.Count; i++)
                    {
                        errorMessages[i] = GetErrorMessage(errors[i]);
                    }

                    errorDictionary.Add(key, errorMessages);
                }
            }
        }

        return errorDictionary;

        static string GetErrorMessage(ModelError error)
        {
            return string.IsNullOrEmpty(error.ErrorMessage) ?
                "The input was not valid." :
                error.ErrorMessage;
        }
    }
}
