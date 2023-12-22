using System.Collections;
using TMPro;
using UnityEngine;

public class MonsterHPChangeUI : MonoBehaviour
{
    public GameObject resultText;
    public MonsterState state;

    private GameObject player;
    private Animator animator;

    private void Start()
    {
        player = GameObject.Find("Model");
        Debug.Assert(player != null, "Error (Null Reference): �÷��̾ �������� �ʽ��ϴ�.");

        Debug.Assert(resultText != null, "Error (Null Reference): �������� ����� �ؽ�Ʈ�� �������� �ʽ��ϴ�.");
    }

    private void Update()
    {
        transform.LookAt(player.transform.position);
    }

    public IEnumerator CreateChangeText(bool isDamage, int addHP)
    {
        if (addHP < 0)
            addHP = 0;

        GameObject text = Instantiate(resultText, transform);
        TextMeshProUGUI mesh = text.GetComponentInChildren<TextMeshProUGUI>();

        animator = text.GetComponent<Animator>();

        // �������� ���� ��� -�� �����
        if (isDamage)
            mesh.text = "-" + addHP;

        // ü���� ���� ��� +�� �����
        else
            mesh.text = "+" + addHP;

        // �ؽ�Ʈ ������ �ٲ�
        ChangeTextColor(mesh, isDamage, addHP);

        while (true)
        {
            // �ִϸ��̼��� ���� ��� ����
            if (text != null && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98)
            {
                Destroy(text);
                yield break;
            }

            yield return null;
        }
    }

    // �ؽ�Ʈ ������ �ٲٴ� �޼ҵ�
    private void ChangeTextColor(TextMeshProUGUI mesh, bool isDamage, int damage)
    {
        // ü�� ��ȭ�� �󸶳� �޾Ҵ��� Ȯ���ϴ� ����
        float damagePercentage;

        // ���������� �ƴ��� �Ǵ� �� ����� ���� ����ȭ ��
        if (isDamage)
            damagePercentage = 50 - ((float)damage * 100 / state.HP)  / 2f;

        else
            damagePercentage = 50 + ((float)damage * 100 / state.HP) / 2f;

        // ����ȭ�� �����͸� �������� ������ ������
        Color startColor = Color.red;
        Color endColor = Color.green;
        Color textColor = Color.Lerp(startColor, endColor, damagePercentage / 100);

        mesh.color = textColor;
    }
}