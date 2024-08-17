using UnityEngine;

public class CameraSpeedDown : MonoBehaviour
{
    [SerializeField] private int perFrame = 2;

    private Camera targetCamera;

    private void Start()
    {
        targetCamera = GetComponent<Camera>();

        targetCamera.enabled = false;

        Debug.Assert(targetCamera != null, "Error (Null Reference) : �ӵ��� ���� ī�޶� �������� �ʽ��ϴ�.");
    }

    private void LateUpdate()
    {
        if (Time.frameCount % perFrame == 0)
            targetCamera.Render();

    }
}
