using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHPBar : MonoBehaviour
{
    public static TextMeshProUGUI currentHpStatic;
    public static TextMeshProUGUI maxHPStatic;

    public static bool isPlayerDeath = false;                   // 플레이어가 죽었는지 여부
    public static bool isPlayerRevive = false;                  // 플레이어가 소생 가능한지 여부

    public TextMeshProUGUI currentHp;                           // 플레이어 현재 체력
    public TextMeshProUGUI maxHp;                               // 플레이어 최대 체력

    [SerializeField] private PlayerState state;                 // 플레이어 현재 상태
    [SerializeField] private PlayerState defualtState;          // 플레이어 기본 스탯
    [SerializeField] private PlayerState levelUpState;          // 플레이어 레벨 업당 증가하는 값
    [SerializeField] private Slider currentUI;                  // 플레이어 체력바
    [SerializeField] private VolumeProfile volume;              // 포스트 프로세스 볼륨
    [SerializeField] private FadeInOutEffect fadeInOutEffect;   // 페이트 인 아웃 효과

    [SerializeField] private float minFilmGrain = 0.0f;
    [SerializeField] private float maxFilmGrain = 0.6f;

    [SerializeField] private float minVignette = 0.0f;
    [SerializeField] private float maxVignette = 0.4f;

    [SerializeField] private float saturationTime = 1.5f;
    private float defaultColorAdjust = 0.0f;

    private static Animator animator;                   // 플레이어 애니메이터
    private static PlayerHPBar playerHPBar;             // 플레이어 체력바 인스턴스
    private static float noDamageTime = 2.0f;           // 피해를 입었을 경우 다음 피해까지 걸리는 시간
    private static bool isDamageable = true;            // 현재 데미지를 받을 수 있는 경우
    private static bool isBlackAndWhite = false;

    private FilmGrain grain;
    private Vignette vignette;
    private ColorAdjustments colorAdjust;

    private void Start()
    {
        Debug.Assert(currentHp != null, "Error (Null Reference) : 플레이어 현재 체력이 존재하지 않습니다.");
        Debug.Assert(maxHp != null, "Error (Null Reference) : 플레이어 최대 체력이 존재하지 않습니다.");
        Debug.Assert(currentUI != null, "Error (Null Reference) : 플레이어 현재 체력바가 존재하지 않습니다.");
        Debug.Assert(volume != null, "Error (Null Reference) : 포스트 프로세스를 실행할 볼륨이 존재하지 않습니다.");

        volume.TryGet<FilmGrain>(out grain);
        volume.TryGet<Vignette>(out vignette);
        volume.TryGet<ColorAdjustments>(out colorAdjust);

        Debug.Assert(grain != null, "Error (Null Reference) : FileGrain이 존재하지 않습니다.");
        Debug.Assert(vignette != null, "Error (Null Reference) : Vignette가 존재하지 않습니다.");
        Debug.Assert(colorAdjust != null, "Error (Null Reference) : ColorCurves가 존재하지 않습니다.");

        grain.intensity.value = 0.0f;
        vignette.intensity.value = 0.0f;
        colorAdjust.saturation.value = 0.0f;
        colorAdjust.active = false;

        currentHp.text = state.HP.ToString("#,##0");
        maxHp.text = (defualtState.HP + (state.Level - 1) * levelUpState.HP).ToString("#,##0");

        animator = transform.parent.GetChild(0).GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : 플레이어 애니메이터가 존재하지 않습니다.");

        currentHpStatic = currentHp;
        maxHPStatic = maxHp;

        playerHPBar = this;
    }

    private void Update()
    {
        float persent = (float)int.Parse(currentHp.text.Replace(",", "")) / int.Parse(maxHp.text.Replace(",", ""));

        currentUI.value = persent;

        grain.intensity.value = Mathf.Lerp(maxFilmGrain, minFilmGrain, persent);
        vignette.intensity.value = Mathf.Lerp(maxVignette, minVignette, persent);

        if (isPlayerDeath)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetMouseButtonDown(0))
                StartCoroutine(Revive());
        }
    }

    private IEnumerator Revive()
    {
        isPlayerDeath = false;

        StartCoroutine(fadeInOutEffect.FadeOut(1.0f));

        yield return new WaitForSeconds(1.5f);

        colorAdjust.saturation.value = 0.0f;
        colorAdjust.active = false;
        currentHp.text = maxHp.text;
        state.HP = int.Parse(currentHp.text);

        animator.SetTrigger("Reset");

        GameManager.info.beforeSceneName = "Prologue";
        SceneManager.LoadScene("Village");
    }

    private IEnumerator DeathEffect()
    {
        float currentTime = 0.0f;
        defaultColorAdjust = colorAdjust.saturation.value;
        colorAdjust.active = true;

        // 점점 회색 빛으로 변하는 효과
        while (currentTime <= saturationTime)
        {
            currentTime += Time.deltaTime;
            float satuation = Mathf.Lerp(0, colorAdjust.saturation.min, currentTime / saturationTime);

            colorAdjust.saturation.value = satuation;

            yield return null;
        }

        isPlayerRevive = true;
    }

    private void PlayerDeath()
    {
        animator.SetTrigger("Death");

        StartCoroutine(DeathEffect());
    }

    // 일정한 시간이 지나고 데미지를 줄 수 있도록 해주는 메소드
    private static IEnumerator DamageDelay()
    {
        float currentDelay = 0.0f;
        while (currentDelay <= noDamageTime)
        {
            currentDelay += Time.deltaTime;

            yield return null;
        }

        isDamageable = true;
    }

    public static void SetDamage(int damage)
    {
        // 플레이어에게 데미지를 줄 수 없는 경우
        if (!isDamageable)
            return;

        // 바로 대미지를 줄 수 없게 설정
        isDamageable = false;

        int hp = Int32.Parse(playerHPBar.currentHp.text.Replace(",", ""));
        playerHPBar.state.HP = hp;

        if (damage >= hp)
        {
            playerHPBar.currentHp.SetText("0");
            isPlayerDeath = true;

            playerHPBar.PlayerDeath();
        }

        else
            playerHPBar.currentHp.SetText((hp - damage).ToString());

        // 플레이어가 죽을 시 데미지 줄 수 있도록 대기시킴
        if (hp - damage > 0)
            playerHPBar.StartCoroutine(DamageDelay());
    }

    public bool Heal(int heal)
    {
        int currentHP = int.Parse(currentHp.text.Replace(",", ""));
        int maxHP = int.Parse(maxHp.text.Replace(",", ""));

        if (currentHP >= maxHP)
            return false;

        int resultHP = currentHP + heal;
        playerHPBar.state.HP = resultHP;

        if (resultHP >= maxHP)
            currentHp.text = maxHP.ToString();

        else
            currentHp.text = resultHP.ToString();

        return true;
    }

    private void OnApplicationQuit()
    {
        grain.intensity.value = 0.0f;
        vignette.intensity.value = 0.0f;
        colorAdjust.saturation.value = defaultColorAdjust;
        colorAdjust.active = false;
    }
}
