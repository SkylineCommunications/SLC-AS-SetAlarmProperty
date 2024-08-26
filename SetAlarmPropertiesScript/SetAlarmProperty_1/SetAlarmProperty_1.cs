namespace SetAlarmProperty_1
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Text;

	using Microsoft.Win32;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;

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
			try
			{
				RunSafe(engine);
			}
			catch (ScriptAbortException)
			{
				// Catch normal abort exceptions (engine.ExitFail or engine.ExitSuccess)
				throw; // Comment if it should be treated as a normal exit of the script.
			}
			catch (ScriptForceAbortException)
			{
				// Catch forced abort exceptions, caused via external maintenance messages.
				throw;
			}
			catch (ScriptTimeoutException)
			{
				// Catch timeout exceptions for when a script has been running for too long.
				throw;
			}
			catch (InteractiveUserDetachedException)
			{
				// Catch a user detaching from the interactive script by closing the window.
				// Only applicable for interactive scripts, can be removed for non-interactive scripts.
				throw;
			}
			catch (Exception e)
			{
				engine.ExitFail("Run|Something went wrong: " + e);
			}
		}

		private void RunSafe(IEngine engine)
		{
			var propertyName = engine.GetScriptParam("Property Name");
			var rootAlarmId = engine.GetScriptParam("Root Alarm Id");

			string[] ids = rootAlarmId.Value.ToString().Split('/');
			string dataminerId = ids[0];
			string elementId = ids[1];
			string alarmId = ids[2];
			var property = engine.GetAlarmProperty(Convert.ToInt32(dataminerId), Convert.ToInt32(alarmId), propertyName.Value);
		}
	}
}
