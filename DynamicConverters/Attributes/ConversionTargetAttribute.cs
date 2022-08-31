using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConverters.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class ConversionTargetAttribute : Attribute
    {
        public Type TargetType { get; protected set; }

        public ConversionTargetAttribute(Type targetType)
        {
            TargetType = targetType;
        }
    }
}
