namespace SetAlarmProperty_1.ValueInput
{
	using System;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	public class ValueInputView : Dialog
	{
		private readonly TextBox _newValueTextBox;

		public ValueInputView(IEngine engine, string propertyName, string existingValue) : base(engine)
		{
			Title = "Enter New Value";

			_newValueTextBox = new TextBox { MinWidth = 250, Text = existingValue };
			OkButton = new Button { Text = "OK", Width = 150 };

			AddWidget(new Label($"Enter new value for property '{propertyName}':"), 0, 0, 1, 4);
			AddWidget(_newValueTextBox, 1, 0, 1, 4);
			AddWidget(OkButton, 2, 3, HorizontalAlignment.Right);

			// Trigger the OkClicked event when the OK button is pressed
			OkButton.Pressed += OnOkButtonClicked;
		}

		public event EventHandler OkClicked;

		public Button OkButton { get; }

		public string GetNewValue()
		{
			return _newValueTextBox.Text;
		}

		private void OnOkButtonClicked(object sender, EventArgs e)
		{
			OkClicked?.Invoke(this, EventArgs.Empty);
		}
	}
}
