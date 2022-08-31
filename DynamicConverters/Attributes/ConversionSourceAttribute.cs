using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConverters.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class ConversionSourceAttribute : Attribute
    {
        public Type SourceType { get; protected set; }

        public ConversionSourceAttribute(Type sourceType)
        {
            SourceType = sourceType;
        }

    }
}
