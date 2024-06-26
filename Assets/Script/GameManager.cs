using UnityEngine;
using UnityEngine.SceneManagement;

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
    public GameObject playerObj;            // 플레이어 오브젝트
    public Alert alert;                     // 알람 시스템

    // 씬
    public string beforeSceneName = "Prologue";     // 이전 씬 이름

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

        SceneManager.sceneLoaded += LevelOnLoad;
    }

    public void Update()
    {
        // 전체 플레이한 시간을 측정함
        playerState.playTotalTime += Time.deltaTime;
        
    }

    public void LevelOnLoad(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.Find("player(Clone)");

        if (player == null && playerObj != null && scene.name.Equals("Village"))
        {
            GameObject startLocation = GameObject.Find("StartLocation");
            player = Instantiate(playerObj, startLocation.transform);
            player.transform.SetParent(null);

            player.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(false);
        }

        if (scene.name == "Desert" && PlayerQuest.quest.nowQuest == "visitDesert")
        {
            PlayerQuest.quest.NextQuest();
            alert.PushAlert("\"첫발을 디딘 모험\" 퀘스트 클리어!", true);
        }

        else if (scene.name == "Forest" && PlayerQuest.quest.nowQuest == "visitForest")
        {
            PlayerQuest.quest.NextQuest();

            if (alert == null)
                return;

            alert.PushAlert("\"신선한 산책\" 퀘스트 클리어!", true);
        }
    }
}
