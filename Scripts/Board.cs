using UnityEngine;
using static Globals;
using System.IO;
using System.Collections.Generic;
using DG.Tweening;


/// <summary>
/// Board setup. An array of gameobjects are created. This array is used to swap objects,
/// easier detection of object locations and checking matches
/// </summary>
public class Board : MonoBehaviour
{
    public GameObject[] tileObjectList;
    public GameObject blueTile, redTile, yellowTile, greenTile, backgroundBox;
    private BackgroundTile[,] backgrounds;
    public static GameObject[,] allTiles;
    public GameObject details;
    private LevelDetail leveldetail;
    // Start is called before the first frame update
    void Start()
    {
        leveldetail = details.GetComponent<LevelDetail>();
        PrepareLevelData();
        backgrounds = new BackgroundTile[leveldetail.width, leveldetail.height];
        allTiles = new GameObject[leveldetail.width, leveldetail.height];

        if (leveldetail.width == 9)
        {
            transform.GetComponent<RectTransform>().localScale -= new Vector3(0.02f, 0.0f, 0.0f);
        }

        Setup();
    }

    // Prepare the board for gameplay
    private void Setup()
    {
        Transform backgroundParent = this.transform.GetChild(0); //Parent that background images will be siblings to
        Transform tileParent = this.transform.GetChild(1); //Parent that actual tiles (gameObjects) will be siblings to

        for (int i = 0; i < leveldetail.width; ++i)
        {
            for (int j = 0; j < leveldetail.height; ++j)
            {
                Vector3 tempPosition = new Vector3(i * Globals.FullTileSize, j * Globals.FullTileSize, 0);
                GameObject backgroundTile = Instantiate(backgroundBox, tempPosition, Quaternion.identity);
                GameObject actualTile = Instantiate(tileObjectList[i + j * leveldetail.width], tempPosition, Quaternion.Euler(-90, 0, 0));


                backgroundTile.transform.SetParent(backgroundParent.transform, false);

                actualTile.transform.SetParent(tileParent.transform, false);
                actualTile.gameObject.name = i + "," + j;

                allTiles[i, j] = actualTile;
            }
        }

        Vector3 calculatedMids = CalculateMidpoint();
        //Board comes in 
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMoveX(0.0f, 0.6f)).Append(transform.DOShakePosition(0.4f, strength: 0.4f, vibrato: 7));

        backgroundParent.transform.localPosition = calculatedMids;
        tileParent.transform.localPosition = calculatedMids;

        this.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = CalculateBackgroundImageSize();
    }

    //Read from the level file and prepare the level data for further use
    private void PrepareLevelData()
    {
        tileObjectList = GetGameObjectList(leveldetail.tileList);//Line indicating color order of objects
    }

    //From string list of colors, create gameObject array that carries the corresponding colored object
    private GameObject[] GetGameObjectList(string[] colorNames)
    {
        GameObject[] tileObjects = new GameObject[leveldetail.width * leveldetail.height];

        Dictionary<string, GameObject> colorToObjectDictionary = new()
        {
            { "b", blueTile },
            { "r", redTile },
            { "g", greenTile },
            { "y", yellowTile }
        };

        for (int i = 0; i < leveldetail.width * leveldetail.height; ++i)
        {
            tileObjects[i] = colorToObjectDictionary[colorNames[i]];
        }

        return tileObjects;
    }


    //Calculations to relocate the whole board to the center of the camera
    private Vector3 CalculateMidpoint()
    {
        float xMid = (float)(((leveldetail.width + 1.0) / 2.0) - 1.0);
        float yMid = (float)(((leveldetail.height + 1.0) / 2.0) - 1.0);

        //added an offset to the yMid because of the info header
        return new Vector3(-(xMid * Globals.FullTileSize), -(yMid * (Globals.FullTileSize + 5.0f)), 0);
    }

    //Find the resize value of the background image
    private Vector2 CalculateBackgroundImageSize()
    {
        float fullWidth = (leveldetail.width * Globals.FullTileSize) + Globals.BorderOffset;
        float fullHeight = (leveldetail.height * Globals.FullTileSize) + Globals.BorderOffset;

        return new Vector2(fullWidth, fullHeight);
    }
}
