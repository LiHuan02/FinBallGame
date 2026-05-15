using Godot;

public partial class DeathArea : Area2D
{
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is RigidBody2D ball && ball.IsInGroup("ball"))
		{
			GD.Print("成功识别到球");
			if (GameState.Instance.Lives <= 0)
			{
				// 没有命了：触发 Game Over
				GetTree().CallGroup("game_ui", "ShowGameOver");
				// 禁用球，防止继续触发
				ball.QueueFree();
			}
			else
			{
				// 生命 -1
				GameState.Instance.Lives--;
				// 更新 UI 命数
				GetTree().CallGroup("game_ui", "UpdateLives");
				GD.Print("剩余: ", GameState.Instance.Lives);
				GD.Print("进入复活过程");
				ball.LinearVelocity = Vector2.Zero;
				ball.AngularVelocity = 0;
				// 直接修改物理服务器中的 body 状态
				var transform = new Transform2D(ball.GlobalRotation, GameState.spawnPoint);
				PhysicsServer2D.BodySetState(ball.GetRid(), PhysicsServer2D.BodyState.Transform, transform);
				GD.Print("小球更新完成");
			}

		}
	}
}