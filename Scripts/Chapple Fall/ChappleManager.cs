using Godot;
using System;

/*
    * Chapple Fall
    * ChappleManager.cs
    * 
    * This script is used to manage the spawing of the chapple prefabs
    *
*/

public partial class ChappleManager : Node
{
    // Declare member variables here
    [Export]
    private PackedScene chapplePrefab;

    [Export]
    private float spawnRate = 1f;

    [Export]
    private MeshInstance3D spawnArea;

    private float spawnTimer = 0f;

    private int score = 0;

    private int fails = 0;

    private int maxFails = 3;

    private Label scoreLabel;

    private Label failLabel;

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        scoreLabel = GetNode<Label>("../UI/ScorePanel/ScoreLabel");
        failLabel = GetNode<Label>("../UI/FailPanel/FailLabel");
    }

    public override void _Process(double delta)
    {
        // Called every frame. Delta is time since last frame.
        // Update game logic here.
        spawnTimer += (float)delta;

        if (spawnTimer >= spawnRate)
        {
            // Spawn chapple
            SpawnChapple();

            // Reset timer
            spawnTimer = 0f;
        }
    }

    public void AddScore(int score)
    {
        this.score += score;
        scoreLabel.Text = $"Score: {this.score}";
    }

    public void AddFail()
    {
        fails++;
        failLabel.Text = $"Fails: {fails}";

        if (fails >= maxFails)
        {
            // Lost game
            LostGame();
        }
    }

    private void SpawnChapple()
    {
        // Create chapple
        RigidBody3D chapple = chapplePrefab.Instantiate<RigidBody3D>();

        // Add to scene
        AddChild(chapple);
        
        // Get half-size of spawn area
        Vector3 halfSize = spawnArea.Scale / 2;

        // Get furthest left and furthest right positions
        float left = spawnArea.GlobalTransform.origin.x - halfSize.x;
        float right = spawnArea.GlobalTransform.origin.x + halfSize.x;
        
        // Get furthest top and furthest bottom positions
        float top = spawnArea.GlobalTransform.origin.y + halfSize.y;
        float bottom = spawnArea.GlobalTransform.origin.y - halfSize.y;
        
        // Set position
        chapple.GlobalTransform = new Transform3D(
            chapple.GlobalTransform.basis,
            new Vector3(
                (float)GD.RandRange(left, right),
                (float)GD.RandRange(bottom, top),
                0f
            )
        );
    }

    private void LostGame()
    {
        // Reload the scene
        GetTree().ReloadCurrentScene();
    }
}
