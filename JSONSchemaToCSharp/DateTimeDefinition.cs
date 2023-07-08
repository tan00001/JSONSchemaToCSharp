using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JSONSchemaToCSharp
{
    internal class DateTimeDefinition : ObjectDefinition
    {
        public DateTimeOffset? Minimum { get; set; }

        public DateTimeOffset? Maximum { get; set; }

        public override bool IsReferenceType
        {
            get
            {
                return false;
            }
        }

        public DateTimeDefinition(JsonElement.ObjectEnumerator attributes)
        {
            foreach (var attribute in attributes)
            {
                switch (attribute.Name)
                {
                    case "title":
                        this.Title = attribute.Value.GetString();
                        break;

                    case "minimum":
                        this.Minimum = attribute.Value.GetDateTimeOffset();
                        break;

                    case "maximum":
                        this.Maximum = attribute.Value.GetDateTimeOffset();
                        break;
                }
            }
        }

        public override string GetName()
        {
            return "DateTimeOffset";
        }

        public override string GetSignature()
        {
            return "DateTimeOffset";
        }
    }
}
