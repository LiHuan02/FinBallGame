using Godot;

/// <summary>
/// 分数显示UI脚本，监听ScoreManager的分数更新信号
/// </summary>
public partial class ScoreUI : CanvasLayer
{
	private Label scoreValueLabel;

	public override void _Ready()
	{
		// 获取分数值标签
		scoreValueLabel = GetNode<Label>("ScoreValue");

		// 连接ScoreManager的分数更新信号
		if (ScoreManager.Instance != null)
		{
			ScoreManager.Instance.Connect("ScoreUpdated", new Callable(this, nameof(OnScoreUpdated)));

			// 初始化显示当前分数
			scoreValueLabel.Text = ScoreManager.GetScore().ToString("D5");
		}
	}

	/// <summary>
	/// 当分数更新时调用此方法
	/// </summary>
	/// <param name="newScore">新分数</param>
	private void OnScoreUpdated(int newScore)
	{
		scoreValueLabel.Text = newScore.ToString("D5"); // 格式化为5位数字，不足补0
	}
}