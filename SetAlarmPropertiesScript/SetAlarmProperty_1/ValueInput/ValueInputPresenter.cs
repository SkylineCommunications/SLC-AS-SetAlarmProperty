﻿namespace SetAlarmProperty_1.ValueInput
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Net.Messages;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	public class ValueInputPresenter
	{
		private readonly IEngine _engine;
		private readonly List<string> _rootAlarmIds;
		private readonly string _propertyName;

		private readonly InteractiveController _interactiveController;

		public ValueInputPresenter(IEngine engine, string rootAlarmIds, string propertyName, InteractiveController interactiveController)
		{
			_engine = engine;

			_rootAlarmIds = ParseRootAlarmIds(rootAlarmIds);
			_propertyName = propertyName;
			_interactiveController = interactiveController;

			string currentValue = GetCurrentValue(_rootAlarmIds.First(), _propertyName);
			if (currentValue == null)
			{
				engine.ExitFail($"Failed to retrieve the property value for the first RootAlarmId {_rootAlarmIds.First()}.");
				return;
			}

			View = new ValueInputView(engine, _propertyName, currentValue);

			View.OkClicked += OnOkButtonClicked;
		}

		public ValueInputView View { get; }

		private void OnOkButtonClicked(object sender, EventArgs e)
		{
			string newValue = View.GetNewValue();

			SetValueForAll(_rootAlarmIds, _propertyName, newValue);

			_engine.GenerateInformation("Property values updated successfully for all Root Alarm IDs.");
			_interactiveController.Stop();
		}

		private List<string> ParseRootAlarmIds(string rootAlarmIdParam)
		{
			// Parse the input RootAlarmId array
			rootAlarmIdParam = rootAlarmIdParam.Trim('[', ']');
			return rootAlarmIdParam.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
								   .Select(id => id.Trim(' ', '\"'))
								   .ToList();
		}

		private void ParseRootAlarmId(string rootAlarmId, out int dataMinerId, out int alarmId)
		{
			string[] ids = rootAlarmId.Split('/');
			if (ids.Length != 2)
			{
				_engine.ExitFail("Invalid RootAlarmId format. Expected format: DataMinerID/AlarmId.");
				dataMinerId = 0;
				alarmId = 0;
				return;
			}

			dataMinerId = Convert.ToInt32(ids[0]);
			alarmId = Convert.ToInt32(ids[1]);
		}

		private string GetCurrentValue(string rootAlarmId, string propertyName)
		{
			try
			{
				ParseRootAlarmId(rootAlarmId, out int dataMinerId, out int alarmId);

				GetAlarmDetailsMessage getAlarmDetailsMessage = new GetAlarmDetailsMessage(dataMinerId, alarmId);
				getAlarmDetailsMessage.RequestFullTree = true;
				DMSMessage[] responseMessage = Engine.SLNet.SendMessage(getAlarmDetailsMessage);

				if (responseMessage.Length == 0)
				{
					_engine.ExitFail("No response received for GetAlarmDetailsMessage.");
					return null;
				}

				AlarmEventMessage alarmDetailsResponse = (AlarmEventMessage)responseMessage[0];
				int elementId = alarmDetailsResponse.ElementID;

				AlarmTreeID alarmTreeID = new AlarmTreeID(new ElementID(dataMinerId, elementId), alarmId);
				GetAlarmTreeDetailsMessage getAlarmTreeDetailsMessage = new GetAlarmTreeDetailsMessage(alarmTreeID);
				DMSMessage[] responseTreeMessage = Engine.SLNet.SendMessage(getAlarmTreeDetailsMessage);

				if (responseTreeMessage.Length == 0)
				{
					_engine.ExitFail("No response received for GetAlarmTreeDetailsMessage.");
					return null;
				}

				AlarmEventMessage alarmEvent = (AlarmEventMessage)responseTreeMessage.Last();
				var property = alarmEvent.Properties.FirstOrDefault(p => p.Name.Equals(propertyName));

				if (property != null)
				{
					return property.Value;
				}
				else
				{
					_engine.ExitFail($"Property '{propertyName}' not found in the alarm event.");
					return null;
				}
			}
			catch (Exception e)
			{
				_engine.ExitFail("Error in GetCurrentValue: " + e.Message);
				return null;
			}
		}

		private void SetValueForAll(List<string> rootAlarmIds, string propertyName, string newValue)
		{
			foreach (var rootAlarmId in rootAlarmIds)
			{
				try
				{
					ParseRootAlarmId(rootAlarmId, out int dataMinerId, out int alarmId);

					_engine.SetAlarmProperty(dataMinerId, alarmId, propertyName, newValue);
				}
				catch (Exception ex)
				{
					_engine.GenerateInformation($"Failed to set the property for RootAlarmId {rootAlarmId}: {ex.Message}");
				}
			}
		}
	}
}
