// ReSharper disable MemberCanBePrivate.Global

using System.Collections.Generic;
using System.Linq;

namespace KapturePlugin
{
    public class LogFormat
    {
        private static readonly List<LogFormat> LogFormats = new List<LogFormat>();
        public static readonly List<string> LogFormatNames = new List<string>();

        public static readonly LogFormat CSV = new LogFormat(0, "CSV", ".csv", 1);
        public static readonly LogFormat JSON = new LogFormat(1, "JSON", ".json", 1);
        public static readonly LogFormat ChatLog = new LogFormat(2, "Chat Log", ".log", 1);

        public LogFormat()
        {
        }

        private LogFormat(int code, string name, string extension, int schemaVersion)
        {
            Code = code;
            Name = name;
            Extension = extension;
            SchemaVersion = schemaVersion;
            LogFormats.Add(this);
            LogFormatNames.Add(name);
        }

        public int Code { get; }
        public string Name { get; }
        public string Extension { get; }
        public int SchemaVersion { get; }

        public override string ToString()
        {
            return Name;
        }

        public static string GetFileName(LogFormat logFormat)
        {
            return "Loot_v" + logFormat.SchemaVersion + logFormat.Extension;
        }

        public static LogFormat GetLogFormatByCode(int code)
        {
            return LogFormats.FirstOrDefault(logFormat => logFormat.Code == code);
        }
    }
}