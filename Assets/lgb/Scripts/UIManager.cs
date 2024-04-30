using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    RectTransform stageClear;

    void Start()
    {
        stageClear = transform.GetChild(4).GetComponent<RectTransform>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(ClearStage());
        }
    }



    public IEnumerator ClearStage()
    {
        stageClear.anchoredPosition = new Vector2(700, 0);
        stageClear.DOAnchorPos(new Vector2(100, 0), 0.3f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.3f);
        stageClear.DOAnchorPos(new Vector2(-100, 0), 0.7f).SetEase(Ease.InQuad);
        yield return new WaitForSeconds(0.7f);
        stageClear.DOAnchorPos(new Vector2(-700, 0), 0.3f).SetEase(Ease.OutQuad);
    }
}
