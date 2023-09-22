using System;
using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour
{
    public GameObject t;        // 상태를 출력할 텍스트의 오브젝트

    private Slider slider;      // 슬라이더
    private Text text;          // 상태를 출력할 텍스트

    public void OnEnable()
    {
        slider = GetComponent<Slider>();
        text = t.GetComponent<Text>();
    }

    public void ChangeSlider()
    {
        if (slider == null)
            return;

        float toValue = slider.value;

        // X축 감도 슬라이더인 경우
        if (gameObject.name == "SensitivityXSlider")
            GameManager.info.mouseSensitivityX = toValue;

        // Y축 감도 슬라이더인 경우
        else if (gameObject.name == "SensitivityYSlider")
            GameManager.info.mouseSensitivityY = toValue;

        // 음악 소리 크기 슬라이더인 경우
        else if (gameObject.name == "MusicSlider")
            GameManager.info.musicVolume = toValue;

        // 음향 소리 크기 슬라이더인 경우
        else if (gameObject.name == "SoundSlider")
            GameManager.info.soundVolume = toValue;

        // 소수점 1번째까지 텍스트 출력
        text.text = Math.Round(toValue, 1).ToString();
    }
}
