using Godot;

public partial class GameUI : CanvasLayer
{
	private Label livesLabel;
	private Label gameOverLabel;
	private SceneTree cachedTree;   // 缓存 SceneTree

	public override void _Ready()
	{
		livesLabel = GetNode<Label>("LeftLives");
		gameOverLabel = GetNode<Label>("GameOver");

		// 初始化显示
		UpdateLives();
		gameOverLabel.Visible = false;

		// 将自身加入组 "game_ui"，供其他节点调用
		AddToGroup("game_ui");

		cachedTree = GetTree();   // 缓存当前场景树
	}

	// 更新命数文字
	public void UpdateLives()
	{
		livesLabel.Text = $"LIVES: {GameState.Instance.Lives}";
	}

	// 显示 Game Over 文字并倒计时返回菜单
	public async void ShowGameOver()
	{
		gameOverLabel.Visible = true;
		// 如果节点已经不在树中，直接返回
		if (!IsInsideTree())
			return;

		// 使用缓存的 SceneTree 创建计时器
		var timer = cachedTree.CreateTimer(2.0f);
		await ToSignal(timer, "timeout");

		// 等待期间可能节点已被销毁，再次检查
		if (!IsInstanceValid(this) || !IsInsideTree())
			return;
		GetTree().ChangeSceneToFile("res://MainMenu.tscn");
	}
}