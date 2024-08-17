using TMPro;
using UnityEngine;

public class LevelExp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelTextInspector;    // ���� ������ ����ϴ� �ؽ�Ʈ
    [SerializeField] private PlayerState upToLevelInspector;        // 1���� �� ����ġ
    [SerializeField] private QuestRepeatFrame questRepeatFrame;     // Level Up ����Ʈ ��ũ��Ʈ
    
    private static TextMeshProUGUI levelText;       // ���� ������ ����ϴ� �ؽ�Ʈ
    private static int upToLevelExp;                // 1���� �� �ʿ� ����ġ ������
    private static int levelToMaxHP;                // 1���� �� �����ϴ� �ִ� ü�� ��

    private static PlayerState playerState;         // �÷��̾� ����
    private static PlayerState upToLevel;           // 1���� �� ����ġ
    private static PlayerHPBar hpBar;               // �÷��̾� ü�¹�
    private static QuestRepeatFrame repeatFrame;    // Level Up ����Ʈ ��ũ��Ʈ static ����

    private bool isOn = true;

    public void Start()
    {
        if (levelText == null)
        {
            levelText = levelTextInspector;
            upToLevelExp = upToLevelInspector.exp;
            levelToMaxHP = upToLevelInspector.HP;
            upToLevel = upToLevelInspector;
        }

        Debug.Assert(levelText != null, "���� (Null Reference): ���� ������ ����� �ؽ�Ʈ�� �������� �ʽ��ϴ�.");
        Debug.Assert(GameManager.info.playerState != null, "���� (Null Reference): ���� �÷��̾� ���°� �������� �ʽ��ϴ�.");
        Debug.Assert(questRepeatFrame != null, "���� (Null Reference): Level ����Ʈ�� �������� �ʽ��ϴ�.");
    
        hpBar = GetComponent<PlayerHPBar>();
        Debug.Assert(hpBar != null, "���� (Null Reference): ü�� �ٰ� �������� �ʽ��ϴ�.");

        repeatFrame = questRepeatFrame;

        playerState = GameManager.info.playerState;
        levelText.text = playerState.Level.ToString();
    }

    private void Update()
    {
        if (isOn && !BossSceneFilm.isFilmEnd)
        {
            isOn = false;

            levelTextInspector.enabled = false;
        }

        else if (!isOn && BossSceneFilm.isFilmEnd)
        {
            isOn = true;

            levelTextInspector.enabled = true;
        }
    }

    // ����ġ�� ���� ������
    private static void UpLevelFromExp(ref int nextExp)
    {
        nextExp -= playerState.Level * upToLevelExp;

        playerState.Level++;
        levelText.text = playerState.Level.ToString();

        playerState.Add(upToLevel);
        HPBarUpdate();
    }

    private static void HPBarUpdate()
    {
        // ������ ������ ���� �÷��̾� ü�� ����
        playerState.HP = 100 + (playerState.Level - 1) * levelToMaxHP;

        // �ִ� ü���� �ִ�� �����ϰ� ���� ü�µ� �ִ�� ����
        hpBar.maxHp.text = playerState.HP.ToString();
        hpBar.currentHp.text = playerState.HP.ToString();
    }

    // ����ġ�� ��� �޼ҵ�
    public static void GetExp(int exp)
    {
        int nextExp = playerState.exp + exp;

        while (nextExp >= playerState.Level * upToLevelExp)
            UpLevelFromExp(ref nextExp);

        repeatFrame.UpdateQuest();

        playerState.exp = nextExp;
    }

    // ������ ��� �޼ҵ�
    public void UpLevel(int level)
    {
        playerState.Level += level;
        levelText.text = playerState.Level.ToString();

        playerState.Add(upToLevel);
        HPBarUpdate();
    }
}