using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager info { get { return gamemanager; } }
    private static GameManager gamemanager;

    // ���콺 ����
    public float mouseSensitivityX = 2.0f;  // x�� ����
    public float mouseSensitivityY = 2.0f;  // y�� ����

    // �Ҹ�
    public float musicVolume = 1.0f;        // ���� �Ҹ�
    public float soundVolume = 1.0f;        // ȿ�� �Ҹ�

    // ��üȭ��
    public bool isFullScreen = false;       // ��üȭ�� ����

    // �÷��̾�
    public PlayerState playerState;         // �÷��̾� ����
    public GameObject playerObj;            // �÷��̾� ������Ʈ
    public Alert alert;                     // �˶� �ý���

    // ��
    public string beforeSceneName = "Prologue";     // ���� �� �̸�

    public void Awake()
    {
        if (gamemanager == null)
        {
            gamemanager = this;
            DontDestroyOnLoad(this);
        }

        else
            Destroy(this);

        Cursor.visible = false;                     // Ŀ���� ����
        Cursor.lockState = CursorLockMode.Locked;   // Ŀ���� �������� �ʵ��� ����

        GameObject player = GameObject.Find("player");

        if (player != null)
            player.transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(false);

        SceneManager.sceneLoaded += LevelOnLoad;
    }

    public void Update()
    {
        // ��ü �÷����� �ð��� ������
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
            alert.PushAlert("\"ù���� ��� ����\" ����Ʈ Ŭ����!", true);
        }

        else if (scene.name == "Forest" && PlayerQuest.quest.nowQuest == "visitForest")
        {
            PlayerQuest.quest.NextQuest();

            if (alert == null)
                return;

            alert.PushAlert("\"�ż��� ��å\" ����Ʈ Ŭ����!", true);
        }
    }
}
