using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public enum WeaponType { NULL, BOW, GUN }

public class PlayerWeaponChanger : MonoBehaviour
{
    public GameObject bow;          // Ȱ ������Ʈ
    public GameObject spin;         // Ȱ�� ������� ��ġ
    public GameObject hand;         // �÷��̾ ���� �� ��ġ
   
    private Animator aniamtor;      // �÷��̾� �ִϸ�����

    public WeaponType weaponType { get; private set; }  // ���� �÷��̾� ����Ÿ��

    private Vector3 bowPosition;    // Ȱ�� ��ġ
    private Vector3 bowRotation;    // Ȱ�� ȸ��

    private bool isChange = false;  // ���� ���Ⱑ ����Ǵ��� ����

    public void Start()
    {
        weaponType = WeaponType.NULL;

        aniamtor = GetComponent<Animator>();

        Debug.Assert(bow != null, "Error (Null Reference) : Ȱ������Ʈ�� �������� �ʽ��ϴ�.");
        Debug.Assert(aniamtor != null, "Error (Null Reference) : �ִϸ��̼� ������Ʈ�� �������� �ʽ��ϴ�.");

        bowPosition = bow.transform.localPosition;
        bowRotation = bow.transform.localRotation.eulerAngles;
    }

    public void Update()
    {
        // ȭ�� ���°� �ƴϰ� ���� 1�� ������ �� Ȱ�� �ٲ� (��, ���� �÷��̾ ���� ��ȭ ���� ��� ���⸦ �ٲ� �� ����)
        if (ChatNPC.isEnd && Input.GetKeyDown(KeyCode.Alpha1) && !aniamtor.GetBool("ArrowReady") && !isChange)
            StartCoroutine(EquidUnEquidBow());
    }

    // Ȱ ���� ���� �޼ҵ�
    private IEnumerator EquidUnEquidBow()
    {
        // ���� ���Ⱑ ������ ǥ��
        isChange = true;

        // Ȱ ����/���� �ִϸ��̼� ����
        aniamtor.SetTrigger("BowReady");

        // ���⸦ �ۿ����� �ʴ� ���¶��
        if (weaponType == WeaponType.NULL)
            weaponType = WeaponType.BOW;    // ���� ���� Ÿ���� Ȱ�� ����

        // ���⸦ �����ϰ� �ִ� �����̰� ��� �غ� ���� �ƴ� ���
        else if (!aniamtor.GetBool("ArrowReady"))
            weaponType = WeaponType.NULL;   // ���� ���¸� �������� ǥ��

        // �ش� Ÿ�Կ� �´� Idle�� ����
        aniamtor.SetFloat("IdleMode", (float)weaponType);

        yield return new WaitForSeconds(0.4f);

        if (weaponType == WeaponType.NULL)
        {
            // � Ȱ ��ġ
            bow.transform.SetParent(spin.transform);
            bow.transform.localPosition = bowPosition;
            bow.transform.localRotation = Quaternion.Euler(bowRotation);
        }

        else
        {
            // �տ� Ȱ ��ġ
            bow.transform.SetParent(hand.transform);
            bow.transform.localPosition = Vector3.zero;
            bow.transform.localRotation = Quaternion.Euler(new Vector3(-100, 0, 0));
        }

        // ���� ���ϴ� ���¸� ����
        isChange = false;
    }
}