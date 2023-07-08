using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JSONSchemaToCSharp
{
    internal class MemberDefinition
    {
        public ObjectDefinition ParentDefinition { get; set; }

        public string Name { get; set; }

        public string? Title { get; set; }

        public bool IsArray { get; set; }

        public bool IsRequired { get; set; }

        public ObjectDefinition Definition { get; set; }

        public MemberDefinition(ObjectDefinition parent, MemberDefinition source)
        {
            this.ParentDefinition = parent;
            this.Name = source.Name;
            this.Title = source.Title;
            this.IsArray = source.IsArray;
            this.IsRequired = source.IsRequired;
            this.Definition = source.Definition;
        }

        public MemberDefinition(ObjectDefinition parent, JsonProperty property, NamespaceDefinition namespaceDefinition)
        {
            ParentDefinition = parent;

            Name = property.Name;

            Title = ObjectDefinition.GetTitle(property.Value);

            var type = ObjectDefinition.GetType(property.Value);
            switch (type)
            {
                case "array":
                    {
                        var items = ObjectDefinition.GetItems(property.Value, namespaceDefinition);
                        if (items == null)
                        {
                            throw new Exception("Cannot find item definition for \"" + Name + "\".");
                        }
                        var itemType = ObjectDefinition.GetType(items.Value);
                        if (string.IsNullOrEmpty(itemType))
                        {
                            throw new Exception("Array item type is not specified.");
                        }
                        Definition = GetDefinition(items.Value, itemType, namespaceDefinition, true);
                        IsArray = true;
                    }
                    break;

                default:
                    Definition = ProcessDefinition(property, namespaceDefinition, type);
                    break;
            }
        }

        private ObjectDefinition ProcessDefinition(JsonProperty property, NamespaceDefinition namespaceDefinition, string? type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                return GetDefinition(property.Value, type, namespaceDefinition, false);
            }

            var enumValues = ObjectDefinition.GetEnumValues(property.Value);
            if (enumValues?.Count > 0)
            {
                type = "string";
                return GetDefinition(property.Value, type, namespaceDefinition, false);
            }

            var referencedDoc = ObjectDefinition.GetReferencedDoc(property.Value, namespaceDefinition);
            if (referencedDoc == null)
            {
                throw new Exception("Property type is not specified.");
            }

            type = ObjectDefinition.GetType(referencedDoc.RootElement);
            if (string.IsNullOrEmpty(type))
            {
                throw new Exception("Property type is not specified.");
            }

            return GetDefinition(referencedDoc.RootElement, type, namespaceDefinition, false);
        }

        private ObjectDefinition GetDefinition(JsonElement definition, string type, NamespaceDefinition namespaceDefinition, bool makeSinglar)
        {
            switch (type)
            {
                case "object":
                    var title = this.Title ?? this.Name;
                    var classDefinition = new ClassDefinition(definition.EnumerateObject(), namespaceDefinition)
                    {
                        Title = makeSinglar ? ObjectDefinition.MakeSingular(title) : title,
                    };
                    classDefinition.ParseObjectDefinitions(namespaceDefinition);
                    return classDefinition;

                case "number":
                    return new NumberDefinition(definition.EnumerateObject());

                case "integer":
                    return new IntegerDefinition(definition.EnumerateObject());

                case "string":
                    var enumValues = ObjectDefinition.GetEnumValues(definition);
                    if (enumValues != null)
                    {
                        var enumDefinition = new EnumDefinition(definition.EnumerateObject(), enumValues);
                        if (string.IsNullOrEmpty(enumDefinition.Title))
                        {
                            enumDefinition.Title = this.Title ?? this.Name;
                            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(enumDefinition.Title));
                        }
                        if (makeSinglar)
                        {
                            enumDefinition.Title = ObjectDefinition.MakeSingular(enumDefinition.Title);
                        }

                        enumDefinition = namespaceDefinition.AddEnumDefinition(enumDefinition);

                        return enumDefinition;
                    }

                    var format = ObjectDefinition.GetFormat(definition);
                    if (format == "date-time")
                    {
                        return new DateTimeDefinition(definition.EnumerateObject());
                    }

                    return new StringDefinition(definition.EnumerateObject());

                default:
                    throw new NotSupportedException();

            }
        }

        internal string GetSignature()
        {
            var memberName = ObjectDefinition.ToDefinitionName(Name);

            if (IsArray)
            {
                return memberName + "(List<" + Definition.GetSignature() + ">)";
            }
            else
            {
                return memberName + "(" + Definition.GetSignature() + ")";
            }
        }

        internal void Write(StreamWriter sw)
        {
            sw.WriteLine("\t/// <summary>");
            sw.WriteLine("\t/// " + Definition.Title + '.');
            sw.WriteLine("\t/// </summary>");

            var memberName = ObjectDefinition.ToDefinitionName(Name);
            if (memberName != Name)
            {
                sw.WriteLine("\t[JsonPropertyName(\"" + ObjectDefinition.AddEscapeCharacters(Name) + "\")]");
            }

            if (this.IsRequired)
            {
                sw.WriteLine("\t[Required]");
            }

            Definition.WriteAttributes(sw);

            var definitionName = Definition.GetName();
            if (IsArray)
            {
                sw.WriteLine("\tpublic List<" + definitionName + "> " + memberName + " { get; private set; }");
            }
            else
            {
                if (Definition.IsReferenceType)
                {
                    definitionName += '?';
                }
                sw.WriteLine("\tpublic " + definitionName + " " + memberName + " { get; set; }");
            }
        }
    }
}
