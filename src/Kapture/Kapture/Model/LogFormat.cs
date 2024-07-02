using System.Collections.Generic;
using System.Linq;

namespace Kapture
{
    /// <summary>
    /// Log format.
    /// </summary>
    public class LogFormat
    {
        /// <summary>
        /// Log formats.
        /// </summary>
        public static readonly List<LogFormat> LogFormats = new ();

        /// <summary>
        /// Log format names.
        /// </summary>
        public static readonly List<string> LogFormatNames = new ();

        /// <summary>
        /// Log format: CSV.
        /// </summary>
        public static readonly LogFormat CSV = new (0, "CSV", ".csv", 1);

        /// <summary>
        /// Log Format: JSON.
        /// </summary>
        public static readonly LogFormat JSON = new (1, "JSON", ".json", 1);

        /// <summary>
        /// Log Format: Chat Log.
        /// </summary>
        public static readonly LogFormat ChatLog = new (2, "Chat Log", ".log", 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="LogFormat"/> class.
        /// </summary>
        public LogFormat()
        {
        }

        private LogFormat(int code, string name, string extension, int schemaVersion)
        {
            this.Code = code;
            this.Name = name;
            this.Extension = extension;
            this.SchemaVersion = schemaVersion;
            LogFormats.Add(this);
            LogFormatNames.Add(name);
        }

        /// <summary>
        /// Gets log format code.
        /// </summary>
        public int Code { get; }

        /// <summary>
        /// Gets log format name.
        /// </summary>
        public string Name { get; } = null!;

        /// <summary>
        /// Gets log format extension.
        /// </summary>
        public string Extension { get; } = null!;

        /// <summary>
        /// Gets log format schema version.
        /// </summary>
        public int SchemaVersion { get; }

        /// <summary>
        /// Get log file name based on format.
        /// </summary>
        /// <param name="logFormat">format.</param>
        /// <returns>file name.</returns>
        public static string GetFileName(LogFormat logFormat)
        {
            return "Loot_v" + logFormat.SchemaVersion + logFormat.Extension;
        }

        /// <summary>
        /// Get Log format by code.
        /// </summary>
        /// <param name="code">log code.</param>
        /// <returns>log format.</returns>
        public static LogFormat GetLogFormatByCode(int code)
        {
            return LogFormats.FirstOrDefault(logFormat => logFormat.Code == code) ?? ChatLog;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
