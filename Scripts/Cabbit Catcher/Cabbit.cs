using Godot;
using System;

public partial class Cabbit : RigidBody3D
{
    private AnimationPlayer animationPlayer;
    private CabbitManager cabbitManager;
    private Node3D cabbitMesh;

    private double justSpawnedTimer = GD.RandRange(0.5f, 1.5f);
    private double pokeOutTimer = GD.RandRange(2f, 3f);
    private double pokeInTimer = GD.RandRange(1f, 2f);
    
    private float jumpForce = 18f;
    private float pokeSpeed = 4f;

    private float randomRotSpeed = (float)GD.RandRange(0.9f, 2f);

    // Random rotation degrees, either 180 or -180
    private float randomRotDegrees = (float)GD.RandRange(0, 2) == 0 ? 180f : -180f;

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
        cabbitMesh = GetNode<Node3D>("CabbitMesh");
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
                position.y = Mathf.Lerp(position.y, 0.25f, pokeSpeed * (float)delta);
                GlobalTransform = new Transform3D(Basis.Identity, position);

                if (pokeOutTimer > 0 && position.y < 0.25f)
                {
                    pokeOutTimer -= delta;
                    break;
                }

                state = State.PokeIn;
                break;
            case State.PokeIn:
                position = GlobalTransform.origin;
                position.y = Mathf.Lerp(position.y, -0.8f, pokeSpeed * (float)delta);
                GlobalTransform = new Transform3D(Basis.Identity, position);

                if (pokeInTimer > 0 && position.y > -0.75f)
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

                
                // Check if current rotation is close to 180
                if (Mathf.Abs(cabbitMesh.RotationDegrees.z - 180f) > 1f)
                {
                    // Rotate Z rotation of mesh either by negative or positive speed * delta. Depending on if randomRotDegrees is 180 or -180
                    cabbitMesh.RotateZ(randomRotSpeed * (float)delta * (randomRotDegrees / Mathf.Abs(randomRotDegrees)));
                }
                

                if (GlobalTransform.origin.y > -1f)
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
