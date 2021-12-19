using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.SettingsComponents.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    internal class Savable : Attribute
    {
        internal string Group { get; private set; }

        internal Savable(string group)
        {
            Group = group;
        }
    }
}
