using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.SettingsComponents.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    internal class Max : Attribute
    {
        internal double Value { get; private set; }

        internal Max(double max)
        {
            Value = max;
        }
    }
}
