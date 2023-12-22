using UnityEngine;
using UnityEngine.UIElements;

public class PlayerRotate : MonoBehaviour
{
    public GameObject headLookAt;       // �Ӹ��� �ٶ���� �ϴ� ������Ʈ
    public GameObject spineLookAt;      // ô�߰� �ٶ���� �ϴ� ������Ʈ
    public GameObject armLookAt;        // Ȱ �� ���� �ٶ���� �ϴ� ������Ʈ

    public GameObject camera;           // ī�޶� ������Ʈ
    public GameObject cameraLookAt;     // ȭ�� ���� ī�޶��� ��ġ

    public PlayerWeaponChanger weaponChanger;

    public static float sensitivityX = 2.0f;    // ���콺 ����(X)
    public static float sensitivityY = 2.0f;    // ���콺 ����(Y)
    public float maxYAngle = 80.0f;             // ���Ʒ� ���� ����

    public Vector2 currentRotation = Vector2.zero;

    private Transform headDefultPos;     // �÷��̾� �Ӹ� ������Ʈ
    private Transform spineDefultPos;    // �÷��̾� ô�� ������Ʈ
    private Transform armDefultPos;      // �÷��̾� �� ������Ʈ

    public void Start()
    {
        sensitivityX = GameManager.info.mouseSensitivityX;
        sensitivityY = GameManager.info.mouseSensitivityY;

        weaponChanger = GetComponent<PlayerWeaponChanger>();

        headDefultPos = new GameObject().transform;
        headDefultPos.localPosition = headLookAt.transform.localPosition;
        headDefultPos.localRotation = headLookAt.transform.localRotation;
        headDefultPos.localScale = headLookAt.transform.localScale;

        spineDefultPos = new GameObject().transform;
        spineDefultPos.localPosition = spineLookAt.transform.localPosition;
        spineDefultPos.localRotation = spineLookAt.transform.localRotation;
        spineDefultPos.localScale = spineLookAt.transform.localScale;

        armDefultPos = new GameObject().transform;
        armDefultPos.localPosition = armLookAt.transform.localPosition;
        armDefultPos.localRotation = armLookAt.transform.localRotation;
        armDefultPos.localScale = armLookAt.transform.localScale;

        Debug.Assert(headLookAt != null, "Error (Null Reference) : ��ǥ������ �������� �ʽ��ϴ�.");
        Debug.Assert(spineLookAt != null, "Error (Null Reference) : ��ǥ������ �������� �ʽ��ϴ�.");
        Debug.Assert(camera != null, "Error (Null Reference) : ī�޶� ������Ʈ�� �������� �ʽ��ϴ�.");
        Debug.Assert(weaponChanger != null, "Error (Null Reference) : ���� ���� ��ũ��Ʈ�� �������� �ʽ��ϴ�.");
    }

    public void Update()
    {
        if (TypeStory.hasActivatedCanvas)
            return;

        // ���콺 ���� ��������
        sensitivityX = GameManager.info.mouseSensitivityX;
        sensitivityY = GameManager.info.mouseSensitivityY;

        // ���콺 �Է� �ޱ�
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY;

        // ���� ȸ�� �� ����
        currentRotation.x += mouseX;
        currentRotation.y -= mouseY;
        currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);

        // ȸ�� ����
        camera.transform.eulerAngles = new Vector3(currentRotation.y, currentRotation.x, 0);
        transform.rotation = Quaternion.Euler(new Vector3(0, currentRotation.x, 0));

        // �Ӹ��� ô�� ��ġ ������Ʈ
        if (headDefultPos == null)
        {
            headDefultPos = new GameObject().transform;
            headDefultPos.localPosition = headLookAt.transform.localPosition;
            headDefultPos.localRotation = headLookAt.transform.localRotation;
            headDefultPos.localScale = headLookAt.transform.localScale;

            spineDefultPos = new GameObject().transform;
            spineDefultPos.localPosition = spineLookAt.transform.localPosition;
            spineDefultPos.localRotation = spineLookAt.transform.localRotation;
            spineDefultPos.localScale = spineLookAt.transform.localScale;

            armDefultPos = new GameObject().transform;
            armDefultPos.localPosition = armLookAt.transform.localPosition;
            armDefultPos.localRotation = armLookAt.transform.localRotation;
            armDefultPos.localScale = armLookAt.transform.localScale;
        }

        headLookAt.transform.localPosition = headDefultPos.transform.localPosition + camera.transform.localRotation * Vector3.forward / 10;
        spineLookAt.transform.localPosition = spineDefultPos.transform.localPosition + camera.transform.localRotation * Vector3.forward / 10;

        armLookAt.transform.localPosition = armDefultPos.transform.localPosition + camera.transform.localRotation * Vector3.forward / 10;
    }
}