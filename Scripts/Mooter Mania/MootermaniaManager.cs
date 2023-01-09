using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class MootermaniaManager : Node3D
{
    private MeshInstance3D spawnArea;
    
    [Export]
    private PackedScene mootermelonPrefab;
    
    [Export]
    private PackedScene mortarmelonPrefab;

    private float spawnAreaLeft;
    private float spawnAreaRight;
    private float spawnAreaTop;
    private float spawnAreaBottom;

    private float spawnTimer = 0f;
    private float spawnRate = 3f;

    private int score = 0;

    private int fails = 0;
    private int maxFails = 3;

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

    private ManiaMovement mootermaniaPlayer;

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        spawnArea = GetNode<MeshInstance3D>("../SpawnArea");
        scoreLabel = GetNode<Label>("../UI/ScorePanel/ScoreLabel");
        failLabel = GetNode<Label>("../UI/FailPanel/FailLabel");
        gameOverPanel = GetNode<Panel>("../UI/GameOverPanel");
        startGamePanel = GetNode<Panel>("../UI/StartGamePanel");
        highScorePanel = GetNode<Panel>("../UI/HighscorePanel");
        highScoreList = highScorePanel.GetNode<ItemList>("List");
        newHighScoreLabel = GetNode<Label>("../UI/NewHighscoreLabel");
        mootermaniaPlayer = GetNode<ManiaMovement>("../Player");

        saveLoc = OS.GetUserDataDir() + "/mootermania_highscores.txt";

        LoadData();

        gameOverPanel.Visible = false;
        newHighScoreLabel.Visible = false;
        startGamePanel.Visible = true;

        if (OS.HasEnvironment("USERNAME")) {
            username = OS.GetEnvironment("USERNAME");
        } else if (OS.HasEnvironment("USER")) {
            username = OS.GetEnvironment("USER");
        }

        if (highScores.Count > 0)
        {
            UpdateHighscoresList();
        }
        else
        {
            highScorePanel.Visible = false;
        }

        Vector3 halfSize = spawnArea.Scale / 2;

        // Get the spawn area's positions
        spawnAreaLeft = spawnArea.GlobalTransform.origin.x - halfSize.x;
        spawnAreaRight = spawnArea.GlobalTransform.origin.x + halfSize.x;
        spawnAreaTop = spawnArea.GlobalTransform.origin.z + halfSize.z;
        spawnAreaBottom = spawnArea.GlobalTransform.origin.z - halfSize.z;
    }

    public override void _Process(double delta)
    {
        if (gameOverPanel.Visible || startGamePanel.Visible)
        {
            return;
        }

        spawnTimer += (float)delta;

        if (spawnTimer >= spawnRate)
        {
            spawnTimer = 0f;
            // Randomise either a mootermelon or a mortarmelon
            if (GD.RandRange(0, 1) < 0.8f)
            {
                SpawnMootermelon();
            }
            else
            {
                SpawnMortarMelon();
            }

            // Increase the spawn rate as the score goes up
            spawnRate = Mathf.Max(0.5f, 3f - (score * 0.05f));
        }
    }

    private void SpawnMootermelon()
    {
        // Spawn a new mootermelon
        MooterMelon newMootermelon = (MooterMelon)mootermelonPrefab.Instantiate();
        AddChild(newMootermelon);

        // Get a random position within the spawn area
        float x = (float)GD.RandRange(spawnAreaLeft, spawnAreaRight);
        float z = (float)GD.RandRange(spawnAreaBottom, spawnAreaTop);

        // Set the position
        newMootermelon.GlobalPosition = new Vector3(x, 1.25f, z);
    }

    private void SpawnMortarMelon()
    {
        // Spawn a new mortarmelon
        MortarMelon newMortarmelon = (MortarMelon)mortarmelonPrefab.Instantiate();
        AddChild(newMortarmelon);

        // Get a random position within the spawn area
        float x = (float)GD.RandRange(spawnAreaLeft, spawnAreaRight);
        float z = (float)GD.RandRange(spawnAreaBottom, spawnAreaTop);

        // Set the position
        newMortarmelon.GlobalPosition = new Vector3(x, 1.25f, z);
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreLabel.Text = $"MooterMelons: {score}";
    }

    public void AddFail()
    {
        fails++;
        failLabel.Text = $"Fails: {fails}/{maxFails}";

        if (fails >= maxFails)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        mootermaniaPlayer.SetSmashing(false);
        gameOverPanel.Visible = true;

        if (highScores.Count == 0 || score > highScores[highScores.Count - 1].Item2)
        {
            newHighScoreLabel.Visible = true;
            highScores.Add(new Tuple<string, int>(username, score));
            highScores.Sort((x, y) => y.Item2.CompareTo(x.Item2));
            UpdateHighscoresList();

            SaveData();
        }

        if (highScores.Count > 0)
        {
            highScorePanel.Visible = true;
        }
    }

    private void SaveData() 
    {
        StreamWriter file = File.CreateText(saveLoc);

        file.WriteLine("Username,MooterMelons");

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

    private void UpdateHighscoresList()
    {
        highScoreList.Clear();
        foreach (Tuple<string, int> score in highScores)
        {
            highScoreList.AddItem(score.Item1 + ": " + score.Item2);
        }
    }

    public void _on_start_game_button_pressed()
    {
        startGamePanel.Visible = false;
        highScorePanel.Visible = false;
        newHighScoreLabel.Visible = false;
        score = 0;
        fails = 0;
        scoreLabel.Text = $"MooterMelons: {score}";
        failLabel.Text = $"Fails: {fails}/{maxFails}";
        mootermaniaPlayer.SetSmashing(true);
    }

    public void _on_back_to_hub_button_pressed()
    {
        GetTree().ChangeSceneToFile("res://Levels/farm_level.tscn");
    }

    public void _on_try_again_button_pressed()
    {
        GetTree().ReloadCurrentScene();
    }
}
