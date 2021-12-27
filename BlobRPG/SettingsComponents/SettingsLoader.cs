using BlobRPG.LoggerComponents;
using BlobRPG.SettingsComponents;
using BlobRPG.SettingsComponents.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.SettingsComponents
{
    public class SettingsLoader : ILogger
    {
        private static new LogModule Module = LogModule.Settings;
        private static Dictionary<string, PropertyInfo> Properties;
        private static Dictionary<string, List<PropertyInfo>> PropertiesGroup;

        private const string ConfigFile = "GameSettings.cfg";

        public static void LoadSettings()
        {
            GetPropertiesIfNull();

            if (!File.Exists(ConfigFile))
            {
                SaveSettings();
                return;
            }

            string[] lines = File.ReadAllLines(ConfigFile);
            string[] data;
            int counter = 0;

            foreach (string line in lines)
            {
                if ((data = line.Split("=", 2, StringSplitOptions.RemoveEmptyEntries)).Length != 2)
                    continue;

                if (Properties.ContainsKey(data[0].ToLower()))
                {
                    if (!SetProperty(Properties[data[0].ToLower()], data[1]))
                    {
                        Log(Module, Error, $"{ data[0] } property couldn't be loaded. Value: \"{ data[1] }\".");
                    }
                    else
                    {
                        counter++;
                    }
                }
            }
            if (counter != Properties.Count)
            {
                SaveSettings();
            }

            Settings.LogFileStream = new FileStream("runtime.log", FileMode.Create);
            Settings.LogFile = new StreamWriter(Settings.LogFileStream);
        }
        public static void SaveSettings()
        {
            GetPropertiesIfNull();

            if (File.Exists(ConfigFile))
                File.Delete(ConfigFile);

            FileStream fs = File.OpenWrite(ConfigFile);
            StreamWriter writer = new StreamWriter(fs);

            bool first = true;

            foreach (string key in PropertiesGroup.Keys)
            {
                if (!first)
                {
                    writer.WriteLine();
                }
                first = false;
                writer.WriteLine($"[{ key }]");
                foreach (PropertyInfo info in PropertiesGroup[key])
                {
                    writer.WriteLine($"{ info.Name }={ info.GetValue(null) }");
                }
            }

            writer.Flush();
            writer.Close();
            fs.Close();
        }

        private static void GetPropertiesIfNull()
        {
            if (Properties == null)
            {
                Properties = new();
                PropertiesGroup = new();

                PropertyInfo[] infos = typeof(Settings).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

                foreach (PropertyInfo info in infos)
                {
                    if (info.CustomAttributes.Any())
                    {
                        List<Attribute> attribs = info.GetCustomAttributes().ToList();

                        foreach (Attribute attribGeneric in attribs)
                        {
                            if (attribGeneric is Savable attr)
                            {
                                Properties.Add(info.Name.ToLower(), info);

                                if (PropertiesGroup.ContainsKey(attr.Group))
                                {
                                    PropertiesGroup[attr.Group].Add(info);
                                }
                                else
                                {
                                    PropertiesGroup[attr.Group] = new List<PropertyInfo>()
                                    {
                                        info
                                    };
                                }

                                break;
                            }
                        }
                    }
                }
            }
        }
        private static bool SetProperty(PropertyInfo info, string value)
        {
            try
            {
                switch (info.PropertyType.Name)
                {
                    case "Boolean":
                        info.SetValue(null, bool.Parse(value));
                        break;

                    case "Byte":
                        info.SetValue(null, (byte)CheckLimit(info, byte.Parse(value)));
                        break;
                    case "SByte":
                        info.SetValue(null, (sbyte)CheckLimit(info, sbyte.Parse(value)));
                        break;

                    case "Int16":
                        info.SetValue(null, (short)CheckLimit(info, short.Parse(value)));
                        break;
                    case "Int32":
                        info.SetValue(null, (int)CheckLimit(info, int.Parse(value)));
                        break;
                    case "Int64":
                        info.SetValue(null, (long)CheckLimit(info, long.Parse(value)));
                        break;

                    case "UInt32":
                        info.SetValue(null, (uint)CheckLimit(info, uint.Parse(value)));
                        break;
                    case "UInt64":
                        info.SetValue(null, (ulong)CheckLimit(info, ulong.Parse(value)));
                        break;

                    case "Single":
                        info.SetValue(null, (float)CheckLimit(info, float.Parse(value)));
                        break;
                    case "Double":
                        info.SetValue(null, CheckLimit(info, double.Parse(value)));
                        break;

                    case "LogSeverity": /* Or any other enum */
                    case "WindowState":
                    case "VSyncMode":
                        info.SetValue(null, Enum.Parse(info.PropertyType, value));
                        break;

                    default:
                        Log(Module, Error, $"Setting type { info.PropertyType.Name } not handled.");
                        return false;

                }
                return true;
            }
            catch
            {
                Log(Module, Error, $"Incorrect type / value for { info.PropertyType.Name }.");
                return false;
            }
        }
       
        private static double CheckLimit(PropertyInfo info, double value)
        {
            double? min = null, max = null;
            List<double> exclude = new List<double>();

            if (info.CustomAttributes.Any())
            {
                List<Attribute> attribs = info.GetCustomAttributes().ToList();

                foreach (Attribute attribGeneric in attribs)
                {
                    if (attribGeneric is Min minAttr)
                    {
                        min = minAttr.Value;
                    }
                    else if (attribGeneric is Max maxAttr)
                    {
                        max = maxAttr.Value;
                    }
                    else if (attribGeneric is ExcludeValue exclAttr)
                    {
                        exclude.AddRange(exclAttr.Values);
                    }
                }
            }
            if (exclude.Contains(value))
                return value;

            if (min.HasValue && value < min.Value)
                return min.Value;
            else if (max.HasValue && value > max.Value)
                return max.Value;

            return value;
        }
    }
}
