using Godot;

public partial class BonusBall : Area2D
{
	[Export] public int ScoreValue = 100;
	[Export] public float EnergyImpulse = 250f;      // 给予球的冲量大小（单位：像素/秒²）
	[Export] public AudioStream bonusSound;


	private bool active = true;
	private AnimationPlayer anim;

	[Signal] public delegate void ScoreAwardedEventHandler(int score);

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		anim = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	private async void OnBodyEntered(Node2D body)
	{
		if (!active) return;

		if (body is RigidBody2D ball)
		{
			// 加分
			ScoreManager.AddScore(ScoreValue);

			// 给予球一个冲量
			Vector2 dir = ball.GlobalPosition.DirectionTo(GlobalPosition).Normalized();
			ball.ApplyCentralImpulse(-dir * EnergyImpulse);   // 注意方向反一下

			// 播放缩放动画
			anim.Play("hit");

			SoundManager.PlaySound(bonusSound);
		}
	}
}