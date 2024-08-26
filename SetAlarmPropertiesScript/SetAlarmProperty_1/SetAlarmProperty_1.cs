namespace SetAlarmProperty_1
{
	using System;
	using SetAlarmProperty_1.PropertyInput;
	using SetAlarmProperty_1.ValueInput;

	using Skyline.DataMiner.Automation;
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
			var rootAlarmId = Convert.ToString(engine.GetScriptParam("Root Alarm ID").Value);
			var propertyName = Convert.ToString(engine.GetScriptParam("Property Name").Value);
			var controller = new InteractiveController(engine);
			var propertySelector = new PropertyInputSelector(engine)
			{
				RootAlarmId = rootAlarmId,
				PropertyName = propertyName,
			};

			if (propertySelector.TryFindProperty())
			{
				var valueInputView = new ValueInputView(engine, propertySelector.PropertyName, propertySelector.FoundProperty);
				var valueInputPresenter = new ValueInputPresenter(valueInputView, propertySelector, engine);

				controller.ShowDialog(valueInputView);
			}
			else
			{
				// If the property cannot be found, exit with failure
				engine.ExitFail("Failed to find the specified property. Please check the Root Alarm ID and Property Name.");
			}
		}
	}
}
