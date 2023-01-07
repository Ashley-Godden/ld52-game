using Godot;
using System;

public partial class Chapple : RigidBody3D
{
    AnimationPlayer animationPlayer;
    ChappleManager chappleManager;

    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("Chapple/AnimationPlayer");
    }

    public override void _Process(double delta)
    {
        if (chappleManager == null && GetParent() != null)
        {
            chappleManager = GetNode<ChappleManager>("../../ChappleManager");
        }

        animationPlayer.Play("Falling");
    }

    public void _on_body_entered(Node body)
    {
        if (body.IsInGroup("ground"))
        {
            chappleManager.AddFail();

            // Destroy the chapple
            this.QueueFree();
        }
    }
}
