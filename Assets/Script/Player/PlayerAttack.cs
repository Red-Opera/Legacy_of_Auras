using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Camera camera;               // �÷��̾� ī�޶�
    public GameObject bow;              // �÷��̾� Ȱ
    public GameObject arrow;            // ȭ�� ������Ʈ

    private Animator animator;          // �÷��̾��� �ִϸ�����
    private WeaponType weaponType;      // ���� �����ϰ� �ִ� ����

    public Transform camaraToMove;              // ī�޶� �̵��� ��ġ
    public Transform defCameraTrans;            // �⺻ ī�޶� ��ġ

    public float cmrMoveTime;                   // ī�޶� ��ǥ�������� �̵��ϴµ� �ɸ��� �ð�
    private float cameraMoveStartTime;          // ī�޶� �̵� ���� �ð�
    private bool isCameraMoving = false;        // ī�޶� �̵� ������ ����
    private bool isReady = false;               // ���� ȭ���� �غ��ϴ��� ����

    public void Start()
    {
        animator = GetComponent<Animator>();

        Debug.Assert(GetComponent<PlayerWeaponChanger>() != null, "Error (Null Reference) : �÷��̾� ���� ��ȭ ������Ʈ�� �������� �ʽ��ϴ�.");
        Debug.Assert(animator != null, "Error (Null Reference) : �ִϸ��̼� ������Ʈ�� �������� �ʽ��ϴ�.");
        Debug.Assert(bow != null, "Error (Null Reference) : Ȱ�� �������� �ʽ��ϴ�.");
        Debug.Assert(arrow != null, "Error (Null Reference) : ȭ���� �������� �ʽ��ϴ�.");
    }

    public void Update()
    {
        // ���������� ��� ���⸦ ���� �ִ��� Ȯ��
        weaponType = GetComponent<PlayerWeaponChanger>().weaponType;

        // Ȱ�� �����ϰ� �ְ� ���콺�� ������ �������� ȭ���� �� �غ� �� (��, �÷��̾ ���� NPC�� ��ȭ ���� ��� ������ �� ����)
        if (ChatNPC.isEnd && Input.GetMouseButton(0) && weaponType == WeaponType.BOW)
        {
            if (!(isCameraMoving || animator.GetBool("ArrowReady")))     // ī�޶� �̵� ���̰ų� ȭ���� �غ�Ǿ��� �� ����
                StartCoroutine(BowAttack());
        }

        else if (weaponType == WeaponType.BOW)
            AttackCancel();

        // ī�޶� �̵��� �� �ִ��� Ȯ���Ͽ� �̵��� �� ���� ��� �̵�
        if (isCameraMoving)
            CameraHasMove(isReady);
    }

    // ȭ�� ���� �޼ҵ�
    private IEnumerator BowAttack()
    {
        // ȭ���� �� �غ��ϴ� �ִϸ��̼� ����
        animator.SetBool("ArrowReady", true);
        isReady = true;

        // ȭ���µ� �ߺ��̵��� ī�޶� �̵�
        CameraMove();

        yield return new WaitForSeconds(1.0f);

        GameObject newBow = Instantiate(arrow, bow.transform.GetChild(0).transform);
        newBow.transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        newBow.transform.localPosition = new Vector3(-6.5f, 0.5f, -0.5f);

        // �Ҹ� ���
        AudioClip clip = bow.GetComponent<AudioSource>().clip;
        bow.GetComponent<AudioSource>().PlayOneShot(clip);
    }

    private void AttackCancel()
    {
        if (isCameraMoving || !animator.GetBool("ArrowReady"))
            return;

        animator.SetBool("ArrowReady", false);
        isReady = false;

        CameraMove();
    }

    // ī�޶� �̵� ���� �޼ҵ�
    private void CameraMove()
    {
        // ī�޶� �̵� ���� ����
        cameraMoveStartTime = Time.time;
        isCameraMoving = true;
    }

    // ī�޶� �̵� �޼ҵ�
    private void CameraHasMove(bool ready)
    {
        // ī�޶� �̵��� �ð�, �̵��Ϸ��� �ۼ�Ʈ
        float elapsedTime = Time.time - cameraMoveStartTime;
        float t = elapsedTime / cmrMoveTime;

        // ȭ���� �غ��ϴ� ���
        if (ready)
        {
            // �ð��� ���� �ڿ������� ī�޶� �̵�
            camera.transform.localPosition = Vector3.Lerp(defCameraTrans.localPosition, camaraToMove.localPosition, t * 2);
            camera.transform.localRotation = Quaternion.Lerp(defCameraTrans.localRotation, camaraToMove.localRotation, t * 2);
        }
        
        else
        {
            camera.transform.localPosition = Vector3.Lerp(camaraToMove.localPosition, defCameraTrans.localPosition, t * 2);
            camera.transform.localRotation = Quaternion.Lerp(camaraToMove.localRotation, defCameraTrans.localRotation, t * 2);
        }

        // ī�޶� �̵��Ϸ��� ��� ī�޶� �̵� ����
        if (t >= cmrMoveTime)
            isCameraMoving = false;
    }
}