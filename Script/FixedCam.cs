using Godot;

public partial class FixedCam : Camera2D
{
	[Export] public float WorldHeight = 1294f;      // 你的弹珠台总高
	[Export] public float ViewHeight = 647f;       // 屏幕可见高度
	[Export] public float SwitchMargin = 20f;      // 离边界多近就切换

	private Node2D _ball;
	private bool _showUpper;

	public override void _Ready()
	{
		_ball = GetParent().GetNode<RigidBody2D>("Ball");
		UpdateCameraPosition();
	}

	public override void _Process(double delta)
	{
		// 如果引用无效（对象已被销毁），重新尝试获取球
		if (_ball == null || !IsInstanceValid(_ball))
		{
			_ball = GetTree().GetFirstNodeInGroup("ball") as Node2D;
		}
		if (_ball == null) return;

		float ballY = _ball.GlobalPosition.Y;
		bool shouldShowUpper = ballY < WorldHeight / 2f;   // 根据世界中线分

		if (shouldShowUpper != _showUpper)
		{
			_showUpper = shouldShowUpper;
			UpdateCameraPosition();
		}
	}

	private void UpdateCameraPosition()
	{
		// 固定视角，不跟随球移动
		float camY = _showUpper ? ViewHeight / 2f : ViewHeight / 2f + ViewHeight;
		Position = new Vector2(Position.X, camY);
		Offset = Vector2.Zero;
	}
}