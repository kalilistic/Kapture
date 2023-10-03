using System;
using System.Collections.Generic;
using System.IO;

using Dalamud.DrunkenToad;
using Dalamud.Logging;

namespace Kapture
{
    /// <summary>
    /// Data manager.
    /// </summary>
    public class PluginDataManager
    {
        /// <summary>
        /// Kapture plugin.
        /// </summary>
        protected readonly IKapturePlugin Plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginDataManager"/> class.
        /// </summary>
        /// <param name="plugin">kapture plugin.</param>
        public PluginDataManager(IKapturePlugin plugin)
        {
            this.Plugin = plugin;
            this.DataPath = KapturePlugin.PluginInterface.ConfigDirectory + "/data/";
            this.CreateDataDirectory();
        }

        /// <summary>
        /// Gets or sets data path.
        /// </summary>
        public string DataPath { get; set; }

        /// <summary>
        /// Create data file.
        /// </summary>
        /// <param name="fileName">filename of data file.</param>
        internal static void CreateDataFile(string fileName)
        {
            var file = File.Create(fileName);
            file.Close();
        }

        /// <summary>
        /// Check if data file exists.
        /// </summary>
        /// <param name="fileName">filename of data file.</param>
        /// <returns>indicator if exists.</returns>
        internal bool DoesDataFileExist(string fileName)
        {
            return File.Exists(this.DataPath + fileName);
        }

        /// <summary>
        /// Append data string.
        /// </summary>
        /// <param name="fileName">filename of data file.</param>
        /// <param name="data">list of strings to append.</param>
        internal void AppendDataStr(string fileName, IEnumerable<string> data)
        {
            try
            {
                File.AppendAllLines(this.DataPath + fileName, data);
            }
            catch (Exception ex)
            {
                PluginLog.LogError(ex, "Failed to write data file.");
            }
        }

        /// <summary>
        /// Create data directory.
        /// </summary>
        internal void CreateDataDirectory()
        {
            try
            {
                Directory.CreateDirectory(this.DataPath);
            }
            catch (Exception ex)
            {
                PluginLog.LogError(ex, "Failed to create data directory.");
            }
        }

        /// <summary>
        /// Init data files.
        /// </summary>
        /// <param name="fileNames">list of file names.</param>
        /// <param name="force">indicator to overwrite files.</param>
        internal void InitDataFiles(IEnumerable<string> fileNames, bool force = false)
        {
            foreach (var fileName in fileNames)
            {
                if (!File.Exists(this.DataPath + fileName) || force)
                    CreateDataFile(this.DataPath + fileName);
            }
        }

        /// <summary>
        /// Save data list.
        /// </summary>
        /// <param name="fileName">filename of data file.</param>
        /// <param name="data">list of strings to append.</param>
        internal void SaveDataList(string fileName, IEnumerable<string> data)
        {
            var dataFile = this.DataPath + fileName;
            var tempFile = this.DataPath + fileName + ".tmp";
            try
            {
                File.Delete(tempFile);
                CreateDataFile(tempFile);
                using (var sw = new StreamWriter(tempFile, false))
                {
                    foreach (var entry in data)
                    {
                        if (!string.IsNullOrEmpty(entry))
                            sw.WriteLine(entry);
                    }
                }

                File.Delete(dataFile);
                File.Move(tempFile, dataFile);
            }
            catch (Exception ex)
            {
                PluginLog.LogError(ex, "Failed to write data file.");
            }
        }

        /// <summary>
        /// Save data string.
        /// </summary>
        /// <param name="fileName">filename of data file.</param>
        /// <param name="data">list of strings to append.</param>
        internal void SaveDataStr(string fileName, string data)
        {
            try
            {
                File.WriteAllText(this.DataPath + fileName, data);
            }
            catch (Exception ex)
            {
                PluginLog.LogError(ex, "Failed to write data file.");
            }
        }

        /// <summary>
        /// Read data string.
        /// </summary>
        /// <param name="fileName">filename of data file.</param>
        /// <returns>data read.</returns>
        internal string ReadDataStr(string fileName)
        {
            try
            {
                return File.ReadAllText(this.DataPath + fileName);
            }
            catch (Exception ex)
            {
                PluginLog.LogError(ex, "Failed to read data file.");
            }

            return string.Empty;
        }

        /// <summary>
        /// Read data list.
        /// </summary>
        /// <param name="fileName">filename of data file.</param>
        /// <returns>data read.</returns>
        internal List<string> ReadDataList(string fileName)
        {
            try
            {
                var data = new List<string> { string.Empty };
                using var sr = new StreamReader(this.DataPath + fileName);
                string line;
                while ((line = sr.ReadLine() ?? string.Empty) != null) data.Add(line);

                return data;
            }
            catch (Exception ex)
            {
                PluginLog.LogError(ex, "Failed to read data file.");
            }

            return new List<string>();
        }
    }
}
