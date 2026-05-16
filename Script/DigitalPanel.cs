using Godot;

public partial class DigitalPanel : Node2D
{
	[Export] public float FastInterval = 0.05f;   // 快速滚动间隔
	[Export] public float SlowInterval = 0.15f;   // 减速间隔
	[Export] public float MinStopTime = 0.8f;     // 最短滚动总时长
	[Export] public float MaxStopTime = 2.5f;     // 最长滚动总时长

	private Label label;
	private int currentDigit = 0;
	private int targetDigit = 0;
	private bool isSpinning = false;

	[Signal] public delegate void StoppedEventHandler(int finalDigit);

	public override void _Ready()
	{
		label = GetNode<Label>("Label");
		label.Text = "0";
	}

	public void StartSpin()
	{
		if (isSpinning) return;
		isSpinning = true;

		targetDigit = new RandomNumberGenerator().RandiRange(0, 9);
		SpinAsync();
	}

	private async void SpinAsync()
	{
		float totalDuration = (float)GD.RandRange(MinStopTime, MaxStopTime);
		float elapsed = 0f;

		// 快速滚动阶段
		while (elapsed < totalDuration * 0.6f)   // 前60%时间快速切换
		{
			currentDigit = (currentDigit + 1) % 10;
			label.Text = currentDigit.ToString();
			await ToSignal(GetTree().CreateTimer(FastInterval), Timer.SignalName.Timeout);
			elapsed += FastInterval;
		}

		// 减速接近目标
		while (currentDigit != targetDigit)
		{
			currentDigit = (currentDigit + 1) % 10;
			label.Text = currentDigit.ToString();

			int stepsLeft = (targetDigit - currentDigit + 10) % 10;
			float delay = SlowInterval + stepsLeft * 0.1f;
			await ToSignal(GetTree().CreateTimer(delay), Timer.SignalName.Timeout);
		}

		isSpinning = false;
		EmitSignal(SignalName.Stopped, targetDigit);
	}

	public int GetCurrentDigit() => currentDigit;

	public void ResetToZero()
	{
		isSpinning = false;
		currentDigit = 0;
		targetDigit = 0;
		label.Text = "0";
	}
}