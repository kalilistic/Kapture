// ReSharper disable MemberInitializerValueIgnored
// ReSharper disable UnusedParameter.Local
// ReSharper disable VirtualMemberCallInConstructor
// ReSharper disable UnusedParameter.Global

using System;
using System.Collections.Generic;
using System.Timers;

namespace KapturePlugin
{
    public class Kapture
    {
        private readonly int _csvSchemaVersion = 1;
        private readonly string _fileName;
        private readonly Queue<string> _logEventQueue = new Queue<string>();
        private readonly Timer _writeTimer;
        private readonly IKapturePlugin _plugin;
        private bool _isProcessing = true;

        public Kapture(IKapturePlugin plugin)
        {
            _plugin = plugin;
            _fileName = "Loot_v" + _csvSchemaVersion + ".csv";
            var alreadyExists = _plugin.DataManager.DoesDataFileExist(_fileName);
            _plugin.DataManager.InitDataFiles(new[] {_fileName});
            if (!alreadyExists) _plugin.DataManager.SaveDataStr(_fileName, LootEvent.GetCsvHeadings() + Environment.NewLine);
            _writeTimer = new Timer
                {Interval = _plugin.Configuration.WriteToLogFrequency, Enabled = true};
            _writeTimer.Elapsed += WriteToLogFile;
            _isProcessing = false;
        }

        private void WriteToLogFile(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (_isProcessing) return;
                _isProcessing = true;
                var logEventsData = new List<string>();
                while (_logEventQueue.Count > 0)
                {
                    var logEventData = _logEventQueue.Dequeue();
                    logEventsData.Add(logEventData);
                }

                if (logEventsData.Count > 0) _plugin.DataManager.AppendDataStr(_fileName, logEventsData);
            }
            catch (Exception ex)
            {
                _plugin.LogError(ex, "Failed to write loot log event.");
            }

            _isProcessing = false;
        }

        public void LogLoot(LootEvent lootEvent)
        {
            if (lootEvent == null) return;
            var lootEventCsv = lootEvent.ToCsv();
            if (string.IsNullOrEmpty(lootEventCsv)) return;
            _logEventQueue.Enqueue(lootEventCsv);
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