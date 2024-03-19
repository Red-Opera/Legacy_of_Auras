using System.Collections;
using TMPro;
using UnityEngine;

public class LassBossHpBar : MonoBehaviour
{
    public GameObject slider;               // 체력 바 슬라이더
    public GameObject sliderColor;          // 체력 바 색깔
    public TextMeshPro remainText;          // 남은 체력을 출력할 텍스트 
    public TextMeshPro totalText;           // 전체 체력을 출력할 텍스트
    public MonsterState state;              // 체력바에 출력할 정보
    public GameObject damageUI;             // 몬스터의 데미지 UI를 넣기 위한 오브젝트
    public MonsterHPChangeUI HPChangeUI;    // 몬스터 데미지를 출력하기 위한 스크립트
    public PlayerState playerState;         // 플레이어 상태
    public int currentHP;                   // 현재 체력
    public int maxHP;                       // 최대 체력

    [SerializeField] private GameObject bossObject; // 최종 보스 오브젝트
    [SerializeField] private GameObject shield;     // 쉴드 오브젝트

    private GameObject player;              // 플레이어 오브젝트
    private Animator animator;              // 몬스터의 애니메이션
    private MonsterDamageSound damageSound; // 몬스터 데미지 사운트
    private MonsterDropItem dropItem;       // 몬스터 드랍 아이템 스크립트
    private Coroutine damaged;              // 데미지 감소하는 코루틴

    private bool isDeath = false;           // 현재 이 몬스터가 사망했는지 확인하기 위한 변수
    private bool isDestroy = false;         // 현재 이 몬스터가 제거되었는지 확인하는 변수

    public void Start()
    {
        player = GameObject.Find("Model");
        Debug.Assert(player != null, "Error (Null Reference) : 플레이어가 존재하지 않습니다.");

        Debug.Assert(state != null, "Error (Null Reference) : 몬스터 상태가 존재하지 않습니다.");
        totalText.text = state.HP.ToString("#,##0");
        maxHP = state.HP;                       // 최대 체력 설정
        currentHP = maxHP;                      // 현재 체력 초기화

        animator = bossObject.GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : 해당 객체에 애니메이터가 존재하지 않습니다.");

        Debug.Assert(damageUI != null, "Error (Null Reference) : 데미지를 출력할 UI가 존재하지 않습니다.");
        HPChangeUI = damageUI.GetComponent<MonsterHPChangeUI>();

        damageSound = bossObject.GetComponent<MonsterDamageSound>();
        Debug.Assert(damageSound != null, "Error (Null Reference) : 소리를 출력할 컴포넌트가 존재하지 않습니다.");

        dropItem = bossObject.GetComponent<MonsterDropItem>();
        Debug.Assert(dropItem != null, "Error (Null Reference) : 아이템 드랍 컴포넌트가 존재하지 않습니다.");

        Debug.Assert(playerState != null, "Error (Null Reference): 플레이어 상태가 존재하지 않습니다.");
        Debug.Assert(shield != null, "Error (Null Reference): 쉴드가 존재하지 않습니다.");

        // 슬라이더 반영
        ApplyHp();
    }

    // 현재 체력을 반영한 메소드
    private IEnumerator ApplyHp()
    {
        int startHP = int.Parse(remainText.text.Replace(",", ""));

        float elapsedTime = 0f;
        float duration = 0.5f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            int current = (int)Mathf.Lerp(startHP, currentHP, t);

            // 현재 체력을 remainText에 표시
            remainText.text = current.ToString("#,##0");

            // Slider의 값을 기반으로 색상 변경
            Color color = Color.Lerp(Color.red, Color.green, current / maxHP);
            sliderColor.GetComponent<MeshRenderer>().material.color = color;

            // 슬라이더에 현재 체력 비율을 표시
            float hpRatio = (float)current / maxHP;
            slider.transform.localScale = new Vector3(hpRatio, slider.transform.localScale.y, slider.transform.localScale.z);

            yield return null;
        }
    }

    // 죽을 때 실행되는 메소드
    private IEnumerator Death()
    {
        animator.SetTrigger("IsDeath");
        isDeath = true;

        while (true)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95 && !isDestroy)
            {
                dropItem.DropItem();
                playerState.kills++;

                isDestroy = true;
            }

            yield return null;
        }
    }

    // 체력 바의 데미지를 감소시키기 위한 메소드
    public void SetDamage(int damage)
    {
        if (isDeath || shield.activeSelf)
            return;

        int realDamage = GameManager.info.playerState.attack + damage - state.def;
        StartCoroutine(HPChangeUI.CreateChangeText(true, realDamage));

        if (realDamage <= 0)
        {
            damageSound.DamageSound(realDamage);
            return;
        }

        if (currentHP - realDamage > 0)
        {
            currentHP -= realDamage;

            damageSound.DamageSound(realDamage);
        }

        else
        {
            currentHP = 0;
            StartCoroutine(Death());
        }
        
        if (damaged != null)
        {
            StopCoroutine(damaged);
            damaged = null;
        }
        
        damaged = StartCoroutine(ApplyHp());
    }

    // 체력 회복할 때 발생되는 메소드
    public void Heal(int value)
    {
        if (isDeath)
            return;

        // 체력 변경 UI 생성
        StartCoroutine(HPChangeUI.CreateChangeText(false, value));

        currentHP += value;
        if (currentHP > maxHP)
            currentHP = maxHP;

        if (damaged != null)
        {
            StopCoroutine(damaged);
            damaged = null;
        }

        damaged = StartCoroutine(ApplyHp());
    }
}