using Godot;

public partial class ScoreboardController : Node2D
{
	[Export] public DigitalPanel Hundreds;
	[Export] public DigitalPanel Tens;
	[Export] public DigitalPanel Units;
	[Export] public Area2D TriggerZone;

	private int stoppedCount = 0;

	public override void _Ready()
	{
		TriggerZone.BodyEntered += OnBodyEntered;
		Hundreds.Stopped += OnPanelStopped;
		Tens.Stopped += OnPanelStopped;
		Units.Stopped += OnPanelStopped;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is not RigidBody2D)
			return;


		stoppedCount = 0;
		Hundreds.StartSpin();
		Tens.StartSpin();
		Units.StartSpin();
	}

	private void OnPanelStopped(int digit)
	{
		stoppedCount++;
		if (stoppedCount >= 3)
		{
			int finalScore = Hundreds.GetCurrentDigit() * 1000
						   + Tens.GetCurrentDigit() * 100
						   + Units.GetCurrentDigit() * 10;

			ScoreManager.AddScore(finalScore);
		}
	}
}