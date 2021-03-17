// ReSharper disable MemberInitializerValueIgnored
// ReSharper disable UnusedParameter.Local
// ReSharper disable VirtualMemberCallInConstructor
// ReSharper disable UnusedParameter.Global

using System;
using System.Collections.Generic;
using System.Timers;

namespace Kapture
{
    public class LootLogger
    {
        private readonly Queue<LootEvent> _logEventQueue = new Queue<LootEvent>();
        private readonly IKapturePlugin _plugin;
        private readonly Timer _writeTimer;
        private bool _isProcessing = true;

        public LootLogger(IKapturePlugin plugin)
        {
            _plugin = plugin;
            SetLogFormat();
            _writeTimer = new Timer
                {Interval = _plugin.Configuration.WriteToLogFrequency, Enabled = true};
            _writeTimer.Elapsed += WriteToLogFile;
            _isProcessing = false;
        }

        public void SetLogFormat()
        {
            var logFormat = LogFormat.GetLogFormatByCode(_plugin.Configuration.LogFormat);
            var fileName = LogFormat.GetFileName(logFormat);
            var alreadyExists = _plugin.DataManager.DoesDataFileExist(fileName);
            _plugin.DataManager.InitDataFiles(new[] {fileName});
            if (!alreadyExists)
            {
                if (logFormat == LogFormat.CSV)
                    _plugin.DataManager.SaveDataStr(fileName, LootEvent.GetCsvHeadings() + Environment.NewLine);
                else
                    _plugin.DataManager.CreateDataFile(fileName);
            }
        }

        private void WriteToLogFile(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (_isProcessing) return;
                _isProcessing = true;
                var logFormat = LogFormat.GetLogFormatByCode(_plugin.Configuration.LogFormat);
                var fileName = LogFormat.GetFileName(logFormat);
                var logEventsData = BuildData(logFormat);
                if (logEventsData.Count > 0) _plugin.DataManager.AppendDataStr(fileName, logEventsData);
            }
            catch (Exception ex)
            {
                _plugin.LogError(ex, "Failed to write loot log event.");
            }

            _isProcessing = false;
        }

        private List<string> BuildData(LogFormat logFormat)
        {
            var logEventsData = new List<string>();
            if (logFormat == LogFormat.CSV)
                while (_logEventQueue.Count > 0)
                {
                    var logEvent = _logEventQueue.Dequeue();
                    logEventsData.Add(logEvent.ToCsv());
                }
            else if (logFormat == LogFormat.JSON)
                while (_logEventQueue.Count > 0)
                {
                    var logEvent = _logEventQueue.Dequeue();
                    logEventsData.Add(logEvent.ToString());
                }
            else if (logFormat == LogFormat.ChatLog)
                while (_logEventQueue.Count > 0)
                {
                    var logEvent = _logEventQueue.Dequeue();
                    logEventsData.Add(logEvent.LootMessage.Message);
                }

            return logEventsData;
        }

        public void LogLoot(LootEvent lootEvent)
        {
            if (lootEvent == null) return;
            _logEventQueue.Enqueue(lootEvent);
        }

        public void Dispose()
        {
            if (!_isProcessing)
            {
                WriteToLogFile(this, null);
                _isProcessing = true;
            }

            _writeTimer.Elapsed -= WriteToLogFile;
            _writeTimer.Stop();
        }
    }
}