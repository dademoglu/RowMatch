using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class StarAnimate : MonoBehaviour
{
    public GameObject star;
    // Start is called before the first frame update
    void Start()
    {
        DOTween.Sequence()
          .Append(transform.DOJump(new Vector3(transform.position.x, transform.position.y, transform.position.z), 2.0f, 1, 2.0f))
          .OnComplete(() => { transform.DOShakeScale(0.4f); })
          .Join(transform.DORotate(new Vector3(0.0f, 360.0f, 0.0f), 1.0f, RotateMode.FastBeyond360)
          .SetLoops(2, LoopType.Restart)
          .SetRelative()
          .SetEase(Ease.Linear));

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
