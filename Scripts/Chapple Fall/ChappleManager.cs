using Godot;
using System;
using System.Collections.Generic;
using System.IO;

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
    private float spawnRate = 3f;
    private float twigSpawnTimer = 0f;
    private float twigSpawnRate = 2f;

    private int score = 0;

    private int fails = 0;

    private int maxFails = 5;

    private string username = "Player";
    private string saveLoc;
    private List<Tuple<string, int>> highScores = new List<Tuple<string, int>>();

    private Label scoreLabel;
    private Label failLabel;
    private Panel gameOverPanel;
    private Panel startGamePanel;
    private Panel highScorePanel;
    private ItemList highScoreList;
    private Label newHighScoreLabel;

    private ChapplePlayer chapplePlayer;

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        scoreLabel = GetNode<Label>("../UI/ScorePanel/ScoreLabel");
        failLabel = GetNode<Label>("../UI/FailPanel/FailLabel");
        gameOverPanel = GetNode<Panel>("../UI/GameOverPanel");
        startGamePanel = GetNode<Panel>("../UI/StartGamePanel");

        highScorePanel = GetNode<Panel>("../UI/HighscorePanel");
        highScoreList = highScorePanel.GetNode<ItemList>("List");
        newHighScoreLabel = GetNode<Label>("../UI/NewHighscoreLabel");

        chapplePlayer = GetNode<ChapplePlayer>("../Player");

        // Get half-size of spawn area
        Vector3 halfSize = spawnArea.Scale / 2;

        // Get furthest left and furthest right positions
        spawnAreaLeft = spawnArea.GlobalTransform.origin.x - halfSize.x;
        spawnAreaRight = spawnArea.GlobalTransform.origin.x + halfSize.x;

        // Get furthest top and furthest bottom positions
        spawnAreaTop = spawnArea.GlobalTransform.origin.y + halfSize.y;
        spawnAreaBottom = spawnArea.GlobalTransform.origin.y - halfSize.y;

        if (OS.HasEnvironment("USERNAME")) {
            username = OS.GetEnvironment("USERNAME");
        } else if (OS.HasEnvironment("USER")) {
            username = OS.GetEnvironment("USER");
        }

        saveLoc = OS.GetUserDataDir() + "/Chapple_Highscores.csv";
        
        LoadData();

        if (highScores.Count == 0)
        {
            highScorePanel.Visible = false;
            return;
        }

        UpdateHighscores();
    }

    public override void _Process(double delta)
    {        
        // Called every frame. Delta is time since last frame.
        // Update game logic here.
        if (!spawning)
            return;

        spawnTimer += (float)delta;
        twigSpawnTimer += (float)delta;

        if (spawnTimer >= spawnRate)
        {
            // Spawn chapple
            SpawnChapple();

            // Reset timer
            spawnTimer = 0f;
        }

        if (twigSpawnTimer >= twigSpawnRate)
        {
            // Spawn twig
            SpawnTwig();

            // Reset timer
            twigSpawnTimer = 0f;
        }
    }

    private void UpdateHighscores() 
    {
        highScoreList.Clear();
        foreach (Tuple<string, int> score in highScores)
        {
            highScoreList.AddItem(score.Item1 + " - " + score.Item2);
        }
    }

    private void SaveData() 
    {
        StreamWriter file = File.CreateText(saveLoc);

        file.WriteLine("Username,Chapples");

        highScores.Sort((x, y) => y.Item2.CompareTo(x.Item2));
        
        foreach (Tuple<string, int> score in highScores)
        {
            file.WriteLine(score.Item1 + "," + score.Item2);
        }

        file.Close();
    }

    private void LoadData() 
    {
        if (File.Exists(saveLoc)) {
            StreamReader file = File.OpenText(saveLoc);

            // Skip first line
            file.ReadLine();

            while (!file.EndOfStream) {
                string line = file.ReadLine();
                string[] values = line.Split(',');

                highScores.Add(new Tuple<string, int>(values[0], int.Parse(values[1])));
            }

            highScores.Sort((x, y) => y.Item2.CompareTo(x.Item2));

            file.Close();
        }
    }

    public void AddScore(int score)
    {
        if (this.score == 0 && score <= 0)
            return;

        this.score += score;
        // Lower spawn rate as the score increases
        spawnRate = Mathf.Clamp(3f - (score / 50f), 0.5f, 3f);

        // Lower twig spawn rate as the score increases
        twigSpawnRate = Mathf.Clamp(2f - (score / 50f), 0.5f, 2f);

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

        // Set player to paused
        chapplePlayer.SetPaused(true);

        gameOverPanel.GetNode<Label>("ScoreLabel").Text = $"Score: {score}";

        // Show game over panel
        gameOverPanel.Visible = true;

        if (highScores.Count == 0 || score > highScores[highScores.Count - 1].Item2)
        {
            newHighScoreLabel.Visible = true;

            // Add new high score
            highScores.Add(new Tuple<string, int>(username, score));

            // Save high scores
            SaveData();
        }
        
        UpdateHighscores();
        highScorePanel.Visible = true;
    }

    public void _on_try_again_button_pressed() 
    {
        GetTree().ReloadCurrentScene();
    }

    public void _on_start_game_button_pressed()
    {
        // Hide start game panel
        startGamePanel.Visible = false;

        // Hide high score panel
        highScorePanel.Visible = false;

        // Start game
        chapplePlayer.SetPaused(false);

        // Set spawning to true
        spawning = true;
    }

    public void _on_back_to_hub_button_pressed()
    {
        GetTree().ChangeSceneToFile("res://Levels/farm_level.tscn");
    }
}
