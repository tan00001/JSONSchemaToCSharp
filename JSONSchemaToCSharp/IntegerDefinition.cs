using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JSONSchemaToCSharp
{
    internal class IntegerDefinition : ObjectDefinition
    {
        public int? Minimum { get; set; }

        public int? Maximum { get; set; }

        public bool ExclusiveMaximum { get; set; }

        public IntegerDefinition(JsonElement.ObjectEnumerator attributes)
        {
            foreach (var attribute in attributes)
            {
                switch (attribute.Name)
                {
                    case "title":
                        this.Title = attribute.Value.GetString();
                        break;

                    case "minimum":
                        this.Minimum = attribute.Value.GetInt32();
                        break;

                    case "maximum":
                        this.Maximum = attribute.Value.GetInt32();
                        break;

                    case "exclusiveMaximum":
                        this.ExclusiveMaximum = attribute.Value.GetBoolean();
                        break;
                }
            }
        }

        public override string GetName()
        {
            return "int";
        }

        public override string GetSignature()
        {
            return "int";
        }

        internal override void WriteAttributes(StreamWriter sw)
        {
            if (Minimum != null)
            {
                if (Maximum != null)
                {
                    sw.WriteLine("\t[Range(" + Minimum.Value + ", " + Maximum.Value + ")]");
                }
                else
                {
                    sw.WriteLine("\t[Range(" + Minimum.Value + ", " + Int32.MaxValue + ")]");
                }
            }
        }
    }
}
