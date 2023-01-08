using Godot;
using System;

public partial class Grabber : RigidBody3D
{
    private AnimationPlayer animationPlayer;

    private CollisionShape3D collisionShape3D;

    private float grabberSpeed = 1f;
    
    private Vector3 grabberStartPosition;
    private Vector3 grabberEndPosition;

    private CabbitManager cabbitManager;

    private bool startGame = false;

    private enum GrabberState
    {
        MovingToStartPosition,
        MovingToEndPosition
    }

    private GrabberState grabberState = GrabberState.MovingToStartPosition;

    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("GrabberArmEmpty/AnimationPlayer");

        animationPlayer.PlaybackDefaultBlendTime = 1f;
        animationPlayer.PlaybackSpeed = 1.5f;

        collisionShape3D = GetNode<CollisionShape3D>("CollisionShape3D");

        grabberStartPosition = Position;
        grabberEndPosition = GetNode<Node3D>("../GrabberEndPosition").Position;

        cabbitManager = GetNode<CabbitManager>("../CabbitManager");
    }

    public override void _Process(double delta)
    {
        if (!startGame)
        {
            return;
        }

        Vector3 position = Position;

        switch (grabberState)
        {
            case GrabberState.MovingToStartPosition:
                if (Position.DistanceTo(grabberStartPosition) < 1f)
                {
                    // Check if space key is pressed
                    if (Input.IsActionJustPressed("ui_accept"))
                    {
                        // Enable collision detection
                        collisionShape3D.Disabled = false;
                        grabberState = GrabberState.MovingToEndPosition;
                        break;
                    }
                }
                
                position.x = Mathf.Lerp(Position.x, grabberStartPosition.x, grabberSpeed * (float)delta);
                Position = position;
                break;
            case GrabberState.MovingToEndPosition:
                if (Position.DistanceTo(grabberEndPosition) < 1f)
                {
                    animationPlayer.Stop();
                    animationPlayer.Play("Null");

                    // Disable collision detection
                    collisionShape3D.Disabled = true;
                    grabberState = GrabberState.MovingToStartPosition;
                    break;
                }

                if (!animationPlayer.IsPlaying())
                {
                    animationPlayer.Play("Grab");
                }

                position.x = Mathf.Lerp(Position.x, grabberEndPosition.x, grabberSpeed * (float)delta);
                Position = position;
                break;
        }
    }

    public void _on_grabber_body_entered(Node body)
    {
        if (body is Cabbit cabbit)
        {
            cabbitManager.CabbitCaught(cabbit);
            body.QueueFree();
        }
    }

    public void StartGame()
    {
        startGame = true;
    }
}
