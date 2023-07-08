using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JSONSchemaToCSharp
{
    internal class ValueDefinition : ObjectDefinition
    {
        public string? Schema { get; set; }

        public string? Type { get; set; }

        public bool AdditionalProperties { get; set; }

        public override bool IsReferenceType
        {
            get
            {
                return false;
            }
        }

        public ValueDefinition(ClassDefinition classDefinition)
            : base(classDefinition)
        {
            this.Schema = classDefinition.Schema;
            this.Title = classDefinition.Title;
            this.Type = classDefinition.Type;
            this.Properties = classDefinition.Properties;
            this.AdditionalProperties = classDefinition.AdditionalProperties;
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
            sw.WriteLine("public struct " + className);
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

            var arrayMembers = Members.Where(m => m.IsArray).ToList();
            if (arrayMembers.Count > 0)
            {
                sw.WriteLine();

                sw.WriteLine("\tpublic " + className + "()");
                sw.WriteLine("\t{");

                for (var i = 0; i < arrayMembers.Count; ++i)
                {
                    var arrayMember = arrayMembers[i];
                    var memberName = ObjectDefinition.ToDefinitionName(arrayMember.Name);
                    sw.WriteLine("\t\t" + memberName + " = new();");
                }
                sw.WriteLine("\t}");
            }

            sw.WriteLine("};");
        }
    }
}
