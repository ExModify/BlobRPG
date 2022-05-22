using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Tools
{
    public class StructReferencer<T> where T : struct
    {
        public T Value { get; set; }

        public StructReferencer(T value)
        {
            Value = value;
        }
    }
}
