using Godot;
using System;
using System.Collections.Generic;

public partial class StartMenu : Control
{
	// 菜单选项文本
	private List<string> options = new List<string> { "1 Player", "2 Players" };
	private int selected = 0;

	// 两个 Label 节点
	private Label labelSingle;
	private Label labelDouble;

	public override void _Ready()
	{
		// 获取节点（根据你实际的名字调整）
		labelSingle = GetNode<Label>("Label1");
		labelDouble = GetNode<Label>("Label2");

		// 初始化显示
		UpdateMenu();
	}

	private void UpdateMenu()
	{
		// 用于存放引用，方便循环
		Label[] labels = { labelSingle, labelDouble };

		for (int i = 0; i < options.Count; i++)
		{
			string text = options[i];
			if (i == selected)
			{
				// 选中态：白色 + 光标箭头
				labels[i].Text = "▶ " + text;
				labels[i].Modulate = new Color(1, 1, 1);   // 纯白
			}
			else
			{
				// 未选中态：灰色，前面用空格对齐
				labels[i].Text = "   " + text;
				labels[i].Modulate = new Color(0.4f, 0.4f, 0.4f);
			}
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_up"))
		{
			selected = (selected - 1 + options.Count) % options.Count;
			UpdateMenu();
		}
		else if (@event.IsActionPressed("ui_down"))
		{
			selected = (selected + 1) % options.Count;
			UpdateMenu();
		}
		else if (@event.IsActionPressed("ui_accept"))
		{
			OnSelect(selected);
		}
	}

	private void OnSelect(int index)
	{
		switch (index)
		{
			case 0:
				// 进入单人游戏：切换到主游戏场景
				var err = GetTree().ChangeSceneToFile("res://GameMain.tscn");
				GameState.ResetGame();
				if (err != Error.Ok)
				{
					GD.PrintErr($"加载游戏失败: {err}");
				}
				break;
			case 1:
				// 进入双人游戏
				GD.Print("双人模式");
				break;
		}
	}
}