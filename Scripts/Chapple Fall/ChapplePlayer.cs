using Godot;
using System;

public partial class ChapplePlayer : CharacterBody3D
{
    // Declare member variables here
    [Export]
    private float moveSpeed = 10f;

    [Export]
    private PhysicsBody3D basket;

    [Export]
    ChappleManager chappleManager;

    [Export]
    private Node3D wickerBasketEmpty;

    [Export]
    private Node3D wickerBasketFull;

    private bool stunned = false;

    private float stunDuration = 1f;

    private float stunTimer = 0f;

    private Panel stunPanel;

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        stunPanel = GetNode<Panel>("../UI/StunPanel");
    }

    public override void _Process(double delta)
    {
        // Check if escape is pressed
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            // Quit the game
            GetTree().Quit();
        }

        // Check if stunned
        if (stunned)
        {
            // Update stun timer
            stunTimer += (float)delta;

            // Check if stun timer is greater than stun duration
            if (stunTimer >= stunDuration)
            {
                // Reset stun timer
                stunTimer = 0f;

                // Set stunned to false
                stunned = false;

                // Set stun panel to invisible
                stunPanel.Visible = false;
            }
        }
        else
        {
            // Move the player
            MovePlayer(delta);
        }
    }

    private void MovePlayer(double delta)
    {
        // Get horizontal input strength
        float horizontal = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");

        // Get input axis
        Vector3 input = new Vector3(horizontal, 0.0f, 0.0f) * GetNode<Camera3D>("../MainCamera").GlobalTransform.basis;

        // Store local variable for velocity
        Vector3 velocity = Velocity;

        // Set velocity
        velocity.x = (input * moveSpeed).x;

        // Apply velocity
        Velocity = velocity * moveSpeed * (float)delta;

        MoveAndSlide();
    }

    private void StunPlayer()
    {
        // Set stunned to true
        stunned = true;

        // Set stun panel to visible
        stunPanel.Visible = true;
    }

    public void _On_Basket_Body_Entered(Node body)
    {
        if (body.IsInGroup("chapples"))
        {
            // Add to score
            chappleManager.AddScore(1);

            // Destroy the chapple
            body.QueueFree();

            // Set wicker basket to full
            if (wickerBasketEmpty.Visible)
            {
                wickerBasketEmpty.Visible = false;
                wickerBasketFull.Visible = true;
            }
        } else if (body.IsInGroup("twigs"))
        {
            StunPlayer();

            // Destroy the twig
            body.QueueFree();
        }
    }
}