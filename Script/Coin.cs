using Godot;

public partial class Coin : Area2D
{
	[Export] public int ScoreValue = 50;
	[Export] public AudioStream CollectSound;

	private bool collected = false;
	private Sprite2D sprite;
	private AnimationPlayer animPlayer;
	private CollisionShape2D collisionShape;

	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite2D");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

		BodyEntered += OnBodyEntered;

		// 加入组，方便开关查找
		AddToGroup("coins");
	}

	private void OnBodyEntered(Node2D body)
	{
		if (collected || body is not RigidBody2D)
			return;

		collected = true;

		// 加分
		ScoreManager.AddScore(ScoreValue);

		// 播放收集动画（隐藏/缩小）
		if (animPlayer != null && animPlayer.HasAnimation("collect"))
			animPlayer.Play("collect");
		else
			sprite.Visible = false;

		// 播放音效
		SoundManager.PlaySound(CollectSound);

		// 禁用碰撞，避免重复触发
		collisionShape.SetDeferred("disabled", true);
	}

	// 由开关调用的重置方法
	public void ResetCoin()
	{
		if (!collected)
			return;

		collected = false;

		// 播放恢复动画
		if (animPlayer != null && animPlayer.HasAnimation("reset"))
			animPlayer.Play("reset");
		else
			sprite.Visible = true;

		// 重新启用碰撞
		collisionShape.SetDeferred("disabled", false);
	}
}