using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JSONSchemaToCSharp
{
    internal class NumberDefinition : ObjectDefinition
    {
        public decimal? Minimum { get; set; }

        public decimal? Maximum { get; set; }

        public bool ExclusiveMaximum { get; set; }

        public decimal? MultipleOf  { get; set;}

        public NumberDefinition(JsonElement.ObjectEnumerator attributes)
        {
            foreach (var attribute in attributes)
            {
                switch (attribute.Name)
                {
                    case "title":
                        this.Title = attribute.Value.GetString();
                        break;

                    case "minimum":
                        this.Minimum = attribute.Value.GetDecimal();
                        break;

                    case "maximum":
                        this.Maximum = attribute.Value.GetDecimal();
                        break;

                    case "exclusiveMaximum":
                        this.ExclusiveMaximum = attribute.Value.GetBoolean();
                        break;

                    case "multipleOf":
                        this.MultipleOf = attribute.Value.GetDecimal();
                        break;
                }
            }
        }

        public override string GetName()
        {
            return "decimal";
        }

        public override string GetSignature()
        {
            return "decimal";
        }

        internal override void WriteAttributes(StreamWriter sw)
        {
            if (Minimum != null)
            {
                if (Maximum != null)
                {
                    sw.WriteLine("\t[Range(typeof(decimal), \"" + Minimum.Value + "\", \"" + Maximum.Value + "\")]");
                }
                else
                {
                    sw.WriteLine("\t[Range(typeof(decimal), \"" + Minimum.Value + "\", \"" + decimal.MaxValue + "\")]");
                }
            }
        }
    }
}
