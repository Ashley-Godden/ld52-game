using Godot;
using System;

public partial class MainCharacterIdle : Node3D
{
    private AnimationPlayer animationPlayer;

    private float idleTimer = 0f;
    private float idleWaitTime = 10f;

    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void _Process(double delta)
    {
        if (animationPlayer.IsPlaying())
        {
            return;
        }

        if (idleTimer > 0)
        {
            idleTimer -= (float)delta;
            return;
        }

        animationPlayer.Play("Idle");
        idleTimer = idleWaitTime;
    }
}
