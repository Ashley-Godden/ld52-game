using Godot;
using System;

public partial class Grabber : RigidBody3D
{
    private AnimationPlayer animationPlayer;

    private CollisionShape3D collisionShape3D;

    private Node3D grabberArmEmpty;
    private Node3D grabberArmFull;

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

        grabberArmEmpty = GetNode<Node3D>("GrabberArmEmpty");
        grabberArmFull = GetNode<Node3D>("GrabberArmFull");

        collisionShape3D = GetNode<CollisionShape3D>("CollisionShape3D");

        grabberStartPosition = GlobalPosition;
        grabberEndPosition = GetNode<Node3D>("../GrabberEndPosition").GlobalPosition;

        cabbitManager = GetNode<CabbitManager>("../CabbitManager");
    }

    public override void _Process(double delta)
    {
        if (!startGame)
        {
            return;
        }

        Vector3 position = GlobalPosition;

        switch (grabberState)
        {
            case GrabberState.MovingToStartPosition:
                if (GlobalPosition.DistanceTo(grabberStartPosition) < 2f)
                {
                    if (!grabberArmEmpty.Visible)
                    {
                        grabberArmEmpty.Visible = true;
                        grabberArmFull.Visible = false;
                    }

                    // Check if space key is pressed
                    if (Input.IsActionJustPressed("ui_accept"))
                    {
                        // Enable collision detection
                        collisionShape3D.Disabled = false;
                        grabberState = GrabberState.MovingToEndPosition;
                        break;
                    }
                }
                
                position.x = Mathf.Lerp(GlobalPosition.x, grabberStartPosition.x, grabberSpeed * (float)delta);
                GlobalPosition = position;
                break;
            case GrabberState.MovingToEndPosition:
                if (GlobalPosition.DistanceTo(grabberEndPosition) < 2f)
                {
                    if (grabberArmEmpty.Visible)
                    {
                        animationPlayer.Stop();
                        animationPlayer.Play("Null");
                    }

                    // Disable collision detection
                    collisionShape3D.Disabled = true;
                    grabberState = GrabberState.MovingToStartPosition;
                    break;
                }

                if (!animationPlayer.IsPlaying() && grabberArmEmpty.Visible)
                {
                    animationPlayer.Play("Grab");
                }

                position.x = Mathf.Lerp(GlobalPosition.x, grabberEndPosition.x, grabberSpeed * (float)delta);
                GlobalPosition = position;
                break;
        }
    }

    public void _on_grabber_body_entered(Node body)
    {
        if (body is Cabbit cabbit)
        {
            cabbitManager.CabbitCaught(cabbit);

            if (cabbit.GetCabbitType() == Cabbit.CabbitType.Cabbit)
            {
                grabberArmEmpty.Visible = false;
                grabberArmFull.Visible = true;
            }
        }
    }

    public void StartGame()
    {
        startGame = true;
    }
}
