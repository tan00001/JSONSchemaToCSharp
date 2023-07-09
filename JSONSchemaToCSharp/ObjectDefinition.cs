using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JSONSchemaToCSharp
{
    internal class ObjectDefinition
    {
        static readonly char[] SpaceSeparaters = new char[] { ' ', '\t' };

        public string? NameSuffix { get; set; }

        public string? Title { get; set; }

        public JsonElement Properties { get; set; }

        public List<MemberDefinition> Members { get; private set; }

        public IList<string?>? RequiredProperties { get; set; }

        public virtual bool IsReferenceType
        {
            get
            {
                return false;
            }
        }

        public ObjectDefinition()
        {
            Members = new List<MemberDefinition>();
        }

        public ObjectDefinition(ObjectDefinition source)
        {
            Members = source.Members.Select(m => new MemberDefinition(this, m)).ToList();
            this.RequiredProperties = source.RequiredProperties;
        }

        public virtual string GetName()
        {
            return "object";
        }

        public virtual string GetSignature()
        {
            return "object";
        }

        public static string? GetFormat(JsonElement defintion)
        {
            foreach (var attribute in defintion.EnumerateObject())
            {
                if (attribute.Name == "format")
                {
                    return attribute.Value.GetString();
                }
            }

            return null;
        }

        public static Int32? GetMaxItem(JsonElement defintion)
        {
            foreach (var attribute in defintion.EnumerateObject())
            {
                if (attribute.Name == "maxItems")
                {
                    return attribute.Value.GetInt32();
                }
            }

            return null;
        }

        public static Int32? GetMinItem(JsonElement defintion)
        {
            foreach (var attribute in defintion.EnumerateObject())
            {
                if (attribute.Name == "minItems")
                {
                    return attribute.Value.GetInt32();
                }
            }

            return null;
        }


        public static string? GetTitle(JsonElement defintion)
        {
            foreach (var attribute in defintion.EnumerateObject())
            {
                if (attribute.Name == "title")
                {
                    return attribute.Value.GetString();
                }
            }

            return null;
        }

        public static string? GetType(JsonElement defintion)
        {
            foreach (var attribute in defintion.EnumerateObject())
            {
                if (attribute.Name == "type")
                {
                    return attribute.Value.GetString();
                }
            }

            return null;
        }

        public static List<string>? GetEnumValues(JsonElement definition)
        {
            foreach (var attribute in definition.EnumerateObject())
            {
                if (attribute.Name == "enum" && attribute.Value.ValueKind == JsonValueKind.Array)
                {
                    return attribute.Value.EnumerateArray().Select(v => v.GetString()!).ToList();
                }
            }

            return null;
        }

        public static JsonElement? GetItems(JsonElement definition, NamespaceDefinition namespaceDefinition)
        {
            foreach (var attribute in definition.EnumerateObject())
            {
                if (attribute.Name == "items")
                {
                    var refDoc = GetReferencedDoc(attribute.Value, namespaceDefinition);
                    if (refDoc != null)
                    {
                        return refDoc.RootElement;
                    }
                    return attribute.Value;
                }
            }

            return null;
        }

        public static JsonDocument? GetReferencedDoc(JsonElement attributeValue, NamespaceDefinition namespaceDefinition)
        {
            if (attributeValue.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            foreach (var subAttribute in attributeValue.EnumerateObject())
            {
                if (subAttribute.Name == "$ref")
                {
                    var referencedDoc = namespaceDefinition.LoadRef(subAttribute.Value.GetString()!);
                    if (referencedDoc.RootElement.ValueKind != JsonValueKind.Object)
                    {
                        throw new Exception("Invalid reference \"" + subAttribute.Value.GetString() + "\". Expected an object definition.");
                    }
                    return referencedDoc;
                }
            }

            return null;
        }

        public void ParseObjectDefinitions(NamespaceDefinition namespaceDefinition)
        {
            foreach (var property in this.Properties.EnumerateObject())
            {
                var member = new MemberDefinition(this, property, namespaceDefinition);

                member.IsRequired = RequiredProperties?.Any(p => p == member.Name) == true;

                if (member.Definition is ClassDefinition memberClassTypeDefinition)
                {
                    if (string.IsNullOrEmpty(memberClassTypeDefinition.Title))
                    {
                        throw new Exception("\"" + property.Name + "\" of the class \"" + this.Title + "\" has no name.");
                    }

                    member.Definition = namespaceDefinition.AddClassDefinition(memberClassTypeDefinition);
                }
                else if (member.Definition is EnumDefinition enumDefinition)
                {
                    if (string.IsNullOrEmpty(enumDefinition.Title))
                    {
                        throw new Exception("\"" + property.Name + "\" of the class \"" + this.Title + "\" has no name.");
                    }

                    member.Definition = namespaceDefinition.AddEnumDefinition(enumDefinition);
                }

                Members.Add(member);
            }
        }

        internal virtual void WriteAttributes(StreamWriter sw, bool isArray)
        {
        }

        internal virtual void Write(StreamWriter sw)
        {
        }

        public static string AddEscapeCharacters(string s)
        {
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        public static string MakeSingular(string s)
        {
            if (s.EndsWith("ies"))
            {
                return s.Substring(0, s.Length - 3) + 'y';
            }

            if (s.EndsWith("ses"))
            {
                return s.Substring(0, s.Length - 2);
            }

            if (s.EndsWith("s"))
            {
                return s.Substring(0, s.Length - 1);
            }

            return s;
        }

        public static string ToDefinitionName(string name)
        {
            var nameSegments = name.Split(SpaceSeparaters);

            var definitionName = string.Join("", nameSegments.Select(s =>
            {
                var segment = new string(s.Where(c => char.IsAsciiLetterOrDigit(c)).ToArray());
                return CapitalizeFirstLetter(segment);
            }));

            if (definitionName.Length == 0)
            {
                return definitionName;
            }

            if (char.IsAsciiLetter(definitionName[0]))
            {
                return definitionName;
            }

            return '_' + definitionName;
        }

        protected static string CapitalizeFirstLetter(string segment)
        {
            if (segment.Length == 0)
            {
                return "";
            }

            if (segment.Length == 1)
            {
                return segment.ToUpper();
            }

            return char.ToUpper(segment[0]) + segment.Substring(1);
        }
    }
}
