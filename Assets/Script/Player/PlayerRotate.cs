using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public GameObject headLookAt;       // �Ӹ��� �ٶ���� �ϴ� ������Ʈ
    public GameObject spineLookAt;      // ô�߰� �ٶ���� �ϴ� ������Ʈ

    public GameObject head;             // �÷��̾� �Ӹ� ������Ʈ
    public GameObject spine;            // �÷��̾� ô�� ������ư
    public GameObject camera;           // ī�޶� ������Ʈ

    public static float sensitivityX = 2.0f;    // ���콺 ����(X)
    public static float sensitivityY = 2.0f;    // ���콺 ����(Y)
    public float maxYAngle = 80.0f;             // ���Ʒ� ���� ����

    public Vector2 currentRotation = Vector2.zero;

    public void Start()
    {
        sensitivityX = GameManager.info.mouseSensitivityX;
        sensitivityY = GameManager.info.mouseSensitivityY;

        Debug.Assert(headLookAt != null, "Error (Null Reference) : ��ǥ������ �������� �ʽ��ϴ�.");
        Debug.Assert(spineLookAt != null, "Error (Null Reference) : ��ǥ������ �������� �ʽ��ϴ�.");
        Debug.Assert(head != null, "Error (Null Reference) : �Ӹ� ������Ʈ�� �������� �ʽ��ϴ�.");
        Debug.Assert(spine != null, "Error (Null Reference) : ô�� ������Ʈ�� �������� �ʽ��ϴ�.");
        Debug.Assert(camera != null, "Error (Null Reference) : ī�޶� ������Ʈ�� �������� �ʽ��ϴ�.");
    }

    public void Update()
    {
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
        headLookAt.transform.position = head.transform.position + camera.transform.forward * 50.0f + new Vector3(0.5f, 0, 0);
        spineLookAt.transform.position = spine.transform.position + camera.transform.forward * 50.0f + new Vector3(0.5f, 0, 0);
    }
}