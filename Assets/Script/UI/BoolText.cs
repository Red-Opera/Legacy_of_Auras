using TMPro;
using UnityEngine;

public class BoolText : MonoBehaviour
{
    public GameObject t;            // �ؽ�Ʈ�� ������ �ִ� ������Ʈ

    private TextMeshProUGUI text;   // �ؽ�Ʈ ������Ʈ

    public void Start()
    {
        text = t.GetComponent<TextMeshProUGUI>();
    }


    public void BoolChanger()
    {
        // ��üȭ�� ��ư�� ���� ���
        if (gameObject.name == "FullScreenButton")
        {
            // ��ü ȭ������ ����
            GameManager.info.isFullScreen = !GameManager.info.isFullScreen;
            Screen.fullScreen = !Screen.fullScreen;
        }

        // off�� ��� on���� ����
        if (text.text == "off")
            text.text = "on";

        else
            text.text = "off";
    }
}
