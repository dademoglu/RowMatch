using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    public TextMeshProUGUI scoreText, highScore;
    // Start is called before the first frame update
    void Start()
    {
        int currentLevel = Globals.SelectedLevel;

        scoreText.text = "Score: " + MainManager.getLastScore();
        highScore.text = "Highest Score: " + PlayerPrefs.GetInt("Level" + currentLevel);

        StartCoroutine(LoadLevelsPopup());
    }

    IEnumerator LoadLevelsPopup()
    {
        yield return new WaitForSeconds(5.0f);
        SceneManager.LoadScene("LevelsPopup");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
