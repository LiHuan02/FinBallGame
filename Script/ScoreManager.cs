using Godot;

public partial class ScoreManager : CanvasLayer
{
	private int score = 0;
	private Label scoreValueLabel;

	public override void _Ready()
	{
		scoreValueLabel = GetNode<Label>("ScoreValue");
		// 连接所有奖励球的信号
		var bonusBalls = GetTree().GetNodesInGroup("bonus_ball");

		foreach (var node in bonusBalls)
		{
			if (node is BonusBall ball)
			{
				ball.ScoreAwarded += OnScoreAwarded;
			}
		}
	}

	private void OnScoreAwarded(int points)
	{
		score += points;
		scoreValueLabel.Text = score.ToString();
	}
}