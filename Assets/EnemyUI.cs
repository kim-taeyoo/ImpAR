using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    Slider hpSlider;
    private int hp;

    void Start()
    {
        hpSlider = transform.GetChild(0).GetComponent<Slider>();
        hp = 100;
        hpSlider.maxValue = hp;
        hpSlider.value = hp;
    }

    void Update()
    {
        transform.LookAt(Camera.main.transform);
        if (Input.GetKeyDown(KeyCode.O))
        {
            GetDamage(20);
        }
    }

    public void GetDamage(int damage)
    {
        hp -= damage;
        if (hp < 0) hp = 0;
        hpSlider.value = hp;
    }
}
