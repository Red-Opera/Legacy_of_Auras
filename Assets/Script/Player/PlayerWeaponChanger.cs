using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public enum WeaponType { NULL, BOW, GUN, AURAS, ITEM1, ITEM2, ITEM3, ITEM4, ITEM5 }

public class PlayerWeaponChanger : MonoBehaviour
{
    [HideInInspector] public bool bowEquid = false; // Ȱ �غ� ������� �����ϴ� ��

    public GameObject bow;                  // Ȱ ������Ʈ
    public GameObject gun;                  // �� ������Ʈ
    public GameObject spin;                 // Ȱ�� ������� ��ġ
    public GameObject hand;                 // �÷��̾ ���� �� ��ġ
    public MultiAimConstraint arrowRig;     // Ȱ ����    
    public GameObject gunUI;                // �� UI
   
    private Animator animator;      // �÷��̾� �ִϸ�����

    public WeaponType weaponType;  // ���� �÷��̾� ����Ÿ��

    private Vector3 bowPosition;    // Ȱ�� ��ġ
    private Vector3 bowRotation;    // Ȱ�� ȸ��

    private Vector3 gunPosition;    // ���� ��ġ
    private Vector3 gunRotation;    // ���� ȸ��

    private bool isChange = false;  // ���� ���Ⱑ ����Ǵ��� ����

    public void Start()
    {
        weaponType = WeaponType.NULL;

        animator = GetComponent<Animator>();

        Debug.Assert(bow != null, "Error (Null Reference) : Ȱ������Ʈ�� �������� �ʽ��ϴ�.");
        Debug.Assert(gun != null, "Error (Null Reference) : �ѿ�����Ʈ�� �������� �ʽ��ϴ�.");
        Debug.Assert(animator != null, "Error (Null Reference) : �ִϸ��̼� ������Ʈ�� �������� �ʽ��ϴ�.");

        bowPosition = bow.transform.localPosition;
        bowRotation = bow.transform.localRotation.eulerAngles;

        gunPosition = gun.transform.localPosition;
        gunRotation = gun.transform.localRotation.eulerAngles;

        arrowRig.weight = 0.0f;
    }

    public void Update()
    {
        if (!ChatNPC.isEnd || isChange || TypeStory.hasActivatedCanvas)
            return;

        // ȭ�� ���°� �ƴϰ� ���� 1�� ������ �� Ȱ�� �ٲ� (��, ���� �÷��̾ ���� ��ȭ ���� ��� ���⸦ �ٲ� �� ����)
        if ((Input.GetKeyDown(KeyCode.Alpha1) && 
            !animator.GetBool("ArrowReady") && 
            (weaponType == WeaponType.NULL || weaponType == WeaponType.BOW)) || bowEquid)
            StartCoroutine(EquidUnEquidBow());

        else if (Input.GetKeyDown(KeyCode.Alpha2) && 
                 (weaponType == WeaponType.NULL || weaponType == WeaponType.GUN) && 
                 gun.activeSelf && !animator.GetCurrentAnimatorStateInfo(1).IsName("ReloadGun"))
            EquidUnEquidGun();

        else if (Input.GetKeyDown(KeyCode.Alpha3) &&
            (weaponType == WeaponType.NULL || weaponType == WeaponType.AURAS) && 
            PlayerGetAurasArrow.isGetArrow)
            StartCoroutine(ChangeAurasArrow());
    }

    // Ȱ ���� ���� �޼ҵ�
    private IEnumerator EquidUnEquidBow()
    {
        // ���� ���Ⱑ ������ ǥ��
        isChange = true;
        bowEquid = false;

        // Ȱ ����/���� �ִϸ��̼� ����
        animator.SetTrigger("BowReady");

        // ���⸦ �ۿ����� �ʴ� ���¶��
        if (weaponType == WeaponType.NULL)
            weaponType = WeaponType.BOW;    // ���� ���� Ÿ���� Ȱ�� ����

        // ���⸦ �����ϰ� �ִ� �����̰� ��� �غ� ���� �ƴ� ���
        else if (!animator.GetBool("ArrowReady"))
            weaponType = WeaponType.NULL;   // ���� ���¸� �������� ǥ��

        // �ش� Ÿ�Կ� �´� Idle�� ����
        animator.SetFloat("IdleMode", (float)weaponType);

        yield return new WaitForSeconds(0.4f);

        if (weaponType != WeaponType.BOW)
        {
            // � Ȱ ��ġ
            bow.transform.SetParent(spin.transform);
            bow.transform.localPosition = bowPosition;
            bow.transform.localRotation = Quaternion.Euler(bowRotation);
            arrowRig.weight = 0.0f;
        }

        else
        {
            // �տ� Ȱ ��ġ
            bow.transform.SetParent(hand.transform);
            bow.transform.localPosition = Vector3.zero;
            bow.transform.localRotation = Quaternion.Euler(new Vector3(-110, 10, 0));
            arrowRig.weight = 1f;
        }

        // ���� ���ϴ� ���¸� ����
        isChange = false;
    }

    // �� ���� �޼ҵ�
    private void EquidUnEquidGun()
    {
        isChange = true;

        if (weaponType == WeaponType.NULL)
        {
            animator.SetBool("GunReady", true);
            weaponType = WeaponType.GUN;
        }

        else
        {
            animator.SetBool("GunReady", false);
            weaponType = WeaponType.NULL;
        }

        animator.SetFloat("IdleMode", (float)weaponType);

        if (weaponType != WeaponType.GUN)
        {
            gun.transform.SetParent(spin.transform);
            gun.transform.localPosition = gunPosition;
            gun.transform.localRotation = Quaternion.Euler(gunRotation);

            gunUI.SetActive(false);
        }

        else
        {
            gun.transform.SetParent(hand.transform);
            gun.transform.localPosition = new Vector3(0.00285f, -0.0026f, -0.00111f);
            gun.transform.localRotation = Quaternion.Euler(new Vector3(-20, 180, -37.5f));

            gunUI.SetActive(true);
        }

        isChange = false;
    }

    private IEnumerator ChangeAurasArrow()
    {
        isChange = true;

        // ������ ȭ�쿡�� �ٲ� ���� ���� Ÿ���� 2���� ����
        if (weaponType == WeaponType.NULL)
        {
            weaponType = WeaponType.AURAS;
            animator.SetFloat("IdleMode", 2.1f);
        }

        else
            weaponType = WeaponType.NULL;

        // ������ ȭ��� �ٲٴ� ��� : 3���� ����, Idle�� �ٲٴ� ��� 2.1�� ����
        float targetMode = (float)weaponType;
        if (weaponType == WeaponType.NULL)
            targetMode = 2.1f;

        while (true)
        {
            // õõ�� �ش� �ִϸ��̼��� �ٲ�� ����
            animator.SetFloat("IdleMode", Mathf.Lerp(animator.GetFloat("IdleMode"), targetMode, 5.0f * Time.deltaTime));

            // ������ ȭ���� ���� 0���� ���� ����, Idle�� ���� ���� 2�ΰ� ��� ����
            if (Mathf.Abs(animator.GetFloat("IdleMode") - targetMode) < 0.1f)
                break;

            yield return null;
        }

        animator.SetFloat("IdleMode", (float)weaponType);

        isChange = false;
    }
}