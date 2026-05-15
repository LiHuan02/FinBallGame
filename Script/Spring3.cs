using Godot;

public partial class Spring3 : Node2D
{
	[Export] public float MaxCompression = 9.0f;      // 最大压缩距离（像素）
	[Export] public float MaxChargeTime = 1.0f;        // 达到最大压缩所需按键时间（秒）
	[Export] public float LaunchForceMultiplier = 600.0f; // 弹出力度倍数（越大弹得越远）
	[Export] public string ChargeAction = "spring_charge";
	[Export] public float StiffnessPerPixel = 20.0f; // 每像素压缩增加的弹簧刚度
	[Export] public float UpwardRestoreImpulseMultiplier = 20.0f; // 用于在压缩时施加向上的恢复冲量
	[Export] public float LaunchUpperImpulseMultiplier = 0.5f; // 发射时施加在上部的向上冲量倍数
	[Export] public float LaunchBallNudge = 4.0f; // 发射前抬起球的像素量，防止卡住

	private RigidBody2D upperBody;
	private RigidBody2D lowerBody;
	private DampedSpringJoint2D springJoint;
	private Area2D detectionArea;

	private float initialRestLength;
	private float minRestLength;
	private float initialStiffness;
	private PhysicsBody2D currentBall;

	private bool isCharging = false;
	private float chargeTime = 0.0f;

	public override void _Ready()
	{
		upperBody = GetNode<RigidBody2D>("SpringTop");
		lowerBody = GetNode<RigidBody2D>("SpringBase");
		springJoint = GetNode<DampedSpringJoint2D>("SpringJoint");

		// 锁定旋转，底座冻结
		upperBody.LockRotation = true;
		lowerBody.LockRotation = true;
		lowerBody.Freeze = true;

		initialRestLength = springJoint.RestLength;
		initialStiffness = springJoint.Stiffness;
		minRestLength = Mathf.Max(1, initialRestLength - MaxCompression);

		detectionArea = GetNode<Area2D>("SpringTop/DetectionArea");
		detectionArea.BodyEntered += OnBallEntered;
		detectionArea.BodyExited += OnBallExited;
	}

	public override void _Process(double delta)
	{
		// 只有球在平台上时才能蓄力
		if (currentBall == null) return;

		bool pressed = Input.IsActionPressed(ChargeAction);

		if (pressed)
		{
			if (!isCharging)
			{
				isCharging = true;
				chargeTime = 0.0f;
			}
			chargeTime += (float)delta;
			if (chargeTime > MaxChargeTime)
				chargeTime = MaxChargeTime;

			float ratio = chargeTime / MaxChargeTime;
			springJoint.RestLength = Mathf.Lerp(initialRestLength, minRestLength, ratio);
		}
		else
		{
			if (isCharging)
			{
				// 松手：先弹射球（使用当前压缩计算力度），再恢复弹簧长度
				LaunchBall();
				springJoint.RestLength = initialRestLength;
				isCharging = false;
				chargeTime = 0.0f;
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		// 强制锁定水平位置，确保弹簧只上下运动
		float fixedX = GlobalPosition.X;
		Vector2 pos;

		pos = upperBody.GlobalPosition;
		pos.X = fixedX;
		upperBody.GlobalPosition = pos;

		pos = lowerBody.GlobalPosition;
		pos.X = fixedX;
		lowerBody.GlobalPosition = pos;

		// 根据实际压缩量动态增加刚度，使得压缩越多回复力越大
		float currentDistance = (upperBody.GlobalPosition - lowerBody.GlobalPosition).Length();
		float compression = Mathf.Clamp(initialRestLength - currentDistance, 0, MaxCompression);
		springJoint.Stiffness = initialStiffness + compression * StiffnessPerPixel;

		// 不再在物理帧施加瞬时向上冲量，避免在场景开始或持续压缩时把球直接弹出。
	}

	private void OnBallEntered(Node2D body)
	{
		if (body is PhysicsBody2D ball && ball != upperBody && ball != lowerBody)
			currentBall = ball;
	}

	private void OnBallExited(Node2D body)
	{
		if (body == currentBall)
			currentBall = null;
	}

	private void LaunchBall()
	{
		if (currentBall == null) return;
		// 使用当前实际压缩量来决定发射力度（压缩越多，力度越大），同时考虑蓄力时间作为加成
		float currentDistance = (upperBody.GlobalPosition - lowerBody.GlobalPosition).Length();
		float actualCompression = Mathf.Clamp(initialRestLength - currentDistance, 0, MaxCompression);
		float ratio = Mathf.Clamp(chargeTime / MaxChargeTime, 0, 1);
		float impulse = actualCompression * LaunchForceMultiplier * (1.0f + ratio);

		// 在发射前小幅抬起小球，避免与上平台发生穿透或卡住
		if (currentBall is RigidBody2D rigidBody)
		{
			rigidBody.GlobalPosition += Vector2.Up * LaunchBallNudge;
			rigidBody.ApplyCentralImpulse(Vector2.Up * impulse);
			// 给上部一个相反方向的瞬时冲量，帮助弹簧回弹
			upperBody.ApplyCentralImpulse(Vector2.Up * (actualCompression * LaunchUpperImpulseMultiplier));
		}
		else if (currentBall is CharacterBody2D characterBody)
		{
			characterBody.Velocity += Vector2.Up * impulse;
		}

		// 可选：播放音效
		// GetNode<AudioStreamPlayer2D>("Sound").Play();
	}
}