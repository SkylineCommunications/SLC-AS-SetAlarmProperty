namespace SetAlarmProperty_1
{
	using System;
	using SetAlarmProperty_1.ValueInput;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Messages;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	/// <summary>
	/// Represents a DataMiner Automation script.
	/// </summary>
	public class Script
	{
		/// <summary>
		/// The script entry point.
		/// </summary>
		/// <param name="engine">Link with SLAutomation process.</param>
		public void Run(IEngine engine)
		{
			// DO NOT REMOVE THIS COMMENTED OUT CODE OR THE SCRIPT WONT RUN!
			// Interactive scripts need to be launched differently.
			// This is determined by a simple string search looking for "engine.ShowUI" in the source code.
			// However, due to the toolkit nuget package, this string cannot be found here.
			// So this comment is here as a workaround.
			// engine.ShowUI();
			try
			{
				RunSafe(engine);
			}
			catch (Exception e)
			{
				engine.ExitFail("Run|Something went wrong: " + e);
			}
		}

		private void RunSafe(IEngine engine)
		{
			var controller = new InteractiveController(engine);
			var rootAlarmIds = engine.GetScriptParam("Root Alarm ID").Value;
			var propertyName = engine.GetScriptParam("Property Name").Value;
			var valueInputPresenter = new ValueInputPresenter(engine, rootAlarmIds, propertyName,controller);

			controller.ShowDialog(valueInputPresenter.View);
		}
	}
}
