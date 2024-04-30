using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    public static UIManager um;
    RectTransform stageClear;

    public Text stageText;
    public Text moneyText;
    public Text enemyText;
    public Text timerText;

    void Start()
    {
        if(um == null)
        {
            um = GetComponent<UIManager>();
            stageText = transform.GetChild(0).GetChild(0).GetComponent<Text>();
            moneyText = transform.GetChild(1).GetChild(1).GetComponent<Text>();
            enemyText = transform.GetChild(2).GetChild(1).GetComponent<Text>();
            timerText = transform.GetChild(3).GetChild(1).GetComponent<Text>();

        }

        stageClear = transform.GetChild(4).GetComponent<RectTransform>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(ClearStageAnim());
        }
    }

    public void ChangeStageNum(int stage)
    {
        stageText.text = "STAGE " + stage;
    }
    public void ChangeMoneyNum(int originMoney, int inputMoney)
    {
        StartCoroutine(CountMoney(originMoney, originMoney + inputMoney));
        if (inputMoney > 0)
        {
            StartCoroutine(InputMoneyAnim(Color.white, inputMoney));
        }
        else
        {
            StartCoroutine(InputMoneyAnim(Color.red, inputMoney));
        }
    }
    private IEnumerator CountMoney(int preMoney, int goalMoney)
    {
        float timer = 0;
        int curMoney;
        while (timer < 1)
        {
            timer += Time.deltaTime * 3;
            curMoney = (int)Mathf.Lerp(preMoney, goalMoney, timer);
            moneyText.text = "" + curMoney;

            yield return null;
        }
    }
    private IEnumerator InputMoneyAnim(Color inputColor, int inputMoney)
    {
        GameObject inputMoneyObj = new GameObject();
        Text inputText = inputMoneyObj.AddComponent<Text>();
        RectTransform inputRT = inputMoneyObj.GetComponent<RectTransform>();
        inputMoneyObj.transform.parent = transform;
        inputText.font = moneyText.font;
        inputText.fontSize = moneyText.fontSize * 2;
        inputText.alignment = TextAnchor.MiddleCenter;
        inputText.horizontalOverflow = HorizontalWrapMode.Overflow;
        inputText.verticalOverflow = VerticalWrapMode.Overflow;
        inputRT.anchoredPosition = Vector3.zero;
        inputRT.sizeDelta = GetComponent<RectTransform>().sizeDelta;
        if (inputMoney > 0)
            inputText.text = "+" + inputMoney;
        else
            inputText.text = "" + inputMoney;
        inputText.color = inputColor;

        float timer = 1;
        while (timer > 0)
        {
            timer -= Time.deltaTime * 2;
            inputRT.anchoredPosition += Vector2.down * 1.5f;
            inputColor.a = timer;
            inputText.color = inputColor;
            yield return null;
        }

        Destroy(inputMoneyObj);
    }

    public void changeEnemyNum()
    {
        enemyText.text = "" + GameManager.gm.enemyManager.enemy.Count;
    }

    public void changeTimer(int timer)
    {
        timerText.text = "" + timer;
    }

    public IEnumerator ClearStageAnim() //스테이지가 클리어 됐을 때 애니메이션
    {
        stageClear.anchoredPosition = new Vector2(700, 0);
        stageClear.DOAnchorPos(new Vector2(100, 0), 0.3f).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(0.3f);
        stageClear.DOAnchorPos(new Vector2(-100, 0), 0.7f).SetEase(Ease.InQuad);
        yield return new WaitForSeconds(0.7f);
        stageClear.DOAnchorPos(new Vector2(-700, 0), 0.3f).SetEase(Ease.OutQuad);
    }
}
