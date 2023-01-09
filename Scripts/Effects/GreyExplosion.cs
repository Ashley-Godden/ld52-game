using Godot;
using System;

public partial class GreyExplosion : Node3D
{
    private AnimationPlayer animPlayer;

    public override void _Ready()
    {
        animPlayer = GetNode<AnimationPlayer>("Model/AnimationPlayer");
        animPlayer.PlaybackSpeed = 4f;
        animPlayer.Play("Grey Explosion2");
    }

    public override void _Process(double delta)
    {
        if (!animPlayer.IsPlaying())
        {
            QueueFree();
        }
    }
}
