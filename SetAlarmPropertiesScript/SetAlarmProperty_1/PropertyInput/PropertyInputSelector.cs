namespace SetAlarmProperty_1.PropertyInput
{
	using System;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Net.Messages;

	public class PropertyInputSelector
	{
		private readonly IEngine engine;

		public PropertyInputSelector(IEngine engine)
		{
			this.engine = engine;
		}

		public string RootAlarmId { get; set; }

		public string PropertyName { get; set; }

		public string FoundProperty { get; private set; }

		public bool TryFindProperty()
		{
			try
			{
				string[] ids = RootAlarmId.Split('/');
				if (ids.Length != 3)
				{
					engine.ExitFail("Invalid RootAlarmId format.");
					return false;
				}

				int dataMinerId = Convert.ToInt32(ids[0]);
				int elementId = Convert.ToInt32(ids[1]);
				int alarmId = Convert.ToInt32(ids[2]);

				ElementID elementID = new ElementID()
				{
					DataMinerID = dataMinerId,
					EID = elementId,
				};

				AlarmTreeID alarmTreeID = new AlarmTreeID(elementID, alarmId);

				GetAlarmTreeDetailsMessage message = new GetAlarmTreeDetailsMessage(alarmTreeID);

				DMSMessage[] responseMessage =Engine.SLNet.SendMessage(message);

				if (responseMessage.Length > 0)
				{
					AlarmEventMessage alarmEvent = (AlarmEventMessage)responseMessage[responseMessage.Length-1];
					FoundProperty = engine.GetAlarmProperty(dataMinerId, elementId, alarmEvent.AlarmID, PropertyName);
					return true;
				}
				return false;
			}
			catch (Exception e)
			{
				engine.ExitFail("Run|The requested property couldn't be found: " + e.Message);
				return false;
			}
		}
	}
}
