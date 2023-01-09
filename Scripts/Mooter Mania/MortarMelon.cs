using Godot;
using System;
using System.Collections.Generic;

public partial class MortarMelon : CharacterBody3D
{
    [Export]
    private PackedScene greyExplosionScene;

    private CollisionShape3D collisionShape;
    private List<RigidBody3D> mortarPieces = new List<RigidBody3D>();
    private Node3D piecesNode;
    private CharacterBody3D player;

    private float deathTimer = 0f;
    private float deathWaitTime = 20f;

    public override void _Ready()
    {
        collisionShape = GetNode<CollisionShape3D>("CollisionShape3D");
        piecesNode = GetNode<Node3D>("Pieces");
        player = GetNode<CharacterBody3D>("/root/MooterMelon Mania/Player");
        
        foreach (Node child in piecesNode.GetChildren()) {
            if (child is RigidBody3D) {
                mortarPieces.Add((RigidBody3D)child);
            }
        }
    }

    public override void _Process(double delta)
    {
        if (deathTimer >= deathWaitTime) {
            Die();
        } else {
            deathTimer += (float)delta;
        }

        // Gradually move towards the player
        Vector3 direction = player.GlobalPosition - GlobalPosition;
        direction.y = 0;

        // Look at direction
        Transform3D transform = new Transform3D(Basis.Identity, Transform.origin).LookingAt(Transform.origin + direction, Vector3.Up);
        Transform = transform;

        Velocity = direction * 30f * (float)delta;
        MoveAndSlide();
    }

    public void Die() {
        GreyExplosion greyExplosion = (GreyExplosion)greyExplosionScene.Instantiate();
        GetNode<Node3D>("..").AddChild(greyExplosion);
        greyExplosion.GlobalPosition = GlobalPosition;

        piecesNode.Visible = true;

        if (mortarPieces.Count != 0) {
            foreach (RigidBody3D mortarPiece in mortarPieces) {
                Vector3 prevPos = mortarPiece.GlobalPosition;
                mortarPiece.Freeze = false;
                piecesNode.RemoveChild(mortarPiece);
                GetNode<Node3D>("..").AddChild(mortarPiece);
                mortarPiece.GlobalPosition = prevPos;
                mortarPiece.ApplyForce(new Vector3(GD.RandRange(100, 150), GD.RandRange(500, 550), GD.RandRange(100, 150)));
                mortarPiece.ApplyTorque(new Vector3(GD.RandRange(70, 100), GD.RandRange(70, 100), GD.RandRange(70, 100)));
            }
        }

        QueueFree();
    }
}
