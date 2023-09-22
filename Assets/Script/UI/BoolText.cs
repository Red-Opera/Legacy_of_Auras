using TMPro;
using UnityEngine;

public class BoolText : MonoBehaviour
{
    public GameObject t;            // 텍스트를 가지고 있는 오브젝트

    private TextMeshProUGUI text;   // 텍스트 컴포넌트

    public void Start()
    {
        text = t.GetComponent<TextMeshProUGUI>();
    }


    public void BoolChanger()
    {
        // 전체화면 버튼을 누른 경우
        if (gameObject.name == "FullScreenButton")
        {
            // 전체 화면으로 설정
            GameManager.info.isFullScreen = !GameManager.info.isFullScreen;
            Screen.fullScreen = !Screen.fullScreen;
        }

        // off인 경우 on으로 변경
        if (text.text == "off")
            text.text = "on";

        else
            text.text = "off";
    }
}
