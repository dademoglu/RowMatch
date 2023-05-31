using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScoreUpdate : MonoBehaviour
{
    public TextMeshPro textBox;
    // Start is called before the first frame update
    void Start()
    {
        int score = PlayerPrefs.GetInt("Level" + Globals.SelectedLevel);
        textBox.text = "Highest Score\n" + (score == -1 ? 0 : score);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
