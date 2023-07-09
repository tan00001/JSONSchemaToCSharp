using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JSONSchemaToCSharp
{
    internal class EnumDefinition : ObjectDefinition
    {
        public readonly IReadOnlyList<string> EnumValues;

        public IReadOnlyList<string>? EnumValueNames;

        public static string? StringConverter { get; set; }

        public static string? StringCollectionConverter { get; set; }


        public bool NeedsValueConversion { get; private set; }

        public EnumDefinition(JsonElement.ObjectEnumerator attributes, IReadOnlyList<string> enumValues)
        {
            foreach (var attribute in attributes)
            {
                switch (attribute.Name)
                {
                    case "title":
                        this.Title = attribute.Value.GetString();
                        break;
                }
            }

            EnumValues = enumValues;

            NeedsValueConversion = EnumValues.Any(v => v != ToDefinitionName(v));
        }

        public override string GetName()
        {
            var className = Title ?? throw new ArgumentNullException(nameof(Title));

            if (!string.IsNullOrEmpty(NameSuffix))
            {
                className += NameSuffix;
            }

            return ToDefinitionName(className);
        }

        public override string GetSignature()
        {
            return '[' + string.Join(", ", EnumValues) + ']';
        }

        internal override void Write(StreamWriter sw)
        {
            var enumName = GetName();

            sw.WriteLine("/// <summary>");
            sw.WriteLine("/// " + Title + '.');
            sw.WriteLine("/// </summary>");

            sw.WriteLine("public enum " + enumName);
            sw.WriteLine("{");

            int enumNumericValue = 0;
            for (var i = 0; i < EnumValues.Count; ++i)
            {
                var enumValue = EnumValues[i];

                string enumValueName;
                if (EnumValueNames != null)
                {
                    enumValueName = EnumValueNames[i];
                }
                else
                {
                    enumValueName = ToDefinitionName(enumValue);
                }

                if (enumValueName != enumValue)
                {
                    sw.WriteLine("\t[JsonPropertyName(\"" + ObjectDefinition.AddEscapeCharacters(enumValue) + "\")]");
                }
                if (i < EnumValues.Count - 1)
                {
                    sw.WriteLine("\t" + enumValueName + " = " + enumNumericValue++ + ',');
                    if (NeedsValueConversion)
                    {
                        sw.WriteLine();
                    }
                }
                else
                {
                    sw.WriteLine("\t" + enumValueName + " = " + enumNumericValue++);
                }
            }

            sw.WriteLine("};");
        }

        internal override void WriteAttributes(StreamWriter sw, bool isArray)
        {
            if (isArray)
            {
                if (!string.IsNullOrEmpty(StringCollectionConverter))
                {
                    sw.WriteLine("\t[JsonConverter(typeof(" + StringCollectionConverter + "))]");
                }
                return;
            }

            if (!string.IsNullOrEmpty(StringConverter))
            {
                sw.WriteLine("\t[JsonConverter(typeof(" + StringConverter + "))]");
            }
        }
    }
}
