using Godot;

public partial class SoundManager : Node
{
	public static SoundManager Instance { get; private set; }

	public override void _Ready()
	{
		Instance = this;
	}

	public static void PlaySound(AudioStream stream, float volumeDb = 0.0f)
	{
		if (stream == null || Instance == null)
			return;

		var player = new AudioStreamPlayer();

		player.Stream = stream;
		player.VolumeDb = volumeDb;

		player.Finished += () => player.QueueFree();

		Instance.AddChild(player);

		player.Play();
	}
}