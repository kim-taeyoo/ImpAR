using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyTextAnimation : MonoBehaviour
{
    Text text;
    int money;

    void Start()
    {
        text = GetComponent<Text>();
        money = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PlusMoney(100);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            PlusMoney(-100);
        }
    }

    public void PlusMoney(int inputMoney)
    {
        StartCoroutine(CountMoney(money, money + inputMoney));
        money += inputMoney;
        if (inputMoney > 0)
        {
            StartCoroutine(InputMoneyAnim(Color.blue, inputMoney));
        }
        else
        {
            StartCoroutine(InputMoneyAnim(Color.red, inputMoney));
        }
    }

    public void MinusMoney(int inputMoney)
    {
        StartCoroutine(CountMoney(money, money - inputMoney));
        money -= inputMoney;
        StartCoroutine(InputMoneyAnim(Color.red, inputMoney));
    }

    private IEnumerator CountMoney(int preMoney, int goalMoney)
    {
        float timer = 0;
        int curMoney = preMoney;
        while (timer < 1)
        {
            timer += Time.deltaTime * 3;
            curMoney = (int)Mathf.Lerp(preMoney, goalMoney, timer);
            text.text = "Money: " + curMoney;

            yield return null;
        }
    }

    private IEnumerator InputMoneyAnim(Color inputColor, int inputMoney)
    {
        GameObject inputMoneyObj = new GameObject();
        Text inputText = inputMoneyObj.AddComponent<Text>();
        RectTransform inputRT = inputMoneyObj.GetComponent<RectTransform>();
        inputMoneyObj.transform.parent = transform;
        inputText.font = text.font;
        inputText.fontSize = text.fontSize;
        inputText.alignment = TextAnchor.MiddleRight;
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
            inputRT.anchoredPosition += Vector2.down / 2;
            inputColor.a = timer;
            inputText.color = inputColor;
            yield return null;
        }

        Destroy(inputMoneyObj);
    }

}
