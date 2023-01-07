using Godot;
using System;

public partial class Chapple : RigidBody3D
{
    AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("Chapple/AnimationPlayer");
    }

    public override void _Process(double delta)
    {
        animationPlayer.Play("Falling");
    }

    public void _on_body_entered(Node body)
    {
        if (body.IsInGroup("ground"))
        {
            // Destroy the chapple
            this.QueueFree();
        }
    }
}
