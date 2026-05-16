using Godot;

public partial class LeftFlipper : Node2D
{
	[Export] public string ActionName = "flip_left"; // 输入映射名
	[Export] public float ActiveAngle = 60f;            // 按下时目标旋转角度（度）
	[Export] public float RestAngle = 0f;               // 松开时回到的角度
	[Export] public float FlipSpeed = 20f;              // 按下时的角速度（越大越快）
	[Export] public float ReturnSpeed = 10f;            // 松手回弹的角速度
	[Export] public bool Clockwise = false;             // 按下时旋转方向（false=逆时针，true=顺时针）
	[Export] private AudioStream flipperSound;

	private RigidBody2D arm;
	private float targetRotation;                       // 目标旋转弧度

	public override void _Ready()
	{
		arm = GetNode<RigidBody2D>("Arm");
		// 初始化角度：将杆手动旋转到 RestAngle
		arm.RotationDegrees = RestAngle;
		targetRotation = Mathf.DegToRad(RestAngle);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed(ActionName))
		{
			SoundManager.PlaySound(flipperSound);
		}
		bool pressed = Input.IsActionPressed(ActionName);
		float targetAngle = pressed ? ActiveAngle : RestAngle;
		// 根据方向转换角度
		float dir = Clockwise ? 1f : -1f;
		targetRotation = Mathf.DegToRad(targetAngle * dir);

		// 计算当前角度与目标角度的差值
		float current = arm.Rotation;
		float diff = targetRotation - current;

		// 根据按键状态选择不同的速度（按下快，松开慢）
		float speed = pressed ? FlipSpeed : ReturnSpeed;

		// 给杆设置角速度，方向朝向目标
		float angularVel = Mathf.Sign(diff) * speed;
		// 如果已经非常接近目标，则固定角度并停止
		if (Mathf.Abs(diff) < 0.02f)
		{
			arm.Rotation = targetRotation;
			arm.AngularVelocity = 0f;
		}
		else
		{
			arm.AngularVelocity = angularVel;
		}
	}
}