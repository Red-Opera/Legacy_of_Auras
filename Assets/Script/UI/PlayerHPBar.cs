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

    public static bool isPlayerDeath = false;                   // �÷��̾ �׾����� ����
    public static bool isPlayerRevive = false;                  // �÷��̾ �һ� �������� ����

    public TextMeshProUGUI currentHp;                           // �÷��̾� ���� ü��
    public TextMeshProUGUI maxHp;                               // �÷��̾� �ִ� ü��

    [SerializeField] private PlayerState state;                 // �÷��̾� ���� ����
    [SerializeField] private PlayerState defualtState;          // �÷��̾� �⺻ ����
    [SerializeField] private PlayerState levelUpState;          // �÷��̾� ���� ���� �����ϴ� ��
    [SerializeField] private Slider currentUI;                  // �÷��̾� ü�¹�
    [SerializeField] private VolumeProfile volume;              // ����Ʈ ���μ��� ����
    [SerializeField] private FadeInOutEffect fadeInOutEffect;   // ����Ʈ �� �ƿ� ȿ��

    [SerializeField] private float minFilmGrain = 0.0f;
    [SerializeField] private float maxFilmGrain = 0.6f;

    [SerializeField] private float minVignette = 0.0f;
    [SerializeField] private float maxVignette = 0.4f;

    [SerializeField] private float saturationTime = 1.5f;
    private float defaultColorAdjust = 0.0f;

    private static Animator animator;                   // �÷��̾� �ִϸ�����
    private static PlayerHPBar playerHPBar;             // �÷��̾� ü�¹� �ν��Ͻ�
    private static float noDamageTime = 2.0f;           // ���ظ� �Ծ��� ��� ���� ���ر��� �ɸ��� �ð�
    private static bool isDamageable = true;            // ���� �������� ���� �� �ִ� ���
    private static bool isBlackAndWhite = false;

    private FilmGrain grain;
    private Vignette vignette;
    private ColorAdjustments colorAdjust;

    private void Start()
    {
        Debug.Assert(currentHp != null, "Error (Null Reference) : �÷��̾� ���� ü���� �������� �ʽ��ϴ�.");
        Debug.Assert(maxHp != null, "Error (Null Reference) : �÷��̾� �ִ� ü���� �������� �ʽ��ϴ�.");
        Debug.Assert(currentUI != null, "Error (Null Reference) : �÷��̾� ���� ü�¹ٰ� �������� �ʽ��ϴ�.");
        Debug.Assert(volume != null, "Error (Null Reference) : ����Ʈ ���μ����� ������ ������ �������� �ʽ��ϴ�.");

        volume.TryGet<FilmGrain>(out grain);
        volume.TryGet<Vignette>(out vignette);
        volume.TryGet<ColorAdjustments>(out colorAdjust);

        Debug.Assert(grain != null, "Error (Null Reference) : FileGrain�� �������� �ʽ��ϴ�.");
        Debug.Assert(vignette != null, "Error (Null Reference) : Vignette�� �������� �ʽ��ϴ�.");
        Debug.Assert(colorAdjust != null, "Error (Null Reference) : ColorCurves�� �������� �ʽ��ϴ�.");

        grain.intensity.value = 0.0f;
        vignette.intensity.value = 0.0f;
        colorAdjust.saturation.value = 0.0f;
        colorAdjust.active = false;

        currentHp.text = state.HP.ToString("#,##0");
        maxHp.text = (defualtState.HP + (state.Level - 1) * levelUpState.HP).ToString("#,##0");

        animator = transform.parent.GetChild(0).GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : �÷��̾� �ִϸ����Ͱ� �������� �ʽ��ϴ�.");

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

        // ���� ȸ�� ������ ���ϴ� ȿ��
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

    // ������ �ð��� ������ �������� �� �� �ֵ��� ���ִ� �޼ҵ�
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
        // �÷��̾�� �������� �� �� ���� ���
        if (!isDamageable)
            return;

        // �ٷ� ������� �� �� ���� ����
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

        // �÷��̾ ���� �� ������ �� �� �ֵ��� ����Ŵ
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
