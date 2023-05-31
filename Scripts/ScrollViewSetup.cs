using UnityEngine;

/// <summary>
/// Create the level bar, check if it's locked on object itself
/// </summary>
public class ScrollViewSetup : MonoBehaviour
{
    public GameObject levelBar;
    // Start is called before the first frame update
    void Start()
    {
        int levelCount = 25;
        if (!PlayerPrefs.HasKey("FurthestLevelPlayed")) PlayerPrefs.SetInt("FurthestLevelPlayed", 1);

        for (int i = 0; i < levelCount; i++)
        {
            GameObject listElement = Instantiate(levelBar);
            listElement.transform.SetParent(this.transform, false);
            listElement.GetComponent<LevelsBar>().level = i + 1;
            listElement.GetComponent<LevelsBar>().PrepareData();
        }
    }
}
