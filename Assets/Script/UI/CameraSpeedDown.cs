using UnityEngine;

public class CameraSpeedDown : MonoBehaviour
{
    [SerializeField] private int perFrame = 2;

    private Camera targetCamera;

    private void Start()
    {
        targetCamera = GetComponent<Camera>();

        targetCamera.enabled = false;

        Debug.Assert(targetCamera != null, "Error (Null Reference) : 속도를 낮출 카메라가 존재하지 않습니다.");
    }

    private void LateUpdate()
    {
        if (Time.frameCount % perFrame == 0)
            targetCamera.Render();

    }
}
