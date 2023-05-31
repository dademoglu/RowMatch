using System;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Responsible for each tiles movement. The "color tiles" are the gameobject that we can actualy move
/// Detects the swipe starting from the object
/// </summary>

public class ColorTile : MonoBehaviour
{
    public GameObject lockBox;
    private Vector2 firstTouchPos;
    private Vector2 finalTouchPos;
    public float swipeAngle;
    public int xLoc;
    public int yLoc;
    public GameObject details;
    private LevelDetail leveldetail;

    private float targetX;
    private float targetY;
    public bool isLocked;

    // Start is called before the first frame update
    void Start()
    {
        string[] loc = name.Split(',');
        xLoc = int.Parse(loc[0]);
        yLoc = int.Parse(loc[1]);
        targetX = this.transform.position.x;
        targetY = this.transform.position.y;
        isLocked = tag == "lock";

        leveldetail = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelDetail>();
        checkMatch();
    }

    public bool checkMatch()
    {
        bool isMatch = true;

        for (int x = 0; x < leveldetail.width; ++x)
        {
            isMatch = Board.allTiles[x, yLoc].CompareTag(gameObject.tag);

            if (!isMatch) return isMatch;
        }

        leveldetail.lockedRows.Add(yLoc);

        for (int x = 0; x < leveldetail.width; ++x)
        {
            Sequence seq = DOTween.Sequence();

            Vector3 tempPos = new Vector3(x * Globals.FullTileSize, yLoc * Globals.FullTileSize, 0);
            GameObject currentObject = Board.allTiles[x, yLoc];

            currentObject.GetComponent<ColorTile>().isLocked = true;

            GameObject newLockBox = Instantiate(lockBox, tempPos, Quaternion.Euler(0, 0, 0));
            newLockBox.GetComponent<ColorTile>().isLocked = true;
            newLockBox.GetComponent<ColorTile>().name = x + "," + yLoc;
            newLockBox.transform.SetParent(this.transform.parent.transform, false);
            Board.allTiles[x, yLoc] = newLockBox;

            seq.Append(currentObject.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 0.4f))
                .Append(newLockBox.transform.DOScale(new Vector3(2f, 2f, 2f), 0.4f))
                .Append(newLockBox.transform.DOShakeScale(1.2f)).OnComplete(() => { Destroy(currentObject); });
        }

        leveldetail.updateScore(tag);
        return isMatch;
    }

    private void OnMouseDown()
    {
        firstTouchPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        finalTouchPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        if (Vector2.Distance(firstTouchPos, finalTouchPos) <= 0.0f)
        {
            Debug.Log("No swipe detected");
        }
        else
        {
            initiateMovement();
        }

    }

    private void initiateMovement()
    {
        (int x, int y) direction = GetDirection();

        int tempx = xLoc + direction.x;
        int tempy = yLoc + direction.y;

        if (tempx >= leveldetail.width || tempx < 0 || tempy >= leveldetail.height || tempy < 0 ||
            Board.allTiles[tempx, tempy].GetComponent<ColorTile>().isLocked || isLocked || leveldetail.moveLeft == 0)
        {
            //Do Nothing
            Debug.Log("Invalid Movement");
        }
        else
        {
            SwapTiles((tempx, tempy));
            leveldetail.UpdateMove();
        }
    }

    private void SwapTiles((int x, int y) location)
    {
        GameObject other = Board.allTiles[location.x, location.y];

        //Replace object's location in tile array
        //Set current object's array index for the the swapped object
        Board.allTiles[xLoc, yLoc] = other;


        //Update actual x-coordinate, y-coordinate values of the other (swapping) object
        other.GetComponent<ColorTile>().xLoc = xLoc;
        other.GetComponent<ColorTile>().yLoc = yLoc;

        //Set this object's x-coordinate and y-coordinate
        xLoc = location.x;
        yLoc = location.y;

        //Set other object's array index for the the current object
        Board.allTiles[xLoc, yLoc] = this.gameObject;

        //Current target's move location (to)
        targetX = other.transform.position.x;
        targetY = other.transform.position.y;


        //Tween move both objects [Idea: create a function that works seperately for both objects]
        float xRotate = tag == "yellow" ? 90.0f : -90.0f;

        Sequence seq = DOTween.Sequence();
        seq.Append(
            other.transform.DOMove(new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), 0.5f))
            .Join(transform.DOMove(new Vector3(targetX, targetY, this.transform.position.z), 0.5f))
            .Join(transform.DORotate(new Vector3(0.0f, 360.0f, 0.0f), 0.6f, RotateMode.LocalAxisAdd));

        //Swap the name's (in a sense IDs) of the objects
        other.name = this.name;
        this.name = xLoc + "," + yLoc;


        checkMatch();
        other.GetComponent<ColorTile>().checkMatch();
    }


    private (int, int) GetDirection()
    {
        float angle = CalculateAngle();

        if (angle > -45 && angle <= 45)
        {
            return (1, 0);
        }
        else if (angle > 45 && angle <= 135)
        {
            return (0, 1);
        }
        else if (angle > 135 || angle <= -135)
        {
            return (-1, 0);
        }
        else if (angle < -45 && angle >= -135)
        {
            return (0, -1);
        }

        return (0, 0);
    }

    private float CalculateAngle()
    {
        return (float)(Mathf.Atan2(finalTouchPos.y - firstTouchPos.y, finalTouchPos.x - firstTouchPos.x) * 180 / Math.PI);
    }
}