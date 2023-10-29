using UnityEngine;

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
    }

    public void Update()
    {
        
    }
}
