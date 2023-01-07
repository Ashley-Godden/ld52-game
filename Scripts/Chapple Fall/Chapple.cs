using Godot;
using System;

public partial class Chapple : RigidBody3D
{
    AnimationPlayer animationPlayer;
    ChappleManager chappleManager;

    Node3D chappleMesh;

    public override void _Ready()
    {
        chappleMesh = GetNode<Node3D>("Chapple");
        animationPlayer = chappleMesh.GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void _Process(double delta)
    {
        if (chappleManager == null && GetParent() != null)
        {
            chappleManager = GetNode<ChappleManager>("../../ChappleManager");
        }

        animationPlayer.Play("Falling");
        
        // Rotate chapple mesh with random speed between 0.5 and 2.5
        chappleMesh.RotateY((float)delta * (float)GD.RandRange(0.5f, 2.5f));
        chappleMesh.RotateX((float)delta * (float)GD.RandRange(0.5f, 2.5f));
        chappleMesh.RotateZ((float)delta * (float)GD.RandRange(0.5f, 2.5f));
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
