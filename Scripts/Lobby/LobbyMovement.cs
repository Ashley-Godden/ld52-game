using Godot;
using System;

public partial class LobbyMovement : CharacterBody3D
{
    private AnimationPlayer animationPlayer;
    private Node3D model;

    private float idleTimer = 0f;
    private float idleWaitTime = 10f;
    private float moveSpeed = 100 * 9;

    public override void _Ready()
    {
        model = GetNode<Node3D>("Model");
        animationPlayer = GetNode<AnimationPlayer>("Model/AnimationPlayer");
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            GetTree().Quit();
        }

        float horizontal = Input.GetActionStrength("move_left") - Input.GetActionStrength("move_right");
        float vertical = Input.GetActionStrength("move_up") - Input.GetActionStrength("move_down");

        Vector3 input = new Vector3(horizontal, 0.0f, vertical);

        // Check direction player should face
        if (input != Vector3.Zero)
        {
            // Set direction player should face
            model.Transform = new Transform3D(Basis.Identity, model.Transform.origin).LookingAt(model.Transform.origin + -input, Vector3.Up);

            // Update idle timer
            idleTimer = 0f;

            // Play walk animation
            animationPlayer.Play("Move");
        }
        else
        {
            // Update idle timer
            idleTimer += (float)delta;

            // Check if idle timer is greater than idle wait time
            if (idleTimer >= idleWaitTime)
            {
                // Play idle animation
                animationPlayer.Play("Idle");
                idleTimer = idleWaitTime;
            }
            return;
        }

        idleTimer = idleWaitTime;

        Vector3 velocity = Velocity;

        velocity.x = input.x;
        velocity.z = input.z;

        Velocity = velocity * moveSpeed * (float)delta;

        MoveAndSlide();
    }
}
