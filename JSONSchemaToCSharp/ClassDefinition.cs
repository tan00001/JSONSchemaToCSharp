using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JSONSchemaToCSharp
{
    [DataContract(Namespace = "")]
    internal class ClassDefinition : ObjectDefinition
    {
        public string? Schema { get; set; }

        public string? Type { get; set; }

        public bool AdditionalProperties { get; set; }

        public override bool IsReferenceType
        {
            get
            {
                return true;
            }
        }

        public ClassDefinition(JsonElement.ObjectEnumerator attributes, NamespaceDefinition namespaceDefinition)
        {
            ReadAttributes(attributes, namespaceDefinition);
        }

        private void ReadAttributes(JsonElement.ObjectEnumerator attributes, NamespaceDefinition namespaceDefinition)
        {
            foreach (var attribute in attributes)
            {
                switch (attribute.Name)
                {
                    case "$ref":
                        var refAttributes = namespaceDefinition.LoadRef(attribute.Value.GetString()!);
                        if (refAttributes.RootElement.ValueKind != JsonValueKind.Object)
                        {
                            throw new Exception("Invalid reference \"" + attribute.Value.GetString() + "\". Expected an object definition.");
                        }
                        ReadAttributes(refAttributes.RootElement.EnumerateObject(), namespaceDefinition);
                        break;

                    case "$schema":
                        this.Schema = attribute.Value.GetString();
                        break;

                    case "title":
                        this.Title = attribute.Value.GetString();
                        break;

                    case "type":
                        this.Type = attribute.Value.GetString();
                        break;

                    case "properties":
                        this.Properties = attribute.Value;
                        break;

                    case "required":
                        this.RequiredProperties = attribute.Value.EnumerateArray().Select(p => p.GetString()).ToList();
                        break;

                    case "additionalProperties":
                        this.AdditionalProperties = attribute.Value.GetBoolean();
                        break;
                }
            }
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
            return "(" + string.Join(", ", Members.Select(m => m.GetSignature())) + ')';
        }

        internal override void Write(StreamWriter sw)
        {
            var className = GetName();

            sw.WriteLine("/// <summary>");
            sw.WriteLine("/// " + Title + '.');
            sw.WriteLine("/// </summary>");
            if (!string.IsNullOrEmpty(Schema))
            {
                sw.WriteLine("[DataContract(Namespace = \"" + Schema + "\")]");
            }
            sw.WriteLine("public class " + className);
            sw.WriteLine("{");

            for (var i = 0; i < Members.Count; ++i)
            {
                var member = Members[i];
                member.Write(sw);
                if (i != Members.Count - 1)
                {
                    sw.WriteLine();
                }
            }

            var requiredArrayMembers = Members.Where(m => m.IsArray && m.IsRequired).ToList();
            if (requiredArrayMembers.Count > 0)
            {
                sw.WriteLine();

                sw.WriteLine("\tpublic " + className + "()");
                sw.WriteLine("\t{");

                for (var i = 0; i < requiredArrayMembers.Count; ++i)
                {
                    var arrayMember = requiredArrayMembers[i];
                    var memberName = ObjectDefinition.ToDefinitionName(arrayMember.Name);
                    sw.WriteLine("\t\t" + memberName + " = new();");
                }
                sw.WriteLine("\t}");
            }

            sw.WriteLine("};");
        }
    }
}
