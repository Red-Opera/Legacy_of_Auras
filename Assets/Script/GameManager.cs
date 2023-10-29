using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager info { get { return gamemanager; } }
    private static GameManager gamemanager;

    // 마우스 감도
    public float mouseSensitivityX = 2.0f;  // x축 기준
    public float mouseSensitivityY = 2.0f;  // y축 기준

    // 소리
    public float musicVolume = 1.0f;        // 음악 소리
    public float soundVolume = 1.0f;        // 효과 소리

    // 전체화면
    public bool isFullScreen = false;       // 전체화면 여부

    // 플레이어
    public PlayerState playerState;         // 플레이어 상태

    public void Awake()
    {
        if (gamemanager == null)
        {
            gamemanager = this;
            DontDestroyOnLoad(this);
        }

        else
            Destroy(this);

        Cursor.visible = false;                     // 커서를 감춤
        Cursor.lockState = CursorLockMode.Locked;   // 커서가 움직이지 않도록 설정

        GameObject player = GameObject.Find("player");

        if (player != null)
            player.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(false);
    }

    public void Update()
    {
        
    }
}
