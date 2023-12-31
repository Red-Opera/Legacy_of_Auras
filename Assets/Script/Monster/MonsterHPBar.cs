using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 몬스터의 체력바를 제어하기 위한 스크립트
public class MonsterHPBar : MonoBehaviour
{
    public GameObject HPCanvas;             // 체력 바를 담기 위한 캠버스
    public Slider slider;                   // 체력 바 슬라이더
    public GameObject sliderColor;          // 체력 바 색깔
    public TextMeshProUGUI remainText;      // 남은 체력을 출력할 텍스트 
    public TextMeshProUGUI totalText;       // 전체 체력을 출력할 텍스트
    public MonsterState state;              // 체력바에 출력할 정보
    public GameObject damageUI;             // 몬스터의 데미지 UI를 넣기 위한 오브젝트
    public MonsterHPChangeUI HPChangeUI;    // 몬스터 데미지를 출력하기 위한 스크립트
    public MonsterControl monsterControl;   // 몬스터의 상태를 제어할 수 있는 스크립트
    public int currentHP;                   // 현재 체력

    private GameObject player;              // 플레이어 오브젝트
    private Animator animator;              // 몬스터의 애니메이션
    private MonsterDamageSound damageSound; // 몬스터 데미지 사운트
    private int maxHP;                      // 최대 체력

    private bool isDeath = false;           // 현재 이 몬스터가 사망했는지 확인하기 위한 변수
    private bool isDestroy = false;         // 현재 이 몬스터가 제거되었는지 확인하는 변수

    public void Start()
    {
        player = GameObject.Find("Model");
        Debug.Assert(player != null, "Error (Null Reference) : 플레이어가 존재하지 않습니다.");

        Debug.Assert(state != null, "Error (Null Reference) : 몬스터 상태가 존재하지 않습니다.");
        totalText.text = state.HP.ToString();
        maxHP = state.HP;                       // 최대 체력 설정
        currentHP = maxHP;                      // 현재 체력 초기화

        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : 해당 객체에 애니메이터가 존재하지 않습니다.");

        Debug.Assert(damageUI != null, "Error (Null Reference) : 데미지를 출력할 UI가 존재하지 않습니다.");
        HPChangeUI = damageUI.GetComponent<MonsterHPChangeUI>();

        damageSound = GetComponent<MonsterDamageSound>();
        Debug.Assert(damageSound != null, "Error (Null Reference) : 소리를 출력할 컴포넌트가 존재하지 않습니다.");

        // 슬라이더 반영
        ApplyHp();
    }

    public void Update()
    {
        LookAtPlayer();
    }

    // 체력 바가 플레이어를 바라보도록 해주는 메소드
    private void LookAtPlayer()
    {
        Vector3 direction = player.transform.position - HPCanvas.transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        HPCanvas.transform.rotation = rotation;
    }

    // 현재 체력을 반영한 메소드
    private void ApplyHp()
    {
        // Slider의 값을 기반으로 색상 변경
        Color color = Color.Lerp(Color.red, Color.green, slider.value);
        sliderColor.GetComponent<Image>().color = color;

        // 현재 체력을 remainText에 표시
        remainText.text = currentHP.ToString();

        // 슬라이더에 현재 체력 비율을 표시
        float hpRatio = (float)currentHP / maxHP;
        slider.value = hpRatio;
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
                Destroy(gameObject, 5.0f);
                isDestroy = true;
            }

            yield return null;
        }
    }

    // 체력 바의 데미지를 감소시키기 위한 메소드
    public void SetDamage(int damage)
    {
        if (isDeath)
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
            StartCoroutine(monsterControl.IdleRotationCoroutine(true));
            currentHP -= realDamage;

            damageSound.DamageSound(realDamage);
        }

        else
        {
            currentHP = 0;
            StartCoroutine(Death());
        }

        ApplyHp();
    }
}