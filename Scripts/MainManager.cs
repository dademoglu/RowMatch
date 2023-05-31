using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// Keeps track of the local progress, saves data if required, downloads level if network is accessable
/// </summary>
public class MainManager : MonoBehaviour
{
    public static MainManager Instance;
    public static Dictionary<int, int> levelScoreDictionary = new Dictionary<int, int>();
    public static int lastScore;

    private void Start()
    {
        lastScore = 0;

        if (!PlayerPrefs.HasKey("Downloaded"))
        {
            PlayerPrefs.SetInt("Downloaded", 11);
        }

        Directory.CreateDirectory(Globals.directoryPath);

        if (!PlayerPrefs.HasKey("InitialSetup"))
        {
            for (int i = 0; i < 10; ++i)
            {
                string fileName = "RM_A" + (i + 1);
                string savePath = string.Format("{0}/{1}", Globals.directoryPath, fileName);
                File.WriteAllText(savePath, File.ReadAllText("Assets/LevelData/" + fileName));
            }

            PlayerPrefs.SetInt("InitialSetup", 1);
        }

        StartCoroutine(DownloadLevels());
    }

    void Update()
    {

    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    //check if the level has been played
    public static bool IsLevelPlayed(int level)
    {
        return levelScoreDictionary.ContainsKey(level);
    }

    //return the score of the level if played, else return -1
    public static int getScoreForLevel(int level)
    {
        if (IsLevelPlayed(level)) return levelScoreDictionary[level];

        return -1;
    }

    //Sets new score for the level
    //Returns true if it's a highscore
    public static void setScoreForLevel(int level, int score)
    {
        lastScore = score;
        levelScoreDictionary[level] = score;
    }

    public static int getLastScore()
    {
        return lastScore;
    }



    private bool isConnectedToInternet()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    IEnumerator DownloadLevels()
    {
        while (PlayerPrefs.GetInt("Downloaded") <= 25)
        {
            int currentLevel = PlayerPrefs.GetInt("Downloaded");

            if (!isConnectedToInternet())
            {
                Debug.Log("No Internet");
                yield return new WaitForSeconds(5.0f);
            }
            else
            {
                Debug.Log("Downloading Level " + currentLevel);

                string fileName = "RM_" + (currentLevel <= 15 ? "A" + currentLevel : "B" + (currentLevel - 15));
                string url = "https://row-match.s3.amazonaws.com/levels/" + fileName;
                UnityWebRequest www = new UnityWebRequest(url);

                www.downloadHandler = new DownloadHandlerBuffer();

                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    string savePath = string.Format("{0}/{1}", Globals.directoryPath, fileName);
                    File.WriteAllText(savePath, www.downloadHandler.text);
                    PlayerPrefs.SetInt("Downloaded", currentLevel + 1);
                }
            }
        }
    }
}
