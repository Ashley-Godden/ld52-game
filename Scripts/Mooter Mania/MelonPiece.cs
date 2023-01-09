using Godot;
using System;

public partial class MelonPiece : RigidBody3D
{
    private float lifeTime = GD.RandRange(3, 5);
    private float lifeTimer = 0f;

    private CollisionShape3D collisionShape;

    public override void _Ready()
    {
        collisionShape = GetNode<CollisionShape3D>("CollisionShape3D");
    }

    public override void _Process(double delta)
    {
        if (!Freeze)
        {
            if (collisionShape.Disabled) {
                collisionShape.Disabled = false;
            }

            lifeTimer += (float)delta;
            if (lifeTimer >= lifeTime)
            {
                QueueFree();
            }
        }
    }
}
