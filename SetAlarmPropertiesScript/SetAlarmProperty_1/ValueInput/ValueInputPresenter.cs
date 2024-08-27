namespace SetAlarmProperty_1.ValueInput
{
	using System;
	using SetAlarmProperty_1.PropertyInput;
	using Skyline.DataMiner.Automation;

	public class ValueInputPresenter
	{
		private readonly ValueInputView _view;
		private readonly PropertyInputSelector _propertySelector;
		private readonly IEngine _engine;

		public ValueInputPresenter(ValueInputView view, PropertyInputSelector propertySelector, IEngine engine)
		{
			_view = view;
			_propertySelector = propertySelector;
			_engine = engine;

			_view.OkClicked += OnOkButtonClicked;
		}

		public event EventHandler Finished;

		private void OnOkButtonClicked(object sender, EventArgs e)
		{
			string newValue = _view.GetNewValue();

			try
			{
				string[] ids = _propertySelector.RootAlarmId.Split('/');
				int dataMinerId = Convert.ToInt32(ids[0]);
				int alarmId = Convert.ToInt32(ids[2]);

				_engine.SetAlarmProperty(dataMinerId, alarmId, _propertySelector.PropertyName, newValue);
			}
			catch (Exception ex)
			{
				_engine.ExitFail("Failed to update property value: " + ex.Message);
			}
		}
	}
}
