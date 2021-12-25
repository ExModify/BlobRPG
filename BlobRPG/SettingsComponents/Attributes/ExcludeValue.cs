using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.SettingsComponents.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    internal class ExcludeValue : Attribute
    {
        internal double[] Values { get; private set; }

        internal ExcludeValue(params double[] values)
        {
            Values = values;
        }
    }
}
