using Godot;
using System;

public partial class Twig : RigidBody3D
{
    Node3D twigMesh;

    public override void _Ready()
    {
        twigMesh = GetNode<Node3D>("Twig");
    }

    public override void _Process(double delta)
    {
        // Rotate twig mesh with random speed between 0.5 and 2.5
        twigMesh.RotateY((float)delta * (float)GD.RandRange(0.5f, 2.5f));
        twigMesh.RotateX((float)delta * (float)GD.RandRange(0.5f, 2.5f));
        twigMesh.RotateZ((float)delta * (float)GD.RandRange(0.5f, 2.5f));
    }

    public void _on_body_entered(Node body)
    {
        if (body.IsInGroup("ground"))
        {
            // Destroy the twig
            this.QueueFree();
        }
    }
}
