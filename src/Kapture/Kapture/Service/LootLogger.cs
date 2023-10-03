using System;
using System.Collections.Generic;
using System.Timers;

using Dalamud.DrunkenToad;
using Dalamud.Logging;

namespace Kapture
{
    /// <summary>
    /// Send loot messages to log file.
    /// </summary>
    public class LootLogger
    {
        private readonly Queue<LootEvent> logEventQueue = new();
        private readonly IKapturePlugin plugin;
        private readonly Timer writeTimer;
        private bool isProcessing;

        /// <summary>
        /// Initializes a new instance of the <see cref="LootLogger"/> class.
        /// </summary>
        /// <param name="plugin">kapture plugin.</param>
        public LootLogger(IKapturePlugin plugin)
        {
            this.plugin = plugin;
            this.SetLogFormat();
            this.writeTimer = new Timer
            {
                Interval = this.plugin.Configuration.WriteToLogFrequency, Enabled = true,
            };
            this.writeTimer.Elapsed += this.WriteToLogFile;
            this.isProcessing = false;
        }

        /// <summary>
        /// Define log format.
        /// </summary>
        public void SetLogFormat()
        {
            var logFormat = LogFormat.GetLogFormatByCode(this.plugin.Configuration.LogFormat);
            var fileName = LogFormat.GetFileName(logFormat);
            var alreadyExists = this.plugin.PluginDataManager.DoesDataFileExist(fileName);
            this.plugin.PluginDataManager.InitDataFiles(new[] { fileName });
            if (!alreadyExists)
            {
                if (logFormat == LogFormat.CSV)
                    this.plugin.PluginDataManager.SaveDataStr(fileName, LootEvent.GetCsvHeadings() + Environment.NewLine);
                else
                    PluginDataManager.CreateDataFile(fileName);
            }
        }

        /// <summary>
        /// Log loot message to file.
        /// </summary>
        /// <param name="lootEvent">loot event to log.</param>
        public void LogLoot(LootEvent lootEvent)
        {
            this.logEventQueue.Enqueue(lootEvent);
        }

        /// <summary>
        /// Dispose service.
        /// </summary>
        public void Dispose()
        {
            if (!this.isProcessing)
            {
                this.WriteToLogFile(this, null);
                this.isProcessing = true;
            }

            this.writeTimer.Elapsed -= this.WriteToLogFile;
            this.writeTimer.Stop();
        }

        private void WriteToLogFile(object? sender, ElapsedEventArgs? e)
        {
            try
            {
                if (this.isProcessing) return;
                this.isProcessing = true;
                var logFormat = LogFormat.GetLogFormatByCode(this.plugin.Configuration.LogFormat);
                var fileName = LogFormat.GetFileName(logFormat);
                var logEventsData = this.BuildData(logFormat);
                if (logEventsData.Count > 0) this.plugin.PluginDataManager.AppendDataStr(fileName, logEventsData);
            }
            catch (Exception ex)
            {
                PluginLog.LogError(ex, "Failed to write loot log event.");
            }

            this.isProcessing = false;
        }

        private List<string> BuildData(LogFormat logFormat)
        {
            var logEventsData = new List<string>();
            if (logFormat == LogFormat.CSV)
            {
                while (this.logEventQueue.Count > 0)
                {
                    var logEvent = this.logEventQueue.Dequeue();
                    logEventsData.Add(logEvent.ToCsv());
                }
            }
            else if (logFormat == LogFormat.JSON)
            {
                while (this.logEventQueue.Count > 0)
                {
                    var logEvent = this.logEventQueue.Dequeue();
                    logEventsData.Add(logEvent.ToString());
                }
            }
            else if (logFormat == LogFormat.ChatLog)
            {
                while (this.logEventQueue.Count > 0)
                {
                    var logEvent = this.logEventQueue.Dequeue();
                    logEventsData.Add(logEvent.LootMessage.Message);
                }
            }

            return logEventsData;
        }
    }
}
