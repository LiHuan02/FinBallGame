using Godot;

public partial class MovingPlatform : AnimatableBody2D
{
	[Export] public float MoveSpeed = 100f;         // 移动速度（像素/秒）
	[Export] public float MoveDistance = 200f;      // 从起始位置移动的最大距离
	[Export] public bool StartMovingRight = true;   // 初始移动方向（true 向右）

	private Vector2 startPosition;                  // 初始位置
	private float currentOffset = 0f;               // 当前偏移量
	private int direction = 1;                      // 当前方向：1 向右，-1 向左

	public override void _Ready()
	{
		startPosition = Position;

		if (!StartMovingRight)
			direction = -1;
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;

		// 计算本帧移动量
		float moveDelta = MoveSpeed * dt * direction;
		currentOffset += moveDelta;

		// 到达端点时反转方向
		if (currentOffset > MoveDistance)
		{
			currentOffset = MoveDistance;
			direction = -1;
		}
		else if (currentOffset < -MoveDistance)
		{
			currentOffset = -MoveDistance;
			direction = 1;
		}

		// 设置新位置
		Position = startPosition + new Vector2(currentOffset, 0);

		// 运动学体需要手动触发同步，确保物理引擎正确更新碰撞
		// 对于 AnimatableBody2D，直接设置 Position 即可；也可使用 MoveAndCollide
	}
}