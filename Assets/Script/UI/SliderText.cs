using System;
using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour
{
    public GameObject t;        // ���¸� ����� �ؽ�Ʈ�� ������Ʈ

    private Slider slider;      // �����̴�
    private Text text;          // ���¸� ����� �ؽ�Ʈ

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

        // X�� ���� �����̴��� ���
        if (gameObject.name == "SensitivityXSlider")
            GameManager.info.mouseSensitivityX = toValue;

        // Y�� ���� �����̴��� ���
        else if (gameObject.name == "SensitivityYSlider")
            GameManager.info.mouseSensitivityY = toValue;

        // ���� �Ҹ� ũ�� �����̴��� ���
        else if (gameObject.name == "MusicSlider")
            GameManager.info.musicVolume = toValue;

        // ���� �Ҹ� ũ�� �����̴��� ���
        else if (gameObject.name == "SoundSlider")
            GameManager.info.soundVolume = toValue;

        // �Ҽ��� 1��°���� �ؽ�Ʈ ���
        text.text = Math.Round(toValue, 1).ToString();
    }
}
