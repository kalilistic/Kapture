using System;

namespace Sample.Mock
{
	public class MockSamplePlugin : ISamplePlugin, IPluginBase
	{
		public MockSamplePlugin()
		{
			PluginName = "Sample";
			Localization = new Localization(this);
			Configuration = new MockConfig();
			SampleService = new MockSampleService();
		}

		public dynamic LoadConfig()
		{
			throw new NotImplementedException();
		}

		void IPluginBase.Dispose()
		{
			throw new NotImplementedException();
		}

		public string PluginFolder()
		{
			throw new NotImplementedException();
		}

		void IPluginBase.UpdateResources()
		{
			throw new NotImplementedException();
		}

		public void CreateDataFolder()
		{
			throw new NotImplementedException();
		}

		public string PluginVersion()
		{
			throw new NotImplementedException();
		}

		public bool IsLoggedIn()
		{
			throw new NotImplementedException();
		}

		public double ConvertHeightToInches(int raceId, int tribeId, int genderId, int sliderHeight)
		{
			throw new NotImplementedException();
		}

		string IPluginBase.GetSeIcon(SeIconChar seIconChar)
		{
			throw new NotImplementedException();
		}

		uint? IPluginBase.GetLocalPlayerHomeWorld()
		{
			throw new NotImplementedException();
		}

		void IPluginBase.LogInfo(string messageTemplate)
		{
			throw new NotImplementedException();
		}

		void IPluginBase.LogInfo(string messageTemplate, params object[] values)
		{
			throw new NotImplementedException();
		}

		void IPluginBase.LogError(string messageTemplate)
		{
			throw new NotImplementedException();
		}

		void IPluginBase.LogError(string messageTemplate, params object[] values)
		{
			throw new NotImplementedException();
		}

		void IPluginBase.LogError(Exception exception, string messageTemplate, params object[] values)
		{
			throw new NotImplementedException();
		}

		bool IPluginBase.IsKeyPressed(ModifierKey.Enum key)
		{
			throw new NotImplementedException();
		}

		bool IPluginBase.IsKeyPressed(PrimaryKey.Enum key)
		{
			throw new NotImplementedException();
		}

		void IPluginBase.SaveConfig(dynamic config)
		{
			throw new NotImplementedException();
		}

		void IPluginBase.PrintMessage(string message)
		{
			throw new NotImplementedException();
		}

		public ISampleService SampleService { get; }
		public SampleConfig Configuration { get; set; }
		public Localization Localization { get; }
		public string PluginName { get; }

		public void PrintHelpMessage()
		{
			throw new NotImplementedException();
		}

		void ISamplePlugin.Dispose()
		{
			throw new NotImplementedException();
		}

		public void SaveConfig()
		{
			throw new NotImplementedException();
		}

		public void SetupCommands()
		{
			throw new NotImplementedException();
		}

		public void RemoveCommands()
		{
			throw new NotImplementedException();
		}

		public void ToggleSample(string command, string args)
		{
			throw new NotImplementedException();
		}

		public void ToggleConfig(string command, string args)
		{
			throw new NotImplementedException();
		}

		public void ExportLocalizable(string command, string args)
		{
			throw new NotImplementedException();
		}

		void ISamplePlugin.PrintMessage(string message)
		{
			throw new NotImplementedException();
		}

		string ISamplePlugin.GetSeIcon(SeIconChar seIconChar)
		{
			throw new NotImplementedException();
		}

		uint? ISamplePlugin.GetLocalPlayerHomeWorld()
		{
			throw new NotImplementedException();
		}

		void ISamplePlugin.LogInfo(string messageTemplate)
		{
			throw new NotImplementedException();
		}

		void ISamplePlugin.LogInfo(string messageTemplate, params object[] values)
		{
			throw new NotImplementedException();
		}

		void ISamplePlugin.LogError(string messageTemplate)
		{
			throw new NotImplementedException();
		}

		void ISamplePlugin.LogError(string messageTemplate, params object[] values)
		{
			throw new NotImplementedException();
		}

		void ISamplePlugin.LogError(Exception exception, string messageTemplate, params object[] values)
		{
			throw new NotImplementedException();
		}

		bool ISamplePlugin.IsKeyPressed(ModifierKey.Enum key)
		{
			throw new NotImplementedException();
		}

		bool ISamplePlugin.IsKeyPressed(PrimaryKey.Enum key)
		{
			throw new NotImplementedException();
		}

		void ISamplePlugin.SaveConfig(dynamic config)
		{
			throw new NotImplementedException();
		}

		void ISamplePlugin.UpdateResources()
		{
			throw new NotImplementedException();
		}
	}
}