using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.LoggerComponents
{
    struct Log
    {
        public string Message { get; private set; }
        public LogSeverity Severity { get; private set; }
        public LogModule Module { get; private set; }

        public Log(string message, LogSeverity severity, LogModule module)
        {
            Message = message;
            Severity = severity;
            Module = module;
        }
    }
}
