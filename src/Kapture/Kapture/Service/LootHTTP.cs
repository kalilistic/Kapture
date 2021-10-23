using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Timers;

using Dalamud.DrunkenToad;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kapture
{
    /// <summary>
    /// Send loot messages to http endpoint.
    /// </summary>
    public class LootHTTP
    {
        private readonly object locker = new ();
        private readonly Queue<string> httpEventQueue = new ();
        private readonly IKapturePlugin plugin;
        private readonly Timer sendTimer;
        private readonly JsonSerializerSettings jsonSerializerSettings;
        private readonly HttpClient httpClient;
        private bool isProcessing;

        /// <summary>
        /// Initializes a new instance of the <see cref="LootHTTP"/> class.
        /// </summary>
        /// <param name="plugin">kapture plugin.</param>
        public LootHTTP(IKapturePlugin plugin)
        {
            this.plugin = plugin;
            this.httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMilliseconds(plugin.Configuration.SendHTTPRequestTimeout),
            };
            this.sendTimer = new Timer
            {
                Interval = this.plugin.Configuration.SendHTTPFrequency, Enabled = true,
            };
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy(),
            };
            this.jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented,
            };
            this.sendTimer.Elapsed += this.SendToHTTPEndpoint;
            this.isProcessing = false;
        }

        /// <summary>
        /// Send Loot to HTTP Endpoint queue.
        /// </summary>
        /// <param name="lootEvent">loot event to send.</param>
        public void SendToHTTPQueue(LootEvent lootEvent)
        {
            lock (this.locker)
            {
                string json = JsonConvert.SerializeObject(
                    string.IsNullOrEmpty(this.plugin.Configuration.HTTPCustomJSON) ?
                        new object[] { lootEvent } :
                        new object[] { lootEvent, this.plugin.Configuration.HTTPCustomJSON },
                    this.jsonSerializerSettings);
                this.httpEventQueue.Enqueue(json);
            }
        }

        /// <summary>
        /// Dispose service.
        /// </summary>
        public void Dispose()
        {
            if (!this.isProcessing)
            {
                this.SendToHTTPEndpoint(this, null);
                this.isProcessing = true;
            }

            this.sendTimer.Elapsed -= this.SendToHTTPEndpoint;
            this.sendTimer.Stop();
            this.httpClient.Dispose();
        }

        private async void SendToHTTPEndpoint(object sender, ElapsedEventArgs? e)
        {
            if (this.httpEventQueue.Count == 0) return;
            try
            {
                if (this.isProcessing) return;
                this.isProcessing = true;
                await this.httpClient.PostAsync(
                    new Uri(this.plugin.Configuration.HTTPEndpoint),
                    new StringContent(this.httpEventQueue.Peek(), Encoding.UTF8, "application/json"));
                this.httpEventQueue.Dequeue();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to send http event.");
            }

            this.isProcessing = false;
        }
    }
}
