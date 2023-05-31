using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// On LevelPopup, each row of data is shown seperately and animations are decided on runtime
/// </summary>

public class LevelsBar : MonoBehaviour
{
    public int level;
    public GameObject rawImageLock, rawImagePlay, Flares;
    public TextMeshProUGUI LevelInfoText, ScoreText;


    public void PrepareData()
    {
        int furthestLevelPlayed = PlayerPrefs.GetInt("FurthestLevelPlayed");
        bool isLocked = furthestLevelPlayed < level;
        string scoreText = isLocked ? "Locked Level" : "No Score";
        string fileName = level <= 15 ? Globals.DataPathText + level : Globals.AlternativeDataPathText + (level - 15);
        string levelText = "Network Required";
        bool isFileExist = File.Exists(fileName);

        if (!isLocked && isFileExist)
        {
            if (PlayerPrefs.HasKey("Level" + level))
                scoreText = "Highest Score: " + PlayerPrefs.GetInt("Level" + level);


            if (PlayerPrefs.HasKey("Unlocked" + level))
            {
                Flares.SetActive(true);
                Invoke(nameof(AnimationsStart), 1.0f);

                PlayerPrefs.DeleteKey("Unlocked" + level);
            }
            else
            {
                rawImageLock.SetActive(false);
                rawImagePlay.SetActive(true);
            }
        }
        else
        {
            rawImageLock.SetActive(true);
            rawImagePlay.SetActive(false);
        }


        if (isFileExist)
        {
            string[] lines = File.ReadAllLines(fileName);
            levelText = "Level " + level + " - " + lines[3].Split(" ")[1] + " Moves";
        }

        LevelInfoText.text = levelText;
        ScoreText.text = scoreText;
    }

    void AnimationsStart()
    {
        rawImageLock.transform.DOShakePosition(1.0f).OnComplete(() => Appear());
    }

    void Appear()
    {
        rawImageLock.SetActive(false);
        rawImagePlay.transform.DOShakePosition(0.5f);
        rawImagePlay.SetActive(true);


    }

    // TODO: Update globals selected level!!
}
