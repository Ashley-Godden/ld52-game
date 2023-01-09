using Godot;
using System;

public partial class ManiaMovement : CharacterBody3D
{
    private AnimationPlayer animationPlayer;
    private MootermaniaManager manager;
    private Node3D model;
    private Area3D hitArea;
    private AnimationPlayer hammerAnimationPlayer;

    private float idleTimer = 0f;
    private float idleWaitTime = 10f;
    private float moveSpeed = 100 * 11;

    private bool canSmash = false;

    public override void _Ready()
    {
        manager = GetNode<MootermaniaManager>("../MootermaniaManager");
        model = GetNode<Node3D>("Model");
        hitArea = model.GetNode<Area3D>("HitArea");
        hammerAnimationPlayer = model.GetNode<AnimationPlayer>("Hammer/AnimationPlayer");
        animationPlayer = GetNode<AnimationPlayer>("Model/AnimationPlayer");

        hammerAnimationPlayer.PlaybackSpeed = 3f;
    }

    private void SwingHammerLogic(double delta)
    {
        if (Input.IsActionPressed("ui_accept") && !hammerAnimationPlayer.IsPlaying())
        {
            // Play swing animation
            hammerAnimationPlayer.Play("Swing");
        }

        if (hammerAnimationPlayer.IsPlaying())
        {
            hitArea.Monitoring = true;
        } else {
            hitArea.Monitoring = false;
        }
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            GetTree().Quit();
        }

        if (!canSmash)
        {
            return;
        }

        SwingHammerLogic(delta);

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

    public void _on_hit_area_body_entered(Node body)
    {
        if (body is MooterMelon)
        {
            MooterMelon melon = (MooterMelon)body;
            melon.Die();
            manager.AddScore(8);
        }
    }

    public void SetSmashing(bool smashing)
    {
        canSmash = smashing;
    }

    public void _on_mortar_area_body_entered(Node body)
    {
        if (body is MortarMelon)
        {
            MortarMelon melon = (MortarMelon)body;
            melon.Die();
            manager.AddFail();
        }
    }
}
