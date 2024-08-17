using UnityEngine;
using UnityEngine.SceneManagement;

public class PixelateUI : MonoBehaviour
{
    [SerializeField] private Canvas pixelCanvas;    // ���� ȭ���� �ȼ��� ����� ĵ����
    [SerializeField] private Camera pixelInCarema;  // �ȼ� ī�޶�

    private void Start()
    {

    }

    private void Update()
    {
        // ���� ���� ���� ���� �ƴ� ��� ��� �Ұ���
        if (pixelCanvas.gameObject.activeSelf && SceneManager.GetActiveScene().name != "Final")
        {
            pixelCanvas.gameObject.SetActive(false);
            pixelInCarema.gameObject.SetActive(false);
        }

        // ���� ���� ���� ���� ��� ĵ���� ��� ����
        else if (!pixelCanvas.gameObject.activeSelf && SceneManager.GetActiveScene().name == "Final")
        {
            pixelCanvas.gameObject.SetActive(true);
            pixelInCarema.gameObject.SetActive(true);
        }
    }
}
