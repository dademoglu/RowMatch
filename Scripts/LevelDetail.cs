using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelDetail : MonoBehaviour
{
    public LevelDetail instance;

    public Coroutine loadlevel;
    public int levelNumber, currentScore;
    public int height;
    public int width;
    public int moveLimit;
    public int moveLeft;
    public string[] tileList;
    public TextMeshProUGUI moveText;
    public TextMeshProUGUI scoreText;
    public List<int> lockedRows;

    public Dictionary<string, int> colorPoints = new()
    {
        { "blue", 200 },
        { "red", 100 },
        { "green", 150 },
        { "yellow", 250 }
    };

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        string fileName = Globals.SelectedLevel <= 15 ?
            Globals.DataPathText + Globals.SelectedLevel :
            Globals.AlternativeDataPathText + (Globals.SelectedLevel - 15);
        string[] lines = File.ReadAllLines(fileName);

        levelNumber = int.Parse(lines[0].Split(" ")[1]); //Line indicating level number
        width = int.Parse(lines[1].Split(" ")[1]);//Line indicating width
        height = int.Parse(lines[2].Split(" ")[1]);//Line indicating height
        moveLimit = int.Parse(lines[3].Split(" ")[1]);//Line indicating moveLimit
        moveLeft = moveLimit;
        tileList = lines[4].Split(" ")[1].Split(",");//Line indicating moveLimit
        currentScore = 0;
        moveText = GameObject.FindGameObjectWithTag("move").GetComponent<TextMeshProUGUI>();
        scoreText = GameObject.FindGameObjectWithTag("score").GetComponent<TextMeshProUGUI>();
        MainManager.setScoreForLevel(levelNumber, currentScore);
        setLevelValues();
    }

    private void setLevelValues()
    {
        GameObject.FindGameObjectWithTag("level").GetComponent<TextMeshProUGUI>().text = levelNumber.ToString();
        moveText.text = moveLimit.ToString();
        scoreText.text = "0";
    }

    public void UpdateMove()
    {
        int currentMoveLeft = int.Parse(moveText.text);
        currentMoveLeft -= 1;
        moveLeft = currentMoveLeft;
        moveText.text = currentMoveLeft.ToString();

        if (CheckGameEnd(currentMoveLeft))
        {
            LevelDone();
        }
    }

    public bool CheckGameEnd(int currentMoveLeft)
    {
        //TODO: PERFORMANCE ISSUES!
        //Not the best way to solve the issue of duplicates but for now, let's keep it
        lockedRows = lockedRows.Distinct().ToList();
        int listLength = lockedRows.Count();

        if (currentMoveLeft == 0 || listLength == height) return true;
        if (listLength == 0) return false;


        lockedRows.Sort();

        Dictionary<string, int> tempCounter = new Dictionary<string, int>()
            {
                { "blue", 0 },
                { "red", 0 },
                { "green", 0 },
                { "yellow", 0 }
            };
        //Simple game end condition
        //Check the colors between locked boxes, if there are enough (>= width) color than there is still solution
        //Does not consider remaining moves or how far the colors are from each other
        for (int i = 0; i < listLength; ++i)
        {
            tempCounter = new Dictionary<string, int>()
            {
                { "blue", 0 },
                { "red", 0 },
                { "green", 0 },
                { "yellow", 0 }
            };
            int j = i + 1;
            int tempHeight = j < listLength ? lockedRows[j] : height;

            for (int y = lockedRows[i] + 1; y < tempHeight; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    tempCounter[Board.allTiles[x, y].tag] += 1;
                }
            }

            foreach (var tag in tempCounter)
            {
                if (tag.Value >= width) return false;
            }
        }

        //Special case to check before the first locked row
        tempCounter = new Dictionary<string, int>()
            {
                { "blue", 0 },
                { "red", 0 },
                { "green", 0 },
                { "yellow", 0 }
            };

        for (int y = 0; y < lockedRows[0]; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                tempCounter[Board.allTiles[x, y].tag] += 1;
            }
        }

        foreach (var tag in tempCounter)
        {
            if (tag.Value >= width) return false;
        }



        return true;
    }

    public void LevelDone()
    {
        UpdatePlayerPref();
        bool IsLevelPlayed = PlayerPrefs.HasKey("Played" + levelNumber);

        Debug.Log(PlayerPrefs.GetInt("Level" + levelNumber) + ", current: " + currentScore + " BOOL: " + (PlayerPrefs.GetInt("Level" + levelNumber) >= currentScore));
        if (IsLevelPlayed && PlayerPrefs.GetInt("Level" + levelNumber) >= currentScore)
        {
            instance.StartCoroutine(instance.LoadLevelAfterDelay("EndOfLevel", 1.5f));
        }
        else
        {
            PlayerPrefs.SetInt("Played" + levelNumber, 1);
            PlayerPrefs.SetInt("Level" + levelNumber, currentScore);
            instance.StartCoroutine(instance.LoadLevelAfterDelay("HighScore", 1.5f));
        }
    }
    public void UpdatePlayerPref()
    {
        if (PlayerPrefs.GetInt("FurthestLevelPlayed") <= levelNumber)
        {
            PlayerPrefs.SetInt("FurthestLevelPlayed", levelNumber + 1);
            PlayerPrefs.SetInt("Unlocked" + (levelNumber + 1), 1);
        }
    }

    IEnumerator LoadLevelAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }


    public void updateScore(string color)
    {
        currentScore += colorPoints[color] * width;
        scoreText.text = currentScore.ToString();
        MainManager.setScoreForLevel(levelNumber, currentScore);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
