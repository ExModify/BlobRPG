using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.LoggerComponents
{
    public abstract class ILogger
    {
        private LogModule? _module = null;
        protected LogModule Module
        {
            get
            {
                if (!_module.HasValue)
                {
                    _module = GetLogModuleFromType(GetType());
                }
                return _module.Value;
            }
            set
            {
                _module = value;
            }
        }

        protected static LogSeverity Debug { get => LogSeverity.Debug; }
        protected static LogSeverity Info { get => LogSeverity.Info; }
        protected static LogSeverity Warning { get => LogSeverity.Warning; }
        protected static LogSeverity Error { get => LogSeverity.Error; }
        protected static LogSeverity Fatal { get => LogSeverity.Fatal; }

        protected void Log(LogSeverity severity, string message)
        {
            Log(Module, severity, message);
        }
        protected static void Log(LogModule module, LogSeverity severity, string message)
        {
            if (Settings.LogSeverity <= severity)
                Console.WriteLine($"{ DateTime.Now:yyyy'/'MM'/'dd' 'HH':'mm':'ss} [{ severity }] [{ module }]: { message }");

            if (Settings.LogSeverity == LogSeverity.Fatal)
            {
                Program.Halt();
            }
        }

        protected static LogModule GetLogModuleFromType(Type type)
        {
            string name = type.Name.ToLower();
            string basename = type.BaseType?.Name.ToLower() ?? "";
            string[] modules = Enum.GetNames(typeof(LogModule));

            foreach (string module in modules)
            {

                if (module.ToLower() == name || module.ToLower() + "core" == name
                    || module.ToLower() == basename || module.ToLower() + "core" == basename)
                {
                    return (LogModule)Enum.Parse(typeof(LogModule), module);
                }
            }

            return LogModule.Main;
        }
    }
}
