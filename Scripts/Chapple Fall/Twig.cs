using Godot;
using System;

public partial class Twig : RigidBody3D
{
    public void _on_body_entered(Node body)
    {
        if (body.IsInGroup("ground"))
        {
            // Destroy the twig
            this.QueueFree();
        }
    }
}
