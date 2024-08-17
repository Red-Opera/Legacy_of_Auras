using TMPro;
using UnityEngine;

public class LevelExp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelTextInspector;    // 현재 레벨을 출력하는 텍스트
    [SerializeField] private PlayerState upToLevelInspector;        // 1레벨 당 증가치
    [SerializeField] private QuestRepeatFrame questRepeatFrame;     // Level Up 퀘스트 스크립트
    
    private static TextMeshProUGUI levelText;       // 현재 레벨을 출력하는 텍스트
    private static int upToLevelExp;                // 1레벨 당 필요 경험치 증가량
    private static int levelToMaxHP;                // 1레벨 당 증가하는 최대 체력 량

    private static PlayerState playerState;         // 플레이어 상태
    private static PlayerState upToLevel;           // 1레벨 당 증가치
    private static PlayerHPBar hpBar;               // 플레이어 체력바
    private static QuestRepeatFrame repeatFrame;    // Level Up 퀘스트 스크립트 static 버전

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

        Debug.Assert(levelText != null, "오류 (Null Reference): 현재 레벨을 출력할 텍스트가 존재하지 않습니다.");
        Debug.Assert(GameManager.info.playerState != null, "오류 (Null Reference): 현재 플레이어 상태가 존재하지 않습니다.");
        Debug.Assert(questRepeatFrame != null, "오류 (Null Reference): Level 퀘스트가 존재하지 않습니다.");
    
        hpBar = GetComponent<PlayerHPBar>();
        Debug.Assert(hpBar != null, "오류 (Null Reference): 체력 바가 존재하지 않습니다.");

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

    // 경험치에 따라서 레벨업
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
        // 증가한 레벨에 따라 플레이어 체력 설정
        playerState.HP = 100 + (playerState.Level - 1) * levelToMaxHP;

        // 최대 체력을 최대로 설정하고 현재 체력도 최대로 설정
        hpBar.maxHp.text = playerState.HP.ToString();
        hpBar.currentHp.text = playerState.HP.ToString();
    }

    // 경험치를 얻는 메소드
    public static void GetExp(int exp)
    {
        int nextExp = playerState.exp + exp;

        while (nextExp >= playerState.Level * upToLevelExp)
            UpLevelFromExp(ref nextExp);

        repeatFrame.UpdateQuest();

        playerState.exp = nextExp;
    }

    // 레벨을 얻는 메소드
    public void UpLevel(int level)
    {
        playerState.Level += level;
        levelText.text = playerState.Level.ToString();

        playerState.Add(upToLevel);
        HPBarUpdate();
    }
}