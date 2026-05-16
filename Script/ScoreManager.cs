using Godot;

public partial class ScoreManager : CanvasLayer
{
	private int score = 0;
	public static ScoreManager Instance { get; private set; }

	// 定义分数更新信号
	[Signal]
	public delegate void ScoreUpdatedEventHandler(int newScore);

	public override void _Ready()
	{
		Instance = this;
	}

	public static void AddScore(int points)
	{
		Instance.score += points;
		// 发射分数更新信号
		Instance.EmitSignal("ScoreUpdated", Instance.score);
	}

	// 获取当前分数
	public static int GetScore()
	{
		return Instance.score;
	}
}