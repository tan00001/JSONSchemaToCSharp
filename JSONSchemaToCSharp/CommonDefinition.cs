using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JSONSchemaToCSharp
{
    internal class CommonDefinition
    {
        public string Name { get; set; }

        public IReadOnlyList<string>? Values { get; set; } 

        public bool IsValueType { get; set; }

        public ObjectDefinition? ObjectDefinition { get; set; }

        public CommonDefinition(JsonElement definition)
        {
            string? name = null;

            foreach (var attribute in definition.EnumerateObject())
            {
                switch (attribute.Name) 
                {
                    case "name":
                        name = attribute.Value.GetString();
                        break;

                    case "isValueType":
                        IsValueType = attribute.Value.GetBoolean();
                        break;

                    case "values":
                        Values = attribute.Value.EnumerateArray().Select(v => v.GetString()!).ToArray();
                        break;
                }
            }

            Name = name ?? throw new ArgumentException("Definition does not specify a name.", nameof(definition));
        }
    }
}
