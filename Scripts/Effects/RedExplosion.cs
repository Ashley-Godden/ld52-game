using Godot;
using System;

public partial class RedExplosion : Node3D
{
    private AnimationPlayer animPlayer;

    public override void _Ready()
    {
        animPlayer = GetNode<AnimationPlayer>("Model/AnimationPlayer");
        animPlayer.PlaybackSpeed = 4f;
        animPlayer.Play("Red Explosion");
    }

    public override void _Process(double delta)
    {
        if (!animPlayer.IsPlaying())
        {
            QueueFree();
        }
    }
}
