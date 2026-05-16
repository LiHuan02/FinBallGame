using Godot;
using System.Collections.Generic;

public partial class CoinResetSwitch : Area2D
{
	[Export] public float Cooldown = 1.0f;          // 触发冷却，防止连续触发
	[Export] public AudioStream SwitchSound;

	private bool canTrigger = true;

	private Sprite2D sprite;


	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite2D");

		sprite.Visible = false;

		BodyEntered += OnBodyEntered;
	}

	private async void OnBodyEntered(Node2D body)
	{
		if (!canTrigger || body is not RigidBody2D)
			return;

		canTrigger = false;

		sprite.Visible = true;

		// 播放音效
		SoundManager.PlaySound(SwitchSound);

		// 恢复所有金币
		ResetAllCoins();

		// 冷却等待
		await ToSignal(GetTree().CreateTimer(Cooldown), Timer.SignalName.Timeout);
		canTrigger = true;
		sprite.Visible = false;
	}

	private void ResetAllCoins()
	{
		// 通过组获取所有金币并调用重置
		var coins = GetTree().GetNodesInGroup("coins");
		foreach (var coin in coins)
		{
			if (coin is Coin c)
				c.ResetCoin();
		}
	}
}