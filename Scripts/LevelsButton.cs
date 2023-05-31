using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Button actions
/// </summary>
public class LevelsButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    public void LoadLevelsScene() => SceneManager.LoadScene("LevelsPopup");
    public void LoadInitialScene() => SceneManager.LoadScene("MainScene");
    public void LevelDone() => GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelDetail>().LevelDone();
    public void TryLevel() => SceneManager.LoadScene("Level");
}
