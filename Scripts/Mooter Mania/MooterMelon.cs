using Godot;
using System;
using System.Collections.Generic;

public partial class MooterMelon : CharacterBody3D
{
    private List<RigidBody3D> melonPieces = new List<RigidBody3D>();
    private Node3D pieces;

    private float changeDirectionTimer = 0f;
    private float changeDirectionWaitTime = 3f;

    private float moveSpeed = 100f * 5f;

    private float moveDirectionX = GD.RandRange(-1, 1);
    private float moveDirectionZ = GD.RandRange(-1, 1);

    public override void _Ready()
    {
        pieces = GetNode<Node3D>("Pieces");

        foreach (Node child in pieces.GetChildren()) {
            if (child is RigidBody3D) {
                melonPieces.Add((RigidBody3D)child);
            }
        }
    }

    public override void _Process(double delta)
    {
        changeDirectionTimer += (float)delta;

        if (changeDirectionTimer >= changeDirectionWaitTime) {
            moveDirectionX = GD.RandRange(-1, 1);
            moveDirectionZ = GD.RandRange(-1, 1);
            changeDirectionTimer = 0f;
        }

        // Move melon transform over time based on input
        float horizontal = moveDirectionX;
        float vertical = moveDirectionZ;

        Vector3 input = new Vector3(horizontal, 0.0f, vertical);

        // Rotate melon to face direction it's moving
        if (input != Vector3.Zero) {
            Transform3D transform = new Transform3D(Basis.Identity, Transform.origin).LookingAt(Transform.origin + input, Vector3.Up);
            Transform = transform;
        }

        Vector3 velocity = input * moveSpeed * (float)delta;
        Velocity = velocity;

        MoveAndSlide();
    }

    public void Die() {
        pieces.Visible = true;

        if (melonPieces.Count != 0) {
            foreach (RigidBody3D melonPiece in melonPieces) {
                Vector3 prevPos = melonPiece.GlobalPosition;
                melonPiece.Freeze = false;
                pieces.RemoveChild(melonPiece);
                GetNode<Node3D>("..").AddChild(melonPiece);
                melonPiece.GlobalPosition = prevPos;
                melonPiece.ApplyForce(new Vector3(GD.RandRange(100, 150), GD.RandRange(500, 550), GD.RandRange(100, 150)));
                melonPiece.ApplyTorque(new Vector3(GD.RandRange(70, 100), GD.RandRange(70, 100), GD.RandRange(70, 100)));
            }
        }

        QueueFree();
    }
}
