using Ardalis.SmartEnum;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace MTUM_Wasm.Server.Web.Swashbuckle
{
    internal sealed class SmartEnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var type = context.Type;

            if (!IsTypeDerivedFromGenericType(type, typeof(SmartEnum<>)) && !IsTypeDerivedFromGenericType(type, typeof(SmartEnum<,>)))
            {
                return;
            }

            var enumValues = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy).Select(d => d.Name);
            var openApiValues = new OpenApiArray();
            openApiValues.AddRange(enumValues.Select(d => new OpenApiString(d)));

            // See https://swagger.io/docs/specification/data-models/enums/
            schema.Type = "string";
            schema.Enum = openApiValues;
            schema.Properties = null;
        }

        private static bool IsTypeDerivedFromGenericType(Type typeToCheck, Type genericType)
        {
            while (true)
            {
                if (typeToCheck == typeof(object))
                {
                    return false;
                }

                if (typeToCheck == null)
                {
                    return false;
                }

                if (typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }

                if (typeToCheck.BaseType == null)
                {
                    return false;
                }

                typeToCheck = typeToCheck.BaseType;
            }
        }
    }
}
