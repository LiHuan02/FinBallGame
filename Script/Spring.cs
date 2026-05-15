using Godot;

public partial class Spring : Node2D
{
	// ---- 可调节参数 ----
	[Export] private float stiffness = 800f;      // 弹性系数 k
	[Export] private float damping = 15f;         // 阻尼系数
	[Export] private float topMass = 1f;          // 顶端质量
	[Export] private float compressSpeed = 60f;  // 按住压缩时的移动速度

	// ---- 内部状态 ----
	private float restLength;     // 弹簧自然长度（由初始位置自动计算）
	private float topY;           // 顶端当前 Y 坐标（相对根节点）
	private float velocityY;      // 顶端当前速度（向下为正）
	private float bottomY;        // 底端固定 Y 坐标

	// ---- 子节点引用 ----
	private AnimatableBody2D topBody;   // 顶端：可推动物体的运动学体
	private Line2D springVisual;
	private StaticBody2D bottom;

	public override void _Ready()
	{
		// 获取必要的子节点
		bottom = GetNode<StaticBody2D>("Bottom");
		topBody = GetNode<AnimatableBody2D>("Top");
		springVisual = GetNode<Line2D>("SpringVisual");

		// 用初始位置关系确定弹簧自然长度
		bottomY = bottom.Position.Y;              // 底端固定位置
		float initialTopY = topBody.Position.Y;   // 顶端初始 Y（场景中手动摆放的位置）
		restLength = bottomY - initialTopY;       // 自然长度 = 底端Y - 顶端初始Y

		// 将当前顶端 Y 同步到内部状态
		topY = initialTopY;

		// 初始绘制弹簧视觉
		UpdateSpringVisual();
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;

		// 检查输入动作：按住 "Lauch" 时主动压缩弹簧
		bool isCompressing = Input.IsActionPressed("Lauch");

		if (isCompressing)
		{
			// 按下时，顶端以固定速度下移（压缩弹簧）
			velocityY = compressSpeed;
			topY += velocityY * dt;

			// 限制最大压缩量，防止顶端穿过底端
			float minY = bottomY - 10f; // 留 10 像素间距
			if (topY > minY)
				topY = minY;
		}
		else
		{
			// 正常弹簧物理模拟
			float currentLength = bottomY - topY;          // 当前弹簧长度
			float compression = restLength - currentLength; // 压缩量（正=压缩，负=拉伸）

			// 弹力：压缩时向上（负方向），拉伸时向下（正方向）
			float springForce = -stiffness * compression;
			// 阻尼力：与速度方向相反
			float dampForce = -damping * velocityY;

			float acceleration = (springForce + dampForce) / topMass;
			velocityY += acceleration * dt;
			topY += velocityY * dt;

			// 底端碰撞限制（不能低于底端）
			float minY = bottomY - 5f;
			if (topY > minY)
			{
				topY = minY;
				velocityY = 0;
			}

			// 拉伸限制：防止弹簧过度拉伸（最多拉到自然长度的 1.2 倍）
			float maxY = bottomY - restLength * 1.2f;
			if (topY < maxY)
			{
				topY = maxY;
				velocityY = 0;
			}
		}

		// 更新顶端 AnimatableBody2D 的位置
		// 直接设置 Position 即可驱动运动学体，物理引擎会自动处理与上方物体的碰撞
		topBody.Position = new Vector2(topBody.Position.X, topY);

		// 更新弹簧的视觉表现
		UpdateSpringVisual();
	}

	/// <summary>
	/// 用 Line2D 绘制从底端到顶端的锯齿状弹簧线。
	/// </summary>
	private void UpdateSpringVisual()
	{
		if (springVisual == null) return;

		Vector2 bottomPos = new Vector2(0, bottomY);
		Vector2 topPos = new Vector2(0, topY);
		int segments = 10; // 分段数，控制锯齿数量

		Vector2[] points = new Vector2[segments + 1];
		for (int i = 0; i <= segments; i++)
		{
			float t = i / (float)segments;
			Vector2 pos = bottomPos.Lerp(topPos, t);
			// 左右偏移形成锯齿效果
			pos.X += (i % 2 == 0) ? 10 : -10;
			points[i] = pos;
		}
		springVisual.Points = points;
	}
}