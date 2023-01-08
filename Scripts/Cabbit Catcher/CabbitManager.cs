using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class CabbitManager : Node
{
    [Export]
    private PackedScene cabbitPrefab;

    [Export]
    private PackedScene redCabbitPrefab;

    private float cabbitSpawnRate = 3f;
    private float redCabbitSpawnRate = 4f;
    private float cabbitSpawnTimer = 0f;
    private float redCabbitSpawnTimer = 0f;    

    private List<Tuple<Vector3, bool>> spawnPoints;

    private string username = "Player";
    private string saveLoc;
    private int cabbitsCaught = 0;
    private int redCabbitsCaught = 0;

    private List<Tuple<string, int>> highScores = new List<Tuple<string, int>>();

    private Label cabbitsCaughtLabel;
    private Label redCabbitsCaughtLabel;
    private Label gameOverScoreLabel;
    private Panel gameOverPanel;
    private Panel startGamePanel;
    private Panel highScorePanel;
    private ItemList highScoreList;
    private Label newHighScoreLabel;

    private Grabber grabber;
    
    public override void _Ready()
    {
        Node3D spawnPointsNode = GetNode<Node3D>("../SpawnPoints");

        spawnPoints = new List<Tuple<Vector3, bool>>();
        
        // Add spawn points to List
        for (int i = 1; i < 7; i++)
        {
            spawnPoints.Add(new Tuple<Vector3, bool>(spawnPointsNode.GetNode<Node3D>("Spawn" + i).GlobalTransform.origin, false));
        }

        cabbitsCaughtLabel = GetNode<Label>("../UI/ScorePanel/ScoreLabel");
        redCabbitsCaughtLabel = GetNode<Label>("../UI/FailsPanel/FailsLabel");
        gameOverScoreLabel = GetNode<Label>("../UI/GameOverPanel/ScoreLabel");

        gameOverPanel = GetNode<Panel>("../UI/GameOverPanel");
        startGamePanel = GetNode<Panel>("../UI/StartGamePanel");

        highScorePanel = GetNode<Panel>("../UI/HighscorePanel");
        highScoreList = highScorePanel.GetNode<ItemList>("List");
        newHighScoreLabel = GetNode<Label>("../UI/NewHighscoreLabel");

        grabber = GetNode<Grabber>("../Grabber");

        if (OS.HasEnvironment("USERNAME")) {
            username = OS.GetEnvironment("USERNAME");
        } else if (OS.HasEnvironment("USER")) {
            username = OS.GetEnvironment("USER");
        }

        saveLoc = OS.GetUserDataDir() + "/Cabbit_Highscores.csv";
        
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
        // Check if escape key is pressed
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            GetTree().Quit();
        }

        if (startGamePanel.Visible || gameOverPanel.Visible)
        {
            return;
        }

        // Spawn cabbits
        cabbitSpawnTimer += (float)delta;
        if (cabbitSpawnTimer >= cabbitSpawnRate)
        {
            cabbitSpawnTimer = 0f;
            SpawnCabbit();

            // Set new random spawn time between 3 and 5 seconds
            cabbitSpawnRate = (float)GD.RandRange(3f, 5f);
        }

        // Spawn red cabbits
        redCabbitSpawnTimer += (float)delta;
        if (redCabbitSpawnTimer >= redCabbitSpawnRate)
        {
            redCabbitSpawnTimer = 0f;
            SpawnRedCabbit();

            // Set new random spawn time between 4 and 6 seconds
            redCabbitSpawnRate = (float)GD.RandRange(3f, 6f);
        }
    }

    public void EditSpawnPoint(int spawnIndex, bool occupied)
    {
        spawnPoints[spawnIndex] = new Tuple<Vector3, bool>(spawnPoints[spawnIndex].Item1, occupied);
    }

    public void CabbitCaught(Cabbit cabbit)
    {
        if (cabbit.GetCabbitType() == Cabbit.CabbitType.Cabbit)
        {
            cabbitsCaught++;
            cabbitsCaughtLabel.Text = "Cabbits Caught: " + cabbitsCaught;
        }
        else
        {
            redCabbitsCaught++;
            redCabbitsCaughtLabel.Text = "Red Cabbits Caught: " + redCabbitsCaught;

            if (redCabbitsCaught >= 3)
            {
                GameOver();
            }
        }

        cabbit.DespawnCabbit();
    }

    private int GetRandomSpawnPoint()
    {
        List<Tuple<Vector3, int>> availableSpawnPoints = new List<Tuple<Vector3, int>>();

        // Add available spawn points to list
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            if (!spawnPoints[i].Item2)
            {
                availableSpawnPoints.Add(new Tuple<Vector3, int>(spawnPoints[i].Item1, i));
            }
        }

        // Select random spawn point
        if (availableSpawnPoints.Count > 0)
        {
            int randomIndex = Mathf.Clamp((int)GD.RandRange(0, availableSpawnPoints.Count), 0, availableSpawnPoints.Count - 1);
            return availableSpawnPoints[randomIndex].Item2;
        }

        return -1;
    }

    private void SpawnCabbit()
    {
        // Get random spawn point
        int spawnPointIndex = GetRandomSpawnPoint();

        if (spawnPointIndex == -1)
        {
            GD.Print("No spawn points available");
            return;
        }

        // Spawn cabbit
        Cabbit cabbit = (Cabbit)cabbitPrefab.Instantiate();
        cabbit.GlobalTransform = new Transform3D(Basis.Identity, spawnPoints[spawnPointIndex].Item1);
        AddChild(cabbit);

        // Set spawn point to occupied
        spawnPoints[spawnPointIndex] = new Tuple<Vector3, bool>(spawnPoints[spawnPointIndex].Item1, true);

        // Set cabbit's spawn point index
        cabbit.SetSpawnIndex(spawnPointIndex);
    }

    private void SpawnRedCabbit()
    {
        // Get random spawn point
        int spawnPointIndex = GetRandomSpawnPoint();

        if (spawnPointIndex == -1)
        {
            GD.Print("No spawn points available");
            return;
        }

        // Spawn red cabbit
        Cabbit redCabbit = (Cabbit)redCabbitPrefab.Instantiate();
        redCabbit.GlobalTransform = new Transform3D(Basis.Identity, spawnPoints[spawnPointIndex].Item1);
        AddChild(redCabbit);

        // Set spawn point to occupied
        spawnPoints[spawnPointIndex] = new Tuple<Vector3, bool>(spawnPoints[spawnPointIndex].Item1, true);

        // Set red cabbit's spawn point index
        redCabbit.SetSpawnIndex(spawnPointIndex);
    }

    private void GameOver()
    {
        gameOverPanel.Visible = true;
        gameOverScoreLabel.Text = "Cabbits Caught: " + cabbitsCaught;

        if (highScores.Count == 0 || cabbitsCaught > highScores[highScores.Count - 1].Item2)
        {
            newHighScoreLabel.Visible = true;

            // Add new high score
            highScores.Add(new Tuple<string, int>(username, cabbitsCaught));

            // Save high scores
            SaveData();
        }
        
        UpdateHighscores();
        highScorePanel.Visible = true;
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

        file.WriteLine("Username,Cabbits Caught");

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

    public void _on_start_game_button_pressed()
    {
        startGamePanel.Visible = false;
        highScorePanel.Visible = false;
        newHighScoreLabel.Visible = false;
        grabber.StartGame();
    }

    public void _on_try_again_button_pressed()
    {
        gameOverPanel.Visible = false;
        GetTree().ReloadCurrentScene();
    }
}
