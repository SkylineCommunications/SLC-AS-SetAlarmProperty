namespace SetAlarmProperty_1.ValueInput
{
	using System;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	public class ValueInputView : Dialog
	{
		public ValueInputView(IEngine engine, string propertyName, string existingValue) : base(engine)
		{
			Title = "New Value";

			NewValueTextBox = new TextBox { MinWidth = 250, Text = existingValue }; 
			OkButton = new Button { Text = "OK", Width = 150 };

			AddWidget(new Label($"Enter new value for property {propertyName}:"), 0, 0, 1, 4);
			AddWidget(NewValueTextBox, 1, 0, 1, 4);
			AddWidget(OkButton, 2, 3, HorizontalAlignment.Right);

			OkButton.Pressed += OnOkButtonClicked;
		}

		public event EventHandler OkClicked;

		public TextBox NewValueTextBox { get; }

		public Button OkButton { get; }

		public string GetNewValue()
		{
			return NewValueTextBox.Text;
		}

		private void OnOkButtonClicked(object sender, EventArgs e)
		{
			OkClicked?.Invoke(this, EventArgs.Empty);
		}
	}
}
