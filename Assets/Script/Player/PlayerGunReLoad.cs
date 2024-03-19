using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerGunReLoad : MonoBehaviour
{
    public TextMeshProUGUI currentText;
    public TextMeshProUGUI remainText;
    public PlayerState state;
    public PlayerGunShotUI shotUI;

    private PlayerWeaponChanger weaponChanger;
    private Animator animator;

    public bool isReLoad { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        weaponChanger = GetComponent<PlayerWeaponChanger>();
        animator = GetComponent<Animator>();

        Debug.Assert(weaponChanger != null, "Error (Null Reference) : �÷��̾ WeaponChanger�� �������� �ʽ��ϴ�.");
        Debug.Assert(animator != null, "Error (Null Reference) : �÷��̾ �ִϸ����Ͱ� �������� �ʽ��ϴ�.");
        Debug.Assert(currentText != null, "Error (Null Reference) : ���� źâ�� �Ѿ� ���� �������� �ʽ��ϴ�.");
        Debug.Assert(remainText != null, "Error (Null Reference) : ���� ��ü �Ѿ� ���� �������� �ʽ��ϴ�.");
        Debug.Assert(state != null, "Error (Null Reference) : �÷��̾� ���°� �������� �ʽ��ϴ�.");
        Debug.Assert(shotUI != null, "Error (Null Reference) : �÷��̾� �� UI�� �������� �ʽ��ϴ�.");
    }

    void Update()
    {
        // �÷��̾ RŰ�� ������ ���� ���� ���� �ִ� ���
        if (Input.GetKeyDown(KeyCode.R) && weaponChanger.weaponType == WeaponType.GUN && !isReLoad)
            StartCoroutine(ReLoad());

    }

    // źâ�� �Ѿ��� ä��� �޼ҵ�
    private IEnumerator ReLoad()
    {
        int remain = Int32.Parse(remainText.text), current = Int32.Parse(currentText.text);
        int moveCount = state.bulletCurrentMax - current;

        if (remain <= 0 || current == state.bulletCurrentMax)
            yield break;

        isReLoad = true;

        // �Ѿ� �����ϴ� �ִϸ��̼��� ���� 
        animator.SetTrigger("Reload");
        while (true)
        {
            // ���� �ִϸ��̼��� �����ϴ� ����ϰ� ���� ��� ������ ���� ����ϴ� ���� ����
            if (animator.GetNextAnimatorStateInfo(1).IsName("ReloadGun"))
            {
                animator.SetBool("Reloading", true);
                break;
            }

            yield return null;
        }
        
        yield return new WaitForSeconds(3.3f);

        // ���� �ִ� �Ѿ��� ���� źâ ũ�⺸�� ���� ���
        if (moveCount > remain)
        {
            // ��� �Ѿ��� źâ���� �ű�
            currentText.SetText(remainText.text.ToString());
            remainText.SetText("0");
            shotUI.SetTextColor();
        }

        else
        {
            // źâ�� �Ѿ��� ���� ä��
            currentText.SetText(state.bulletCurrentMax.ToString());
            remainText.SetText((remain - moveCount).ToString());
            shotUI.SetTextColor();
        }

        // �ٽ� ���� ��� �ִ� ���·� ����
        animator.SetBool("Reloading", false);

        isReLoad = false;
    }
}
