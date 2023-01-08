using Godot;
using System;

public partial class Cabbit : RigidBody3D
{
    private AnimationPlayer animationPlayer;
    private CabbitManager cabbitManager;

    private double justSpawnedTimer = GD.RandRange(0.5f, 1.5f);
    private double pokeOutTimer = GD.RandRange(2f, 3f);
    private double pokeInTimer = GD.RandRange(1f, 2f);
    
    private float jumpForce = 13.2f;
    private float pokeSpeed = 3.5f;

    private enum State
    {
        Spawned,
        PokeOut,
        PokeIn,
        Jump,
        Despawn
    }

    public enum CabbitType
    {
        Cabbit,
        RedCabbit
    }

    private State state = State.Spawned;

    [Export]
    private CabbitType cabbitType = CabbitType.Cabbit;

    private int spawnIndex = 0;

    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("CabbitMesh/AnimationPlayer");
        cabbitManager = GetParent<CabbitManager>();
    }

    public override void _Process(double delta)
    {
        animationPlayer.Play("Ear Twitch");

        Vector3 position;
        
        switch (state)
        {
            case State.Spawned:
                if (justSpawnedTimer > 0)
                {
                    justSpawnedTimer -= delta;
                    break;
                }

                state = State.PokeOut;
                break;
            case State.PokeOut:
                position = GlobalTransform.origin;
                position.y = Mathf.Lerp(position.y, 0.3f, pokeSpeed * (float)delta);
                GlobalTransform = new Transform3D(Basis.Identity, position);

                if (pokeOutTimer > 0 && position.y < 0.3f)
                {
                    pokeOutTimer -= delta;
                    break;
                }

                state = State.PokeIn;
                break;
            case State.PokeIn:
                position = GlobalTransform.origin;
                position.y = Mathf.Lerp(position.y, -0.4f, pokeSpeed * (float)delta);
                GlobalTransform = new Transform3D(Basis.Identity, position);

                if (pokeInTimer > 0 && position.y > -0.4f)
                {
                    pokeInTimer -= delta;
                    break;
                }

                state = State.Jump;
                break;
            case State.Jump:
                if (Freeze) {
                    Freeze = false;
                    ApplyImpulse(Vector3.Up * jumpForce);
                }

                if (GlobalTransform.origin.y > -0.4f)
                {
                    break;
                }

                state = State.Despawn;
                break;
            case State.Despawn:
                DespawnCabbit();
                break;
        }
    }

    public void DespawnCabbit()
    {
        cabbitManager.EditSpawnPoint(spawnIndex, false);
        QueueFree();
    }

    public void SetSpawnIndex(int index)
    {
        spawnIndex = index;
    }

    public CabbitType GetCabbitType()
    {
        return cabbitType;
    }
}
