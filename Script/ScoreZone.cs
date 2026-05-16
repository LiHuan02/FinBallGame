using Godot;

public partial class ScoreZone : Area2D
{
	[Export] public int ScoreValue = 50;           // 通过时获得的分数
	[Export] public AudioStream ScoreSound;        // 触发时播放的音效

	public override void _Ready()
	{
		// 连接信号
		BodyEntered += OnBodyEntered;
	}

	private async void OnBodyEntered(Node2D body)
	{
		// 只对球（RigidBody2D）响应，且允许触发时
		if (body is not RigidBody2D)
			return;

		// 加分
		ScoreManager.AddScore(ScoreValue);

		// 播放音效（如果设置了的话）
		SoundManager.PlaySound(ScoreSound);

	}

}