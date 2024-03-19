using System.Collections;
using TMPro;
using UnityEngine;

public class LassBossHpBar : MonoBehaviour
{
    public GameObject slider;               // ü�� �� �����̴�
    public GameObject sliderColor;          // ü�� �� ����
    public TextMeshPro remainText;          // ���� ü���� ����� �ؽ�Ʈ 
    public TextMeshPro totalText;           // ��ü ü���� ����� �ؽ�Ʈ
    public MonsterState state;              // ü�¹ٿ� ����� ����
    public GameObject damageUI;             // ������ ������ UI�� �ֱ� ���� ������Ʈ
    public MonsterHPChangeUI HPChangeUI;    // ���� �������� ����ϱ� ���� ��ũ��Ʈ
    public PlayerState playerState;         // �÷��̾� ����
    public int currentHP;                   // ���� ü��
    public int maxHP;                       // �ִ� ü��

    [SerializeField] private GameObject bossObject; // ���� ���� ������Ʈ
    [SerializeField] private GameObject shield;     // ���� ������Ʈ

    private GameObject player;              // �÷��̾� ������Ʈ
    private Animator animator;              // ������ �ִϸ��̼�
    private MonsterDamageSound damageSound; // ���� ������ ���Ʈ
    private MonsterDropItem dropItem;       // ���� ��� ������ ��ũ��Ʈ
    private Coroutine damaged;              // ������ �����ϴ� �ڷ�ƾ

    private bool isDeath = false;           // ���� �� ���Ͱ� ����ߴ��� Ȯ���ϱ� ���� ����
    private bool isDestroy = false;         // ���� �� ���Ͱ� ���ŵǾ����� Ȯ���ϴ� ����

    public void Start()
    {
        player = GameObject.Find("Model");
        Debug.Assert(player != null, "Error (Null Reference) : �÷��̾ �������� �ʽ��ϴ�.");

        Debug.Assert(state != null, "Error (Null Reference) : ���� ���°� �������� �ʽ��ϴ�.");
        totalText.text = state.HP.ToString("#,##0");
        maxHP = state.HP;                       // �ִ� ü�� ����
        currentHP = maxHP;                      // ���� ü�� �ʱ�ȭ

        animator = bossObject.GetComponent<Animator>();
        Debug.Assert(animator != null, "Error (Null Reference) : �ش� ��ü�� �ִϸ����Ͱ� �������� �ʽ��ϴ�.");

        Debug.Assert(damageUI != null, "Error (Null Reference) : �������� ����� UI�� �������� �ʽ��ϴ�.");
        HPChangeUI = damageUI.GetComponent<MonsterHPChangeUI>();

        damageSound = bossObject.GetComponent<MonsterDamageSound>();
        Debug.Assert(damageSound != null, "Error (Null Reference) : �Ҹ��� ����� ������Ʈ�� �������� �ʽ��ϴ�.");

        dropItem = bossObject.GetComponent<MonsterDropItem>();
        Debug.Assert(dropItem != null, "Error (Null Reference) : ������ ��� ������Ʈ�� �������� �ʽ��ϴ�.");

        Debug.Assert(playerState != null, "Error (Null Reference): �÷��̾� ���°� �������� �ʽ��ϴ�.");
        Debug.Assert(shield != null, "Error (Null Reference): ���尡 �������� �ʽ��ϴ�.");

        // �����̴� �ݿ�
        ApplyHp();
    }

    // ���� ü���� �ݿ��� �޼ҵ�
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

            // ���� ü���� remainText�� ǥ��
            remainText.text = current.ToString("#,##0");

            // Slider�� ���� ������� ���� ����
            Color color = Color.Lerp(Color.red, Color.green, current / maxHP);
            sliderColor.GetComponent<MeshRenderer>().material.color = color;

            // �����̴��� ���� ü�� ������ ǥ��
            float hpRatio = (float)current / maxHP;
            slider.transform.localScale = new Vector3(hpRatio, slider.transform.localScale.y, slider.transform.localScale.z);

            yield return null;
        }
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
                dropItem.DropItem();
                playerState.kills++;

                isDestroy = true;
            }

            yield return null;
        }
    }

    // ü�� ���� �������� ���ҽ�Ű�� ���� �޼ҵ�
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

    // ü�� ȸ���� �� �߻��Ǵ� �޼ҵ�
    public void Heal(int value)
    {
        if (isDeath)
            return;

        // ü�� ���� UI ����
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