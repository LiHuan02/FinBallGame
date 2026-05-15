using Godot;

public partial class GameState : Node
{
	public static GameState Instance { get; private set; }

	public int Score { get; set; } = 0;
	public int Lives { get; set; } = 2;

	public static Vector2 spawnPoint = new(941, 940);

	public override void _EnterTree()
	{
		Instance = this;
	}

	// 重置状态（新游戏时调用）
	public static void ResetGame()
	{
		Instance.Score = 0;
		Instance.Lives = 2;
	}
}