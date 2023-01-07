using Godot;
using System;

/*
    * Chapple Fall
    * ChappleManager.cs
    * 
    * This script is used to manage the spawing of the chapple prefabs and twig prefabs.
    *
*/

public partial class ChappleManager : Node
{
    // Declare member variables here
    [Export]
    private PackedScene chapplePrefab;

    [Export]
    private PackedScene twigPrefab;

    [Export]
    private MeshInstance3D spawnArea;
    
    private bool spawning = false;

    private float spawnAreaLeft;
    private float spawnAreaRight;
    private float spawnAreaTop;
    private float spawnAreaBottom;

    private float spawnTimer = 0f;

    private int score = 0;

    private int fails = 0;

    private int maxFails = 5;

    private float spawnRate = 3f;

    private Label scoreLabel;
    private Label failLabel;
    private Panel gameOverPanel;
    private Panel startGamePanel;

    private ChapplePlayer chapplePlayer;

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        scoreLabel = GetNode<Label>("../UI/ScorePanel/ScoreLabel");
        failLabel = GetNode<Label>("../UI/FailPanel/FailLabel");
        gameOverPanel = GetNode<Panel>("../UI/GameOverPanel");
        startGamePanel = GetNode<Panel>("../UI/StartGamePanel");

        chapplePlayer = GetNode<ChapplePlayer>("../Player");

        // Get half-size of spawn area
        Vector3 halfSize = spawnArea.Scale / 2;

        // Get furthest left and furthest right positions
        spawnAreaLeft = spawnArea.GlobalTransform.origin.x - halfSize.x;
        spawnAreaRight = spawnArea.GlobalTransform.origin.x + halfSize.x;

        // Get furthest top and furthest bottom positions
        spawnAreaTop = spawnArea.GlobalTransform.origin.y + halfSize.y;
        spawnAreaBottom = spawnArea.GlobalTransform.origin.y - halfSize.y;
    }

    public override void _Process(double delta)
    {        
        // Called every frame. Delta is time since last frame.
        // Update game logic here.
        if (!spawning)
            return;

        spawnTimer += (float)delta;

        if (spawnTimer >= spawnRate)
        {
            // Spawn chapple or twig
            if (GD.Randf() < 0.8f)
            {
                // Spawn chapple
                SpawnChapple();
            }
            else
            {
                // Spawn twig
                SpawnTwig();
            }

            // Reset timer
            spawnTimer = 0f;
        }

        // Lower spawn rate as the score increases
        spawnRate = Mathf.Clamp(3f - (score / 100f), 0.5f, 3f);
    }

    public void AddScore(int score)
    {
        this.score += score;
        scoreLabel.Text = $"Chapples: {this.score}";
    }

    public void AddFail()
    {
        if (fails >= maxFails)
            return;

        fails++;
        failLabel.Text = $"Fails: {fails}/{maxFails}";

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

        // Set position
        chapple.GlobalTransform = new Transform3D(
            chapple.GlobalTransform.basis,
            new Vector3(
                (float)GD.RandRange(spawnAreaLeft, spawnAreaRight),
                (float)GD.RandRange(spawnAreaTop, spawnAreaBottom),
                0f
            )
        );

        // Randomize rotation
        chapple.RotationDegrees = new Vector3(
            (float)GD.RandRange(0f, 360f),
            (float)GD.RandRange(0f, 360f),
            (float)GD.RandRange(0f, 360f)
        );
    }

    private void SpawnTwig()
    {
        // Create twig
        RigidBody3D twig = twigPrefab.Instantiate<RigidBody3D>();

        // Add to scene
        AddChild(twig);

        // Set position
        twig.GlobalTransform = new Transform3D(
            twig.GlobalTransform.basis,
            new Vector3(
                (float)GD.RandRange(spawnAreaLeft, spawnAreaRight),
                (float)GD.RandRange(spawnAreaTop, spawnAreaBottom),
                0f
            )
        );
    }

    private void LostGame()
    {
        // Set spawning to false
        spawning = false;

        gameOverPanel.GetNode<Label>("ScoreLabel").Text = $"Score: {score}";

        // Show game over panel
        gameOverPanel.Visible = true;
    }

    public void _on_try_again_button_pressed() 
    {
        GetTree().ReloadCurrentScene();
    }

    public void _on_start_game_button_pressed()
    {
        // Hide start game panel
        startGamePanel.Visible = false;

        // Start game
        chapplePlayer.StartGame();

        // Set spawning to true
        spawning = true;
    }
}
