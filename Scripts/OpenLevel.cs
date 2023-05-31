using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenLevel : MonoBehaviour
{
    public GameObject button;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadSelectedLevelScene()
    {
        Globals.SelectedLevel = this.transform.parent.GetComponent<LevelsBar>().level;
        SceneManager.LoadScene("Level");
    }
}
