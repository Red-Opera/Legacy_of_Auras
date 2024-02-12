using UnityEngine;

public class ShieldLensEffect : MonoBehaviour
{
    private GameObject playerCam;
    private Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        playerCam = GameObject.Find("Camera");
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenPoint = playerCam.GetComponent<Camera>().WorldToScreenPoint(transform.position);
        screenPoint.x = screenPoint.x / Screen.width;
        screenPoint.y = screenPoint.y / Screen.height;

        renderer.material.SetVector("_ObjScreenPos", screenPoint);
    }
}