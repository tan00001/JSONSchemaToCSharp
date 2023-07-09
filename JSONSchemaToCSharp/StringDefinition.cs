using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JSONSchemaToCSharp
{
    internal class StringDefinition : ObjectDefinition
    {
        public int? MinLength { get; set; }

        public int? MaxLength { get; set; }

        public override bool IsReferenceType
        {
            get
            {
                return true;
            }
        }

        public StringDefinition(JsonElement.ObjectEnumerator attributes)
        {
            foreach (var attribute in attributes)
            {
                switch (attribute.Name)
                {
                    case "title":
                        this.Title = attribute.Value.GetString();
                        break;

                    case "minLength":
                        this.MinLength = attribute.Value.GetInt32();
                        break;

                    case "maxLength":
                        this.MaxLength = attribute.Value.GetInt32();
                        break;
                }
            }
        }

        public override string GetName()
        {
            return "string";
        }

        public override string GetSignature()
        {
            return "string";
        }

        internal override void WriteAttributes(StreamWriter sw, bool isArray)
        {
            if (isArray)
            {
                return;
            }

            if (MaxLength != null)
            {
                if (MinLength != null)
                {
                    sw.WriteLine("\t[StringLength(" + MaxLength.Value + ", MinimumLength = " + MinLength.Value + ")]");
                }
                else
                {
                    sw.WriteLine("\t[StringLength(" + MaxLength.Value + ")]");
                }
            }
        }
    }
}
