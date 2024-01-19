using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public enum WeaponType { NULL, BOW, GUN }

public class PlayerWeaponChanger : MonoBehaviour
{
    public GameObject bow;                  // Ȱ ������Ʈ
    public GameObject gun;                  // �� ������Ʈ
    public GameObject spin;                 // Ȱ�� ������� ��ġ
    public GameObject hand;                 // �÷��̾ ���� �� ��ġ
    public MultiAimConstraint arrowRig;     // Ȱ ����    
    public GameObject gunUI;                // �� UI
   
    private Animator aniamtor;      // �÷��̾� �ִϸ�����

    public WeaponType weaponType { get; private set; }  // ���� �÷��̾� ����Ÿ��

    private Vector3 bowPosition;    // Ȱ�� ��ġ
    private Vector3 bowRotation;    // Ȱ�� ȸ��

    private Vector3 gunPosition;    // ���� ��ġ
    private Vector3 gunRotation;    // ���� ȸ��

    private bool isChange = false;  // ���� ���Ⱑ ����Ǵ��� ����

    public void Start()
    {
        weaponType = WeaponType.NULL;

        aniamtor = GetComponent<Animator>();

        Debug.Assert(bow != null, "Error (Null Reference) : Ȱ������Ʈ�� �������� �ʽ��ϴ�.");
        Debug.Assert(gun != null, "Error (Null Reference) : �ѿ�����Ʈ�� �������� �ʽ��ϴ�.");
        Debug.Assert(aniamtor != null, "Error (Null Reference) : �ִϸ��̼� ������Ʈ�� �������� �ʽ��ϴ�.");

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
        if (Input.GetKeyDown(KeyCode.Alpha1) && !aniamtor.GetBool("ArrowReady") && weaponType != WeaponType.GUN)
            StartCoroutine(EquidUnEquidBow());

        else if (Input.GetKeyDown(KeyCode.Alpha2) && weaponType != WeaponType.BOW && gun.activeSelf)
            EquidUnEquidGun();
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
            aniamtor.SetBool("GunReady", true);
            weaponType = WeaponType.GUN;
        }

        else
        {
            aniamtor.SetBool("GunReady", false);
            weaponType = WeaponType.NULL;
        }

        aniamtor.SetFloat("IdleMode", (float)weaponType);

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
}