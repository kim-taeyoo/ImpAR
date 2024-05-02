using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Audio;

public class SettingManager : MonoBehaviour
{
    [SerializeField] private Sprite highSpeaker;
    [SerializeField] private Sprite middleSpeaker;
    [SerializeField] private Sprite lowSpeaker;
    [SerializeField] private Sprite muteSpeaker;
    public AudioMixer mixer;
        


    int stage = 1; //몇번째 스테이지인지
    //int sound = 10; //음량 크기 몇인지     PlayerPrefs 로 대체
    Text soundText;
    Text stageText;
    Image speaker;
    RectTransform settingUIs;
    RectTransform creditCard;


    private void Awake()
    {
        soundText = transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
        stageText = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        speaker = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Image>();
        settingUIs = transform.GetChild(0).GetComponent<RectTransform>();
        creditCard = settingUIs.GetChild(settingUIs.childCount - 1).GetChild(0).GetComponent <RectTransform>();
    }
    void Start()
    {
        if (PlayerPrefs.GetInt("Sound") == null)
        {
            PlayerPrefs.SetInt("Sound", 10);
        }
        mixer.SetFloat("MasterVolume", PlayerPrefs.GetInt("Sound") * 8 - 80);
        ChangeSpeakerImage();
        gameObject.SetActive(false);
    }

    public void ActiveSetting() //세팅창 활성화
    {
        gameObject.SetActive(true);
        stageText.text = "STAGE " + stage;
        soundText.text = "" + PlayerPrefs.GetInt("Sound");
        ChangeSpeakerImage();
        settingUIs.localScale = Vector3.zero;
        settingUIs.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(UpdateType.Normal,true);
        //Time.timeScale = 0;
    }

    public void InactiveSetting() //세팅창 비활성화
    {
        gameObject.SetActive(false);
        InactiveCredit();
        //Time.timeScale = 1;
    }

    public void ActiveCredit() //크레딧 활성화
    {
        creditCard.parent.gameObject.SetActive(true);
        creditCard.anchoredPosition = new Vector3(0, -1500, 0);
        creditCard.DOAnchorPos(Vector3.zero, 0.5f).SetEase(Ease.OutBack).SetUpdate(UpdateType.Normal, true);
    }
    
    public void InactiveCredit() //크레딧 비활성화
    {
       creditCard.parent.gameObject.SetActive(false);
    }

    public void SoundUp() //소리 1 올리기
    {
        PlayerPrefs.SetInt("Sound", PlayerPrefs.GetInt("Sound") + 1);
        if (PlayerPrefs.GetInt("Sound") > 10) PlayerPrefs.SetInt("Sound", 10);
        soundText.text = "" + PlayerPrefs.GetInt("Sound");
        ChangeSpeakerImage();
        mixer.SetFloat("MasterVolume", PlayerPrefs.GetInt("Sound") * 8 - 80);
    }
    public void SoundDown() //소리 1 낮추기
    {
        PlayerPrefs.SetInt("Sound", PlayerPrefs.GetInt("Sound") - 1);
        if (PlayerPrefs.GetInt("Sound") < 0) PlayerPrefs.SetInt("Sound", 0);
        soundText.text = "" + PlayerPrefs.GetInt("Sound");
        ChangeSpeakerImage();
        mixer.SetFloat("MasterVolume", PlayerPrefs.GetInt("Sound") * 8 - 80);
    }

    void ChangeSpeakerImage() //스피커 이미지 바꾸기
    {
        if (highSpeaker)
        {
            if (PlayerPrefs.GetInt("Sound") > 7)
            {
                speaker.sprite = highSpeaker;
            }
            else if (PlayerPrefs.GetInt("Sound") > 3)
            {
                speaker.sprite = middleSpeaker;
            }
            else if (PlayerPrefs.GetInt("Sound") > 0)
            {
                speaker.sprite = lowSpeaker;
            }
            else
            {
                speaker.sprite = muteSpeaker;
            }
        }
    }
}
