using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ������ ü�¹ٸ� �����ϱ� ���� ��ũ��Ʈ
public class MonsterHPBar : MonoBehaviour
{
    public GameObject HPCanvas;             // ü�� �ٸ� ��� ���� ķ����
    public Slider slider;                   // ü�� �� �����̴�
    public GameObject sliderColor;          // ü�� �� ����
    public TextMeshProUGUI remainText;      // ���� ü���� ����� �ؽ�Ʈ 
    public TextMeshProUGUI totalText;       // ��ü ü���� ����� �ؽ�Ʈ
    public MonsterState state;              // ü�¹ٿ� ����� ����
    public GameObject damageUI;             // ������ ������ UI�� �ֱ� ���� ������Ʈ
    public MonsterHPChangeUI HPChangeUI;    // ���� �������� ����ϱ� ���� ��ũ��Ʈ
    public MonsterControl monsterControl;   // ������ ���¸� ������ �� �ִ� ��ũ��Ʈ
    public int currentHP;                   // ���� ü��

    private GameObject player;              // �÷��̾� ������Ʈ
    private Animator animator;              // ������ �ִϸ��̼�
    private MonsterDamageSound damageSound; // ���� ������ ���Ʈ
    private int maxHP;                      // �ִ� ü��

    private bool isDeath = false;           // ���� �� ���Ͱ� ����ߴ��� Ȯ���ϱ� ���� ����
    private bool isDestroy = false;         // ���� �� ���Ͱ� ���ŵǾ����� Ȯ���ϴ� ����

    public void Start()
    {
        player = GameObject.Find("Model");
        Debug.Assert(player != null, "Error (Null Reference) : �÷��̾ �������� �ʽ��ϴ�.");

        Debug.Assert(state != null, "Error (Null Reference) : ���� ���°� �������� �ʽ��ϴ�.");
        totalText.text = state.HP.ToString();
        maxHP = state.HP;                       // �ִ� ü�� ����
        currentHP = maxHP;                      // ���� ü�� �ʱ�ȭ

        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : �ش� ��ü�� �ִϸ����Ͱ� �������� �ʽ��ϴ�.");

        Debug.Assert(damageUI != null, "Error (Null Reference) : �������� ����� UI�� �������� �ʽ��ϴ�.");
        HPChangeUI = damageUI.GetComponent<MonsterHPChangeUI>();

        damageSound = GetComponent<MonsterDamageSound>();
        Debug.Assert(damageSound != null, "Error (Null Reference) : �Ҹ��� ����� ������Ʈ�� �������� �ʽ��ϴ�.");

        // �����̴� �ݿ�
        ApplyHp();
    }

    public void Update()
    {
        LookAtPlayer();
    }

    // ü�� �ٰ� �÷��̾ �ٶ󺸵��� ���ִ� �޼ҵ�
    private void LookAtPlayer()
    {
        Vector3 direction = player.transform.position - HPCanvas.transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        HPCanvas.transform.rotation = rotation;
    }

    // ���� ü���� �ݿ��� �޼ҵ�
    private void ApplyHp()
    {
        // Slider�� ���� ������� ���� ����
        Color color = Color.Lerp(Color.red, Color.green, slider.value);
        sliderColor.GetComponent<Image>().color = color;

        // ���� ü���� remainText�� ǥ��
        remainText.text = currentHP.ToString();

        // �����̴��� ���� ü�� ������ ǥ��
        float hpRatio = (float)currentHP / maxHP;
        slider.value = hpRatio;
    }

    // ���� �� ����Ǵ� �޼ҵ�
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

    // ü�� ���� �������� ���ҽ�Ű�� ���� �޼ҵ�
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