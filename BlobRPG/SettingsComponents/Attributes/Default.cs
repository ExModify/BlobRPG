using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.SettingsComponents.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    internal class Default : Attribute
    {
        internal object Value { get; private set; }

        internal Default(object value)
        {
            Value = value;
        }
    }
}
