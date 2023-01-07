using Godot;
using System;

public partial class Chapple : RigidBody3D
{
    // Declare member variables here
    ChappleManager chappleManager;

    public void _on_body_entered(Node body)
    {
        if (chappleManager == null)
            chappleManager = GetNode<ChappleManager>("../../ChappleManager");

        if (body.IsInGroup("ground"))
        {
            // Add to fail
            chappleManager.AddFail();

            // Destroy the chapple
            this.QueueFree();
        }
    }
}
